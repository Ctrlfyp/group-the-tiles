using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTileGroup
{
    public Dictionary<Vector2, GameTileComponent> tiles;
    public string sign;

    public GameTileGroup()
    {
        tiles = new Dictionary<Vector2, GameTileComponent>();
    }

    public GameTileGroup(List<GameTileComponent> tileList)
    {
        tiles = new Dictionary<Vector2, GameTileComponent>();
        foreach (GameTileComponent tile in tileList)
        {
            tiles.Add(tile.gameTile.location, tile);
            tile.text.text = sign;
        }
        SetBorders();
    }

    public void AddTile(GameTileComponent gameTileComponent)
    {
        tiles.Add(gameTileComponent.gameTile.location, gameTileComponent);
        gameTileComponent.text.text = sign;
        SetBorders();
    }

    public void RemoveTile(GameTileComponent gameTileComponent)
    {
        tiles.Remove(gameTileComponent.gameTile.location);
        gameTileComponent.text.text = "";
        SetBorders();
    }

    public void ClearGroup()
    {
        foreach (GameTileComponent tile in tiles.Values)
        {
            tile.text.text = "";
        }
        foreach (GameTileComponent tile in tiles.Values)
        {
            tile.topDivider.enabled = false;
            tile.rightDivider.enabled = false;
            tile.bottomDivider.enabled = false;
            tile.leftDivider.enabled = false;
        }
        tiles.Clear();
    }

    public void SetSign(string sign)
    {
        this.sign = sign;
        foreach (GameTileComponent tile in tiles.Values)
        {
            tile.text.text = sign;
        }
    }

    public void SetBorders()
    {
        foreach (GameTileComponent tile in tiles.Values)
        {
            tile.topDivider.enabled = false;
            tile.rightDivider.enabled = false;
            tile.bottomDivider.enabled = false;
            tile.leftDivider.enabled = false;
        }

        Dictionary<Vector2, GameTileComponent> tileLocMap = new Dictionary<Vector2, GameTileComponent>();
        foreach (GameTileComponent tile in tiles.Values)
        {
            tileLocMap.Add(tile.gameTile.location, tile);
        }
        foreach (GameTileComponent tile in tiles.Values)
        {
            if (!tileLocMap.ContainsKey(tile.gameTile.location + new Vector2(-1, 0)))
            {
                tile.leftDivider.enabled = true;
            }
            if (!tileLocMap.ContainsKey(tile.gameTile.location + new Vector2(1, 0)))
            {
                tile.rightDivider.enabled = true;
            }
            if (!tileLocMap.ContainsKey(tile.gameTile.location + new Vector2(0, -1)))
            {
                tile.topDivider.enabled = true;
            }
            if (!tileLocMap.ContainsKey(tile.gameTile.location + new Vector2(0, 1)))
            {
                tile.bottomDivider.enabled = true;
            }
        }
    }
}
