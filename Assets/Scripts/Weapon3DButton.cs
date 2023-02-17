using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Weapon3DButton : MonoBehaviour
{
    public UnityEvent weaponSelect = new UnityEvent();
    public AudioSource Hover;
    bool highlight;

    void OnMouseUpAsButton()
    {
        weaponSelect.Invoke();
    }
    private void OnMouseOver()
    {
        if(!highlight)
        {
            Color c = GetComponentInParent<MeshRenderer>().material.color;
            c.a = 0.2f;
            GetComponentInParent<MeshRenderer>().material.color = c;
            Hover.Play();
            highlight = true;
        }
    }
    private void OnMouseExit()
    {
        Color c = GetComponentInParent<MeshRenderer>().material.color;
        c.a = 0.75f;
        GetComponentInParent<MeshRenderer>().material.color = c;
        highlight = false;
    }

}
