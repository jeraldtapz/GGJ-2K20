using System.Collections.Generic;
using System.Linq;
using Modules.Towers;
using UnityEngine;

namespace Modules.Units
{
    public class AttackerUnit : TowerTargetingUnit
    {
        [SerializeField] private float attackRange;
        [SerializeField] private float aggroRange;
        
        private AttackerUnitData attackerUnitData;
        private WaitForSeconds attackCooldown;
        
        private Dictionary<int, BaseUnit> defenderMap;
        private Dictionary<int, BaseUnit> repairMap;
        private Dictionary<int, TowerBehaviour> towerMap;
        private BaseUnit currentTarget;
        private bool isOnAttackUnitsLoop = false;
        
        public override void Initialize(object data = null)
        {
            base.Initialize(data);
            defenderMap = new Dictionary<int, BaseUnit>();
            repairMap = new Dictionary<int, BaseUnit>();
            towerMap = new Dictionary<int, TowerBehaviour>();
        }
        
        public override void SetData<T>(T data)
        {
            base.SetData(data);

            attackerUnitData = data as AttackerUnitData;
        }

        public bool HasUnitsToAttack()
        {
            return defenderMap.Count > 0 || repairMap.Count > 0;
        }
        
        protected override void Update()
        {
            if (currentTarget != null)
            {
                if (isOnAttackUnitsLoop && !unitGravityBody.IsMoving)
                    return;
                
                Vector3 posA = transform.position;
                posA.z = 0;

                Vector3 posB = currentTarget.transform.position;
                posB.z = 0;

                if (GetDistanceToTargetUnit(posA, posB) < attackRange)
                {
                    unitGravityBody.SetToStop();
                    AttackUnitsLoop();
                }
                else
                {
                    unitGravityBody.SetToMove();
                }
            }
            else if (defenderMap.Count > 0)
            {
                currentTarget = defenderMap.ToArray()[0].Value;
            }
            else if (repairMap.Count > 0)
            {
                currentTarget = repairMap.ToArray()[0].Value;
            }
            else if ((unitGravityBody.IsMoving || isOnAttackUnitsLoop) && TargetTower != null && GetDistanceToTargetTower() < stopDistance)
            {
                isOnAttackUnitsLoop = false;
                unitGravityBody.SetToStop();
                AttackTowerLoop();
                // RepairLoop();t
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            BaseUnit baseUnit = other.GetComponent<BaseUnit>();

            if (baseUnit != null)
            {
                if (baseUnit.Role == UnitRole.Defend)
                {
                    defenderMap.Add(baseUnit.GetInstanceID(), baseUnit);
                }
                else if (baseUnit.Role == UnitRole.Repair)
                {
                    repairMap.Add(baseUnit.GetInstanceID(), baseUnit);
                }
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            BaseUnit baseUnit = other.GetComponent<BaseUnit>();

            if (baseUnit != null)
            {
                if (baseUnit.Role == UnitRole.Defend && defenderMap.ContainsKey(baseUnit.GetInstanceID()))
                {
                    defenderMap.Remove(baseUnit.GetInstanceID());
                }
                else if (baseUnit.Role == UnitRole.Repair && repairMap.ContainsKey(baseUnit.GetInstanceID()))
                {
                    repairMap.Remove(baseUnit.GetInstanceID());
                }
            }
        }
        
        protected async void AttackUnitsLoop()
        {
            isOnAttackUnitsLoop = true;
            BaseUnit baseUnit = currentTarget;
            while (!unitGravityBody.IsMoving && CurrentHealth > 0 && currentTarget == baseUnit && currentTarget.CurrentHealth > 0)
            {
                // if (TargetTower != null)
                //     return;
                
                baseUnit = currentTarget;
                await new WaitForSeconds(0.5f);
                print($"Attacked Unit {currentTarget.name} by {attackerUnitData.AttackAmount}");
                currentTarget.TakeDamage(attackerUnitData.AttackAmount);
                await new WaitForSeconds(attackerUnitData.AttackCooldown);
            }

            if (currentTarget.CurrentHealth <= 0)
            {
                if (currentTarget.Role == UnitRole.Repair)
                    repairMap.Remove(currentTarget.GetInstanceID());
                else if (currentTarget.Role == UnitRole.Defend)
                    defenderMap.Remove(currentTarget.GetInstanceID());
                
                currentTarget = null;
            }

            TargetTower = TowerManager.GetTowerToAttack(Direction);
            unitGravityBody.SetToMove();
        }

        protected async void AttackTowerLoop()
        {
            TowerBehaviour initialTowerTarget = TargetTower;
            while (!unitGravityBody.IsMoving && CurrentHealth > 0 && TargetTower == initialTowerTarget && TargetTower.CurrentHealth > 0 && currentTarget == null)
            {
                initialTowerTarget = TargetTower;
                await new WaitForSeconds(0.5f);
                print($"Attacked tower {TargetTower.name} by {attackerUnitData.AttackAmount}");
                TargetTower.TakeDamage(attackerUnitData.AttackAmount);
                await new WaitForSeconds(attackerUnitData.AttackCooldown);
            }

            TargetTower = null;
        }
        
        protected float GetDistanceToTargetUnit(Vector3 posA, Vector3 posB)
        {
            return Mathf.Abs((posA - posB).magnitude);
        }

        #region Debug

#if UNITY_EDITOR

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            
            if(currentTarget != null)
                Gizmos.DrawLine(transform.position, currentTarget.transform.position);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, aggroRange);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
#endif
        
        #endregion
    }
}