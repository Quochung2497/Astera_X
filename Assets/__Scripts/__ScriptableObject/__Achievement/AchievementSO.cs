using Course.Utility.Events;
using UnityEngine;

namespace Course.ScriptableObject
{
    [CreateAssetMenu(fileName = "AchievementSO", menuName = "Scriptable Objects/Achievements/Achievement")]
    public class AchievementSO : UnityEngine.ScriptableObject
    {
        [field: SerializeField]
        public string Title { get; private set; }
        [field: SerializeField]
        [TextArea] 
        public string Description{ get; private set; }
        [field: SerializeField]
        public ConditionSO Condition{ get; private set; }
    }
}
