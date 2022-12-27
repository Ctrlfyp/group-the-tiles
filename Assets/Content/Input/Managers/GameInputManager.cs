using System;
using UnityEngine;

namespace GameInputSystem
{
    public abstract class GameInputManager
    {
        public GameInputSystem gameInputSystem;

        public abstract void OnEnd();
        public abstract void OnStart();
    }
}
