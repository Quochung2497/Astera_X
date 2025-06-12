using System;
using UnityEngine;

namespace Course.Control.Player
{
    public interface IJumpBehaviour
    {
        /// <summary>
        /// Fired the moment the ship “disappears” (just before teleport).
        /// Effects that should play on disappearance subscribe here.
        /// </summary>
        event Action OnDisappear;

        /// <summary>
        /// Fired the moment the ship “reappears” (immediately after teleport).
        /// Effects that should play on reappearance subscribe here.
        /// </summary>
        event Action OnReappear;
    }
}

