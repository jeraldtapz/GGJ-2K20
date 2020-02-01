using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;

namespace Modules.General
{
    public class CompositeInitializableBehaviour : SerializedMonoBehaviour, ICompositeInitializable
    {
        #region Serialized Fields

        [OdinSerialize] [TabGroup("Data", "Initialization")]
        private List<IInitializable> initializables = null;

        #endregion Serialized Fields

        #region Properties

        public List<IInitializable> Initializables => initializables;
        public List<IDisposable> Disposables { get; private set; } = null;

        #endregion Properties

        #region Public Methods

        /// <summary>
        /// Creates Disposables instance and initializes all initializables
        /// </summary>
        /// <param name="data"></param>
        public virtual void Initialize(object data = null)
        {
            Disposables = new List<IDisposable>();

            if (initializables == null)
                return;

            foreach (IInitializable initializable in Initializables)
            {
                initializable.Initialize(this);
            }

            Disposables.AddRange(initializables);
        }

        public virtual void Dispose()
        {
            foreach (IDisposable disposable in Disposables)
            {
                disposable.Dispose();
            }

            Disposables.Clear();
        }

        #endregion Public Methods
    }
}