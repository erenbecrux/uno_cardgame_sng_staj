using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoundEndUIController : MonoBehaviour
{
    public Canvas GameOverMenuCanvas;
    public Canvas RoundOverMenuCanvas;

    public TextMeshProUGUI GameWinnerText;
    public TextMeshProUGUI PointsTextGameOver;
    public TextMeshProUGUI RoundWinnerText;
    public TextMeshProUGUI PointsTextRoundOver;

    /// <summary>
    /// Shows game over ui menu
    /// </summary>
    public void ShowGameOverMenu(int winnerID)
    {
        GameWinnerText.text = "Player " + winnerID + " won the game!";
        PointsTextGameOver.text = "Player 0: " + GameDataSingleton.Instance.Points[0] + "\n" + "Player 1: " + GameDataSingleton.Instance.Points[1] + "\n" + "Player 2: " + GameDataSingleton.Instance.Points[2] + "\n" + "Player 3: " + GameDataSingleton.Instance.Points[3] + "\n";
        GameOverMenuCanvas.gameObject.SetActive(true);
    }

    /// <summary>
    /// Shows round over ui menu
    /// </summary>
    public void ShowRoundOverMenu(int winnerID)
    {
        RoundWinnerText.text = "Player " + winnerID + " won the round!";
        PointsTextRoundOver.text = "Player 0: " + GameDataSingleton.Instance.Points[0] + "\n" + "Player 1: " + GameDataSingleton.Instance.Points[1] + "\n" + "Player 2: " + GameDataSingleton.Instance.Points[2] + "\n" + "Player 3: " + GameDataSingleton.Instance.Points[3] + "\n";
        RoundOverMenuCanvas.gameObject.SetActive(true);
    } 
}
