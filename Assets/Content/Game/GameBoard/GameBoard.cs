using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;

public enum GAME_BOARD_SHAPE
{
    SQUARE, TRIANGLE, HEXAGON
}

[Serializable]
public class GameBoard
{
    public Vector2 size;
    public GAME_BOARD_SHAPE gameBoardShape;
    public bool setFirstRowBias;
    [JsonIgnore]
    public Dictionary<Vector2, GameTile> tiles;


    [JsonIgnore]
    public List<GameTile> _serializedTiles;
    public List<GameTile> serializedTiles
    {
        get { return _serializedTiles; }
        set
        {
            tiles = value.ToDictionary(x => x.location, x => x);
            _serializedTiles = value;
        }
    }
    public GameBoard()
    {
        tiles = new Dictionary<Vector2, GameTile>();
    }
}
