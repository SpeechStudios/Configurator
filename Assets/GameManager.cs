using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject MenuUIPanel;
    public Transform Cam_Pos_Select;
    public List<GameObject> cars;

    [Header("UI")]
    public TextMeshProUGUI carCost;
    public TextMeshProUGUI carDescription;


    private GameObject CurrentUIPanel;
    private Camera cam;
    private int currentCar;
    void Start()
    {
        CurrentUIPanel = MenuUIPanel;
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        
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
    public void Next()
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
    public void Previous()
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
    public void DisplayCarData()
    {
        carCost.text = "$" + cars[currentCar].GetComponent<CarData>().Cost.ToString();
        carDescription.text = cars[currentCar].GetComponent<CarData>().Description;
    }
    #endregion

}
