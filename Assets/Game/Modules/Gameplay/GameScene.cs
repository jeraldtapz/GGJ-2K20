using Game.Modules;
using Modules.General;
using Modules.SceneSystem;
using Modules.Towers;
using Modules.Units;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Modules.Game
{
    public class GameScene : Scene
    {
        [SerializeField] private SpawnManager spawnManager = null;
        [SerializeField] private string debugUnitId = string.Empty;
        [SerializeField] private int initialGoldValue = 100;

        [SerializeField] private TowerBehaviour towerBehaviourA = null;
        

        private GoldManager goldManager = null;

        public override void Initialize(object data = null)
        {
            base.Initialize(data);
            
            goldManager = new GoldManager(initialGoldValue, 7.5f);
        }
        
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.J))
                SpawnDebugUnitsLeft(1);
            if(Input.GetKeyDown(KeyCode.K))
                SpawnDebugUnitsRight(1);
        }


        [Button]
        private async void SpawnDebugUnitsLeft(int quantity)
        {
            for (int i = 0; i < quantity; i++)
            {
                TowerTargetingUnit towerTargetingUnit = spawnManager.Spawn(debugUnitId).GetComponent<TowerTargetingUnit>();
                towerTargetingUnit.SetDirection(Direction.Left);
                towerTargetingUnit.SetTargetTower(towerBehaviourA);
                await new WaitForSeconds(0.5f);
            }
        }
        
        [Button]
        private async void SpawnDebugUnitsRight(int quantity)
        {
            for (int i = 0; i < quantity; i++)
            {
                spawnManager.Spawn(debugUnitId).GetComponent<BaseUnit>().SetDirection(Direction.Right);
                await new WaitForSeconds(0.5f);
            }
        }
    }
}
