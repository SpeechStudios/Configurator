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
public class SavedCar
{
    public int CarIndex;
}


[System.Serializable]
public class JsonableListWrapper<SavedWeapon>
{
    public List<SavedWeapon> list;
    public JsonableListWrapper(List<SavedWeapon> list) => this.list = list;
}

public class SaveData : MonoBehaviour
{
    public static void SaveCarWeaponData(CarData savedData, string slot)
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
    public static List<SavedWeapon> LoadWeaponFile(string slot)
    {
        string json = PlayerPrefs.GetString(slot);
        List<SavedWeapon> loadedData = JsonUtility.FromJson<JsonableListWrapper<SavedWeapon>>(json).list;
        return loadedData;
    }

    public static void SaveCarData(List<CarStatistics> savedData)
    {
        List<SavedCar> carData = new();
        foreach (var item in savedData)
        {
            SavedCar savedCar = new();
            savedCar.CarIndex = item.Index;
            carData.Add(savedCar);
        }
        string json = JsonUtility.ToJson(new JsonableListWrapper<SavedCar>(carData));
        PlayerPrefs.SetString("CarData", json);
    }
    public static List<SavedCar> LoadCarFile(string slot)
    {
        string json = PlayerPrefs.GetString(slot);
        List<SavedCar> loadedData = JsonUtility.FromJson<JsonableListWrapper<SavedCar>>(json).list;
        return loadedData;
    }
}
