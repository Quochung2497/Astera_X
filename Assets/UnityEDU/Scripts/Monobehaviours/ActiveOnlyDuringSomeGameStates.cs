using System.Collections;
using Course.UI;
using Course.Utility.Events;
using UnityEngine;

public class ActiveOnlyDuringSomeGameStates : MonoBehaviour {

    [EnumFlag]
    [SerializeField]
    [Tooltip("Which GameState(s) this object should be active in")]
    private GameState activeStates = GameState.all;
    
    [SerializeField]
    private GameObject[] activeObjects;
    
    private EventBinding<GameStateChangedEvent> _binding;

    void OnEnable()
    {
        // subscribe even if GameObject.disabled
        _binding = new EventBinding<GameStateChangedEvent>(OnStateChanged);
        EventBus<GameStateChangedEvent>.Register(_binding);
    }

    void OnDisable()
    {
        EventBus<GameStateChangedEvent>.Deregister(_binding);
    }

    
    private void OnStateChanged(GameStateChangedEvent evt)
    {
        bool shouldBeOn = (activeStates & evt.NewState) == evt.NewState;
        foreach (var go in activeObjects)
        {
            go.SetActive(shouldBeOn);
        }
    }
}
