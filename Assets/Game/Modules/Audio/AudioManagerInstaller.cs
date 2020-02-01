using UnityEngine;
using Zenject;

namespace Modules.Audio
{
    public class AudioManagerInstaller : MonoInstaller
    {
        [SerializeField] private AudioManager audioManagerPrefab = null;
        public override void InstallBindings()
        {
            Container.Bind<AudioManager>().FromComponentInNewPrefab(audioManagerPrefab)
                .AsSingle().NonLazy();
        }
    }
}