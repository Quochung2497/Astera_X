using UnityEngine;

[System.Flags]
public enum GameState
{
     none      = 0,
     mainMenu  = 1 << 0,  // 1
     preLevel  = 1 << 1,  // 2
     level     = 1 << 2,  // 4
     postLevel = 1 << 3,  // 8
     gameOver  = 1 << 4,  // 16
     paused    = 1 << 5,  // 32

     all = 0xFFFFFFF
}
