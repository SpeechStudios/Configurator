using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpPitch : MonoBehaviour
{
    public float desiredPitch;
    public float OriginalVolume;
    public bool LerpUp;
    private AudioSource aud;
    public GameManager gm;

    // Update is called once per frame
    private void OnEnable()
    {
        aud = GetComponent<AudioSource>();
        StartCoroutine(nameof(NewLerp));
    }
    private void Update()
    {
        if (!gm.CarShopUIPanel.activeInHierarchy)
        {
            aud.volume = 0.05f;
        }
        else
        {
            aud.volume = OriginalVolume;
        }
    }
    IEnumerator NewLerp()
    {
        if (LerpUp)
        {
            for (float f = desiredPitch - 2; f < desiredPitch; f += 0.035f)
            {
                aud.pitch = f;
                yield return new WaitForSeconds(0.01f);
            }
        }
        else
        {
            for (float f = desiredPitch + 2; f > desiredPitch; f -= 0.035f)
            {
                aud.pitch = f;
                yield return new WaitForSeconds(0.01f);
            }
        }
    }
}
