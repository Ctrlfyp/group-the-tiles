using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameInputSystem
{
    public class GroupEditManager : GameInputManager
    {
        public GameTileGroup currentGroup;

        public GroupEditManager(GameTileGroup currentGroup)
        {
            this.currentGroup = currentGroup;
        }

        public override void OnStart() {}

        public override void OnEnd() {}

        public void OnTileOver(object sender, EventArgs args)
        {
            GameTileComponent gameTileComponent = sender as GameTileComponent;
            GameScene gameScene = ComponentUtility.gameScene;

            GameTileGroup group = GroupTileUtility.GetExistingGroup(gameScene.groups, gameTileComponent.gameTile.location);
            if (currentGroup.Equals(group))
            {
                return;
            }
            else if (group != null && currentGroup.Equals(group))
            {
                GroupTileUtility.RemoveGroup(gameScene.groups, group);
            }

            currentGroup.AddTile(gameTileComponent);
        }

        public void OnTileExit(object sender, EventArgs args)
        {
            GameTileComponent gameTileComponent = sender as GameTileComponent;
        }

        public void OnTileDown(object sender, EventArgs args)
        {
            GameTileComponent gameTileComponent = sender as GameTileComponent;
        }

        public void OnTileUp(object sender, EventArgs args)
        {
            GameTileComponent gameTileComponent = sender as GameTileComponent;

            GameScene gameScene = gameInputSystem.gameObject.GetComponent<GameScene>();
            gameScene.gameInputSystem.SetupNewManager(nameof(IdleManager), gameScene);
        }
    }
}
