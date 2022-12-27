using GameInputSystem;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameBoardComponent : MonoBehaviour
{
    public event EventHandler<MouseEventArgs> MouseButtonDown;
    public event EventHandler<MouseEventArgs> MouseButtonUp;
    public event EventHandler<Vector2> MouseMoved;

    void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (Input.GetMouseButtonDown(0))
        {
            MouseButtonDown?.Invoke(this, new MouseEventArgs(0, Input.mousePosition));
        }
        else if (Input.GetMouseButtonDown(1))
        {
            MouseButtonDown?.Invoke(this, new MouseEventArgs(1, Input.mousePosition));
        }
    }

    void OnMouseUp()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (Input.GetMouseButtonUp(0))
        {
            MouseButtonUp?.Invoke(this, new MouseEventArgs(0, Input.mousePosition));
        }
        else if (Input.GetMouseButtonUp(1))
        {
            MouseButtonUp?.Invoke(this, new MouseEventArgs(1, Input.mousePosition));
        }
    }

    void OnMouseDrag()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        MouseMoved?.Invoke(this, Input.mousePosition);
    }
}
