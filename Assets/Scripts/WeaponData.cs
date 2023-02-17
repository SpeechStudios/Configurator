using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon")]
public class WeaponData : ScriptableObject
{
    public string Name_ID;
    public GameObject WeaponPrefab;
    public int weaponIndex;

    public int[] Cost;
    public float[] DPS;
    public float[] Armor;
    public string[] SpecialEffect;

    [HideInInspector] public int Level;
    [HideInInspector] public bool Purchased;
    [HideInInspector] public bool Equipped;
}
