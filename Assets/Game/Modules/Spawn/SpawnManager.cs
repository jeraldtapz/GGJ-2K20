using System.Collections.Generic;
using Gravity;
using Modules.Pooling;
using Modules.Units;
using UnityEngine;
using Zenject;

namespace Modules.Game
{
    public class SpawnManager : MonoBehaviour
    {
        [Inject] private readonly PoolManager poolManager = null;

        [SerializeField] private GravityAttractor attractor = null;
        [SerializeField] private Transform allySpawnPoint = null;
        [SerializeField] private Transform enemySpawnPoint = null;
        [SerializeField] private Transform spawnParent = null;
        
        public Poolable Spawn(string id, bool isAlly = true)
        {
            Poolable poolable = poolManager.Spawn(id, isAlly ? allySpawnPoint.position : enemySpawnPoint.position, spawnParent);

            UnitGravityBody body = poolable.GetComponent<UnitGravityBody>();
            body.Initialize();
            body.SetAttractor(attractor);
            return poolable;
        }
    }
}