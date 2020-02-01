using System;
using System.Collections.Generic;
using Modules.CameraSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Modules.Audio
{
    public class AudioManager : SerializedMonoBehaviour
    {
        [Inject(Id = "Main Camera")] private readonly GameCamera mainCamera = null;
        
        [SerializeField] private List<AudioSource> sfxAudioSources = null;
        [SerializeField] private List<AudioSource> bgmAudioSources = null;
        [SerializeField] private Transform sfxAudioSourceParent = null;
        [SerializeField] private Transform bgmAudioSourceParent = null;
        [SerializeField] private GameObject audioSourcePrefab = null;
        
        [ShowInInspector]
        private Dictionary<int, AudioClip> sfxMap;
        
        [ShowInInspector]
        private Dictionary<int, AudioClip> bgmMap;
        
        [ShowInInspector]
        private Dictionary<int, AudioSet> loadedAudioSets;

        private Stack<AudioSource> availableBgmAudioSources = null;
        private Stack<AudioSource> availableSfxAudioSources = null;

        private Dictionary<int, AudioSource> currentlyPlayingBgmAudioSources = null;
        private Dictionary<int, AudioSource> currentlyPlayingSfxAudioSources = null;

        private float sfxVolume;
        private float bgmVolume;
        private float masterVolume;

        private Dictionary<int, AudioSource> playingBgmCache = null;
        private Vector3 posCache = Vector3.zero;

        private void Awake()
        {
            sfxMap = new Dictionary<int, AudioClip>();
            bgmMap = new Dictionary<int, AudioClip>();
            loadedAudioSets = new Dictionary<int, AudioSet>();
            
            availableBgmAudioSources = new Stack<AudioSource>();
            availableSfxAudioSources = new Stack<AudioSource>();
            
            currentlyPlayingBgmAudioSources = new Dictionary<int, AudioSource>();
            currentlyPlayingSfxAudioSources = new Dictionary<int, AudioSource>();

            playingBgmCache = new Dictionary<int, AudioSource>();

            foreach (AudioSource bgmAudioSource in bgmAudioSources)
            {
                availableBgmAudioSources.Push(bgmAudioSource);
            }

            foreach (AudioSource sfxAudioSource in sfxAudioSources)
            {
                availableSfxAudioSources.Push(sfxAudioSource);
            }

            sfxVolume = 1f;
            bgmVolume = 1f;
            masterVolume = 1f;
        }

        public void LoadAudioSet(AudioSet audioSet)
        {
            print($"Attempting to load AudioSet:{audioSet}");
            
            foreach (KeyValuePair<string,AudioClip> bgm in audioSet.BgmMap)
            {
                int hash = Animator.StringToHash(bgm.Key);
                
                if(bgmMap.ContainsKey(hash))
                    throw new Exception($"The BGM key {bgm.Key} of audio set {audioSet.AudioSetName} is already loaded");
                
                bgmMap.Add(hash, bgm.Value);
            }

            foreach (KeyValuePair<string, AudioClip> sfx in audioSet.SfxMap)
            {
                int hash = Animator.StringToHash(sfx.Key);
                
                if(sfxMap.ContainsKey(hash))
                    throw new Exception($"The SFX key {sfx.Key} of audio set {audioSet.AudioSetName} is already loaded");
                
                sfxMap.Add(hash, sfx.Value);
            }
            
            loadedAudioSets.Add(Animator.StringToHash(audioSet.AudioSetName), audioSet);
            
            print($"Successfully loaded AudioSet:{audioSet}");
        }

        public void UnloadAudioSet(string audioSetName)
        {
            print($"Attempting to unload AudioSet:{audioSetName}");
            
            int hash = Animator.StringToHash(audioSetName);
            
            if(!loadedAudioSets.ContainsKey(hash))
                throw new Exception($"Can't unload audio set with name {audioSetName}, it doesn't exist");

            AudioSet audioSetToRemove = loadedAudioSets[hash];

            foreach (KeyValuePair<string,AudioClip> keyValuePair in audioSetToRemove.BgmMap)
            {
                bgmMap.Remove(Animator.StringToHash(keyValuePair.Key));
            }

            foreach (KeyValuePair<string,AudioClip> keyValuePair in audioSetToRemove.SfxMap)
            {
                sfxMap.Remove(Animator.StringToHash(keyValuePair.Key));
            }

            loadedAudioSets.Remove(hash);
            
            print($"Successfully unloaded AudioSet: {audioSetName}");
        }

        public void UnloadAll()
        {
            MonoBehaviour.print($"Unloading all loaded audio");
            sfxMap.Clear();
            bgmMap.Clear();
            loadedAudioSets.Clear();
        }

        public async void PlaySfx(string id, Transform position = null, float volume = -1f, bool loop = false, bool fadeIn = false)
        {
            int hash = Animator.StringToHash(id);
            
            if(!sfxMap.ContainsKey(hash))
                throw new Exception($"Can't find sfx with id {id}");

            if (availableSfxAudioSources.Count == 0)
                SpawnAdditionalSfxAudioSources(5);
            
            //get values
            AudioSource source = availableSfxAudioSources.Pop();
            AudioClip clip = sfxMap[hash];
            float length = clip.length;
                
            //set volume
            if (volume < 0)
                source.volume = sfxVolume * masterVolume;
            else
                source.volume = volume;
            
            //set loop
            source.loop = loop;
            
            //set position
            posCache = position == null ? mainCamera.gameObject.transform.position : position.position;
            posCache.z = mainCamera.gameObject.transform.position.z;
            source.gameObject.transform.position = posCache;
            
            //set clip
            source.clip = clip;
            source.Play();
            currentlyPlayingSfxAudioSources.Add(source.GetInstanceID(), source);
            
            await new WaitForSeconds(length);
            
            currentlyPlayingSfxAudioSources.Remove(source.GetInstanceID());
            ReturnSourceToPool(false, source);
        }
        
        public async void PlaySfxDelayed(string id, float delay = 0f, Transform position = null, float volume = -1f, bool loop = false, bool fadeIn = false)
        {
            if (delay > 0)
                await new WaitForSeconds(delay);
            
            PlaySfx(id, position, volume, loop, fadeIn);
        }
        
        public async void PlayBgm(string id, float volume = -1f, bool loop = true, bool fadeIn = true)
        {
            int hash = Animator.StringToHash(id);
            
            if(!bgmMap.ContainsKey(hash))
                throw new Exception($"Can't find bgm with id {id}");

            if (availableBgmAudioSources.Count == 0)
                SpawnAdditionalBgmAudioSources(5);
            
            //get values
            AudioSource source = availableBgmAudioSources.Pop();
            AudioClip clip = bgmMap[hash];
            float length = clip.length;
            
            //set volume
            if (volume < 0)
                source.volume = bgmVolume * masterVolume;
            else
                source.volume = volume;
            
            
            //set loop
            source.loop = loop;
            
            //set clip
            source.clip = clip;
            
            source.Play();
            currentlyPlayingBgmAudioSources.Add(source.GetInstanceID(), source);


            if (!loop)
            {
                await new WaitForSeconds(length);
            
                currentlyPlayingBgmAudioSources.Remove(source.GetInstanceID());
            
                ReturnSourceToPool(true, source);
            }
        }

        public async void StopBgm()
        {
            foreach (KeyValuePair<int,AudioSource> currentlyPlayingBgmAudioSource in currentlyPlayingBgmAudioSources)
            {
                ReturnSourceToPool(true, currentlyPlayingBgmAudioSource.Value);
            }
            currentlyPlayingBgmAudioSources.Clear();
        }

        public async void StopBgmFade(float duration)
        {
            float timer = 0f;
            
            playingBgmCache.Clear();
            float[] currentVolumes = new float[currentlyPlayingBgmAudioSources.Count];

            int index = 0;
            foreach (KeyValuePair<int,AudioSource> bgm in currentlyPlayingBgmAudioSources)
            {
                playingBgmCache.Add(bgm.Key, bgm.Value);
                currentVolumes[index++] = bgm.Value.volume;
            }
            
            currentlyPlayingBgmAudioSources.Clear();

            while (timer < duration)
            {
                timer += Time.deltaTime;
                index = 0;
                foreach (KeyValuePair<int,AudioSource> bgm in playingBgmCache)
                {
                    bgm.Value.volume = Mathf.Lerp(currentVolumes[index], 0, timer / duration);
                    index++;
                }
                await new WaitForEndOfFrame();
            }

            foreach (KeyValuePair<int,AudioSource> keyValuePair in playingBgmCache)
            {
                ReturnSourceToPool(true, keyValuePair.Value);
            }
        }

        public void SetBgmVolume(float volume)
        {
            bgmVolume = volume;
        }

        public void SetSfxVolume(float volume)
        {
            sfxVolume = volume;
        }

        public void SetMasterVolume(float volume)
        {
            masterVolume = volume;
        }

        private void SpawnAdditionalSfxAudioSources(int count)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject obj = Instantiate(audioSourcePrefab, sfxAudioSourceParent);
                obj.name = "Sfx AudioSource Additional";
                AudioSource source = obj.GetComponent<AudioSource>();
                // source.loop = false;
                // source.playOnAwake = false;
                // source.clip = null;
                // source.spatialBlend = 1f;
                // source.maxDistance = 50f;
                // source.dopplerLevel = 0;

                // obj.transform.parent = sfxAudioSourceParent;
                
                availableSfxAudioSources.Push(source);
            }
        }

        private void SpawnAdditionalBgmAudioSources(int count)
        {
            for (int i = 0; i < count; i++)
            {       
                GameObject obj = new GameObject("Bgm AudioSource Additional");
                AudioSource source = obj.AddComponent<AudioSource>();
                source.loop = true;
                source.playOnAwake = false;
                source.clip = null;

                obj.transform.parent = bgmAudioSourceParent;
                
                availableBgmAudioSources.Push(source);
            }
        }

        private void ReturnSourceToPool(bool isBgm, AudioSource audioSource)
        {
            audioSource.Stop();
            audioSource.clip = null;
            if (isBgm)
            {
                availableBgmAudioSources.Push(audioSource);                
            }
            else
            {
                availableSfxAudioSources.Push(audioSource);
            }
        }
    }
}