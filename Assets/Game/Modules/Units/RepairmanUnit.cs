namespace Modules.Units
{
    public class RepairmanUnit : TowerTargetingUnit
    {
        protected override void Update()
        {
            if (TargetTower == null)
                return;

            if (GetDistanceToTarget() < stopDistance)
            {
                unitGravityBody.SetToStop();
            }
        }
    }
}