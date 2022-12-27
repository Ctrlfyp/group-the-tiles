using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static GameSave;
using System.Linq;

public class SaveManager
{
    public GameSave currentSave;

    public void LoadSave()
    {
        SavePlatformManager(false);
    }

    public void SaveGame()
    {
        SavePlatformManager(true);
    }

    public void ResetSave()
    {
        this.currentSave = new GameSave();
        SaveGame();
    }

    private void SavePlatformManager(bool isSave)
    {
        if (isSave)
        {
            // currentSave check
            if (currentSave == null)
            {
                currentSave = new GameSave();
            }
        }

        switch (Application.platform)
        {
            case RuntimePlatform.Android:       // Android
                Debug.Log((isSave ? "Saving" : "Loading") + " on Android");
                // AndroidSaveManager(isSave);
                WindowsSaveManager(isSave);
                break;
            case RuntimePlatform.IPhonePlayer:  // IOS
                Debug.Log((isSave ? "Saving" : "Loading") + " on iOS");
                // IOSSaveManager(isSave);
                WindowsSaveManager(isSave);
                break;
            case RuntimePlatform.OSXPlayer:     // Mac
                Debug.Log((isSave ? "Saving" : "Loading") + " on Mac");
                // MacSaveManager(isSave);
                WindowsSaveManager(isSave);
                break;
            default:                            // Windows and everything else
                Debug.Log((isSave ? "Saving" : "Loading") + " on Windows");
                WindowsSaveManager(isSave);
                break;
        }
    }

    private void AndroidSaveManager(bool isSave) { }

    private void IOSSaveManager(bool isSave) { }

    private void MacSaveManager(bool isSave) { }

    private void WindowsSaveManager(bool isSave)
    {
        if (isSave)
        {
            // TODO Make this into binary in the future
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new JsonUtility.Vec2Conv());
            settings.Formatting = Formatting.Indented;

            FileStream file = File.Create(Application.persistentDataPath + "/Save.save");
            file.Close();
            File.WriteAllText(Application.persistentDataPath + "/Save.save", JsonConvert.SerializeObject(currentSave, settings));
        }
        else
        {
            if (File.Exists(Application.persistentDataPath + "/Save.save"))
            {
                using (StreamReader r = new StreamReader(Application.persistentDataPath + "/Save.save"))
                {
                    string json = r.ReadToEnd();
                    JsonSerializerSettings settings = new JsonSerializerSettings();
                    settings.Converters.Add(new JsonUtility.Vec2Conv());
                    settings.Formatting = Formatting.Indented;
                    currentSave = JsonConvert.DeserializeObject<GameSave>(json, settings);
                    r.Close();
                }
            }

            // Read fallback
            if (currentSave == null)
            {
                currentSave = new GameSave();
            }
        }
    }

    public int GetLatestLevelId()
    {
        List<string> keys = currentSave.playedData.Keys.ToList();
        if (keys.Count == 0)
        {
            return 0;
        }
        else
        {
            keys.Sort();
            while (keys.Count > 0)
            {
                // pop the last element
                string key = keys[keys.Count - 1];
                keys.RemoveAt(keys.Count - 1);

                // get the score
                int score = currentSave.playedData[key].bestScore;

                // check if the score gets at least 1 star
                GameLevel level = GameManager.dataManager.LoadGameLevelFromId(key);

                // get first element of level.difficultyScoreRequired
                int difficulty = level.difficultyScoreRequired.First().Value;
                if (score >= difficulty)
                {
                    return int.Parse(key);
                }
            }
        }
        return 0;
    }

    public int GetCurrency()
    {
        return currentSave.currency;
    }

    public void AddCurrency(int amount)
    {
        currentSave.currency += amount;
        SaveGame();
    }

    public void RemoveCurrency(int amount)
    {
        currentSave.currency -= amount;
        SaveGame();
    }

    public HashSet<string> GetAllUnlockedThemesIds()
    {
        return currentSave.unlockedThemesIds;
    }

    public HashSet<string> GetAllUnlockedItemIds()
    {
        HashSet<string> result = new HashSet<string>();
        result.UnionWith(currentSave.unlockedThemesIds);
        result.UnionWith(currentSave.unlockedItemIds);
        return result;
    }


    public void BuyItem(ShopCatalogueItem item)
    {
        string itemId = item.id;
        int itemPrice = (int)item.price;

        // switch by item type
        switch (item.type)
        {
            case 0: // theme
                currentSave.unlockedThemesIds.Add(itemId);
                break;
            default: // item
                currentSave.unlockedItemIds.Add(itemId);
                break;
        }
        RemoveCurrency(itemPrice);
    }

    public void SetCurrentLevel(string itemId, int itemPrice)
    {
        currentSave.unlockedItemIds.Add(itemId);
        RemoveCurrency(itemPrice);
    }

    public void AddLevelProgress(GameLevel level, LevelProgress levelProgress)
    {
        if (currentSave.playedData.ContainsKey(levelProgress.levelId))
        {
            LevelProgress existingProgress = currentSave.playedData[levelProgress.levelId];
            switch (level.gameMode)
            {
                case GameLevel.GAME_MODE.CLASSIC:
                case GameLevel.GAME_MODE.ENDURANCE:
                case GameLevel.GAME_MODE.RACE:
                case GameLevel.GAME_MODE.ZEN:
                default:
                    existingProgress.bestScore = Mathf.Max(levelProgress.bestScore);
                    break;
            }
            currentSave.playedData[levelProgress.levelId] = existingProgress;
        }
        else
        {
            currentSave.playedData[levelProgress.levelId] = levelProgress;
        }
        SaveGame();
    }
}
