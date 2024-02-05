using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public static int PointsNeededToWin = 500;
    public static int ActionCardPoints = 20;
    public static int WildCardPoints = 50;

    public static Vector3 OffSetValueOne = new Vector3(1.75f, 0, 0);
    public static Vector3 OffSetValueTwo = new Vector3(1.25f, 0, 0);
    public static Vector3 OffSetValueThree = new Vector3(0.75f, 0, 0);

    public static int NumOfCardOffSetOne = 10;
    public static int NumOfCardOffSetTwo = 24;
    public static int NumOfCardOffSetThree = 40;

    public static float BotUnoProbability = 0.8f;

    public static Vector2 MarketOffSetVector = new Vector2(1050, 0);
    public static int MinimumBet = 20;
    public static int AdReward = 20;

    public static float BotWaitingTime = 2f;
    public static float BotWaitingTimeAfterDrawing = 1.5f;
    public static float CardMoveTime = 0.75f;
    public static float TableSettingTime = 0.75f;
    public static float DrawPileSettingTimeMultiplier = 0.005f;
    public static float HandSortingTimeMultiplier = 0.05f;

    public static float PlayerPlayTime = 10f;
}
