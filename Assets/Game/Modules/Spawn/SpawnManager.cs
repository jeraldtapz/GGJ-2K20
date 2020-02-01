using System.Collections.Generic;
using Modules.Pooling;
using UnityEngine;
using Zenject;

namespace Modules.Game
{
    public class SpawnManager : MonoBehaviour
    {
        [Inject] private readonly PoolManager poolManager = null;

        [SerializeField] private Transform spawnPoint = null;
        [SerializeField] private Transform spawnParent = null;
        
        public IEnumerable<Poolable> Spawn(string id, int quantity)
        {
            List<Poolable> poolables = new List<Poolable>();
            for (int i = 0; i < quantity; i++)
            {
                poolables.Add(poolManager.Spawn(id, spawnPoint.position, spawnParent));
            }

            return poolables;
        }
    }
}