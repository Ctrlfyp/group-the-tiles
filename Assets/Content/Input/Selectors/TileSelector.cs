using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameInputSystem
{
    public class TileSelector : GameInputSelector
    {
        private Dictionary<Vector2, GameTileComponent> tiles;

        public event EventHandler TileExit;
        public event EventHandler TileOver;
        public event EventHandler TileDown;
        public event EventHandler TileUp;

        public TileSelector()
        {
            tiles = new Dictionary<Vector2, GameTileComponent>();
        }

        private void OnTileOver(object sender, EventArgs args)
        {
            GameTileComponent gameTileComponent = sender as GameTileComponent;
            TileOver?.Invoke(sender, EventArgs.Empty);

            if (Input.GetMouseButtonDown(1))
            {
                TileDown?.Invoke(sender, EventArgs.Empty);
            }
        }

        private void OnTileExit(object sender, EventArgs args)
        {
            GameTileComponent gameTileComponent = sender as GameTileComponent;
            TileExit?.Invoke(sender, EventArgs.Empty);
        }

        private void OnTileDown(object sender, EventArgs args)
        {
            GameTileComponent gameTileComponent = sender as GameTileComponent;
            TileDown?.Invoke(sender, EventArgs.Empty);
        }

        private void OnTileUp(object sender, EventArgs args)
        {
            GameTileComponent gameTileComponent = sender as GameTileComponent;
            TileUp?.Invoke(sender, EventArgs.Empty);
        }

        public void SetTiles(List<GameTileComponent> gameTiles)
        {
            foreach (GameTileComponent component in tiles.Values)
            {
                component.TileDown -= OnTileDown;
                component.TileOver -= OnTileOver;
                component.TileExit -= OnTileExit;
                component.TileUp -= OnTileUp;
            }
            tiles.Clear();

            foreach (GameTileComponent component in gameTiles)
            {
                tiles.Add(component.gameTile.location, component);
                component.TileDown += OnTileDown;
                component.TileOver += OnTileOver;
                component.TileExit += OnTileExit;
                component.TileUp += OnTileUp;
            }
        }
    }
}