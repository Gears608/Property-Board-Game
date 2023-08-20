using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSFX : MonoBehaviour
{
    [SerializeField] private AudioSource aFX;
    [SerializeField] private AudioClip hoverFX;
    [SerializeField] private AudioClip clickFX;

    /// <summary>
    /// Set the audioclip to be played whencursor hovers on top of the button.
    /// The sound plays once.
    /// </summary>
    public void HoverSound()
    {
        aFX.PlayOneShot(hoverFX);

    }

    /// <summary>
    /// Set the audioclip to be played whencursor clicks the button.
    /// The sound plays once
    /// </summary>
    public void ClickSound()
    {
        aFX.PlayOneShot(clickFX);
    }


}
