using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int[] PlayerPoints = { 0, 0, 0, 0 }; // 0: Player, 1: RightBot, 2: OppositeBot, 3: LeftBot
    public bool[] UnoButtonStates = { false, false, false, false }; // 0: Player, 1: RightBot, 2: OppositeBot, 3: LeftBot

    public int[] PlayerCardIDs;
    public int[] RightBotCardIDs;
    public int[] OppositeBotCardIDs;
    public int[] LeftBotCardIDs;
    public int[] DiscardPileIDs;
    public int[] DrawPileIDs;

    public int SavedTurnStateIndex;
    public int SavedCardColor;
    public int SavedCardNumber;

    public bool IsSoundEffectsOn;
    public bool IsGameSaved;
    public bool IsNewGame;
    public bool IsCounterClockWise;
    public bool IsTableSet;

    public int PlayerMoney;
    public float BetAmount;

    public int[] OwnedThemeIDs;
    public int CurrentThemeID;

    public PlayerData(GameDataSingleton gameData)
    {
        this.PlayerPoints = gameData.Points;
        this.UnoButtonStates = gameData.UnoButtonStates;

        PlayerCardIDs = new int[gameData.PlayerCardIDs.Length];
        this.PlayerCardIDs = gameData.PlayerCardIDs;

        RightBotCardIDs = new int[gameData.RightBotCardIDs.Length];
        this.RightBotCardIDs = gameData.RightBotCardIDs;

        OppositeBotCardIDs = new int[gameData.OppositeBotCardIDs.Length];
        this.OppositeBotCardIDs = gameData.OppositeBotCardIDs;

        LeftBotCardIDs = new int[gameData.LeftBotCardIDs.Length];
        this.LeftBotCardIDs = gameData.LeftBotCardIDs;

        DiscardPileIDs = new int[gameData.DiscardPileIDs.Length];
        this.DiscardPileIDs = gameData.DiscardPileIDs;

        DrawPileIDs = new int[gameData.DrawPileIDs.Length];
        this.DrawPileIDs = gameData.DrawPileIDs;      

        this.SavedTurnStateIndex = gameData.SavedTurnStateIndex;
        this.SavedCardColor = gameData.SavedCardColor;
        this.SavedCardNumber = gameData.SavedCardNumber;

        this.IsSoundEffectsOn = gameData.IsSoundEffectsOn;
        this.IsGameSaved = gameData.IsGameSaved;
        this.IsNewGame = gameData.IsNewGame;
        this.IsCounterClockWise = gameData.IsCounterClockWise;
        this.IsTableSet = gameData.IsTableSet;

        this.PlayerMoney = gameData.PlayerMoney;
        this.BetAmount = gameData.BetAmount;

        OwnedThemeIDs = new int[gameData.OwnedThemeIDs.Count];
        for(int i = 0; i < gameData.OwnedThemeIDs.Count; i++)
        {
            this.OwnedThemeIDs[i] = (gameData.OwnedThemeIDs[i]);
        }

        this.CurrentThemeID = gameData.CurrentThemeID;

    }
    
}
