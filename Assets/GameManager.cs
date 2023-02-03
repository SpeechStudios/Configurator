using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform Cam_Pos_Main;
    public Transform Cam_Pos_Select;
    public Transform Cam_Pos_Customize;
    public List<GameObject> cars;
    public float TransitionTime = 0.5f;

    [Header("UI")]
    public GameObject MenuUIPanel;
    public GameObject CarShopUIPanel, GarrageUIPanel, CustomizeUIPanel, WeaponShopUIPanel;
    public TextMeshProUGUI CarNameUI, CarCostUI, CarDescriptionUI;
    public TextMeshProUGUI WeaponNameUI, WeaponCostUI, WeaponDescriptionUI;
    public GameObject PurchaseCompleteUI;

    private Camera cam;
    private int currentCar;

    [Header("Garage")]
    public Transform carSlotTransform;
    public GameObject G_Next_PreviousPanel;
    public GameObject CustomizeButton, PurchaseCarButton;
    private GameObject currentCarPrefab;
    [HideInInspector] public List<CarStatistics> carSlots;

    [Header("Customization")]
    private GameObject CurrentWeaponPrefab;
    private Weapon currentWeapon;
    private int currentWeaponIndex;
    private int currentweaponHolder;



    void Start()
    {
        PlayerPrefs.DeleteAll();
        CarShopUIPanel.SetActive(false);
        GarrageUIPanel.SetActive(false);
        CustomizeUIPanel.SetActive(false);
        WeaponShopUIPanel.SetActive(false);
        cam = Camera.main;
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
        CarNameUI.text = data.Name;
        CarCostUI.text = "$" + data.Cost;
        CarDescriptionUI.text = data.Description;
    }
    public void PurchaseCar()
    {
        carSlots.Add(cars[currentCar].GetComponent<CarStatistics>());
        PurchaseCompleteUI.SetActive(true);
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
        GameObject car = Instantiate(cars[carSlots[currentCar].Index], carSlotTransform.position, carSlotTransform.rotation);
        car.SetActive(true);
        currentCarPrefab = car;
        if (PlayerPrefs.HasKey(currentCar.ToString()))
        {
            List<SavedWeapon> data = SaveData.LoadFile(currentCar.ToString());
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
            if (item.PurchasedWeapon != null && item.InstantiatedWeapon == null)
            {
                item.WeaponTransform.parent.GetComponent<MeshRenderer>().enabled = false;
                item.InstantiatedWeapon = Instantiate(item.PurchasedWeapon, item.WeaponTransform.position, item.WeaponTransform.rotation);
                item.InstantiatedWeapon.transform.localScale = item.WeaponTransform.localScale;
            }
        }
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
        currentWeapon.PurchasedWeapon = DataManager.Instance.GetWeaponPrefabByName(CurrentWeaponPrefab.GetComponent<WeaponData>().Name);
        WeaponShopToCustomize();
        SaveData.SaveCarData(currentCarPrefab.GetComponent<CarStatistics>().carData, currentCar.ToString());
    }
    public void DisplayWeaponData()
    {
        WeaponData data = currentWeapon.availableWeapons[currentWeaponIndex].GetComponent<WeaponData>();
        WeaponNameUI.text = data.Name;
        WeaponCostUI.text = "$" + data.Cost;
        WeaponDescriptionUI.text = data.Description;
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
    }
    public void MenuToGarrage()
    {
        MenuUIPanel.SetActive(false);
        GarrageUIPanel.SetActive(true);
        currentCar = 0;
        LeanTween.move(cam.gameObject, Cam_Pos_Select, TransitionTime);
        LeanTween.rotate(cam.gameObject, Cam_Pos_Select.rotation.eulerAngles, TransitionTime);
        OpenGarrage(currentCar);
    }
    public void CarShopToMenu()
    {
        CarShopUIPanel.SetActive(false);
        MenuUIPanel.SetActive(true);
        cars[currentCar].SetActive(false);
        LeanTween.move(cam.gameObject, Cam_Pos_Main, TransitionTime);
        LeanTween.rotate(cam.gameObject, Cam_Pos_Main.rotation.eulerAngles, TransitionTime);
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
    #endregion
}
