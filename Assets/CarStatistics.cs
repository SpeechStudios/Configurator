using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CarStatistics : MonoBehaviour
{
    public string Name;
    public int Index;
    public int Cost;
    public string Description;
    public Material mat;
    public Color SavedColor;
    public CarData carData;
}
[System.Serializable]
public class CarData
{
    public List<Weapon> weapons;
}

[System.Serializable]
public class Weapon
{
    public List<GameObject> availableWeapons;
    public Transform WeaponTransform;
    public Transform WeaponCamTransform;
    public GameObject PurchasedWeapon;
    [HideInInspector] public GameObject InstantiatedWeapon;
    public int WeaponSelectIndex;
}

