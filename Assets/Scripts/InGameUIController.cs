using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIController : MonoBehaviour
{
    public Canvas DrawnCardPopUpCanvas;
    public Canvas WildCardPopUpCanvas;
    public Button MenuButton;
    public Slider PlayerTimer;

    public GameObject UnoPanel;
    public GameObject YourTurnPanel;
    public GameObject GameRotationImage;

    public TextMeshProUGUI MoneyText;
    public TextMeshProUGUI BetText;

    private void Start()
    {
        MoneyText.text = GameDataSingleton.Instance.PlayerMoney.ToString();
        BetText.text = "Current Bet: " + GameDataSingleton.Instance.BetAmount.ToString();
    }

    /// <summary>
    /// Changes player's uno button condition
    /// </summary>
    public void OnClickUnoButton()
    {
        if (ControllerReference.Instance.PlayerController.PlayerDeck.Count == 2)
        {
            ControllerReference.Instance.PlayerController.isClickedUno = true;
            OpenUnoPanel();
        }
    }

    /// <summary>
    /// Opens uno panel in InGameCanvas
    /// </summary>
    public void OpenUnoPanel()
    {
        UnoPanel.SetActive(true);
    }

    /// <summary>
    /// Opens your turn panel in InGameCanvas
    /// </summary>
    public void OpenYourTurnPanel()
    {
        YourTurnPanel.SetActive(true);
    }

    /// <summary>
    /// Shows drawn card options ui pop up
    /// </summary>
    public void ShowDrawnCardPopUp()
    {
        DrawnCardPopUpCanvas.gameObject.SetActive(true);
    }

    /// <summary>
    /// Plays drawn card
    /// </summary>
    public void OnClickPlayCard()
    {
        ControllerReference.Instance.PlayerController.PlayDrawnCard();
    }

    /// <summary>
    /// Keeps drawn card
    /// </summary>
    public void OnClickKeepCard()
    {
        GameManager.Instance.EndTurn();
    }

    /// <summary>
    /// Shows wild card color choosing ui pop up
    /// </summary>
    public void ShowWildCardPopUp()
    {
        WildCardPopUpCanvas.gameObject.SetActive(true);
    }

    /// <summary>
    /// Sets current card color according to given index
    /// </summary>
    public void ChooseColor(int colorIndex)
    {
        GameManager.Instance.CurrentCardColor = GameManager.Instance.CardColors[colorIndex];
        GameManager.Instance.EndTurn();
    }

    public void ReverseGameRotationImage()
    {
        GameRotationImage.transform.localScale = new Vector3(-GameRotationImage.transform.localScale.x, GameRotationImage.transform.localScale.y, GameRotationImage.transform.localScale.z);
    }
}
