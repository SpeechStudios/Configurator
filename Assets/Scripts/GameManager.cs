using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Transform Cam_Pos_Main;
    public Transform Cam_Pos_Select;
    public Transform Cam_Pos_Customize;
    public Vector3 PreviousPosition;
    public List<GameObject> cars;
    public float TransitionTime = 0.5f;
    public int AvailableMoney = 200000;

    [Header("UI")]
    public TextMeshProUGUI AvailableMoneyText;
    public GameObject MenuUIPanel;
    public GameObject CustomizeUIPanel, ColorShopUIPanel;
    public TextMeshProUGUI ColorCost;
    public GameObject PurchaseCompleteUI, PurchaseComplete, PriceShow;
    public Transform PurchaseCompletePosition, priceShowPosition;
    public AudioClip NoMoney;
    public Sprite Mute, UnMute;
    public Image MuteButton;
    public AudioSource BG_Music;
    bool muted;

    private Camera cam;
    private int currentCar;

    [Header("CarShop")]
    public GameObject CarShopUIPanel;
    public TextMeshProUGUI CarNameUI, CarCostUI;
    public GameObject[] CarDescriptionUI;
    public Slider carSpeedBar, carArmorBar;
    public TextMeshProUGUI speedText, armorText;
    public AudioClip PurchaseAudio;

    [Header("WeaponShop")]
    public GameObject WeaponShopUIPanel;
    public TextMeshProUGUI WeaponNameUI, WeaponCostUI, WeaponSpecialUI, WeaponPurchaseUI;
    public Slider weaponDpsBar, weaponArmorBar, weapondDpsUpgradeBar, weapondArmorUpgradeBar;
    public TextMeshProUGUI weaponDPSText, weaponArmorText;

    [Header("Garage")]
    public GameObject GarrageUIPanel;
    public GameObject G_Next_PreviousPanel;
    public GameObject CustomizeButton, PurchaseCarButton;
    private GameObject currentCarPrefab;
    [HideInInspector] public List<CarStatistics> carSlots;

    [Header("Customization")]
    public Button purchasedButton;
    public GameObject UpgradePanel;
    public List<Image> upgradeFill;
    public AudioClip MaxLevel;
    private GameObject CurrentWeaponPrefab;
    private WeaponData CurrentWeaponData;
    private WeaponHolder currentWeapon;
    private int currentWeaponIndex;
    private int currentweaponHolder;
    private bool Customization;
    private bool highlight;
    private GameObject storredHighlight;
    private Color OriginalColor;
    private MaterialHolder selectedmat;




    void Start()
    {
        PlayerPrefs.DeleteAll();
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
            AvailableMoneyText.text = AvailableMoney.ToString();
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
        carSpeedBar.value = data.Speed / 500;
        carArmorBar.value = data.Armor / 500;
        speedText.text = data.Speed.ToString();
        armorText.text = data.Armor.ToString();

        for (int i = 0; i < 3; i++)
        {
            CarDescriptionUI[i].SetActive(false);
            if (data.WeaponSlots[i] !="")
            {
                data.WeaponSlots[i] = data.WeaponSlots[i].Replace("\\n", "\n");
                CarDescriptionUI[i].SetActive(true);
                CarDescriptionUI[i].GetComponentInChildren<TextMeshProUGUI>().text = data.WeaponSlots[i];
            }
        }

    }
    public void PurchaseCar()
    {
        if (AvailableMoney >= cars[currentCar].GetComponent<CarStatistics>().Cost)
        {
            carSlots.Add(cars[currentCar].GetComponent<CarStatistics>());
            PurchaseCompleteUI.SetActive(true);
            GameObject PC = Instantiate(PurchaseComplete, PurchaseCompletePosition);
            PC.transform.SetParent(PurchaseCompletePosition, false);
            GameObject PS = Instantiate(PriceShow, priceShowPosition);
            PS.transform.SetParent(priceShowPosition, false);
            PS.GetComponent<TextMeshProUGUI>().text = "-$" + cars[currentCar].GetComponent<CarStatistics>().Cost.ToString();
            SavedCar car = new();
            car.CarIndex = cars[currentCar].GetComponent<CarStatistics>().GarrageSlot;
            car.color = cars[currentCar].GetComponent<CarStatistics>().SavedColor;
            SaveData.SaveCarData(car, (carSlots.Count -1).ToString());
            PlayerPrefs.SetInt("CarData", carSlots.Count);
            AvailableMoney -= cars[currentCar].GetComponent<CarStatistics>().Cost;
            DisplayMoney();
            AudioSource.PlayClipAtPoint(PurchaseAudio, transform.position);
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
        foreach (var item in currentCarPrefab.GetComponent<CarStatistics>().weaponHolders)
        {
            if (item.EquippedWeapon != null)
            {
                Destroy(item.InstantiatedWeapon);
            }
        }
        Destroy(currentCarPrefab);
    }
    public void InstantiateNewCar()
    {
        GameObject car = Instantiate(cars[carSlots[currentCar].GarrageSlot], cars[carSlots[currentCar].GarrageSlot].transform.position, cars[carSlots[currentCar].GarrageSlot].transform.rotation);
        SavedCar savedCarData = SaveData.LoadCarFile(currentCar.ToString());
        car.GetComponent<CarStatistics>().mat.color = savedCarData.color;
        car.GetComponent<CarStatistics>().SavedColor = savedCarData.color;
        car.SetActive(true);
        currentCarPrefab = car;
        LoadWeapons();
       
    }
    #endregion

    #region Customization
    public void OpenCustomization()
    {
        foreach (var item in currentCarPrefab.GetComponent<CarStatistics>().weaponHolders)
        {
            item.WeaponTransform.parent.gameObject.SetActive(true);
            if (item.EquippedWeapon != null)
            {
                item.WeaponTransform.parent.GetComponent<MeshRenderer>().enabled = false;
                item.WeaponTransform.parent.GetChild(3).GetComponent<MeshRenderer>().enabled = false;
                if (item.InstantiatedWeapon == null)
                {
                    item.InstantiatedWeapon = Instantiate(item.EquippedWeapon.WeaponPrefab, item.WeaponTransform.position, item.WeaponTransform.rotation);
                    item.InstantiatedWeapon.transform.localScale = item.WeaponTransform.localScale;
                    DisplayWeaponData(item.InstantiatedWeapon, item.EquippedWeapon);
                }
            }

        }
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
        if(currentCarPrefab.GetComponent<CarStatistics>().weaponHolders[selectedWeaponHolder].InstantiatedWeapon != null)
        {
            Destroy(currentCarPrefab.GetComponent<CarStatistics>().weaponHolders[selectedWeaponHolder].InstantiatedWeapon);
            currentWeaponIndex = currentCarPrefab.GetComponent<CarStatistics>().weaponHolders[selectedWeaponHolder].EquippedWeapon.weaponIndex;
        }
        else
        {
            currentWeaponIndex = 0;
        }
        CustomizeToWeaponShop(DestroyPrevious);
    }
    public void C_Next()
    {
        if (currentWeaponIndex < currentCarPrefab.GetComponent<CarStatistics>().weaponHolders[currentweaponHolder].AvailableWeapons.Count -1)
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
            currentWeaponIndex = currentCarPrefab.GetComponent<CarStatistics>().weaponHolders[currentweaponHolder].AvailableWeapons.Count -1;
            InstantiateNewWeapon();
        }
    }
    void InstantiateNewWeapon()
    {
        currentWeapon = currentCarPrefab.GetComponent<CarStatistics>().weaponHolders[currentweaponHolder];
        CurrentWeaponData = currentWeapon.AvailableWeapons[currentWeaponIndex];
        GameObject Weapon = Instantiate(CurrentWeaponData.WeaponPrefab, currentWeapon.WeaponTransform.position, currentWeapon.WeaponTransform.rotation);
        Weapon.transform.localScale = currentWeapon.WeaponTransform.localScale;
        CurrentWeaponPrefab = Weapon;
        DisplayWeaponData(Weapon, CurrentWeaponData);
    }
    public void PurchaseWeaponButton()
    {
        if(CurrentWeaponData.Purchased)
        {
            if(!CurrentWeaponData.Equipped)
            {
                EquipWeapon();
                DisplayWeaponData(CurrentWeaponPrefab, CurrentWeaponData);
                return;
            }
            UpgradeWeapon();
            return;
        }
        else
        {
            PurchaseWeapon();
        }
    }
    public void EquipWeapon()
    {
        foreach (var item in currentWeapon.AvailableWeapons)
        {
            item.Equipped = false;
        }
        CurrentWeaponData.Equipped = true;
        currentWeapon.EquippedWeapon = CurrentWeaponData;
        SaveWeapons();
    }
    public void PurchaseWeapon()
    {
        if (AvailableMoney >= CurrentWeaponData.Cost[0])
        {
            CurrentWeaponData.Purchased = true;
            EquipWeapon();
            currentWeapon.EquippedWeapon = CurrentWeaponData;
            SaveWeapons();
            GameObject PC = Instantiate(PurchaseComplete, PurchaseCompletePosition);
            PC.transform.SetParent(PurchaseCompletePosition, false);
            GameObject PS = Instantiate(PriceShow, priceShowPosition);
            PS.transform.SetParent(priceShowPosition, false);
            PS.GetComponent<TextMeshProUGUI>().text = "-$" + CurrentWeaponData.Cost[0].ToString();
            AvailableMoney -= CurrentWeaponData.Cost[0];
            DisplayMoney();
            DisplayWeaponData(CurrentWeaponPrefab, CurrentWeaponData);
            AudioSource.PlayClipAtPoint(PurchaseAudio, transform.position, 0.3f);
        }
        else
        {
            AudioSource.PlayClipAtPoint(NoMoney, transform.position);
        }
    }
    public void UpgradeWeapon()
    {
        if (AvailableMoney >= CurrentWeaponData.Cost[CurrentWeaponData.Level])
        {
            GameObject PC = Instantiate(PurchaseComplete, PurchaseCompletePosition);
            PC.transform.SetParent(PurchaseCompletePosition, false);
            GameObject PS = Instantiate(PriceShow, priceShowPosition);
            PS.transform.SetParent(priceShowPosition, false);
            PS.GetComponent<TextMeshProUGUI>().text = "-$" + CurrentWeaponData.Cost[CurrentWeaponData.Level+1].ToString();
            AvailableMoney -= CurrentWeaponData.Cost[CurrentWeaponData.Level+1];
            DisplayMoney();
            CurrentWeaponData.Level++;
            DisplayWeaponData(CurrentWeaponPrefab, CurrentWeaponData);
            SaveWeapons();
            if(CurrentWeaponData.Level == 4)
            {
                AudioSource.PlayClipAtPoint(MaxLevel, transform.position);
            }

        }
    }

    public void LoadWeapons()
    {
        foreach (var item in currentCarPrefab.GetComponent<CarStatistics>().weaponHolders)
        {
            for (int i = 0; i < item.AvailableWeapons.Count; i++)
            {
                WeaponData newData = Instantiate(item.AvailableWeapons[i]);
                if (PlayerPrefs.HasKey((item.WeaponHolderIndex + " " + currentCar).ToString()))
                {
                    List<SavedWeapon> data = SaveData.LoadWeaponFile((item.WeaponHolderIndex + " " + currentCar).ToString());
                    foreach (var savedWeapon in data)
                    {
                        if (savedWeapon.Name_ID == item.AvailableWeapons[i].Name_ID)
                        {
                            newData.Level = savedWeapon.Level;
                            newData.Purchased = savedWeapon.Purchased;
                            newData.Equipped = savedWeapon.Equipped;
                        }
                    }
                    if (newData.Equipped)
                    {
                        item.EquippedWeapon = newData;
                        item.WeaponTransform.parent.GetComponent<MeshRenderer>().enabled = false;
                        item.WeaponTransform.parent.GetChild(3).GetComponent<MeshRenderer>().enabled = false;
                        item.InstantiatedWeapon = Instantiate(newData.WeaponPrefab, item.WeaponTransform.position, item.WeaponTransform.rotation);
                        item.InstantiatedWeapon.transform.localScale = item.WeaponTransform.localScale;
                        DisplayWeaponData(item.InstantiatedWeapon, newData);
                    }
                }
                item.AvailableWeapons[i] = newData;
            }
        }
    }
    public void SaveWeapons()
    {
        foreach (var weaponHolder in currentCarPrefab.GetComponent<CarStatistics>().weaponHolders)
        {
            List<SavedWeapon> savedWeps = new();
            foreach (var item in weaponHolder.AvailableWeapons)
            {
                SavedWeapon savedWeapon = new();
                savedWeapon.Name_ID = item.Name_ID;
                savedWeapon.Purchased = item.Purchased;
                savedWeapon.Level = item.Level;
                savedWeapon.Equipped = item.Equipped;
                savedWeapon.WeaponHolderIndex = weaponHolder.WeaponHolderIndex;
                savedWeps.Add(savedWeapon);
            }
            SaveData.SaveCarWeaponData(savedWeps, (weaponHolder.WeaponHolderIndex + " " + currentCar).ToString());
        }
    }
    public void DisplayWeaponData(GameObject Prefab, WeaponData data)
    {
        for (int i = 0; i < Prefab.transform.childCount; i++)
        {
            Prefab.transform.GetChild(i).gameObject.SetActive(false);
        }

        if (!data.Equipped && data.Purchased)
        {
            WeaponCostUI.text = "";
            WeaponPurchaseUI.text = "Equip";
            UpgradePanel.SetActive(false);
            purchasedButton.interactable = true;
            WeaponNameUI.text = data.Name_ID;
            weaponDpsBar.value = (data.DPS[data.Level] / 500f);
            weaponArmorBar.value = (data.Armor[data.Level] / 500f);
            weapondArmorUpgradeBar.value = 0;
            weapondDpsUpgradeBar.value = 0;
            weaponDPSText.text = data.DPS[data.Level].ToString();
            weaponArmorText.text = data.Armor[data.Level].ToString();
            WeaponSpecialUI.text = data.SpecialEffect[data.Level];
            Prefab.transform.GetChild(data.Level).gameObject.SetActive(true);
            return;
        }
        if (!data.Purchased)
        {
            WeaponPurchaseUI.text = "Purchase";
            UpgradePanel.SetActive(true);
            purchasedButton.interactable = true;
            WeaponNameUI.text = data.Name_ID;
            WeaponCostUI.text = ("$" + data.Cost[0]).ToString();
            weaponDpsBar.value = (data.DPS[0] / 500f);
            weaponArmorBar.value = (data.Armor[0] / 500f);
            weapondArmorUpgradeBar.value = 0;
            weapondDpsUpgradeBar.value = 0;
            weaponDPSText.text = data.DPS[data.Level].ToString();
            weaponArmorText.text = data.Armor[data.Level].ToString();
            WeaponSpecialUI.text = data.SpecialEffect[0];
            Prefab.transform.GetChild(0).gameObject.SetActive(true);
            foreach (var item in upgradeFill)
            {
                item.color = Color.grey;
            }
            upgradeFill[0].color = Color.red;
            return;
        }
        if(data.Purchased && data.Level < 4 && WeaponShopUIPanel.activeInHierarchy)
        {
            WeaponPurchaseUI.text = "Upgrade";
            UpgradePanel.SetActive(true);
            purchasedButton.interactable = true;
            WeaponNameUI.text = data.Name_ID;
            WeaponCostUI.text = ("$" + data.Cost[data.Level+1]).ToString();
            weaponDpsBar.value = (data.DPS[data.Level] / 500f);
            weaponArmorBar.value = (data.Armor[data.Level + 1] / 500f);
            weapondDpsUpgradeBar.value = (data.DPS[data.Level + 1] / 500f);
            weapondArmorUpgradeBar.value = (data.Armor[data.Level + 1] / 500f);
            weaponDPSText.text = data.DPS[data.Level] + " -> " + "<color=green> " + data.DPS[data.Level + 1] + "</color>";
            weaponArmorText.text = data.Armor[data.Level] + " -> " + "<color=green> " + data.Armor[data.Level + 1] + "</color>";
            WeaponSpecialUI.text = data.SpecialEffect[data.Level + 1];
            Prefab.transform.GetChild(data.Level + 1).gameObject.SetActive(true);
            foreach (var item in upgradeFill)
            {
                item.color = Color.grey;
            }
            for (int i = 0; i < data.Level+1; i++)
            {
                upgradeFill[i].color = Color.green;
            }
            upgradeFill[data.Level+1].color = Color.red;
        }
        if(data.Equipped && !WeaponShopUIPanel.activeInHierarchy)
        {
            Prefab.transform.GetChild(data.Level).gameObject.SetActive(true);
        }
        if (data.Purchased && data.Level == 4)
        {
            Prefab.transform.GetChild(data.Level).gameObject.SetActive(true);
            UpgradePanel.SetActive(true);
            purchasedButton.interactable = false;
            WeaponNameUI.text = data.Name_ID;
            WeaponCostUI.text = "Max Level";
            WeaponPurchaseUI.text = "Max Level";
            weaponDpsBar.value = (data.DPS[data.Level] / 500f);
            weaponArmorBar.value = (data.Armor[data.Level] / 500f);
            weaponDPSText.text = data.DPS[data.Level].ToString();
            weaponArmorText.text = data.Armor[data.Level].ToString();
            WeaponSpecialUI.text = data.SpecialEffect[data.Level];
            weapondArmorUpgradeBar.value = 0;
            weapondDpsUpgradeBar.value = 0;
            foreach (var item in upgradeFill)
            {
                item.color = Color.green;
            }
        }

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
                car.CarIndex = carSlots[currentCar].GarrageSlot;
                car.color = selectedmat.myMat.color;
                SaveData.SaveCarData(car, currentCar.ToString());
                GameObject PC = Instantiate(PurchaseComplete, PurchaseCompletePosition);
                PC.transform.SetParent(PurchaseCompletePosition, false);
                GameObject PS = Instantiate(PriceShow, priceShowPosition);
                PS.transform.SetParent(priceShowPosition, false);
                PS.GetComponent<TextMeshProUGUI>().text = "-$" + selectedmat.Cost.ToString();
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
            foreach (var item in currentCarPrefab.GetComponent<CarStatistics>().weaponHolders)
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
        foreach (var item in currentCarPrefab.GetComponent<CarStatistics>().weaponHolders)
        {
            item.WeaponTransform.parent.gameObject.SetActive(false);
        }
        Customization = false;
    }
    public void CustomizeToWeaponShop(bool Destroyprevious)
    {
        CustomizeUIPanel.SetActive(false);
        WeaponShopUIPanel.SetActive(true);
        if (currentWeapon != null && Destroyprevious)
        {
            Destroy(currentWeapon.InstantiatedWeapon);
        }
        InstantiateNewWeapon();
        foreach (var item in currentCarPrefab.GetComponent<CarStatistics>().weaponHolders)
        {
            item.WeaponTransform.parent.gameObject.SetActive(false);
        }
        PreviousPosition = cam.transform.position;
        LeanTween.move(cam.gameObject, currentWeapon.WeaponCamTransform.position, TransitionTime);
        LeanTween.rotate(cam.gameObject, currentWeapon.WeaponCamTransform.rotation.eulerAngles, TransitionTime);
        DisplayWeaponData(CurrentWeaponPrefab, CurrentWeaponData);
        cam.GetComponent<CameraCustomization>().enabled = false;
        Customization = false;
    }
    public void WeaponShopToCustomize()
    {
        WeaponShopUIPanel.SetActive(false);
        CustomizeUIPanel.SetActive(true);
        LeanTween.move(cam.gameObject, PreviousPosition, TransitionTime);
        LeanTween.rotate(cam.gameObject, Cam_Pos_Customize.rotation.eulerAngles, TransitionTime);
        Destroy(CurrentWeaponPrefab);
        cam.GetComponent<CameraCustomization>().enabled = true;
        OpenCustomization();
    }
    public void CustomizeToColorShop()
    {
        CustomizeUIPanel.SetActive(false);
        ColorShopUIPanel.SetActive(true);
        LeanTween.move(cam.gameObject, Cam_Pos_Select, TransitionTime);
        LeanTween.rotate(cam.gameObject, Cam_Pos_Select.rotation.eulerAngles, TransitionTime);
        foreach (var item in currentCarPrefab.GetComponent<CarStatistics>().weaponHolders)
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
    public void Mute_UnMute()
    {
        if(!muted)
        {
            Debug.Log("Unmuted");
            MuteButton.sprite = Mute;
            BG_Music.volume = 0;
            muted = true;
        }
        else
        {
            Debug.Log("Muted");
            MuteButton.sprite = UnMute;
            BG_Music.volume = 0.15f;
            muted = false;
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
    #endregion
}
