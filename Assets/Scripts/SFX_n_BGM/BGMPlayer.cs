using System.Collections;
using System;
using UnityEngine;
using Unity.Audio;

public class BGMPlayer : MonoBehaviour
{
    public static BGMPlayer instance;
    private bool isPlay;
    [SerializeField] private AudioClip bgm1;
    [SerializeField] private AudioClip bgm2;
    [SerializeField] private AudioSource audioSource;


    /// <summary>
    /// Set BGM GameObject as an instance that will not be removed upon scene changing,
    /// but will remove the instance if there are one more instance existed.
    /// Execute the function once the game is in the Awake() state.
    /// </summary>
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Set the background music rotation when game is running. It consists of 2 music pieces.
    /// </summary>
    /// <returns>null while</returns>
    public IEnumerator BGMRotation()
    {
        while (isPlay == true)
        {
            audioSource.PlayOneShot(bgm1);
            yield return new WaitForSeconds(bgm1.length);
            audioSource.PlayOneShot(bgm2);
            yield return new WaitForSeconds(bgm2.length + 1.0f); // +1 second for better transition
        }
    }

    /// <summary>
    /// Called once at the start of the scene.
    /// </summary>
    void Start()
    {
        isPlay = true;
        StartCoroutine(BGMRotation());
    }

    /// <summary>
    /// Get the current volume value of music in audio source.
    /// </summary>
    /// <returns>Current Volume of music</returns>
    private float GetBGMVolume()
    {
        return audioSource.volume;
    }
    
}
