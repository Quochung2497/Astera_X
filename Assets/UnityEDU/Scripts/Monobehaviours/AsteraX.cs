//#define DEBUG_AsteraX_LogMethods
//#define DEBUG_AsteraX_RespawnNotifications

using System.Collections;
using System.Collections.Generic;
using Course.Attribute;
using Course.Attribute.Bullet;
using Course.Core;
using Course.ScriptableObject;
using UnityEngine;

public class AsteraX : MonoBehaviour
{
    // Private Singleton-style instance. Accessed by static property S later in script
    static private AsteraX _S;

    static List<Asteroid> ASTEROIDS;
    static List<Bullet> BULLETS;
    static private eGameState _GAME_STATE = eGameState.mainMenu;

    // If you use a fully-qualified class name like this, you don't need "using UnityEngine.UI;" above.
    static UnityEngine.UI.Text SCORE_GT;

    // This is an automatic property
    public static int SCORE { get; private set; }

    const float MIN_ASTEROID_DIST_FROM_PLAYER_SHIP = 5;
    const float DELAY_BEFORE_RELOADING_SCENE = 4;

    public delegate void CallbackDelegate(); // Set up a generic delegate type.

    static public CallbackDelegate GAME_STATE_CHANGE_DELEGATE;

    public delegate void CallbackDelegateV3(Vector3 v); // Set up a Vector3 delegate type.

    // System.Flags changes how eGameStates are viewed in the Inspector and lets multiple 
    //  values be selected simultaneously (similar to how Physics Layers are selected).
    // It's only valid for the game to ever be in one state, but I've added System.Flags
    //  here to demonstrate it and to make the ActiveOnlyDuringSomeGameStates script easier
    //  to view and modify in the Inspector.
    // When you use System.Flags, you still need to set each enum value so that it aligns 
    //  with a power of 2. You can also define enums that combine two or more values,
    //  for example the all value below that combines all other possible values.
    [System.Flags]
    public enum eGameState
    {
        // Decimal      // Binary
        none = 0, // 00000000
        mainMenu = 1, // 00000001
        preLevel = 2, // 00000010
        level = 4, // 00000100
        postLevel = 8, // 00001000
        gameOver = 16, // 00010000
        all = 0xFFFFFFF // 11111111111111111111111111111111
    }

    [Header("Set in Inspector")] [Tooltip("This sets the AsteroidsScriptableObject to be used throughout the game.")]
    public AsteroidsConfig asteroidsSO;

    [Header("These reflect static fields and are otherwise unused")]
    [SerializeField]
    [Tooltip("This private field shows the game state in the Inspector and is set by the "
             + "GAME_STATE_CHANGE_DELEGATE whenever GAME_STATE changes.")]
    protected eGameState _gameState;

    private void Awake()
    {
#if DEBUG_AsteraX_LogMethods
        Debug.Log("AsteraX:Awake()");
#endif

        S = this;

        GAME_STATE_CHANGE_DELEGATE += delegate()
        {
            // This is an example of a C# anonymous delegate. It's used to set the state of
            //  _gameState every time GAME_STATE changes.
            // Anonymous delegates like this do create "closures" like "this" below, which 
            //  stores the value of this when the anonymous delegate was created. Closures
            //  can be slow, but in this case, it is so rarely used that it doesn't matter.
            this._gameState = AsteraX.GAME_STATE;
            S._gameState = AsteraX.GAME_STATE;
        };

        // This strange use of _gameState as an intermediary in the following lines 
        //  is solely to stop the Warning from popping up in the Console telling you 
        //  that _gameState was assigned but not used.
        _gameState = eGameState.mainMenu;
        GAME_STATE = _gameState;
    }

    private void OnDestroy()
    {
        AsteraX.GAME_STATE = AsteraX.eGameState.none;
    }


    public void EndGame()
    {
        GAME_STATE = eGameState.gameOver;
        Invoke("ReloadScene", DELAY_BEFORE_RELOADING_SCENE);
    }

    void ReloadScene()
    {
        // Reload the scene to restart the game
        // Note: This exposes a long-time Unity bug where reloading the scene 
        //  during gameplay within the Editor causes the lighting to all go 
        //  dark and the engine to think that it needs to rebuild the lighting.
        //  This bug does not cause any issues outside of the Editor.
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }



    // ---------------- Static Section ---------------- //

    /// <summary>
    /// <para>This static public property provides some protection for the Singleton _S.</para>
    /// <para>get {} does return null, but throws an error first.</para>
    /// <para>set {} allows overwrite of _S by a 2nd instance, but throws an error first.</para>
    /// <para>Another advantage of using a property here is that it allows you to place
    /// a breakpoint in the set clause and then look at the call stack if you fear that 
    /// something random is setting your _S value.</para>
    /// </summary>
    static private AsteraX S
    {
        get
        {
            if (_S == null)
            {
                Debug.LogError("AsteraX:S getter - Attempt to get value of S before it has been set.");
                return null;
            }

            return _S;
        }
        set
        {
            if (_S != null)
            {
                Debug.LogError("AsteraX:S setter - Attempt to set S when it has already been set.");
            }

            _S = value;
        }
    }


    static public eGameState GAME_STATE
    {
        get { return _GAME_STATE; }
        set
        {
            if (value != _GAME_STATE)
            {
                _GAME_STATE = value;
                // Need to update all of the handlers
                // Any time you use a delegate, you run the risk of it not having any handlers
                //  assigned to it. In that case, it is null and will throw a null reference
                //  exception if you try to call it. So *any* time you call a delegate, you 
                //  should check beforehand to make sure it's not null.
                if (GAME_STATE_CHANGE_DELEGATE != null)
                {
                    GAME_STATE_CHANGE_DELEGATE();
                }
            }
        }
    }


    static public void AddAsteroid(Asteroid asteroid)
    {
        if (ASTEROIDS.IndexOf(asteroid) == -1)
        {
            ASTEROIDS.Add(asteroid);
        }
    }

    static public void RemoveAsteroid(Asteroid asteroid)
    {
        if (ASTEROIDS.IndexOf(asteroid) != -1)
        {
            ASTEROIDS.Remove(asteroid);
        }
    }


    static public void GameOver()
    {
        _S.EndGame();
    }


    static public void AddScore(int num)
    {
        // Find the ScoreGT Text field only once.
        if (SCORE_GT == null)
        {
            GameObject go = GameObject.Find("ScoreGT");
            if (go != null)
            {
                SCORE_GT = go.GetComponent<UnityEngine.UI.Text>();
            }
            else
            {
                Debug.LogError("AsteraX:AddScore() - Could not find a GameObject named ScoreGT.");
                return;
            }

            SCORE = 0;
        }

        // SCORE holds the definitive score for the game.
        SCORE += num;

        // Show the score on screen. For info on numeric formatting like "N0", see:
        //  https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings
        SCORE_GT.text = SCORE.ToString("N0");
    }


    const int RESPAWN_DIVISIONS = 8;
    const int RESPAWN_AVOID_EDGES = 2; // Note: This number must be greater than 0!
    static Vector3[,] RESPAWN_POINTS;
}