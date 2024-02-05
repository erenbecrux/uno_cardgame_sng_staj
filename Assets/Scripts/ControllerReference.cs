using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerReference : MonoBehaviour
{
    public static ControllerReference Instance;

    public DeckController DeckController;

    public PlayerController PlayerController;

    public BotController RightBotController;
    public BotController OppositeBotController;
    public BotController LeftBotController;

    public DrawPileController DrawPileController;

    public StoreController StoreController;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
}
