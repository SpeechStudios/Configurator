using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Transform Cam_Pos_Main;
    public Transform Cam_Pos_Select;
    public Transform Cam_Pos_Customize;
    public List<GameObject> cars;
    public float TransitionTime = 0.5f;
    public int AvailableMoney = 200000;

    [Header("UI")]
    public TextMeshProUGUI AvailableMoneyText;
    public GameObject MenuUIPanel;
    public GameObject CarShopUIPanel, GarrageUIPanel, CustomizeUIPanel, WeaponShopUIPanel, ColorShopUIPanel;
    public TextMeshProUGUI CarNameUI, CarCostUI, CarDescriptionUI;
    public TextMeshProUGUI WeaponNameUI, WeaponCostUI, WeaponDescriptionUI;
    public TextMeshProUGUI ColorCost;
    public GameObject PurchaseCompleteUI, PurchaseComplete, PriceShow;
    public AudioClip NoMoney;

    private Camera cam;
    private int currentCar;

    [Header("Garage")]

    public GameObject G_Next_PreviousPanel;
    public GameObject CustomizeButton, PurchaseCarButton;
    private GameObject currentCarPrefab;
    public List<CarStatistics> carSlots;

    [Header("Customization")]
    private GameObject CurrentWeaponPrefab;
    private Weapon currentWeapon;
    private int currentWeaponIndex;
    private int currentweaponHolder;
    private bool Customization;
    private bool highlight;
    private GameObject storredHighlight;
    private Color OriginalColor;
    private MaterialHolder selectedmat;



    void Start()
    {
        if(PlayerPrefs.HasKey("CarData"))
        {
            for (int i = 0; i < PlayerPrefs.GetInt("CarData"); i++)
            {
                SavedCar car = SaveData.LoadCarFile(i.ToString());
                carSlots.Add(cars[car.CarIndex].GetComponent<CarStatistics>());
            }
        }
        if(PlayerPrefs.HasKey("Money"))
        {
            AvailableMoney = PlayerPrefs.GetInt("Money");
        }
        CarShopUIPanel.SetActive(false);
        GarrageUIPanel.SetActive(false);
        CustomizeUIPanel.SetActive(false);
        WeaponShopUIPanel.SetActive(false);
        ColorShopUIPanel.SetActive(false);
        cam = Camera.main;
    }
    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Weapon")) && Customization)
        {
            if(storredHighlight && hit.transform.gameObject !=storredHighlight)
            {
                foreach (var item in storredHighlight.transform.GetComponentsInChildren<MeshRenderer>())
                {
                    item.material.SetColor("_EmissionColor", Color.black);
                }
                highlight = false;
            }
            if (!highlight)
            {
                foreach (var item in hit.transform.GetComponentsInChildren<MeshRenderer>())
                {
                    item.material.SetColor("_EmissionColor", Color.grey);
                }
                highlight = true;
                storredHighlight = hit.transform.gameObject;
            }
        }
        else
        {
            if (storredHighlight)
            {
                foreach (var item in storredHighlight.transform.GetComponentsInChildren<MeshRenderer>())
                {
                    item.material.SetColor("_EmissionColor", Color.black);
                }
                storredHighlight = null;
            }
            highlight = false;
        }
    }
    private void DisplayMoney()
    {
        AvailableMoneyText.text = "$" + AvailableMoney.ToString();
        PlayerPrefs.SetInt("Money", AvailableMoney);
    }

    #region CarShop
    public void S_Next()
    {
        if (currentCar < cars.Count - 1)
        {
            cars[currentCar].SetActive(false);
            currentCar++;
            cars[currentCar].SetActive(true);
        }
        else
        {
            cars[currentCar].SetActive(false);
            currentCar = 0;
            cars[currentCar].SetActive(true);
        }
        DisplayCarData();
    }
    public void S_Previous()
    {
        if (currentCar > 0)
        {
            cars[currentCar].SetActive(false);
            currentCar--;
            cars[currentCar].SetActive(true);
        }
        else
        {
            cars[currentCar].SetActive(false);
            currentCar = cars.Count - 1;
            cars[currentCar].SetActive(true);
        }
        DisplayCarData();
    }
    public void DisplayCarData()
    {
        CarStatistics data = cars[currentCar].GetComponent<CarStatistics>();
        data.mat.color = data.SavedColor;
        CarNameUI.text = data.Name;
        CarCostUI.text = "$" + data.Cost;
        data.Description = data.Description.Replace("\\n", "\n");
        CarDescriptionUI.text = data.Description;
    }
    public void PurchaseCar()
    {
        if (AvailableMoney >= cars[currentCar].GetComponent<CarStatistics>().Cost)
        {
            carSlots.Add(cars[currentCar].GetComponent<CarStatistics>());
            PurchaseCompleteUI.SetActive(true);
            PurchaseComplete.SetActive(true);
            PriceShow.GetComponent<TextMeshProUGUI>().text = "-$" + cars[currentCar].GetComponent<CarStatistics>().Cost.ToString();
            PriceShow.SetActive(true);
            SavedCar car = new();
            car.CarIndex = cars[currentCar].GetComponent<CarStatistics>().Index;
            car.color = cars[currentCar].GetComponent<CarStatistics>().SavedColor;
            SaveData.SaveCarData(car, (carSlots.Count -1).ToString());
            PlayerPrefs.SetInt("CarData", carSlots.Count);
            AvailableMoney -= cars[currentCar].GetComponent<CarStatistics>().Cost;
            DisplayMoney();
        }
        else
        {
            AudioSource.PlayClipAtPoint(NoMoney, transform.position);
        }
    }
    public void ContinueShopping()
    {
        PurchaseCompleteUI.SetActive(false);
    }

    #endregion

    #region Garage
    public void OpenGarrage(int carChoice)
    {
        if (carSlots.Count == 0)
        {
            G_Next_PreviousPanel.SetActive(false);
            CustomizeButton.SetActive(false);
            PurchaseCarButton.SetActive(true);
            return;
        }
        else
        {
            CustomizeButton.SetActive(true);
            PurchaseCarButton.SetActive(false);
        }
        if (carSlots.Count > 0)
        {
            currentCar = carChoice;
            InstantiateNewCar();
        }
        if (carSlots.Count == 1)
        {
            G_Next_PreviousPanel.SetActive(false);
        }
        else
        {
            G_Next_PreviousPanel.SetActive(true);
        }
    }
    public void G_Next()
    {
        if (currentCar < carSlots.Count - 1)
        {
            DestroyOldCar();
            currentCar++;
            InstantiateNewCar();
        }
        else
        {
            DestroyOldCar();
            currentCar = 0;
            InstantiateNewCar();
        }
    }
    public void G_Previous()
    {
        if (currentCar > 0)
        {
            DestroyOldCar();
            currentCar--;
            InstantiateNewCar();
        }
        else
        {
            DestroyOldCar();
            currentCar = carSlots.Count - 1;
            InstantiateNewCar();
        }
    }
    public void DestroyOldCar()
    {
        foreach (var item in currentCarPrefab.GetComponent<CarStatistics>().carData.weapons)
        {
            if (item.PurchasedWeapon != null)
            {
                Destroy(item.InstantiatedWeapon);
            }
        }
        Destroy(currentCarPrefab);
    }
    public void InstantiateNewCar()
    {
        GameObject car = Instantiate(cars[carSlots[currentCar].Index], cars[carSlots[currentCar].Index].transform.position, cars[carSlots[currentCar].Index].transform.rotation);
        SavedCar savedCarData = SaveData.LoadCarFile(currentCar.ToString());
        Debug.Log(savedCarData.CarIndex);
        car.GetComponent<CarStatistics>().mat.color = savedCarData.color;
        car.GetComponent<CarStatistics>().SavedColor = savedCarData.color;
        car.SetActive(true);
        currentCarPrefab = car;
        if (PlayerPrefs.HasKey(currentCar.ToString()))
        {
            List<SavedWeapon> data = SaveData.LoadWeaponFile(currentCar.ToString());
            foreach (var item in data)
            {
                car.GetComponent<CarStatistics>().carData.weapons[item.WeaponSelectIndex].PurchasedWeapon = DataManager.Instance.GetWeaponPrefabByName(item.WeaponName);
            }
            foreach (var item in currentCarPrefab.GetComponent<CarStatistics>().carData.weapons)
            {
                if (item.PurchasedWeapon != null)
                {
                    item.WeaponTransform.parent.GetComponent<MeshRenderer>().enabled = false;
                    item.InstantiatedWeapon = Instantiate(item.PurchasedWeapon, item.WeaponTransform.position, item.WeaponTransform.rotation);
                    item.InstantiatedWeapon.transform.localScale = item.WeaponTransform.localScale;
                }
            }
        }
    }
    #endregion

    #region Customization
    public void OpenCustomization()
    {
        foreach (var item in currentCarPrefab.GetComponent<CarStatistics>().carData.weapons)
        {
            item.WeaponTransform.parent.gameObject.SetActive(true);
            if (item.PurchasedWeapon != null)
            {
                item.WeaponTransform.parent.GetComponent<MeshRenderer>().enabled = false;
                if (item.InstantiatedWeapon == null)
                {
                    item.InstantiatedWeapon = Instantiate(item.PurchasedWeapon, item.WeaponTransform.position, item.WeaponTransform.rotation);
                    item.InstantiatedWeapon.transform.localScale = item.WeaponTransform.localScale;
                }
            }

        }
        cam.GetComponent<CameraCustomization>().enabled = true;
        Customization = true;
    }
    public void AquireNewWeapon(int selectedWeaponHolder)
    {
        bool DestroyPrevious;
        if (currentweaponHolder == selectedWeaponHolder)
        {
            DestroyPrevious = true; 
        }
        else
        {
            DestroyPrevious = false;
        }
        currentweaponHolder = selectedWeaponHolder;
        if(currentCarPrefab.GetComponent<CarStatistics>().carData.weapons[selectedWeaponHolder].PurchasedWeapon)
        {
            Destroy(currentCarPrefab.GetComponent<CarStatistics>().carData.weapons[selectedWeaponHolder].InstantiatedWeapon);
        }
        CustomizeToWeaponShop(DestroyPrevious);
    }
    public void C_Next()
    {
        if (currentWeaponIndex < currentCarPrefab.GetComponent<CarStatistics>().carData.weapons[currentweaponHolder].availableWeapons.Count -1)
        {
            Destroy(CurrentWeaponPrefab);
            currentWeaponIndex++;
            InstantiateNewWeapon();
        }
        else
        {
            Destroy(CurrentWeaponPrefab);
            currentWeaponIndex = 0;
            InstantiateNewWeapon();
        }
        DisplayWeaponData();
    }
    public void C_Previous()
    {
        if (currentWeaponIndex > 0)
        {
            Destroy(CurrentWeaponPrefab);
            currentWeaponIndex--;
            InstantiateNewWeapon();
        }
        else
        {
            Destroy(CurrentWeaponPrefab);
            currentWeaponIndex = currentCarPrefab.GetComponent<CarStatistics>().carData.weapons[currentweaponHolder].availableWeapons.Count -1;
            InstantiateNewWeapon();
        }
        DisplayWeaponData();
    }
    void InstantiateNewWeapon()
    {
        currentWeapon = currentCarPrefab.GetComponent<CarStatistics>().carData.weapons[currentweaponHolder];
        GameObject Weapon = Instantiate(currentWeapon.availableWeapons[currentWeaponIndex], currentWeapon.WeaponTransform.position, currentWeapon.WeaponTransform.rotation);
        Weapon.transform.localScale = currentWeapon.WeaponTransform.localScale;
        CurrentWeaponPrefab = Weapon;
    }
    public void PurchaseWeapon()
    {
        if (AvailableMoney >= CurrentWeaponPrefab.GetComponent<WeaponData>().Cost)
        {
            currentWeapon.PurchasedWeapon = DataManager.Instance.GetWeaponPrefabByName(CurrentWeaponPrefab.GetComponent<WeaponData>().Name);
            WeaponShopToCustomize();
            SaveData.SaveCarWeaponData(currentCarPrefab.GetComponent<CarStatistics>().carData, currentCar.ToString());
            PurchaseComplete.SetActive(true);
            PriceShow.GetComponent<TextMeshProUGUI>().text = "-$" + CurrentWeaponPrefab.GetComponent<WeaponData>().Cost.ToString();
            PriceShow.SetActive(true);
            AvailableMoney -= CurrentWeaponPrefab.GetComponent<WeaponData>().Cost;
            DisplayMoney();
        }
        else
        {
            AudioSource.PlayClipAtPoint(NoMoney, transform.position);
        }
    }
    public void DisplayWeaponData()
    {
        WeaponData data = currentWeapon.availableWeapons[currentWeaponIndex].GetComponent<WeaponData>();
        WeaponNameUI.text = data.Name;
        WeaponCostUI.text = "$" + data.Cost;
        WeaponDescriptionUI.text = data.Description;
    }

    public void HoverColor(MaterialHolder mat)
    {
        currentCarPrefab.GetComponent<CarStatistics>().mat.color = mat.myMat.color;
        ColorCost.text = "$" + mat.Cost.ToString();
    }
    public void HoverExit()
    {
        if (selectedmat != null)
        {
            currentCarPrefab.GetComponent<CarStatistics>().mat.color = selectedmat.myMat.color;
            ColorCost.text = "$" + selectedmat.Cost.ToString();
        }
        else
        {
            currentCarPrefab.GetComponent<CarStatistics>().mat.color = OriginalColor;
            ColorCost.text = "";
        }
    }
    public void SelectColor(MaterialHolder mat)
    {
        selectedmat = mat;
    }
    public void PurchaseColor()
    {
        if (selectedmat != null)
        {
            if (AvailableMoney >= selectedmat.Cost)
            {
                currentCarPrefab.GetComponent<CarStatistics>().SavedColor = selectedmat.myMat.color;
                OriginalColor = selectedmat.myMat.color;
                SavedCar car = new();
                car.CarIndex = carSlots[currentCar].Index;
                car.color = selectedmat.myMat.color;
                SaveData.SaveCarData(car, currentCar.ToString());
                PriceShow.GetComponent<TextMeshProUGUI>().text = "-$" + selectedmat.Cost.ToString();
                PriceShow.SetActive(true);
                AvailableMoney -= selectedmat.Cost;
                DisplayMoney();
                return;
            }
        }
        AudioSource.PlayClipAtPoint(NoMoney, transform.position);
    }

    #endregion


    #region UIPanelButtons
    public void MenuToCarShop()
    {
        MenuUIPanel.SetActive(false);
        CarShopUIPanel.SetActive(true);
        cars[0].SetActive(true);
        currentCar = 0;
        LeanTween.move(cam.gameObject, Cam_Pos_Select, TransitionTime);
        LeanTween.rotate(cam.gameObject, Cam_Pos_Select.rotation.eulerAngles, TransitionTime);
        DisplayCarData();
        cam.GetComponent<CameraCustomization>().enabled = true;
    }
    public void MenuToGarrage()
    {
        MenuUIPanel.SetActive(false);
        GarrageUIPanel.SetActive(true);
        currentCar = 0;
        LeanTween.move(cam.gameObject, Cam_Pos_Select, TransitionTime);
        LeanTween.rotate(cam.gameObject, Cam_Pos_Select.rotation.eulerAngles, TransitionTime);
        OpenGarrage(currentCar);
        cam.GetComponent<CameraCustomization>().enabled = true;
    }
    public void CarShopToMenu()
    {
        CarShopUIPanel.SetActive(false);
        MenuUIPanel.SetActive(true);
        cars[currentCar].SetActive(false);
        LeanTween.move(cam.gameObject, Cam_Pos_Main, TransitionTime);
        LeanTween.rotate(cam.gameObject, Cam_Pos_Main.rotation.eulerAngles, TransitionTime);
        cam.GetComponent<CameraCustomization>().enabled = false;
    }
    public void CarShopToCustomize()
    {
        CarShopUIPanel.SetActive(false);
        CustomizeUIPanel.SetActive(true);
        cars[currentCar].SetActive(false);
        PurchaseCompleteUI.SetActive(false);
        LeanTween.move(cam.gameObject, Cam_Pos_Customize, TransitionTime);
        LeanTween.rotate(cam.gameObject, Cam_Pos_Customize.rotation.eulerAngles, TransitionTime);
        OpenGarrage(carSlots.Count - 1);
        currentCar = carSlots.Count - 1;
        OpenCustomization();
    }
    public void GarageToMenu()
    {
        GarrageUIPanel.SetActive(false);
        MenuUIPanel.SetActive(true);
        if (currentCarPrefab)
            foreach (var item in currentCarPrefab.GetComponent<CarStatistics>().carData.weapons)
            {
                Destroy(item.InstantiatedWeapon);
            }
        Destroy(currentCarPrefab);
        LeanTween.move(cam.gameObject, Cam_Pos_Main, TransitionTime);
        LeanTween.rotate(cam.gameObject, Cam_Pos_Main.rotation.eulerAngles, TransitionTime);
        cam.GetComponent<CameraCustomization>().enabled = false;
    }
    public void GarageToCustomize()
    {
        GarrageUIPanel.SetActive(false);
        CustomizeUIPanel.SetActive(true);
        LeanTween.move(cam.gameObject, Cam_Pos_Customize, TransitionTime);
        LeanTween.rotate(cam.gameObject, Cam_Pos_Customize.rotation.eulerAngles, TransitionTime);
        OpenCustomization();
    }
    public void GarageToCarShop()
    {
        GarrageUIPanel.SetActive(false);
        CarShopUIPanel.SetActive(true);
        cars[0].SetActive(true);
        currentCar = 0;
    }
    public void CustomizeToGarage()
    {
        CustomizeUIPanel.SetActive(false);
        GarrageUIPanel.SetActive(true);
        LeanTween.move(cam.gameObject, Cam_Pos_Select, TransitionTime);
        LeanTween.rotate(cam.gameObject, Cam_Pos_Select.rotation.eulerAngles, TransitionTime);
        foreach (var item in currentCarPrefab.GetComponent<CarStatistics>().carData.weapons)
        {
            item.WeaponTransform.parent.gameObject.SetActive(false);
        }
        Customization = false;
    }
    public void CustomizeToWeaponShop(bool Destroyprevious)
    {
        CustomizeUIPanel.SetActive(false);
        WeaponShopUIPanel.SetActive(true);
        currentWeaponIndex = 0;
        if (currentWeapon !=null && Destroyprevious)
            Destroy(currentWeapon.InstantiatedWeapon);
        InstantiateNewWeapon();
        foreach (var item in currentCarPrefab.GetComponent<CarStatistics>().carData.weapons)
        {
            item.WeaponTransform.parent.gameObject.SetActive(false);
        }
        LeanTween.move(cam.gameObject, currentWeapon.WeaponCamTransform.position, TransitionTime);
        LeanTween.rotate(cam.gameObject, currentWeapon.WeaponCamTransform.rotation.eulerAngles, TransitionTime);
        DisplayWeaponData();
        cam.GetComponent<CameraCustomization>().enabled = false;
        Customization = false;
    }
    public void WeaponShopToCustomize()
    {
        WeaponShopUIPanel.SetActive(false);
        CustomizeUIPanel.SetActive(true);
        LeanTween.move(cam.gameObject, Cam_Pos_Customize, TransitionTime);
        LeanTween.rotate(cam.gameObject, Cam_Pos_Customize.rotation.eulerAngles, TransitionTime);
        Destroy(CurrentWeaponPrefab);
        OpenCustomization();
    }
    public void CustomizeToColorShop()
    {
        CustomizeUIPanel.SetActive(false);
        ColorShopUIPanel.SetActive(true);
        LeanTween.move(cam.gameObject, Cam_Pos_Select, TransitionTime);
        LeanTween.rotate(cam.gameObject, Cam_Pos_Select.rotation.eulerAngles, TransitionTime);
        foreach (var item in currentCarPrefab.GetComponent<CarStatistics>().carData.weapons)
        {
            item.WeaponTransform.parent.gameObject.SetActive(false);
        }
        OriginalColor = currentCarPrefab.GetComponent<CarStatistics>().mat.color;
        selectedmat = null;
    }
    public void ColorShopToCustomize()
    {
        ColorShopUIPanel.SetActive(false);
        CustomizeUIPanel.SetActive(true);
        LeanTween.move(cam.gameObject, Cam_Pos_Customize, TransitionTime);
        LeanTween.rotate(cam.gameObject, Cam_Pos_Customize.rotation.eulerAngles, TransitionTime);
        OpenCustomization();
        currentCarPrefab.GetComponent<CarStatistics>().mat.color = OriginalColor;

    }
    public void Restart()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(0);
    }
    #endregion
}
