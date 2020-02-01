using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Modules.SceneSystem
{
    //CURRENT LIMITATION, SCENES CAN'T BE LOADED TWICE

    using UScene = UnityEngine.SceneManagement.Scene;

    public class SceneLoader
    {
        private readonly Dictionary<int, Scene> sceneMap = null;

        public SceneLoader(PreloadedSceneData sceneData = null)
        {
            sceneMap = new Dictionary<int, Scene>();

            if (sceneData == null)
                return;

            foreach (KeyValuePair<string, Scene> keyValuePair in sceneData.SceneMap)
            {
                sceneMap.Add(Animator.StringToHash(keyValuePair.Key), keyValuePair.Value);
            }
        }

        public async Task<Scene> LoadSceneAsync(string sceneName)
        {
            int hash = Animator.StringToHash(sceneName);

            if (!sceneMap.ContainsKey(hash))
            {
                sceneMap.Add(Animator.StringToHash(sceneName), null);
                await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            }

            UScene scene = SceneManager.GetSceneByName(sceneName);

            GameObject[] rootObjects = scene.GetRootGameObjects();

            foreach (GameObject rootObject in rootObjects)
            {
                Scene sceneTemp = rootObject.GetComponent<Scene>();
                if (sceneTemp == null)
                    continue;

                sceneMap[hash] = sceneTemp;

                if (sceneTemp.CanInitialize())
                    await sceneTemp.InitializeAsync();

                //sceneTemp.Enable();

                return sceneTemp;
            }

            return null;
        }

        public async Task UnloadSceneAsync(string sceneName)
        {
            await SceneManager.UnloadSceneAsync(sceneName);

            int hash = Animator.StringToHash(sceneName);

            if (!sceneMap.ContainsKey(hash))
                throw new KeyNotFoundException($"There is no scene loaded with the name {sceneName}");

            await sceneMap[hash].DeInitializeAsync();
            sceneMap.Remove(hash);
        }

        public Scene GetScene(int sceneHash)
        {
            if (!HasScene(sceneHash))
            {
                throw new UnknownSceneException(sceneHash);
            }

            return sceneMap[sceneHash];
        }

        public Scene GetScene(string sceneName)
        {
            return GetScene(Animator.StringToHash(sceneName));
        }

        public bool HasScene(string sceneName)
        {
            return HasScene(Animator.StringToHash(sceneName));
        }

        public bool HasScene(int hash)
        {
            return sceneMap.ContainsKey(hash);
        }
    }

    public class UnknownSceneException : Exception
    {
        public UnknownSceneException(string sceneName) : base($"Unknown scene with name {sceneName}")
        {
        }

        public UnknownSceneException(int sceneHash) : base($"Unknown scene with hash {sceneHash}")
        {
        }
    }
}