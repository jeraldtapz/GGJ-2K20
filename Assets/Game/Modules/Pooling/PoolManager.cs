using Modules.CameraSystem;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Modules.Pooling
{
    [UsedImplicitly]
    public class PoolManager
    {
        private readonly Dictionary<int, PoolableData> objectPoolData;

        private readonly Dictionary<int, Stack<Poolable>> poolableMap;
        private Transform parentTransform;
        private RectTransform parentUITransform;
        private readonly GameCamera gameCamera;

        public PoolManager([Inject(Id = "Main Camera")] GameCamera cam, List<PoolData> poolData)
        {
            objectPoolData = new Dictionary<int, PoolableData>();
            poolableMap = new Dictionary<int, Stack<Poolable>>();
            gameCamera = cam;
            ProcessPoolData(poolData);
        }

        private void ProcessPoolData(IEnumerable<PoolData> poolData)
        {
            parentTransform = (new GameObject("Pool Parent")).transform;

            GameObject parentUIGameObject = new GameObject("Pool UI Parent");
            Canvas canvas = (Canvas)(parentUIGameObject.AddComponent(typeof(Canvas)));
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = gameCamera.GetComponent<Camera>();

            CanvasScaler scaler = parentUIGameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            parentUITransform = parentUIGameObject.GetComponent<RectTransform>();

            foreach (PoolData data in poolData)
            {
                foreach (KeyValuePair<string, PoolableData> pair in data.ObjectPoolData)
                {
                    int id = Animator.StringToHash(pair.Key);
                    poolableMap.Add(id, new Stack<Poolable>(5));
                    objectPoolData.Add(id, pair.Value);

                    Transform parent = pair.Value.Poolable.IsUI ? parentUITransform : parentTransform;

                    for (int i = 0; i < pair.Value.Quantity; i++)
                    {
                        Poolable obj = Object.Instantiate(pair.Value.Poolable, parent);
                        obj.Initialize(this);
                        poolableMap[id].Push(obj);
                    }
                }
            }
        }

        public Poolable Spawn(string id, Vector3 position, Transform parent = null)
        {
            return Spawn(Animator.StringToHash(id), position, parent);
        }

        public Poolable Spawn(int id, Vector3 position, Transform parent = null)
        {
            if (!poolableMap.ContainsKey(id))
                throw new KeyNotFoundException($"No Poolable found with id {id}");

            Poolable poolable;

            //If there's an available poolable
            if (poolableMap[id].Count > 0)
            {
                poolable = poolableMap[id].Pop();

                if (parent != null)
                    poolable.transform.SetParent(parent);

                poolable.Spawn();
            }
            else
            {
                //Create a new instance if there is none available
                poolable = Object.Instantiate(objectPoolData[id].Poolable,
                    objectPoolData[id].Poolable.IsUI ? parentUITransform : parentTransform);
                poolable.Initialize(this);

                //no need to put it in the map since we're going to pop it back anyways

                poolable.Spawn();
            }

            if (poolable.IsUI)
            {
                poolable.GetComponent<RectTransform>().anchoredPosition = position;
            }
            else
            {
                poolable.transform.position = position;
            }

            return poolable;
        }

        public void Despawn(Poolable poolable)
        {
            poolable.Despawn();
            poolable.transform.SetParent(poolable.IsUI ? parentUITransform : parentTransform);
            poolableMap[poolable.Hash].Push(poolable);
        }
    }
}