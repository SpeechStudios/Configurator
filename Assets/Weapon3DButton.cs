using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Weapon3DButton : MonoBehaviour
{
    public UnityEvent weaponSelect = new UnityEvent();

    void OnMouseUpAsButton()
    {
        weaponSelect.Invoke();
    }
}
