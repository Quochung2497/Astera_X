using System;
using UnityEngine;

public interface IInputReader
{
    Vector2 MoveDirection { get; }
    event Action<Vector2> OnMoveEvent;
    event Action<Vector2> OnAimEvent;
    event Action<Vector2> OnFireEvent;
}
