namespace Course.Services.Achievements
{
    /// <summary>
    /// Holds all the runtime state for one ConditionSO:
    /// - subscribes/unsubscribes event handlers
    /// - tracks the counter or seen set
    /// - invokes config.RaiseMet() when done
    /// </summary>
    public interface IConditionState
    {
        /// <summary>Prime the “already met” flag on the SO itself, if loading from save.</summary>
        void PrimeMet();

        /// <summary>Initialize subscriptions & counters.</summary>
        void Initialize();

        /// <summary>Unsubscribe & tear down.</summary>
        void Uninitialize();

        /// <summary>If PersistProgress==true, this is the value to save/load.</summary>
        float Counter { get; set; }

        /// <summary>How many total needed—just a pass through to the SO’s RequiredCount.</summary>
        float RequiredCount { get; }
        
        /// <summary>True if this condition’s counter belongs in the save.</summary>
        bool ShouldPersist { get; }
    }
}