using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameSave
{
    [Serializable]
    public class LevelProgress
    {
        public string levelId;
        public int bestScore;
    }
    [Serializable]
    public class GameSaveStatistics
    {
        public int totalPlaytime;
        public int totalGametime;
        public int starsCollected;
        public int starsSelected;
    }

    public int currency;
    public GameSaveStatistics gameSaveStatistics;
    public Dictionary<string, LevelProgress> playedData;
    public HashSet<string> unlockedThemesIds;
    public HashSet<string> unlockedItemIds;

    public GameSave()
    {
        gameSaveStatistics = new GameSaveStatistics();
        playedData = new Dictionary<string, LevelProgress>();
        unlockedThemesIds = new HashSet<string>();
        unlockedItemIds = new HashSet<string>();
    }
}
