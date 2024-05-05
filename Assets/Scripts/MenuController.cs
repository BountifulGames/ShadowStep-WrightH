using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject mainMenuBackground;
    [SerializeField] private GameObject instructionsBackground;

    // Code for running the main menu UI
    public void OnPlayButtonPress()
    {
        SceneManager.LoadScene("Level1");
    }

    public void OnInstructionsButtonPress()
    {
        mainMenuBackground.SetActive(false);
        instructionsBackground.SetActive(true);
    }

    public void OnQuitButtonPress()
    {
        Application.Quit();
    }

    public void OnBackButtonPress()
    {
        instructionsBackground.SetActive(false);
        mainMenuBackground.SetActive(true);
    }
}
