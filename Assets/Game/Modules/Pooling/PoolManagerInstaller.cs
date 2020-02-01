using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Modules.Pooling
{
    public class PoolManagerInstaller : MonoInstaller
    {
        [SerializeField]
        private List<PoolData> poolData = null;

#if UNITY_EDITOR
        private Dictionary<int, PoolData> hashes = null;
#endif

        public override void InstallBindings()
        {
            Container.Bind<PoolManager>().To<PoolManager>().AsSingle()
                .WithArguments(poolData);
        }

        #region Editor Only

#if UNITY_EDITOR
        
        private void OnValidate()
        {
            if(hashes == null)
                hashes = new Dictionary<int, PoolData>();
            
            foreach (PoolData data in poolData)
            {
                foreach (KeyValuePair<string, PoolableData> poolDataKvp in data.ObjectPoolData)
                {
                    if (hashes.ContainsKey(Animator.StringToHash(poolDataKvp.Key)))
                        throw new Exception(
                            $"ID {poolDataKvp.Key} of poolable {poolDataKvp.Value.Poolable.gameObject.name} already exists in " +
                            $"{hashes[Animator.StringToHash(poolDataKvp.Key)]}");
                    else
                    {
                        hashes.Add(Animator.StringToHash(poolDataKvp.Key), data);
                    }
                }
            }
        }
        

        [Button]
        private void Validate()
        {
            OnValidate();
        }
        
#endif
        
        #endregion
    }
}