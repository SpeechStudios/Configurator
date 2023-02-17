using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SavedWeapon
{
    public string Name_ID;
    public int WeaponHolderIndex;
    public bool Purchased;
    public bool Equipped;
    public int Level;
}
[System.Serializable]
public class SavedCar
{
    public int CarIndex;
    public Color color;
}


[System.Serializable]
public class JsonableListWrapper<SavedWeapon>
{
    public List<SavedWeapon> list;
    public JsonableListWrapper(List<SavedWeapon> list) => this.list = list;
}

public class SaveData : MonoBehaviour
{
    public static void SaveCarWeaponData(List<SavedWeapon> savedData, string slot)
    {
        string json = JsonUtility.ToJson(new JsonableListWrapper<SavedWeapon>(savedData));
        PlayerPrefs.SetString(slot, json);
    }
    public static List<SavedWeapon> LoadWeaponFile(string slot)
    {
        string json = PlayerPrefs.GetString(slot);
        List<SavedWeapon> loadedData = JsonUtility.FromJson<JsonableListWrapper<SavedWeapon>>(json).list;
        return loadedData;
    }

    public static void SaveCarData(SavedCar carData, string slot)
    {
        string json = JsonUtility.ToJson(carData);
        PlayerPrefs.SetString(slot, json);
    }
    public static SavedCar LoadCarFile(string slot)
    {
        string json = PlayerPrefs.GetString(slot);
        SavedCar loadedData = JsonUtility.FromJson<SavedCar>(json);
        return loadedData;
    }
}
