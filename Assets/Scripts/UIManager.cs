using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public RoundEndUIController RoundEndUIController;
    public SettingsUIController SettingsUIController;
    public InGameUIController InGameUIController;

    public InterstitialAds InterstitialAds;

    public GameObject Background;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // Sets background to current theme in game data
        Background.GetComponent<SpriteRenderer>().sprite = GameDataSingleton.Instance.CurrentTheme.BackgroundSprite;

        // Sets main menu pop up button state
        InGameUIController.MenuButton.interactable = GameDataSingleton.Instance.IsTableSet;
    }

    /// <summary>
    /// Restarts the game
    /// </summary>
    public void OnClickRestart()
    {
        GameDataSingleton.Instance.IsNewGame = true;
        GameManager.Instance.RestartGame();
    }

    /// <summary>
    /// Loads the main menu scene and kills all tweens without resetting current points
    /// </summary>
    public void OnClickMainMenuButton()
    {
        Debug.Log("Main menu is loading...");
        GameManager.Instance.SaveCardIDs();
        GameManager.Instance.SaveGameState();
        GameDataSingleton.Instance.IsGameSaved = true;
        GameDataSingleton.Instance.IsNewGame = false;
        DOTween.KillAll();

        // for testing...
        SceneManager.LoadScene("MainMenuScene");
        //StartCoroutine(InterstitialAds.ShowInterstitialAdAfterWaiting(0.5f));
    }

    /// <summary>
    /// Loads main menu after round end, saves current points and set booleans to creat new table for next round
    /// </summary>
    public void OnClickMainMenuAtRoundEnd()
    {
        Debug.Log("Main menu is loading...");
        GameDataSingleton.Instance.IsGameSaved = true;
        GameDataSingleton.Instance.IsNewGame = true;
        SceneManager.LoadScene("MainMenuScene");
    }

    /// <summary>
    /// Loads the main menu after game end, resets current points
    /// </summary>
    public void OnClickMainMenuAtGameEnd()
    {
        Debug.Log("Main menu is loading...");
        GameDataSingleton.Instance.IsGameSaved = false;
        GameDataSingleton.Instance.IsNewGame = true;
        GameDataSingleton.Instance.ResetPoints();
        SceneManager.LoadScene("MainMenuScene");
    }
}
