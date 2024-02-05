using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameDataSingleton : MonoBehaviour
{
    public static GameDataSingleton Instance;

    public int[] Points = { 0, 0, 0, 0 }; // 0: Player, 1: RightBot, 2: OppositeBot, 3: LeftBot
    public bool[] UnoButtonStates = { false, false, false, false }; // 0: Player, 1: RightBot, 2: OppositeBot, 3: LeftBot

    public int[] PlayerCardIDs;
    public int[] RightBotCardIDs;
    public int[] OppositeBotCardIDs;
    public int[] LeftBotCardIDs;
    public int[] DiscardPileIDs;
    public int[] DrawPileIDs;

    public int SavedTurnStateIndex; // saves the next player to play
    public int SavedCardColor;
    public int SavedCardNumber;

    public bool IsSoundEffectsOn = false; // to control sound effects
    public bool IsGameSaved = false; // to control continue option
    public bool IsNewGame = true; // to control creating new table
    public bool IsCounterClockWise = true; // saves the game rotation
    public bool IsTableSet = false; // to control menu popup state

    public int PlayerMoney;
    public float BetAmount;

    public List<int> OwnedThemeIDs;
    public int CurrentThemeID;
    public ThemeDesign CurrentTheme;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        // Loads data from dat file or creates new file
        LoadGameData();
    }

    private void OnApplicationQuit()
    {
        SaveGameData();
    }

    private void OnApplicationPause(bool pause)
    {
#if UNITY_ANDROID
        SaveGameData();
#endif
    }

    /// <summary>
    /// Resets each player's points
    /// </summary>
    public void ResetPoints()
    {
        for(int i = 0; i < 4; i++)
        {
            Points[i] = 0;
        }
    }

    public void ApplyCurrentBet()
    {
        PlayerMoney = PlayerMoney - (int) BetAmount;
    }

    /// <summary>
    /// Saves the current game data into json file
    /// </summary>
    public void SaveGameData()
    {
        SaveSystem.SaveGame(this);
    }

    /// <summary>
    /// Loads the data in the json file into GameDataSingleton
    /// </summary>
    public void LoadGameData()
    {
        PlayerData data = SaveSystem.LoadGame();

        Points = data.PlayerPoints;
        UnoButtonStates = data.UnoButtonStates;

        PlayerCardIDs = new int[data.PlayerCardIDs.Length];
        PlayerCardIDs = data.PlayerCardIDs;

        RightBotCardIDs = new int[data.RightBotCardIDs.Length];
        RightBotCardIDs = data.RightBotCardIDs;

        OppositeBotCardIDs = new int[data.OppositeBotCardIDs.Length];
        OppositeBotCardIDs = data.OppositeBotCardIDs;

        LeftBotCardIDs = new int[data.LeftBotCardIDs.Length];
        LeftBotCardIDs = data.LeftBotCardIDs;

        DiscardPileIDs = new int[data.DiscardPileIDs.Length];
        DiscardPileIDs = data.DiscardPileIDs;

        DrawPileIDs = new int[data.DrawPileIDs.Length];
        DrawPileIDs = data.DrawPileIDs;
        
        SavedTurnStateIndex = data.SavedTurnStateIndex;
        SavedCardColor = data.SavedCardColor;
        SavedCardNumber = data.SavedCardNumber;

        IsSoundEffectsOn = data.IsSoundEffectsOn;
        IsGameSaved = data.IsGameSaved;
        IsNewGame = data.IsNewGame;
        IsCounterClockWise = data.IsCounterClockWise;
        IsTableSet = data.IsTableSet;

        PlayerMoney = data.PlayerMoney;
        BetAmount = data.BetAmount;

        OwnedThemeIDs = new List<int>();
        for (int i = 0; i < data.OwnedThemeIDs.Length; i++)
        {
            OwnedThemeIDs.Add(data.OwnedThemeIDs[i]);
        }

        CurrentThemeID = data.CurrentThemeID;
    }
}
