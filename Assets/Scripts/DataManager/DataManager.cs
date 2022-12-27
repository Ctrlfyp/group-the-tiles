using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using static Constants;

public class DataManager
{
    public Dictionary<string, GameTheme> loadedThemes;
    public Dictionary<string, GameLevel> loadedLevels;
    public static List<string> levelPaths = new List<string>();
    public static GameTheme currentTheme;

    public void LoadData()
    {
        loadedThemes = new Dictionary<string, GameTheme>();
        loadedLevels = new Dictionary<string, GameLevel>();
        GetFiles();
    }

    private void GetFiles()
    {
        // Get all levels and themes
        UnityEngine.Object[] levelsJsonList = Resources.LoadAll("Levels", typeof(TextAsset));
        UnityEngine.Object[] themesJsonList = Resources.LoadAll("Themes", typeof(TextAsset));

        // Parse the levels
        foreach (UnityEngine.Object curLevel in levelsJsonList)
        {
            levelPaths.Add(curLevel.name);
        }

        // Parse the themes
        foreach (UnityEngine.Object curTheme in themesJsonList)
        {
            LoadThemeFromName(curTheme.name);
        }

        LoadPlayerData();
    }

    private void LoadPlayerData()
    {
        string selectedTheme = PlayerPrefs.GetString(Constants.playerPrefThemeKey, "DefaultTheme");
        LoadThemeFromName(selectedTheme, true);
    }

    public List<string> GetGameLevelPaths()
    {
        return levelPaths;
    }

    public List<string> GetAllThemeNames()
    {
        List<string> useableThemes = new List<string> { "DefaultTheme" };
        HashSet<string> unlockedThemes = GameManager.saveManager.GetAllUnlockedThemesIds();

        // get all the names of the themes with id in unlockedThemes inside loadedThemes
        foreach (string themeId in unlockedThemes)
        {
            if (loadedThemes.ContainsKey(themeId))
            {
                useableThemes.Add(loadedThemes[themeId].name);
            }
        }
        return useableThemes;
    }

    public GameLevel LoadGameLevelFromId(string id)
    {
        if (loadedLevels.ContainsKey(id))
        {
            // make a copy of the level
            return DeepCloneGameLevel(loadedLevels[id]);
        }

        string filePath = $"Levels/Level{id}";
        TextAsset json = Resources.Load<TextAsset>(filePath);
        if (json == null)
        {
            return null;
        }

        GameLevel level = JsonConvert.DeserializeObject<GameLevel>(json.text, Constants.serializerSettings);

        loadedLevels.Add(id, level);
        return DeepCloneGameLevel(level);
    }

    public GameTheme LoadThemeFromName(string name, bool setTheme = false)
    {
        TextAsset themeJsonObject = Resources.Load<TextAsset>("Themes/" + name);
        GameTheme newlyLoadedTheme = JsonConvert.DeserializeObject<GameTheme>(themeJsonObject.text, Constants.serializerSettings);

        if (loadedThemes.ContainsKey(newlyLoadedTheme.id))
        {
            if (setTheme)
            {
                currentTheme = loadedThemes[newlyLoadedTheme.id];
            }
            return currentTheme;
        }

        loadedThemes.Add(newlyLoadedTheme.id, newlyLoadedTheme);

        if (setTheme)
        {
            currentTheme = newlyLoadedTheme;
        }
        return currentTheme;
    }

    public GameLevel DeepCloneGameLevel(GameLevel level)
    {
        GameLevel newGameLevel = new GameLevel();
        newGameLevel.id = level.id;
        newGameLevel.gameMode = level.gameMode;
        newGameLevel.totalTime = level.totalTime;
        newGameLevel.totalMoves = level.totalMoves;
        newGameLevel.totalColours = level.totalColours;
        newGameLevel.difficultyScoreRequired = new Dictionary<int, int>(level.difficultyScoreRequired);

        newGameLevel.board = new GameBoard();
        newGameLevel.board.size = level.board.size;
        newGameLevel.board.gameBoardShape = level.board.gameBoardShape;
        newGameLevel.board.setFirstRowBias = level.board.setFirstRowBias;
        newGameLevel.board.serializedTiles = level.board.tiles.Values.ToList();

        newGameLevel.restrictions = level.restrictions;
        return newGameLevel;
    }
}
