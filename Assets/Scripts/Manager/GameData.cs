using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameData
{
    public static int magnetId;
    public static int protectId;
    public static int coinDoubleId;
    public static int invincibilityId;
    public static float distanceBestRecord;
    public static int coin;

    public static bool ingameUiOn;
    public static bool mainMenuUiOn = true;
    public static bool gameOverUiOn;
    public static bool upGradeUiOn;

    public static float sfxSound;
    public static float bgmSound;

    static GameData()
    {
        magnetId = SaveLoadManager.Data.magnetId;
        protectId = SaveLoadManager.Data.protectId;
        coinDoubleId = SaveLoadManager.Data.coinDoubleId;
        invincibilityId = SaveLoadManager.Data.invincibilityId;
        distanceBestRecord = SaveLoadManager.Data.distanceBestRecord;
        coin = SaveLoadManager.Data.coin;
    }
    public static void GameDataSet()
    {
        magnetId = SaveLoadManager.Data.magnetId;
        protectId = SaveLoadManager.Data.protectId;
        coinDoubleId = SaveLoadManager.Data.coinDoubleId;
        invincibilityId = SaveLoadManager.Data.invincibilityId;
        distanceBestRecord = SaveLoadManager.Data.distanceBestRecord;
        coin = SaveLoadManager.Data.coin;
    }
}
