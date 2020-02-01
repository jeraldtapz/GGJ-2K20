using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR

using UnityEditor;

#endif


namespace Modules.Pooling
{
    [CreateAssetMenu(menuName = "FALLEN TEAR/Pool Data")]
    public class PoolData : SerializedScriptableObject
    {
        [OdinSerialize]
        public Dictionary<string, PoolableData> ObjectPoolData;


#if UNITY_EDITOR
        
        [Button]
        private void SetKeysFromPoolableId()
        {
             Undo.RecordObject(this, name);
            Dictionary<string, PoolableData> newPool = new Dictionary<string, PoolableData>();
            
            foreach (KeyValuePair<string,PoolableData> kvp in ObjectPoolData)
            {
                newPool.Add(kvp.Value.Poolable.Id, kvp.Value);
            }
            
            ObjectPoolData.Clear();

            foreach (KeyValuePair<string,PoolableData> poolableData in newPool)
            {
                ObjectPoolData.Add(poolableData.Key, poolableData.Value);
            }
        }
#endif

    }

    public class PoolableData
    {
        public int Quantity;
        public Poolable Poolable;
    }
}