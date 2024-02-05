using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public Button ContinueButton;

    private void Start()
    {
        SetContinueButtonState();
    }

    /// <summary>
    /// Sets continue button's interactibility according to existence of the saved game
    /// </summary>
    private void SetContinueButtonState()
    {
        if (GameDataSingleton.Instance.IsGameSaved)
        {
            ContinueButton.interactable = true;
        }
        else if (!GameDataSingleton.Instance.IsGameSaved)
        {
            ContinueButton.interactable = false;
        }
    }

    public void OnClickContinueButton()
    {
        if(GameDataSingleton.Instance.IsGameSaved)
        {
            Debug.Log("Game is continuing...");
            SceneManager.LoadScene("InGameScene");
        }      
    }
}
