using System.Collections.Generic;
using Gravity;
using Modules.Pooling;
using Modules.SceneSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Modules.Game
{
    public class GameScene : Scene
    {
        [SerializeField] private SpawnManager spawnManager = null;
        [SerializeField] private string debugUnitId = string.Empty;
        
        [Button]
        private void SpawnDebugUnits(int quantity, bool isRight)
        {
            IEnumerable<Poolable> poolables = spawnManager.Spawn(debugUnitId, quantity);

            foreach (Poolable poolable in poolables)
            {
                poolable.GetComponent<UnitGravityBody>().SetDirection(isRight);
            }
        }
    }
}
