using Course.Utility.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Course.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasGroupHandle : MonoBehaviour {
        [SerializeField] GameState activeState = GameState.paused;
        private CanvasGroup _cg;
        private EventBinding<GameStateChangedEvent> _binding;

        void Awake() {
            _cg = GetComponent<CanvasGroup>();
        }

        void OnEnable() {
            _binding = new EventBinding<GameStateChangedEvent>(OnStateChanged);
            EventBus<GameStateChangedEvent>.Register(_binding);
        }

        void OnDisable() {
            EventBus<GameStateChangedEvent>.Deregister(_binding);
        }

        private void OnStateChanged(GameStateChangedEvent e) {
            bool shouldBeActive = e.NewState == activeState;
            _cg.interactable     = shouldBeActive;
            _cg.blocksRaycasts   = shouldBeActive;
        }
    }
}
