using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VolController : MonoBehaviour
{
    [Header("Volume Setting")]
    [SerializeField] private TMP_Text volumeTextValue = null;
    [SerializeField] private Slider volumeSlider = null;
    

    [Header("Sound Symbols")]
    [SerializeField] private GameObject soundSymbol;
    [SerializeField] private GameObject muteSoundSymbol;


    /// <summary>
    /// Initialise the default values of all sounds, 
    /// if it is not the default it will be the current volume.
    /// </summary>
    void Start()
    {
        if (AudioListener.volume != 1.0f)
        {
            SetVolume(AudioListener.volume);
            volumeSlider.value = AudioListener.volume;
        }
        else
        {
            AudioListener.volume = 1.0f;
            volumeTextValue.text = "1.0";
            volumeSlider.value = 1.0f;

        }
    }

    /// <summary>
    /// Set the overall sound volume in game. Enable mute sound symbol if volume is 0.0
    /// </summary>
    /// <param name="volume"> The volume of sound in game. </param>
    private void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        volumeTextValue.text = volume.ToString("0.0");

        if (volume == 0.0)
        {
            muteSoundSymbol.SetActive(true);
            soundSymbol.SetActive(false);
        }
        else
        {
            soundSymbol.SetActive(true);
            muteSoundSymbol.SetActive(false);
        }
    }


}
