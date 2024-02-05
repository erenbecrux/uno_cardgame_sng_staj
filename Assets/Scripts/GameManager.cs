using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public TurnState TurnState;

    public CardColorEnums[] CardColors = { CardColorEnums.Red, CardColorEnums.Yellow, CardColorEnums.Green, CardColorEnums.Blue };
    public CardTypeEnums CurrentCardType;
    public CardColorEnums CurrentCardColor;
    public int CurrentCardNumber;

    private TurnState[] TurnStates = {TurnState.PlayerTurn, TurnState.RightBotTurn, TurnState.OppositeBotTurn, TurnState.LeftBotTurn};
    private int nextTurnStateIndex = 0;
    private bool isCounterClockWise = true;

    public BotController[] botControllers; // 0: RightBot, 1: OppositeBot, 2: LeftBot
    private bool isRoundOver = false;
    public bool isGameOver = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        StartGame();
    }

    private void Update()
    {
        if(TurnState == TurnState.PlayerTurn)
        {
#if UNITY_EDITOR
             if(Input.GetMouseButtonDown(0))
            {
                ControllerReference.Instance.PlayerController.CheckClickedCard();
            }
#endif

#if UNITY_ANDROID
             if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                ControllerReference.Instance.PlayerController.CheckClickedCard();
            }
#endif

             if(isGameOver == false)
            {
                // Update timer
                ControllerReference.Instance.PlayerController.PlayerTimer += Time.deltaTime;
                UIManager.Instance.InGameUIController.PlayerTimer.value = ControllerReference.Instance.PlayerController.PlayerTimer / Constants.PlayerPlayTime;

                if (ControllerReference.Instance.PlayerController.PlayerTimer >= Constants.PlayerPlayTime)
                {
                    Debug.Log("Player time is over!");
                    ControllerReference.Instance.PlayerController.DrawTwoCardsPlayer();
                    EndTurn();
                }
            }
             
        }
    }


    /// <summary>
    /// Current type, color and number is set to given card's attributes
    /// </summary>
    public void SetCurrentCardAttributes(Card cardToSet)
    {
        CurrentCardType = cardToSet.CardType;
        CurrentCardColor = cardToSet.CardColorGameplay;
        CurrentCardNumber = cardToSet.CardNumber;
    }

    /// <summary>
    /// Decides next player and sets turnstate accordingly
    /// </summary>
    public void SetNextPlayerTurnState()
    {
        if(isCounterClockWise)
        {
            nextTurnStateIndex = nextTurnStateIndex + 1;
        }
        else if(!isCounterClockWise)
        {
            nextTurnStateIndex = nextTurnStateIndex + 3;     
        }

        if (nextTurnStateIndex > 3)
        {
            nextTurnStateIndex = nextTurnStateIndex % 4;
        }

        TurnState = TurnStates[nextTurnStateIndex];

    }

    public void ReverseGameplayRotation()
    {
        if(isCounterClockWise)
        {
            isCounterClockWise = false;
        }
        else if(!isCounterClockWise)
        {
            isCounterClockWise = true;
        }

        UIManager.Instance.InGameUIController.ReverseGameRotationImage();
    }

    /// <summary>
    /// According to selected start option, either sets turnstate to player, sets card table and calls StartTurn or loads previous sessions data from the gamedata and calls StartTurn
    /// </summary>
    private void StartGame()
    {
        if(GameDataSingleton.Instance.IsNewGame)
        {
            TurnState = TurnState.PlayerTurn;
            //StartTurn is called inside the SetCardTable
            StartCoroutine(ControllerReference.Instance.DeckController.SetCardTable());
        }
        else if(GameDataSingleton.Instance.IsGameSaved)
        {
            // Loads previous session turnstate from the gamedata file
            nextTurnStateIndex = GameDataSingleton.Instance.SavedTurnStateIndex;
            TurnState = TurnStates[nextTurnStateIndex];

            // Loads previous session current card attributes from the gamedata file
            CurrentCardColor = CardColors[GameDataSingleton.Instance.SavedCardColor];
            CurrentCardNumber = GameDataSingleton.Instance.SavedCardNumber;

            // Loads previous session uno button states from the gamedata file
            ControllerReference.Instance.PlayerController.isClickedUno = GameDataSingleton.Instance.UnoButtonStates[0];
            for(int i = 0; i < 3; i++)
            {
                botControllers[i].isClickedUno = GameDataSingleton.Instance.UnoButtonStates[i + 1];
            }

            // Loads previous session game rotation from the gamedata file
            if (GameDataSingleton.Instance.IsCounterClockWise == false)
            {
                ReverseGameplayRotation();
            }

            // Loads previous session table from the gamedata file
            ControllerReference.Instance.DeckController.LoadCardTable();
            StartTurn();
        }
           
    }

    /// <summary>
    /// According to current turn state, finds playable cards for player or makes bots play cards
    /// </summary>
    public void StartTurn()
    {
        if(TurnState == TurnState.PlayerTurn)
        {
            ControllerReference.Instance.PlayerController.PlayerTimer = 0f;
            UIManager.Instance.InGameUIController.OpenYourTurnPanel();
            ControllerReference.Instance.PlayerController.FindPlayableCards();
        }
        else
        {
            for(int i = 0; i < 3; i++)
            {
                if(TurnState == TurnStates[i+1])
                {
                    StartCoroutine(botControllers[i].PlayOneTurnBot());
                }
            }
        }
    }

    /// <summary>
    /// Checks if there are enough cards in draw pile to continue, if uno button is clicked. Also applies rules according to played cards. Finally, checks if the round is over.
    /// </summary>
    public void EndTurn()
    {
        // Checks if there are enough cards in draw pile to continue the game
        if (ControllerReference.Instance.DrawPileController.DrawPileDeck.Count < 6)
        {            
            ControllerReference.Instance.DrawPileController.ShuffleDiscardPile();
            Debug.Log("Discard pile is shuffled");
        }

        // Checks if uno button is clicked by player when required
        if (ControllerReference.Instance.PlayerController.PlayerDeck.Count == 1)
        {
            if(ControllerReference.Instance.PlayerController.isClickedUno == false)
            {
                // Penalty draw 2 cards
                ControllerReference.Instance.PlayerController.DrawTwoCardsPlayer();
            }
        }
        else
        {
            // Reset uno button if there are more than two cards in hand
            ControllerReference.Instance.PlayerController.isClickedUno = false;
        }

        // Checks if uno button is clicked by bots when required
        for (int i = 0; i < 3; i++)
        {
            if (botControllers[i].BotDeck.Count == 1)
            {
                if (botControllers[i].isClickedUno == false)
                {
                    // Penalty draw 2 cards
                    botControllers[i].DrawTwoCardsBot();
                }
            }
            else
            {
                // Reset uno button if there are more than two cards in hand
                botControllers[i].isClickedUno = false;
            }
        }

        // Applies game rules according to played card types
        if (CurrentCardType == CardTypeEnums.SkipCard)
        {
            // Skip turn by setting next player two times
            SetNextPlayerTurnState();
            SetNextPlayerTurnState();
        }
        else if (CurrentCardType == CardTypeEnums.DrawTwoCard)
        {
            // Set the next player whom the rule will be applied
            SetNextPlayerTurnState();

            // Draw two cards
            if (TurnState == TurnState.PlayerTurn)
            {
                ControllerReference.Instance.PlayerController.DrawTwoCardsPlayer();
            }
            else
            {
                for(int i = 0; i < 3; i++)
                {
                    if(TurnState == TurnStates[i+1])
                    {
                        botControllers[i].DrawTwoCardsBot();
                    }
                }
            }

            // Skip turn by setting next player again
            SetNextPlayerTurnState();
        }
        else if (CurrentCardType == CardTypeEnums.ReverseCard)
        {
            // Reverse gameplay rotation and set the next player turn
            ReverseGameplayRotation();
            SetNextPlayerTurnState();
        }
        else if (CurrentCardType == CardTypeEnums.WildCard)
        {
            // Choosing next color logic is controlled by bot controllers and player controller
            SetNextPlayerTurnState();
        }
        else if (CurrentCardType == CardTypeEnums.WildDrawFourCard)
        {
            // Set the next player whom the rule will be applied
            SetNextPlayerTurnState();

            // Draw four cards
            if (TurnState == TurnState.PlayerTurn)
            {
                ControllerReference.Instance.PlayerController.DrawTwoCardsPlayer();
                ControllerReference.Instance.PlayerController.DrawTwoCardsPlayer();
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    if (TurnState == TurnStates[i + 1])
                    {
                        botControllers[i].DrawTwoCardsBot();
                        botControllers[i].DrawTwoCardsBot();
                    }
                }
            }

            // Skip turn by setting next player again
            SetNextPlayerTurnState();
        }
        else
        {
            // If a normal card is played, set the next player turn
            SetNextPlayerTurnState();
        }

        // Checks if round is over
        if (ControllerReference.Instance.PlayerController.PlayerDeck.Count == 0)
        {
            Debug.Log("Player Won");
            isRoundOver = true;
            RoundOver(0); // Call the RoundOver method with 0 for player
        }
        else
        {
            for(int i = 0; i < 3; i++)
            {
                if (botControllers[i].BotDeck.Count == 0)
                {
                    Debug.Log("Bot" + i + " Won");
                    isRoundOver = true;
                    RoundOver(i + 1);
                }
            }
        }

        // If the round is not finished, game continues to next player
        if(!isRoundOver)
        {
            // At the end call StartTurn
            CurrentCardType = CardTypeEnums.NormalCard; // To avoid that skipped player can be affected by previous card type
            StartTurn();
        } 
    }

    /// <summary>
    /// Calculates scores and adds to the round winner's points
    /// </summary>
    private void RoundOver(int winnerID)
    {
        Debug.Log("ROUND FINISHED!");

        // Calculates score
        int totalScore = CalculateScore();

        // Adds total score to winner's points
        for(int i = 0; i < 4; i++)
        {
            if(winnerID == i)
            {
                GameDataSingleton.Instance.Points[i] += totalScore;
            }
        }

        // Checks the game end
        for (int i = 0; i <4; i++)
        {
            if (GameDataSingleton.Instance.Points[i] >= Constants.PointsNeededToWin)
            {
                GameOver(i);
            }
        }

        // If the game is not over, show the round over ui menu
        if(!isGameOver)
        {
            UIManager.Instance.RoundEndUIController.ShowRoundOverMenu(winnerID);
        }

    }

    /// <summary>
    /// Calculates each player's score and returns it
    /// </summary>
    private int CalculateScore()
    {
        int totalScore = 0;
        int playerHand = ControllerReference.Instance.PlayerController.CalculatePlayerHand();
        int rightBotHand = ControllerReference.Instance.RightBotController.CalculateBotHand();
        int oppositeBotHand = ControllerReference.Instance.OppositeBotController.CalculateBotHand();
        int leftBotHand = ControllerReference.Instance.LeftBotController.CalculateBotHand();

        totalScore = playerHand + rightBotHand + oppositeBotHand + leftBotHand;

        return totalScore;
    }

    /// <summary>
    /// Sets the game as finished and shows game over ui menu
    /// </summary>
    private void GameOver(int winnerID)
    {
        isGameOver = true;

        // Player obtains all the bet money if the winner is player
        if(winnerID == 0)
        {
            GameDataSingleton.Instance.PlayerMoney = GameDataSingleton.Instance.PlayerMoney + ((int) GameDataSingleton.Instance.BetAmount * 4);
        }

        Debug.Log("Player " + winnerID + " won the game");
        UIManager.Instance.RoundEndUIController.ShowGameOverMenu(winnerID);
        AudioManager.Instance.PlayWinClip();

        StartCoroutine(UIManager.Instance.InterstitialAds.ShowInterstitialAdAfterWaiting(1f));
    }

    /// <summary>
    /// Reset points, kills all tweens and reloads the game scene
    /// </summary>
    public void RestartGame()
    {
        Debug.Log("Game is restarting...");
        GameDataSingleton.Instance.ResetPoints();
        GameDataSingleton.Instance.ApplyCurrentBet();
        DOTween.KillAll();
        SceneManager.LoadScene("InGameScene");
    }

    /// <summary>
    /// Move on to the next round by reloading the game scene
    /// </summary>
    public void LoadNextRound()
    {
        Debug.Log("New Round is starting...");
        GameDataSingleton.Instance.IsNewGame = true;
        SceneManager.LoadScene("InGameScene");
    }

    /// <summary>
    /// Saves current cards' IDs' to the GameDataSingleton's corresponding variables
    /// </summary>
    public void SaveCardIDs()
    {
        GameDataSingleton.Instance.PlayerCardIDs = new int[ControllerReference.Instance.PlayerController.PlayerDeck.Count];
        for (int i = 0; i < ControllerReference.Instance.PlayerController.PlayerDeck.Count; i++)
        {
            GameDataSingleton.Instance.PlayerCardIDs[i] = ControllerReference.Instance.PlayerController.PlayerDeck[i].CardID;
        }

        GameDataSingleton.Instance.RightBotCardIDs = new int[ControllerReference.Instance.RightBotController.BotDeck.Count];
        for (int i = 0; i < ControllerReference.Instance.RightBotController.BotDeck.Count; i++)
        {
            GameDataSingleton.Instance.RightBotCardIDs[i] = ControllerReference.Instance.RightBotController.BotDeck[i].CardID;
        }

        GameDataSingleton.Instance.OppositeBotCardIDs = new int[ControllerReference.Instance.OppositeBotController.BotDeck.Count];
        for (int i = 0; i < ControllerReference.Instance.OppositeBotController.BotDeck.Count; i++)
        {
            GameDataSingleton.Instance.OppositeBotCardIDs[i] = ControllerReference.Instance.OppositeBotController.BotDeck[i].CardID;
        }

        GameDataSingleton.Instance.LeftBotCardIDs = new int[ControllerReference.Instance.LeftBotController.BotDeck.Count];
        for (int i = 0; i < ControllerReference.Instance.LeftBotController.BotDeck.Count; i++)
        {
            GameDataSingleton.Instance.LeftBotCardIDs[i] = ControllerReference.Instance.LeftBotController.BotDeck[i].CardID;
        }

        GameDataSingleton.Instance.DiscardPileIDs = new int[ControllerReference.Instance.DrawPileController.DiscardPileDeck.Count];
        for (int i = 0; i < ControllerReference.Instance.DrawPileController.DiscardPileDeck.Count; i++)
        {
            GameDataSingleton.Instance.DiscardPileIDs[i] = ControllerReference.Instance.DrawPileController.DiscardPileDeck[i].CardID;
        }

        GameDataSingleton.Instance.DrawPileIDs = new int[ControllerReference.Instance.DrawPileController.DrawPileDeck.Count];
        for (int i = 0; i < ControllerReference.Instance.DrawPileController.DrawPileDeck.Count; i++)
        {
            GameDataSingleton.Instance.DrawPileIDs[i] = ControllerReference.Instance.DrawPileController.DrawPileDeck[i].CardID;
        }
    }

    /// <summary>
    /// Saves the next player's ID into GameDataSingleton and current state of the game rotation
    /// </summary>
    public void SaveGameState()
    {
        GameDataSingleton.Instance.SavedTurnStateIndex = nextTurnStateIndex;
        GameDataSingleton.Instance.IsCounterClockWise = isCounterClockWise;
        GameDataSingleton.Instance.SavedCardColor = ((int)CurrentCardColor);
        GameDataSingleton.Instance.SavedCardNumber = CurrentCardNumber;

        GameDataSingleton.Instance.UnoButtonStates[0] = ControllerReference.Instance.PlayerController.isClickedUno;     
        for(int i = 0; i < 3; i ++)
        {
            GameDataSingleton.Instance.UnoButtonStates[i + 1] = botControllers[i].isClickedUno;
        }
    }
}