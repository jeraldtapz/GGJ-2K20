using JetBrains.Annotations;

namespace Modules.CameraSystem
{
    public interface IShake
    {
        [UsedImplicitly]
        void Shake(CameraShakeData data = null);
    }
}