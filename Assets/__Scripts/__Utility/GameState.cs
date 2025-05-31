using UnityEngine;

[System.Flags]
public enum GameState
{
     none = 0,
     mainMenu = 1,
     preLevel = 2,
     level = 4,
     postLevel = 8,
     gameOver = 16,
     all = 0xFFFFFFF
}
