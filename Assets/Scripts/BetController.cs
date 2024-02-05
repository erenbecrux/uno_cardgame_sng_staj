using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BetController : MonoBehaviour
{
    public Slider BetSlider;
    public TextMeshProUGUI BetText;

    private void Start()
    {
        // Initialize bet slider values and bet text
        BetSlider.minValue = Constants.MinimumBet;
        BetSlider.maxValue = GameDataSingleton.Instance.PlayerMoney;
        SetBetText();
    }

    /// <summary>
    /// Sets bet text to current value of the bet slider
    /// </summary>
    public void SetBetText()
    {
        BetText.text = "Current Bet: " + BetSlider.value;
    }

    /// <summary>
    /// Applies current bet amount
    /// </summary>
    private void SetCurrentBet()
    {
        GameDataSingleton.Instance.BetAmount = BetSlider.value;
    }

    /// <summary>
    /// Applies current bet and starts new game
    /// </summary>
    public void OnClickBetButton()
    {
        Debug.Log("Game is starting...");
        SetCurrentBet();
        GameDataSingleton.Instance.ApplyCurrentBet();
        GameDataSingleton.Instance.ResetPoints();
        GameDataSingleton.Instance.IsNewGame = true;
        SceneManager.LoadScene("InGameScene");
    }
}
