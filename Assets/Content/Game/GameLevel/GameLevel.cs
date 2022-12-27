using System;
using System.Collections.Generic;

[Serializable]
public class GameLevel
{
    public enum GAME_MODE
    {
        CLASSIC, RACE, ENDURANCE, ZEN
    }

    public string id;
    public GAME_MODE gameMode;
    public int totalTime = 60;
    public int totalMoves = 10;
    public int totalColours = 2;
    public Dictionary<int, int> difficultyScoreRequired;
    public GameBoard board;
    public LevelRestrictionGroup restrictions;

    public GameLevel()
    {
        board = new GameBoard();
        restrictions = new LevelRestrictionGroup();
        difficultyScoreRequired = new Dictionary<int, int>()
        {
            { 1, 10 },
            { 2, 20 },
            { 3, 30 },
        };
    }
}
