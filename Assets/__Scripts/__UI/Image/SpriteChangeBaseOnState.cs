using Course.Core;
using Course.Utility.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Course.UI
{
    [RequireComponent(typeof(ActionButton))]
    public class SpriteChangeBaseOnState : MonoBehaviour
    {
        [Header("Assign these in the Inspector:")]
        [SerializeField] 
        private Image image = null;
        [EnumFlag]
        [SerializeField]
        private GameState toggleStates = GameState.all;
        
        [Tooltip("Sprite to show when the game is NOT paused (the ‘pause’ icon).")]
        [SerializeField] private Sprite spriteNormal = null;

        [Tooltip("Sprite to show when the game IS paused (the ‘play’ icon).")]
        [SerializeField] private Sprite spriteToggled  = null;
        
        private EventBinding<GameStateChangedEvent> _binding;

        private void OnEnable()
        {
            _binding = new EventBinding<GameStateChangedEvent>(OnGameStateChanged);
            EventBus<GameStateChangedEvent>.Register(_binding);
        }

        private void OnDisable()
        {
            EventBus<GameStateChangedEvent>.Deregister(_binding);
        }

        private void Start()
        {
            UpdateIcon(GameManager.TryGetInstance()?.currentStates ?? GameState.none);
        }

        private void OnGameStateChanged(GameStateChangedEvent e)
        {
            UpdateIcon(e.NewState);
        }

        private void UpdateIcon(GameState state)
        {
            if (image == null) return;

            bool isPaused = (state & toggleStates) != 0;
            if (isPaused && spriteToggled != null)
            {
                SetIcon(spriteToggled);
            }
            else if (!isPaused && spriteNormal != null)
            {
                SetIcon(spriteNormal);
            }
        }
        private void SetIcon(Sprite newSprite)
        {
            image.sprite = newSprite;
        }
    }
}
