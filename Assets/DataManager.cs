using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    public List<GameObject> weapons;
    public Dictionary<string, GameObject> _cache;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        _cache = weapons.ToDictionary(wep => wep.GetComponent<WeaponData>().Name, wep => wep);
    }

    public GameObject GetWeaponPrefabByName(string Name)
    {
        if (_cache.TryGetValue(Name, out GameObject wep))
        {
            return wep;
        }
        return null;
    }
    // Update is called once per frame
}