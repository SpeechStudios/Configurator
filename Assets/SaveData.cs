using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SavedWeapon
{
    public string WeaponName;
    public int WeaponSelectIndex;
}

[System.Serializable]
public class JsonableListWrapper<SavedWeapon>
{
    public List<SavedWeapon> list;
    public JsonableListWrapper(List<SavedWeapon> list) => this.list = list;
}

public class SaveData : MonoBehaviour
{
    public static void SaveCarData(CarData savedData, string slot)
    {
        List<SavedWeapon> weaponNames = new();
        foreach (var item in savedData.weapons)
        {
            if (item.PurchasedWeapon)
            {
                SavedWeapon saveWep = new();
                saveWep.WeaponName = item.PurchasedWeapon.GetComponent<WeaponData>().Name;
                saveWep.WeaponSelectIndex = item.WeaponSelectIndex;
                weaponNames.Add(saveWep);
            }
        }
        string json = JsonUtility.ToJson(new JsonableListWrapper<SavedWeapon>(weaponNames));
        PlayerPrefs.SetString(slot, json);
    }
    public static List<SavedWeapon> LoadFile(string slot)
    {
        string json = PlayerPrefs.GetString(slot);
        List<SavedWeapon> loadedData = JsonUtility.FromJson<JsonableListWrapper<SavedWeapon>>(json).list;
        return loadedData;
    }
}
