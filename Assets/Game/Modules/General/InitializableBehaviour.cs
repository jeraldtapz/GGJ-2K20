using Sirenix.OdinInspector;

namespace Modules.General
{
    public class InitializableBehaviour : SerializedMonoBehaviour, IInitializable
    {
        public virtual void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public virtual void Initialize(object data = null)
        {
            throw new System.NotImplementedException();
        }
    }
}