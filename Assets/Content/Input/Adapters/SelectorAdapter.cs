using System.Linq;
using UI;
using UnityEngine;

namespace GameInputSystem
{
    public class SelectorAdapter : MonoBehaviour
    {
        public static void SetupNewManager(GameInputManager managerType, GameInputSystem gameInputSystem, params object[] arguments)
        {
            if (managerType == null)
            {
                return;
            }

            if (managerType is IdleManager)
            {
                GameScene gameScene = arguments[0] as GameScene;
                IdleManager idleManager = managerType as IdleManager;

                TileSelector tileSelector = new TileSelector();
                tileSelector.SetTiles(gameScene.gameBoardComponent.GetComponentsInChildren<GameTileComponent>().ToList());
                tileSelector.TileDown += idleManager.OnTileDown;
                tileSelector.TileExit += idleManager.OnTileExit;
                tileSelector.TileOver += idleManager.OnTileOver;
                gameInputSystem.currentGameInputSelectors.Add(tileSelector);

                GenericInputSelector genericInputSelector = gameInputSystem.gameObject.AddComponent<GenericInputSelector>();
                genericInputSelector.InputOccured += idleManager.InputOccured;
                gameInputSystem.currentGameInputSelectors.Add(genericInputSelector);
            }
            else if (managerType is GroupEditManager)
            {
                GameScene gameScene = arguments[0] as GameScene;
                GroupEditManager groupEditManager = managerType as GroupEditManager;

                TileSelector tileSelector = new TileSelector();
                tileSelector.SetTiles(gameScene.gameBoardComponent.GetComponentsInChildren<GameTileComponent>().ToList());
                tileSelector.TileUp += groupEditManager.OnTileUp;
                tileSelector.TileDown += groupEditManager.OnTileDown;
                tileSelector.TileExit += groupEditManager.OnTileExit;
                tileSelector.TileOver += groupEditManager.OnTileOver;
                gameInputSystem.currentGameInputSelectors.Add(tileSelector);
            }
            else if (managerType is LevelSelectIdleManager)
            {
                LevelSelectView levelSelectView = arguments[0] as LevelSelectView;
                LevelSelectIdleManager levelSelectIdleManager = managerType as LevelSelectIdleManager;

                LevelBackgroundSelector levelBackgroundSelector = new LevelBackgroundSelector(levelSelectView.background);
                levelBackgroundSelector.MouseButtonDown += levelSelectIdleManager.OnMouseDown;
                levelBackgroundSelector.MouseButtonUp += levelSelectIdleManager.OnMouseUp;
                levelBackgroundSelector.MouseMoved += levelSelectIdleManager.OnMouseMove;
                gameInputSystem.currentGameInputSelectors.Add(levelBackgroundSelector);

                LevelNodeSelector levelNodeSelector = new LevelNodeSelector();
                levelNodeSelector.SetLevels(levelSelectView.levelSelectCanvas.GetComponentsInChildren<LevelButton>().ToList());
                levelNodeSelector.LevelDown += levelSelectIdleManager.OnLevelDown;
                gameInputSystem.currentGameInputSelectors.Add(levelNodeSelector);
            }
            else if (managerType is LevelEditManager)
            {
                GameScene gameScene = arguments[0] as GameScene;
                LevelEditManager levelEditManager = managerType as LevelEditManager;

                TileSelector tileSelector = new TileSelector();
                tileSelector.SetTiles(gameScene.gameBoardComponent.GetComponentsInChildren<GameTileComponent>().ToList());
                tileSelector.TileDown += levelEditManager.OnTileDown;
                tileSelector.TileExit += levelEditManager.OnTileExit;
                tileSelector.TileOver += levelEditManager.OnTileOver;
                gameInputSystem.currentGameInputSelectors.Add(tileSelector);

                GameBoardSelector gameBoardSelector = new GameBoardSelector(gameScene.gameBoardComponent);
                gameBoardSelector.MouseButtonDown += levelEditManager.OnGameBoardMouseDown;
                gameInputSystem.currentGameInputSelectors.Add(gameBoardSelector);

                GenericInputSelector genericInputSelector = gameInputSystem.gameObject.AddComponent<GenericInputSelector>();
                genericInputSelector.InputOccured += levelEditManager.InputOccured;
                gameInputSystem.currentGameInputSelectors.Add(genericInputSelector);
            }
        }

        public static void RemoveManager(GameInputManager managerType, GameInputSystem gameInputSystem)
        {
            if (managerType == null)
            {
                return;
            }

            if (managerType is IdleManager)
            {
                IdleManager idleManager = managerType as IdleManager;

                TileSelector tileSelector = gameInputSystem.currentGameInputSelectors.OfType<TileSelector>().First();
                tileSelector.TileDown -= idleManager.OnTileDown;
                tileSelector.TileExit -= idleManager.OnTileExit;
                tileSelector.TileOver -= idleManager.OnTileOver;

                GenericInputSelector genericInputSelector = gameInputSystem.currentGameInputSelectors.OfType<GenericInputSelector>().First();
                genericInputSelector.InputOccured -= idleManager.InputOccured;
            }
            else if (managerType is GroupEditManager)
            {
                GroupEditManager groupEditManager = managerType as GroupEditManager;

                TileSelector tileSelector = gameInputSystem.currentGameInputSelectors.OfType<TileSelector>().First();
                tileSelector.TileUp -= groupEditManager.OnTileUp;
                tileSelector.TileDown -= groupEditManager.OnTileDown;
                tileSelector.TileExit -= groupEditManager.OnTileExit;
                tileSelector.TileOver -= groupEditManager.OnTileOver;
            }
            else if (managerType is LevelSelectIdleManager)
            {
                LevelSelectIdleManager levelSelectIdleManager = managerType as LevelSelectIdleManager;

                LevelBackgroundSelector levelBackgroundSelector = gameInputSystem.currentGameInputSelectors.OfType<LevelBackgroundSelector>().First();
                levelBackgroundSelector.MouseButtonDown -= levelSelectIdleManager.OnMouseDown;
                levelBackgroundSelector.MouseButtonUp -= levelSelectIdleManager.OnMouseUp;
                levelBackgroundSelector.MouseMoved -= levelSelectIdleManager.OnMouseMove;

                LevelNodeSelector levelNodeSelector = gameInputSystem.currentGameInputSelectors.OfType<LevelNodeSelector>().First();
                levelNodeSelector.LevelDown -= levelSelectIdleManager.OnLevelDown;
            }
            else if (managerType is LevelEditManager)
            {
                LevelEditManager levelEditManager = managerType as LevelEditManager;

                GameBoardSelector gameBoardSelector = gameInputSystem.currentGameInputSelectors.OfType<GameBoardSelector>().First();
                gameBoardSelector.MouseButtonDown -= levelEditManager.OnGameBoardMouseDown;

                TileSelector tileSelector = gameInputSystem.currentGameInputSelectors.OfType<TileSelector>().First();
                tileSelector.TileDown -= levelEditManager.OnTileDown;
                tileSelector.TileExit -= levelEditManager.OnTileExit;
                tileSelector.TileOver -= levelEditManager.OnTileOver;

                GenericInputSelector genericInputSelector = gameInputSystem.currentGameInputSelectors.OfType<GenericInputSelector>().First();
                genericInputSelector.InputOccured -= levelEditManager.InputOccured;
            }

            gameInputSystem.currentGameInputSelectors.OfType<Component>().ToList().ForEach(selector => Destroy(selector));
            gameInputSystem.currentGameInputSelectors.Clear();
        }
    }
}