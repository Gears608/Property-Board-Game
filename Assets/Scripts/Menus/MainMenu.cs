using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private float buffer = 1f;

    /// <summary>
    /// Switching scenes after a buffer time, so that the user listen to the button SFX
    /// for immersion.
    /// </summary>      
    private IEnumerator BufferedLoadScene(string scene)
    {
        yield return new WaitForSecondsRealtime(buffer);
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }

    /// <summary>
    /// Start the game by loading to gameScene.  
    /// by OnClick functions in Unity Engine
    /// </summary>
    public void PlayGame()
    {
        StartCoroutine(BufferedLoadScene("gameScene"));
    }

    /// <summary>
    /// Navigate to rules menu.
    /// </summary>
    public void RuleMenu()
    {
        StartCoroutine(BufferedLoadScene("Rules"));
    }

    /// <summary>
    /// Navigate back to main menu.
    /// </summary>
    public void BackMainMenu()
    {
        StartCoroutine(BufferedLoadScene("MainMenu"));
    }


    /// <summary>
    /// Quit the game. Also will log a message if succeeds.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("exited game");
    }

}
