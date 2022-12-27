using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameManager
{
    public static SaveManager saveManager;
    public static DataManager dataManager;

    static GameManager()
    {
        saveManager = new SaveManager();
        dataManager = new DataManager();
    }
}
