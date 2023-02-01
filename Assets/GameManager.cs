using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Transform Cam_Pos_Main;
    public Transform Cam_Pos_Select;
    public Transform Cam_Pos_Customize;
    public List<GameObject> cars;

    [Header("UI")]
    public GameObject MenuUIPanel;
    public TextMeshProUGUI carCost;
    public TextMeshProUGUI carDescription;
    public GameObject PurchaseCompleteUI;


    private GameObject CurrentUIPanel;
    private Camera cam;
    private int currentCar;

    [Header("Garage")]
    public Transform carSlotTransform;
    public GameObject Next_PreviousPanel;
    public GameObject CustomizeButton, PurchaseCarButton;

    [HideInInspector] public List<CarData> carSlots;
    private GameObject currentCarPrefab;
    void Start()
    {
        CurrentUIPanel = MenuUIPanel;
        cam = Camera.main;
    }

    #region CarShop
    public void CarShopButton(GameObject CarShopUIPanel)
    {
        cars[0].SetActive(true);
        currentCar = 0;
        LeanTween.move(cam.gameObject, Cam_Pos_Select, 0.5f);
        LeanTween.rotate(cam.gameObject, Cam_Pos_Select.rotation.eulerAngles, 0.5f);
        CurrentUIPanel.SetActive(false);
        CarShopUIPanel.SetActive(true);
        CurrentUIPanel = CarShopUIPanel;
        DisplayCarData();
    }
    public void S_Next()
    {
        if(currentCar < cars.Count -1)
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
            currentCar = cars.Count-1;
            cars[currentCar].SetActive(true);
        }
        DisplayCarData();
    }
    public void PurchaseCar()
    {
        carSlots.Add(cars[currentCar].GetComponent<CarData>());
        PurchaseCompleteUI.SetActive(true);
    }
    public void ContinueShopping()
    {
        PurchaseCompleteUI.SetActive(false);
    }
    public void CustomizeCar(GameObject CustomizeUIPanel)
    {
        cars[currentCar].SetActive(false);
        PurchaseCompleteUI.SetActive(false);
        CurrentUIPanel.SetActive(false);
        CustomizeUIPanel.SetActive(true);
        CurrentUIPanel = CustomizeUIPanel;
        LeanTween.move(cam.gameObject, Cam_Pos_Customize, 0.5f);
        LeanTween.rotate(cam.gameObject, Cam_Pos_Customize.rotation.eulerAngles, 0.5f);
        OpenGarrage(carSlots.Count-1);

    }
    public void DisplayCarData()
    {
        carCost.text = "$" + cars[currentCar].GetComponent<CarData>().Cost.ToString();
        carDescription.text = cars[currentCar].GetComponent<CarData>().Description;
    }
    #endregion

    #region Garage
    public void GarrageShopButton(GameObject GarrageUIPanel)
    {
        currentCar = 0;
        LeanTween.move(cam.gameObject, Cam_Pos_Select, 0.5f);
        LeanTween.rotate(cam.gameObject, Cam_Pos_Select.rotation.eulerAngles, 0.5f);
        CurrentUIPanel.SetActive(false);
        GarrageUIPanel.SetActive(true);
        CurrentUIPanel = GarrageUIPanel;
        OpenGarrage(currentCar);
    }
    public void OpenGarrage(int carChoice)
    {
        if (carSlots.Count == 0)
        {
            Next_PreviousPanel.SetActive(false);
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
            Next_PreviousPanel.SetActive(false);
        }
        else
        {
            Next_PreviousPanel.SetActive(true);
        }
    }
    public void G_Next()
    {
        if (currentCar < carSlots.Count - 1)
        {
            Destroy(currentCarPrefab);
            currentCar++;
            InstantiateNewCar();
        }
        else
        {
            Destroy(currentCarPrefab);
            currentCar = 0;
            InstantiateNewCar();
        }
    }
    public void G_Previous()
    {
        if (currentCar > 0)
        {
            Destroy(currentCarPrefab);
            currentCar--;
            InstantiateNewCar();
        }
        else
        {
            Destroy(currentCarPrefab);
            currentCar = carSlots.Count - 1;
            InstantiateNewCar();
        }
    }
    public void InstantiateNewCar()
    {
        GameObject car = Instantiate(cars[carSlots[currentCar].Index], carSlotTransform.position, carSlotTransform.rotation);
        car.SetActive(true);
        currentCarPrefab = car;
    }
    #endregion
    public void Menu()
    {
        CurrentUIPanel.SetActive(false);
        MenuUIPanel.SetActive(true);
        if(cars[currentCar].activeInHierarchy)
            cars[currentCar].SetActive(false);
        if(currentCarPrefab)
            Destroy(currentCarPrefab);
        LeanTween.move(cam.gameObject, Cam_Pos_Main, 0.5f);
        LeanTween.rotate(cam.gameObject, Cam_Pos_Main.rotation.eulerAngles, 0.5f);
        CurrentUIPanel = MenuUIPanel;
    }

}
