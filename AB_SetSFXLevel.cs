using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AB_SetSFXLevel : MonoBehaviour
{
    public AudioMixer mixer;

    public void SetVol(float sliderValue)
    {
        mixer.SetFloat("SFXVol", Mathf.Log10(sliderValue) * 20);
    }
}
