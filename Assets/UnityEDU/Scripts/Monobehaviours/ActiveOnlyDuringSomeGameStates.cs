using Course.Utility.Events;
using UnityEngine;

public class ActiveOnlyDuringSomeGameStates : MonoBehaviour {

    [EnumFlag]
    [SerializeField]
    [Tooltip("Which GameState(s) this object should be active in")]
    private GameState activeStates = GameState.all;
    
    [SerializeField]
    private GameObject[] activeObjects;
    
    [SerializeField] private bool editorOnly = false;
    
    private EventBinding<GameStateChangedEvent> _binding;

    private void Awake()
    {
        // If we're in a real (non-editor) build and this is editor-only, kill everything now.
#if !UNITY_EDITOR
        if (editorOnly)
        {
            foreach (var go in activeObjects)
                go.SetActive(false);
        }
#endif
    }
    
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
