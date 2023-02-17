using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class ScreenShake : MonoBehaviour
{
    public PostProcessVolume volume;
    ChromaticAberration chrome;

    private void Start()
    {
        volume.profile.TryGetSettings(out chrome);
        StartCoroutine(nameof(ScreenChrome));
    }
    IEnumerator ScreenChrome()
    {
        while (true)
        {
            for (float f = 0; f < 1.05f; f += 0.003f)
            {
                chrome.intensity.value = f;
                yield return new WaitForSeconds(0.01f);
            }
            yield return new WaitForSeconds(0.1f);
            for (float f = 1; f > -0.05f; f -= 0.003f)
            {
                chrome.intensity.value = f;
                yield return new WaitForSeconds(0.05f);
            }
            chrome.intensity.value = 0f;
        }
    }
}
