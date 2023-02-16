using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PurchaseComplete : MonoBehaviour
{
    Vector3 startPos;
    public float Speed = 1;
    void OnEnable()
    {
        startPos = transform.position;
        StopCoroutine("UpAndFade");
        StartCoroutine(nameof(UpAndFade));
    }
    public IEnumerator UpAndFade()
    {
        gameObject.SetActive(true);
        for (float f = 1; f > -0.05; f-=0.05f)
        {
            transform.position += Vector3.up * Speed;
            Color c = GetComponent<TextMeshProUGUI>().color;
            c.a = f;
            GetComponent<TextMeshProUGUI>().color = c;
            yield return new WaitForSeconds(0.05f);
        }
        gameObject.SetActive(false);
        transform.position = startPos;
        Color d = GetComponent<TextMeshProUGUI>().color;
        d.a = 1;
        GetComponent<TextMeshProUGUI>().color = d;
    }

}
