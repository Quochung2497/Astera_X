using System;
using UnityEngine;

public interface IInputReader
{
    event Action<Vector2> OnMoveEvent;
    event Action<Vector2> OnAimEvent;
    event Action         OnFireEvent;
    void Initialize();
    void Shutdown();
}
