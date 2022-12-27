using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameInputSystem
{
    public class IdleManager : GameInputManager
    {
        private Boolean isListening = true;

        public List<GameTile> selectedTiles;
        private SoundBoard soundBoard;


        public override void OnStart() 
        {

            soundBoard = SceneManager.GetActiveScene().GetRootGameObjects().First((gameObject) => gameObject.name.Equals("GameScene")).GetComponent<GameScene>().soundBoard;
        }

        public override void OnEnd() { }

        public void StartListening()
        {
            isListening = true;
        }

        public void StopListening()
        {
            isListening = false;
        }

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
            if (!isListening)
            {
                return;
            }

            GameTileComponent gameTileComponent = sender as GameTileComponent;
            GameScene gameScene = gameInputSystem.gameObject.GetComponent<GameScene>();

            GameTileGroup group = GroupTileUtility.GetExistingGroup(gameScene.groups, gameTileComponent.gameTile.location);
            if (group != null)
            {
                GroupTileUtility.RemoveGroup(gameScene.groups, group);
            }

            if (Input.GetMouseButtonDown(1))
            {
                soundBoard.PlaySound(soundBoard.Select2);
                return;
            }

            group = GroupTileUtility.CreateGroup(gameScene.groups);
            group.AddTile(gameTileComponent);

            soundBoard.PlaySound(soundBoard.Select1);
            gameScene.gameInputSystem.SetupNewManager(nameof(GroupEditManager), gameScene, group);
        }

        public void InputOccured(object sender, EventArgs args)
        {
            // TODO this is currently assuming its always a enter/right click fix later
            GameScene gameScene = gameInputSystem.gameObject.GetComponent<GameScene>();
            gameScene.SubmitSelection();
        }
    }
}
