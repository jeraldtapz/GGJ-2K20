using Modules.Towers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Modules.Units
{
    public class TowerTargetingUnit : BaseUnit
    {
        [SerializeField] protected float stopDistance = 1f;

        protected TowerBehaviour TargetTower = null;
        protected TowerManager TowerManager = null;

        public void SetTargetTower(TowerBehaviour tower)
        {
            TargetTower = tower;
            stopDistance = Random.Range(stopDistance - 2, stopDistance + 2);
        }

        public void SetTowerManager(TowerManager tManager)
        {
            TowerManager = tManager;
        }

        protected virtual void Update()
        {
            if (TargetTower == null)
                return;

            if (GetDistanceToTargetTower() < stopDistance)
            {
                unitGravityBody.SetToStop();
            }
        }

        protected float GetDistanceToTargetTower()
        {
            Vector3 posA = TargetTower.transform.position;
            Vector3 posB = transform.position;
            posA.z = 0;
            posB.z = 0;
            return Mathf.Abs((posA - posB).magnitude);
        }
        
#if UNITY_EDITOR

        protected virtual void OnDrawGizmos()
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