using Modules.Towers;
using UnityEngine;

namespace Modules.Units
{
    public class RepairmanUnit : TowerTargetingUnit
    {
        private RepairmanUnitData repairUnitData;
        private WaitForSeconds repairCooldown;


        public override void SetData<T>(T data)
        {
            base.SetData(data);

            repairUnitData = data as RepairmanUnitData;
        }

        protected override void Update()
        {
            if (TargetTower == null)
                return;

            if (unitGravityBody.IsMoving && GetDistanceToTargetTower() < stopDistance)
            {
                unitGravityBody.SetToStop();
                RepairLoop();
            }
        }

        protected async void RepairLoop()
        {
            TowerBehaviour initialTowerTarget = TargetTower;
            while (!unitGravityBody.IsMoving && CurrentHealth > 0 && TargetTower == initialTowerTarget && TargetTower.CurrentHealth < 100)
            {
                initialTowerTarget = TargetTower;
                await new WaitForSeconds(0.5f);
                print($"Repaired tower {TargetTower.name} by {repairUnitData.RepairAmount}");
                TargetTower.Repair(repairUnitData.RepairAmount);
                await new WaitForSeconds(repairUnitData.RepairCooldown);
            }
        }
        
    }
}