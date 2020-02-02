using UnityEngine;

namespace Modules.Units
{
    [CreateAssetMenu(menuName = "GGJ/Attacker Unit Data")]
    public class AttackerUnitData : UnitData
    {
        public int AttackAmount;
        public int AttackCooldown;
    }
}