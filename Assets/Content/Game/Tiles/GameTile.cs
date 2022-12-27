using Newtonsoft.Json;
using System;
using UnityEngine;

[Serializable]
public class GameTile
{
    public enum TILE_TYPE
    {
        NORMAL, LINEH, LINEV, BOMB, RAINBOW
    }

    public Vector2 location;
    [JsonIgnore]
    public Color colour
    {
        // get { return Constants.tileColourMap[Constants.indexToColourStringMap[colourIndex]]; }
        get
        {
            return DataManager.currentTheme.colors[colourIndex];
        }
    }
    public int colourIndex;

    public TILE_TYPE tileType;

    public GameTile Clone()
    {
        GameTile copy = new GameTile();
        copy.location = location;
        copy.colourIndex = colourIndex;
        copy.tileType = tileType;

        return copy;
    }


    public GameTile()
    {
        location = new Vector2();
    }
}
