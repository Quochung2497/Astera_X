using Course.Control.Player;

namespace Course.Effect
{
    public class OnReAppearEffect : OnTriggerEffect<IJumpBehaviour>
    {
        public void Initialize(IJumpBehaviour jumpBehaviour)
        {
            base.Initialize(jumpBehaviour);
            _owner.OnReappear += HandleOnTriggerEffect;
            _initialized = true;
        }
        
        protected override void OnEnable()
        {
            if(!_initialized || _owner == null)
                return;
            _owner.OnReappear += HandleOnTriggerEffect;
        }


        protected override void OnDisable()
        {
            if(!_initialized || _owner == null)
                return;
            _owner.OnReappear -= HandleOnTriggerEffect;
        }

        protected override float GetScaleFactor()
        {
            // For the player ship, we can return a constant scale factor since the effect size is fixed
            return 1f; // This could be adjusted based on game design needs
        }
    }
}