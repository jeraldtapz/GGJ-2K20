using UnityEngine;
using Zenject;

namespace Modules.SceneSystem
{
    public class SceneLoaderInstaller : MonoInstaller
    {
        [SerializeField] private PreloadedSceneData preloadedSceneData = null;

        public override void InstallBindings()
        {
            Container.Bind<SceneLoader>().To<SceneLoader>().AsSingle()
                .WithArguments(preloadedSceneData).NonLazy();
        }
    }
}