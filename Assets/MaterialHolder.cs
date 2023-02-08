using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaterialHolder : MonoBehaviour
{
    public Material myMat;
    public int Cost;
    private void Start()
    {
        GetComponent<Image>().color = myMat.color;
    }
}
