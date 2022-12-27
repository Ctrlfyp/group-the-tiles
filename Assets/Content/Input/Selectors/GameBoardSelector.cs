using GameInputSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardSelector : GameInputSelector
{
    public event EventHandler<MouseEventArgs> MouseButtonDown;
    public event EventHandler<MouseEventArgs> MouseButtonUp;
    public event EventHandler<Vector2> MouseMoved;

    public GameBoardSelector(GameBoardComponent gameBoardComponent)
    {
        gameBoardComponent.MouseButtonDown += OnMouseDown;
        gameBoardComponent.MouseButtonUp += OnMouseUp;
        gameBoardComponent.MouseMoved += OnMouseMoved;
    }

    private void OnMouseDown(object sender, MouseEventArgs mouseEventArgs)
    {
        MouseButtonDown?.Invoke(sender, mouseEventArgs);
    }

    private void OnMouseUp(object sender, MouseEventArgs mouseEventArgs)
    {
        MouseButtonUp?.Invoke(sender, mouseEventArgs);
    }

    private void OnMouseMoved(object sender, Vector2 location)
    {
        MouseMoved?.Invoke(sender, location);
    }
}