using System;
using Modules.Towers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Modules.Units
{
    public class TowerTargetingUnit : BaseUnit
    {
        [SerializeField] private float stopDistance = 1f;

        private TowerBehaviour targetTower = null;

        public void SetTargetTower(TowerBehaviour tower)
        {
            targetTower = tower;
            stopDistance = Random.Range(stopDistance - 2, stopDistance + 2);
        }

        protected virtual void Update()
        {
            if (targetTower == null)
                return;

            if (GetDistanceToTarget() < stopDistance)
            {
                unitGravityBody.SetToStop();
            }
        }

        private float GetDistanceToTarget()
        {
            return Mathf.Abs((targetTower.transform.position - transform.position).magnitude);
        }

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            if (targetTower != null)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(transform.position, targetTower.transform.position);
            }
        }
#endif
    }
}