using Modules.CameraSystem;
using Modules.General;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Threading.Tasks;
using Modules.Audio;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

// ReSharper disable ConvertToAutoPropertyWithPrivateSetter
// ReSharper disable ConvertToNullCoalescingCompoundAssignment
// ReSharper disable ConvertToAutoProperty

namespace Modules.SceneSystem
{
    [RequireComponent(typeof(SceneContext))]
    public class Scene : CompositeInitializableBehaviour
    {
        #region Protected Collections

        protected Dictionary<int, Scene> LoadedSubScenesMap = null;
        protected Stack<string> LoadedSubScenesStack = null;

        #endregion Protected Collections

        #region Injected Fields

        [Inject] protected readonly SceneLoader SceneLoader = null;
        [Inject] protected readonly GameCamera Cam = null;
        [Inject] protected readonly AudioManager AudioManager = null;

        #endregion Injected Fields

        #region Serialized Fields

        [SerializeField]
        [TabGroup("Data", "Base Scene")]
        private Canvas[] canvases = null;

        [SerializeField]
        [TabGroup("Data", "Base Scene")]
        private string sceneName = null;

        [SerializeField] [TabGroup("Data", "Base Scene")]
        protected AudioSet[] audioSets;

        #endregion Serialized Fields

        #region Protected Fields

        protected int InitializationStartOnFrame = -1;
        protected int InitializationEndOnFrame = -1;

        #endregion Protected Fields

        #region Properties

        public string SceneName => sceneName;

        public bool IsInitialized { get; protected set; }

        #endregion Properties

        #region Public Methods

        public virtual async Task InitializeAsync(bool setToInitialized = true)
        {
#if UNITY_EDITOR

            if (!CanInitialize())
                return;
#endif

            print($"Initializing scene: {SceneName} at {Time.frameCount}");

            InitializationStartOnFrame = Time.frameCount;

            Initialize(this);

            LoadedSubScenesStack = new Stack<string>();
            LoadedSubScenesMap = new Dictionary<int, Scene>();

            SetupCanvas();
            
            //audio
            if (audioSets != null)
            {
                foreach (var audioSet in audioSets)
                {
                    AudioManager.LoadAudioSet(audioSet);
                }
            }

            if (setToInitialized)
            {
                IsInitialized = true;
                InitializationEndOnFrame = Time.frameCount;
            }

            await new WaitForEndOfFrame();
        }

        public virtual async Task DeInitializeAsync()
        {
            print($"Uninitializing scene: {SceneName} at {Time.frameCount}");

            if(audioSets != null)
                foreach (AudioSet audioSet in audioSets)
                {
                    AudioManager.UnloadAudioSet(audioSet.AudioSetName);
                }

            base.Dispose();

            IsInitialized = false;
            await new WaitForEndOfFrame();
        }

        public virtual void Enable()
        {
            gameObject.SetActive(true);
        }

        public virtual void Disable()
        {
            gameObject.SetActive(false);
        }

        public virtual async void GoToScene(string sceneToLoad)
        {
            int hash = Animator.StringToHash(sceneToLoad);

            if (LoadedSubScenesMap.ContainsKey(hash))
            {
                LoadedSubScenesMap[hash].Enable();
                LoadedSubScenesStack.Push(sceneToLoad);
            }
            else
            {
                LoadedSubScenesStack.Push(sceneToLoad);
                Scene scene = await SceneLoader.LoadSceneAsync(sceneToLoad);
                scene.Enable();
                LoadedSubScenesMap.Add(hash, scene);
            }
        }

        public virtual void UnloadLastScene()
        {
            int hash = Animator.StringToHash(LoadedSubScenesStack.Pop());
            LoadedSubScenesMap[hash].Disable();
        }

        public bool CanInitialize()
        {
            return !IsInitialized && InitializationStartOnFrame < 0;
        }

        #endregion Public Methods

        #region Debug

#if UNITY_EDITOR || UNITY_STANDALONE && HIRA_DEV

        protected virtual async void Start()
        {
            if (CanInitialize())
                await InitializeAsync();
        }

#endif

        #endregion Debug

        #region Private Methods

        [ContextMenu(itemName: "Get Name from Scene", false, 0)]
        private void GetSceneName()
        {
            sceneName = gameObject.scene.name;
        }

        private void SetupCanvas()
        {
            if (canvases == null)
                return;
            foreach (Canvas canvas in canvases)
            {
                canvas.worldCamera = Cam.GetComponent<Camera>();

                CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
                scaler.referenceResolution = new Vector2(1920f, 1080f);
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            }
        }

        #endregion Private Methods
    }
}