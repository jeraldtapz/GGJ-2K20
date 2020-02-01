using Modules.Towers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Modules.Units
{
    public class TowerTargetingUnit : BaseUnit
    {
        [SerializeField] protected float stopDistance = 1f;

        protected TowerBehaviour TargetTower = null;

        public void SetTargetTower(TowerBehaviour tower)
        {
            TargetTower = tower;
            stopDistance = Random.Range(stopDistance - 2, stopDistance + 2);
        }

        protected virtual void Update()
        {
            if (TargetTower == null)
                return;

            if (GetDistanceToTarget() < stopDistance)
            {
                unitGravityBody.SetToStop();
            }
        }

        protected float GetDistanceToTarget()
        {
            return Mathf.Abs((TargetTower.transform.position - transform.position).magnitude);
        }

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            if (TargetTower != null)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(transform.position, TargetTower.transform.position);
            }
        }
#endif
    }
}