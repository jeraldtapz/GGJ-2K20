using UnityEngine;
using Zenject;

namespace Modules.CameraSystem
{
    public class CameraInstaller : MonoInstaller
    {
        [SerializeField] private GameCamera mainCamera = null;

        public override void InstallBindings()
        {
            Container.Bind<GameCamera>().WithId("Main Camera")
                .FromComponentInNewPrefab(mainCamera).AsSingle().NonLazy();
        }
    }
}