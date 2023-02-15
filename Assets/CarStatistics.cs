using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CarStatistics : MonoBehaviour
{
    public string Name;
    public int GarrageSlot;
    public int Cost;
    public string[] WeaponSlots;
    public float Speed;
    public float Armor;
    public Material mat;
    public Color SavedColor;
    public List<WeaponHolder> weaponHolders;
}

[System.Serializable]
public class WeaponHolder
{
    public List<WeaponData> AvailableWeapons;
    public Transform WeaponTransform;
    public Transform WeaponCamTransform;
    [HideInInspector] public WeaponData EquippedWeapon;
    [HideInInspector] public GameObject InstantiatedWeapon;
    public int WeaponHolderIndex;
}

