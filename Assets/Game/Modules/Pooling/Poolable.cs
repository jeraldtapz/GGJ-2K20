using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Modules.Pooling
{
    using IInitializable = General.IInitializable;
    
    public class Poolable : SerializedMonoBehaviour, IInitializable
    {
        #region Protected Fields

        protected PoolManager PoolManager = null;

        #endregion

        #region Serialized Fields

        [SerializeField] 
        [TabGroup("Data", "Poolable")]
        [FormerlySerializedAs("Id")]
        private string id = string.Empty;
        
        [SerializeField] 
        [TabGroup("Data", "Poolable")]
        [FormerlySerializedAs("IsUI")]
        private bool isUi = false;
        
        [SerializeField]
        [TabGroup("Data", "Poolable")]
        protected Animator animator = null;

        #endregion

        #region Properties

        public string Id => id;
        
        public bool IsUI => isUi;
        public int Hash { get; private set; }
        public Vector3 DefaultScale { get; private set; }
        
        public List<IDisposable> Disposables { get; private set; } = null;

        #endregion

        private void GetIdFromGameObjectName()
        {
            id = gameObject.name;
        }
        
        [Button]
        protected virtual void AutoInit()
        {
            GetIdFromGameObjectName();
            animator = GetComponent<Animator>();
        }
        
        public virtual void Initialize(object data = null)
        {
            PoolManager = (PoolManager)data;
            Hash = Animator.StringToHash(Id);
            gameObject.name = Id;
            Disposables = new List<IDisposable>();

            DefaultScale = transform.localScale;

            Disable();
        }

        public virtual void Spawn()
        {
            gameObject.SetActive(true);
            transform.localScale = DefaultScale;
        }

        /// <summary>
        /// This should be called when you want to return it to the pool
        /// Default implementation is to Disable the poolable but inheriting classes can override
        /// </summary>
        public virtual void Despawn()
        {
            Disable();
        }

        /// <summary>
        /// This should be called whenever you want to disable this poolable
        /// Doesn't necessarily mean that this poolable is back in the pool
        /// Default implementation is to set the game object to inactive
        /// Inheriting classes can alter the implementation
        /// </summary>
        
        /// <summary>
        /// This should be called when the poolable is the one that decides when it should go back to the pool
        /// </summary>
        public void DespawnSelf()
        {
            PoolManager.Despawn(this);
        }

        public void Dispose()
        {
            foreach (IDisposable disposable in Disposables)
            {
                disposable.Dispose();
            }
        }

        protected virtual void Disable()
        {
            gameObject.SetActive(false);
        }

        
    }
}