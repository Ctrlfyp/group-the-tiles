using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class GameUtility
{
    public enum AxisDirection
    {
        VERTICAL, HORIZONTAL
    }

    public static Dictionary<Vector2, GameTile> GetAxisTiles(GameLevel level, GameTile currentTile, AxisDirection direction)
    {
        Dictionary<Vector2, GameTile> resultTiles = new Dictionary<Vector2, GameTile>();
        // current tile's location
        Vector2 currentLocation = currentTile.location;
        Dictionary<Vector2, GameTile> allTiles = level.board.tiles;

        List<Vector2> lineTiles = new List<Vector2>();

        if (direction == AxisDirection.HORIZONTAL)
        {
            for (int i = 0; i < level.board.size.x; i++)
            {
                lineTiles.Add(new Vector2(i, currentLocation.y));
            }
        }

        if (direction == AxisDirection.VERTICAL)
        {
            for (int i = 0; i < level.board.size.y; i++)
            {
                lineTiles.Add(new Vector2(currentLocation.x, i));
            }
        }

        // for each adjacentTiles
        foreach (Vector2 lineTile in lineTiles)
        {
            if (allTiles.ContainsKey(lineTile) && !resultTiles.ContainsKey(lineTile))
            {
                resultTiles.Add(lineTile, allTiles[lineTile]);
            }
        }
        return resultTiles;
    }

    public static Dictionary<Vector2, GameTile> GetAdjacentTiles(GameLevel level, GameTile currentTile)
    {
        Dictionary<Vector2, GameTile> resultTiles = new Dictionary<Vector2, GameTile>();
        // current tile's location
        Vector2 currentLocation = currentTile.location;
        Dictionary<Vector2, GameTile> allTiles = level.board.tiles;
        List<Vector2> adjacentTiles = new List<Vector2>();

        for (int x = (int)currentLocation.x - 1; x <= (int)currentLocation.x + 1; x++)
        {
            for (int y = (int)currentLocation.y - 1; y <= (int)currentLocation.y + 1; y++)
            {
                Vector2 adjacentLocation = new Vector2(x, y);
                if (allTiles.ContainsKey(adjacentLocation))
                {
                    resultTiles.Add(adjacentLocation, allTiles[adjacentLocation]);
                }
            }
        }
        return resultTiles;
    }

}
