using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameInputSystem
{
    public class LevelEditManager : GameInputManager
    {
        public override void OnStart() { }

        public override void OnEnd() { }

        public void OnTileOver(object sender, EventArgs args)
        {
            GameTileComponent gameTileComponent = sender as GameTileComponent;
        }

        public void OnTileExit(object sender, EventArgs args)
        {
            GameTileComponent gameTileComponent = sender as GameTileComponent;
        }

        public void OnTileDown(object sender, EventArgs args)
        {
            GameTileComponent gameTileComponent = sender as GameTileComponent;
            GameScene gameScene = gameInputSystem.gameObject.GetComponent<GameScene>();
            int totalColours = gameScene.currentLevel.totalColours;

            if (gameTileComponent != null)
            {
                GameTile gameTile = gameTileComponent.gameTile;
                if (!gameScene.currentLevel.board.tiles.ContainsKey(gameTile.location))
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        gameScene.currentLevel.board.tiles.Add(gameTile.location, gameTile);
                        gameScene.currentLevel.board.serializedTiles = gameScene.currentLevel.board.tiles.Select(x => x.Value).ToList();
                        gameTileComponent.spriteRenderer.enabled = true;
                    }
                }
                else
                {
                    if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftControl))
                    {
                        gameTile.tileType = (GameTile.TILE_TYPE)(((int)gameTile.tileType + 1) % Enum.GetValues(typeof(GameTile.TILE_TYPE)).Length);
                        gameTileComponent.SetTileType(gameTile.tileType);

                        return;
                    }
                    else if (Input.GetMouseButtonDown(0))
                    {

                        gameTile.colourIndex = (gameTileComponent.gameTile.colourIndex + 1) % totalColours;
                        gameTileComponent.SetTileColor(gameTile.colourIndex);
                        return;
                    }
                    else if (Input.GetMouseButtonDown(1))
                    {
                        gameScene.currentLevel.board.tiles.Remove(gameTile.location);
                        gameTileComponent.spriteRenderer.enabled = false;
                        gameScene.currentLevel.board.serializedTiles = gameScene.currentLevel.board.tiles.Select(x => x.Value).ToList();
                        return;
                    }
                }

            }
        }

        public void OnGameBoardMouseDown(object sender, MouseEventArgs args)
        {
        }

        public void InputOccured(object sender, EventArgs args)
        {
        }
    }
}
