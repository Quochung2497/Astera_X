using Course.Control.Player;

namespace Course.Effect
{
    public class OnDisappearEffect : OnTriggerEffect<IJumpBehaviour>
    {
        public void Initialize(IJumpBehaviour jumpBehaviour)
        {
            base.Initialize(jumpBehaviour);
            _owner.OnDisappear += HandleOnTriggerEffect;
            _initialized = true;
        }
        
        protected override void OnEnable()
        {
            if(!_initialized || _owner == null)
                return;
            _owner.OnDisappear += HandleOnTriggerEffect;
        }


        protected override void OnDisable()
        {
            if(!_initialized || _owner == null)
                return;
            _owner.OnDisappear -= HandleOnTriggerEffect;
        }

        protected override float GetScaleFactor()
        {
            // For the player ship, we can return a constant scale factor since the effect size is fixed
            return 1f; // This could be adjusted based on game design needs
        }
    }
}