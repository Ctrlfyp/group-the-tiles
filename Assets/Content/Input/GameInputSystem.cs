using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameInputSystem
{
    public class GameInputSystem : MonoBehaviour
    {
        public GameInputManager currentGameInputManager;
        public List<GameInputSelector> currentGameInputSelectors;

        private void Awake()
        {
            currentGameInputSelectors = new List<GameInputSelector>();
        }

        public void SetupNewManager(string type, params object[] arguments)
        {
            RemoveExistingManager();

            if (type is nameof(IdleManager))
            {
                currentGameInputManager = new IdleManager();
                SelectorAdapter.SetupNewManager(currentGameInputManager, this, arguments);
            }
            else if (type is nameof(GroupEditManager))
            {
                currentGameInputManager = new GroupEditManager(arguments[1] as GameTileGroup);
                SelectorAdapter.SetupNewManager(currentGameInputManager, this, arguments);
            }
            else if(type is nameof(LevelSelectIdleManager))
            {
                currentGameInputManager = new LevelSelectIdleManager();
                SelectorAdapter.SetupNewManager(currentGameInputManager, this, arguments);
            }
            else if (type is nameof(LevelEditManager))
            {
                currentGameInputManager = new LevelEditManager();
                SelectorAdapter.SetupNewManager(currentGameInputManager, this, arguments);
            }
            currentGameInputManager.gameInputSystem = this;
            currentGameInputManager.OnStart();
        }

        public void RemoveExistingManager()
        {
            SelectorAdapter.RemoveManager(currentGameInputManager, this);
            if (currentGameInputManager != null)
            {
                currentGameInputManager.OnEnd();
            }
            currentGameInputManager = null;
        }
    }
}
