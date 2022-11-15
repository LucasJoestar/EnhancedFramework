// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedFramework.Core.GameStates;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EnhancedFramework.Core {
    /// <summary>
    /// Interface used to indicate when an object has been fully initialized during a loading state.
    /// <br/> After loading scenes, the <see cref="EnhancedSceneManager"/>
    /// waits for all <see cref="ILoadingProcessor"/> to be initialized before moving to the next phase.
    /// <para/>
    /// You can register an object from the <see cref="EnhancedSceneManager"/> using
    /// <br/> <see cref="EnhancedSceneManager.RegisterProcessor(ILoadingProcessor)"/>
    /// and <see cref="EnhancedSceneManager.UnregisterProcessor(ILoadingProcessor)"/>.
    /// </summary>
    public interface ILoadingProcessor {
        #region Content
        /// <summary>
        /// Indicates if the object has been fully initialized or is still processing.
        /// </summary>
        bool IsProcessing { get; }
        #endregion
    }

    /// <summary>
    /// Use this interface to receive callbacks on the <see cref="EnhancedSceneManager"/> loading state.
    /// </summary>
    public interface ILoadingCallbackReceiver {
        #region Content
        /// <summary>
        /// Called when starting a loading operation.
        /// </summary>
        void OnStartLoading();

        /// <summary>
        /// Called whenever the current loading state of the <see cref="EnhancedSceneManager"/> change.
        /// </summary>
        /// <param name="_state">Current loading step operation.</param>
        void OnLoadingState(EnhancedSceneManager.LoadingState _state);

        /// <summary>
        /// Called when completing a loading operation.
        /// </summary>
        void OnStopLoading();
        #endregion
    }

    /// <summary>
    /// Use this interface to receive callbacks when the <see cref="EnhancedSceneManager"/> is unloading scenes.
    /// </summary>
    public interface IUnloadingCallbackReceiver {
        #region Content
        /// <summary>
        /// Called before a specific <see cref="SceneBundle"/> starts being unloaded.
        /// </summary>
        /// <param name="_bundle">The <see cref="SceneBundle"/> that is about to be unloaded.</param>
        public void OnPreUnloadBundle(SceneBundle _bundle);
        #endregion
    }

    /// <summary>
    /// Default <see cref="SceneManagerBehaviour"/>,
    /// allowing to pause the game when entering in a loading state.
    /// </summary>
    [Serializable, DisplayName("<Default>")]
    public class DefaultSceneManagerBehaviour : SceneManagerBehaviour {
        #region Global Members
        [SerializeField] private bool pauseGameOnLoading = true;
        #endregion

        #region Loading
        private const int ChronosPriority = 999;
        private readonly int chronosID = EnhancedUtility.GenerateGUID();

        // -----------------------

        public override void OnStartLoading() {
            base.OnStartLoading();

            // Pauses the game when entering in a loading state.
            if (pauseGameOnLoading) {
                ChronosManager.Instance.ApplyOverride(chronosID, 0f, ChronosPriority);
            }
        }

        public override bool CompleteLoading() {
            return true;
        }

        public override void OnStopLoading() {
            base.OnStopLoading();

            // Unpause when exiting the loading state.
            if (pauseGameOnLoading) {
                ChronosManager.Instance.RemoveOverride(chronosID);
            }
        }
        #endregion

        #region Unloading
        public override bool CompleteUnloading() {
            return true;
        }
        #endregion
    }

    /// <summary>
    /// Singleton instance managing the loading and unloading of scenes in the game.
    /// <br/> Deals with <see cref="SceneBundle"/> and the game core scene.
    /// <para/>
    /// To create and implement you own behaviours, simply create
    /// a new <see cref="SceneManagerBehaviour"/> class and assign it to this instance.
    /// <br/> Then, get access to it using the <see cref="Behaviour"/>
    /// property to call your own functions and receive multiple callbacks.
    /// </summary>
    public class EnhancedSceneManager : EnhancedSingleton<EnhancedSceneManager> {
        #region Loading State
        /// <summary>
        /// Used to reference the current state of this class when performing a loading operation.
        /// </summary>
        public enum LoadingState {
            Unloading = -1,
            Inactive = 0,
            Starting,
            Loading,
            FreeingMemory,
            WaitingForInitialization,
            WaitingForBehaviour,
            Complete
        }
        #endregion

        public override UpdateRegistration UpdateRegistration => base.UpdateRegistration | UpdateRegistration.Init;

        #region Global Members
        public const int DefaultOperationPriority = 0;

        [Section("Enhanced Scene Manager")]

        [SerializeField]
        private SerializedType<SceneManagerBehaviour> behaviourType = new SerializedType<SceneManagerBehaviour>(SerializedTypeConstraint.None, typeof(DefaultSceneManagerBehaviour));

        [Space(5f)]

        [SerializeField]
        private SerializedType<ILoadingState> loadingStateType = new SerializedType<ILoadingState>(SerializedTypeConstraint.None, typeof(DefaultLoadingGameState));

        [SerializeField]
        private SerializedType<IUnloadingState> unloadingStateType = new SerializedType<IUnloadingState>(SerializedTypeConstraint.None, typeof(DefaultUnloadingGameState));

        [Space(10f)]

        [SerializeField, Enhanced, Required, Tooltip("The first scene to load when starting the game")] private SceneBundle firstScene = null;
        [SerializeField, Enhanced, ReadOnly] private LoadingState state = LoadingState.Inactive;

        [Space(10f), HorizontalLine(SuperColor.Grey, 1f), Space(10f)]

        [SerializeReference, Block] private SceneManagerBehaviour behaviour = new DefaultSceneManagerBehaviour();

        // -----------------------

        /// <summary>
        /// The current state of this scene manager.
        /// </summary>
        public LoadingState State {
            get { return state; }
        }

        /// <summary>
        /// The behaviour class of this scene manager.
        /// <br/> Use this to call your own specific behaviours.
        /// </summary>
        public SceneManagerBehaviour Behaviour {
            get { return behaviour; }
        }

        // -----------------------

        private readonly List<ILoadingProcessor> processors                     = new List<ILoadingProcessor>();
        private readonly List<ILoadingCallbackReceiver> loadingCallbacks        = new List<ILoadingCallbackReceiver>();
        private readonly List<IUnloadingCallbackReceiver> unloadingCallbacks    = new List<IUnloadingCallbackReceiver>();
        #endregion

        #region Enhanced Behaviour
        protected override void OnInit() {
            base.OnInit();

            #if UNITY_EDITOR
            if (SceneManager.sceneCount == 1) {
                // Load the first scene if only core is loaded.
                LoadFirstScene();
            } else {
                // If all scenes are already loaded, enters in a loading state as soon as possible.
                // Used to prepare the scene and wait for all objects to be fully initialized.
                DoPerformLoading();
            }
            #else
            LoadFirstScene();
            #endif

            // ----- Local Methods ----- \\

            void LoadFirstScene() {
                if (firstScene.IsValid()) {
                    Instance.LoadSceneBundle(firstScene, LoadSceneMode.Additive);
                }
            }
        }

        // -----------------------

        #if UNITY_EDITOR
        private void Awake() {
            RefreshBehaviour();
        }

        private void OnValidate() {
            RefreshBehaviour();
        }

        // -----------------------

        private void RefreshBehaviour() {
            if (Application.isPlaying) {
                return;
            }

            // Create a new behaviour if its type changed.
            if (behaviourType.Type != behaviour.GetType()) {
                var _behaviour = Activator.CreateInstance(behaviourType.Type);
                behaviour = EnhancedUtility.CopyObjectContent(behaviour, _behaviour) as SceneManagerBehaviour;

                EditorUtility.SetDirty(this);
            }
        }
        #endif
        #endregion

        #region Registration
        /// <summary>
        /// Registers a new <see cref="ILoadingProcessor"/> instance.
        /// <para/>
        /// Should be called on object initialization. Keep in mind to unregister it on deactivation.
        /// </summary>
        /// <param name="_processor">The <see cref="ILoadingProcessor"/> to register.</param>
        public void RegisterProcessor(ILoadingProcessor _processor) {
            processors.Add(_processor);
        }

        /// <summary>
        /// Unregisters a specific <see cref="ILoadingProcessor"/> instance.
        /// <para/>
        /// Should be called when the object is deactivated or destroyed.
        /// </summary>
        /// <param name="_processor">The <see cref="ILoadingProcessor"/> to unregister.</param>
        public void UnregisterProcessor(ILoadingProcessor _processor) {
            processors.Remove(_processor);
        }

        // -----------------------

        /// <summary>
        /// Registers a new <see cref="ILoadingCallbackReceiver"/> instance.
        /// <para/>
        /// Should be called on object initialization. Keep in mind to unregister it on deactivation.
        /// </summary>
        /// <param name="_callback">The <see cref="ILoadingCallbackReceiver"/> to register.</param>
        public void RegisterCallbackReceiver(ILoadingCallbackReceiver _callback) {
            loadingCallbacks.Add(_callback);
        }

        /// <summary>
        /// Unregisters a specific <see cref="ILoadingCallbackReceiver"/> instance.
        /// <para/>
        /// Should be called when the object is deactivated or destroyed.
        /// </summary>
        /// <param name="_callback">The <see cref="ILoadingCallbackReceiver"/> to unregister.</param>
        public void UnregisterCallbackReceiver(ILoadingCallbackReceiver _callback) {
            loadingCallbacks.Remove(_callback);
        }

        // -----------------------

        /// <summary>
        /// Registers a new <see cref="IUnloadingCallbackReceiver"/> instance.
        /// <para/>
        /// Should be called on object initialization. Keep in mind to unregister it on deactivation.
        /// </summary>
        /// <param name="_callback">The <see cref="IUnloadingCallbackReceiver"/> to register.</param>
        public void RegisterUnloadingCallbackReceiver(IUnloadingCallbackReceiver _callback) {
            unloadingCallbacks.Add(_callback);
        }

        /// <summary>
        /// Unregisters a specific <see cref="IUnloadingCallbackReceiver"/> instance.
        /// <para/>
        /// Should be called when the object is deactivated or destroyed.
        /// </summary>
        /// <param name="_callback">The <see cref="IUnloadingCallbackReceiver"/> to unregister.</param>
        public void UnregisterUnloadingCallbackReceiver(IUnloadingCallbackReceiver _callback) {
            unloadingCallbacks.Remove(_callback);
        }
        #endregion

        #region Loading
        private const float BeforeUnloadingDelay = .2f;
        private const float AfterLoadingDelay = .25f;

        private readonly PairCollection<SceneBundle, LoadSceneMode> loadingBundles = new PairCollection<SceneBundle, LoadSceneMode>();

        private Coroutine loadingCoroutine = null;
        private GameState loadingState = null;

        // -----------------------

        /// <summary>
        /// /!\ Should only be called from a <see cref="LoadingGameState"/> /!\
        /// <para/>
        /// Starts loading all previously requested <see cref="SceneBundle"/>.
        /// </summary>
        public void StartLoading() {
            #if DEVELOPMENT
            if (loadingCoroutine != null) {
                this.LogError("SceneManager - Starting a new loading operation while another is still running | Stoping the previous coroutine");
                StopCoroutine(loadingCoroutine);
            }
            #endif

            // Loading.
            behaviour.OnSetupLoading(loadingBundles);
            loadingCoroutine = StartCoroutine(PerformLoading());

            // Callbacks.
            foreach (ILoadingCallbackReceiver _object in loadingCallbacks) {
                _object.OnStartLoading();
            }
        }

        /// <summary>
        /// /!\ Should only be called from a <see cref="LoadingGameState"/> /!\
        /// <para/>
        /// Stops all active loading operation(s).
        /// </summary>
        public void StopLoading() {
            if (state != LoadingState.Complete) {
                this.LogWarning("Scene Loading prematurely canceled before completion");
                StopCoroutine(loadingCoroutine);
            }

            // Callbacks.
            foreach (ILoadingCallbackReceiver _object in loadingCallbacks) {
                _object.OnStopLoading();
            }

            loadingCoroutine = null;

            SetLoadingState(LoadingState.Inactive);
            behaviour.OnStopLoading();
        }

        // -----------------------

        /// <summary>
        /// Requests to load a specific <see cref="SceneBundle"/>.
        /// </summary>
        /// <param name="_bundle">The <see cref="SceneBundle"/> to load.</param>
        /// <param name="_mode">The mode used to load the bundle.</param>
        public void LoadSceneBundle(SceneBundle _bundle, LoadSceneMode _mode) {
            loadingBundles.Add(new Pair<SceneBundle, LoadSceneMode>(_bundle, _mode));
            DoPerformLoading();
        }

        private void DoPerformLoading() {
            // Create a new loading state if none.
            // Loading will only start when the state ask for it.
            if (!loadingState.IsActive()) {
                loadingState = GameState.CreateState(loadingStateType);
            }
        }

        private IEnumerator PerformLoading() {
            // Wait for all unloading operations to be compelete.
            while (state == LoadingState.Unloading) {
                yield return null;
            }

            // Initialization.
            SetLoadingState(LoadingState.Starting);
            behaviour.OnStartLoading();

            yield return null;

            // Terminates all gameplay states.
            GameStateManager.Instance.PopNonPersistentStates();

            yield return new WaitForSecondsRealtime(BeforeUnloadingDelay);

            SetLoadingState(LoadingState.Loading);

            // Load bundles.
            for (int i = 0; i < loadingBundles.Count; i++) {
                var _pair = loadingBundles[i];
                SceneBundle _bundle = _pair.First;
                LoadSceneMode _mode = _pair.Second;

                behaviour.OnPreLoadBundle(_bundle, _mode);

                // When wanting to load the bundle all alone,
                // simply unload all scenes except core, then load it additively.
                if (_mode == LoadSceneMode.Single) {
                    for (int j = SceneManager.sceneCount; j-- > 0;) {
                        Scene _scene = SceneManager.GetSceneAt(i);

                        if (!_scene.IsCoreScene()) {
                            yield return SceneManager.UnloadSceneAsync(_scene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
                        }
                    }
                }

                var _operation = _bundle.LoadAsync(LoadSceneMode.Additive);
                behaviour.OnLoadBundle(_operation, i, loadingBundles.Count);

                yield return _operation;
            }

            loadingBundles.Clear();

            // Free memory.
            SetLoadingState(LoadingState.FreeingMemory);
            yield return FreeMemory();

            SetLoadingState(LoadingState.WaitingForInitialization);

            // Once all scenes have been loaded, wait before checking initialized behaviours,
            // as some may not be awaken yet.
            yield return new WaitForSecondsRealtime(AfterLoadingDelay);

            // Wait until all behaviours are loaded and initialized.
            while (IsAnyProcessorActive()) {
                yield return null;
            }

            SetLoadingState(LoadingState.WaitingForBehaviour);

            // Before completion, make sure the loading state is the current active one,
            // and let the behaviour complete its requested operations (like a press for any key).
            while (!loadingState.IsCurrentState || !behaviour.CompleteLoading()) {
                yield return null;
            }

            SetLoadingState(LoadingState.Complete);

            // Terminates state.
            loadingState.RemoveState();
        }
        #endregion

        #region Unloading
        private readonly PairCollection<SceneBundle, UnloadSceneOptions> unloadingBundles = new PairCollection<SceneBundle, UnloadSceneOptions>();

        private Coroutine unloadingCoroutine = null;
        private GameState unloadingState = null;

        // -----------------------

        /// <summary>
        /// /!\ Should only be called from a <see cref="UnloadingGameState"/> /!\
        /// <para/>
        /// Starts unloading all previously requested <see cref="SceneBundle"/>.
        /// </summary>
        public void StartUnloading() {
            #if DEVELOPMENT
            if (unloadingCoroutine != null) {
                this.LogError("SceneManager - Starting a new unloading operation while another is still running | Stoping the previous coroutine");
                StopCoroutine(unloadingCoroutine);
            }
            #endif

            // Unloading.
            behaviour.OnSetupUnloading(unloadingBundles);
            unloadingCoroutine = StartCoroutine(PerformUnloading());
        }

        /// <summary>
        /// /!\ Should only be called from a <see cref="UnloadingGameState"/> /!\
        /// <para/>
        /// Stops all active unloading operation(s).
        /// </summary>
        public void StopUnloading() {
            if (state == LoadingState.Unloading) {
                this.LogWarning("Scene Unloading prematurely canceled before completion");
                StopCoroutine(unloadingCoroutine);
            }

            unloadingCoroutine = null;

            SetLoadingState(LoadingState.Inactive);
            behaviour.OnStopUnloading();
        }

        // -----------------------

        /// <summary>
        /// Requests to unload a specific <see cref="SceneBundle"/>.
        /// </summary>
        /// <param name="_bundle">The <see cref="SceneBundle"/> to unload.</param>
        /// <param name="_options">The options used to unload the bundle.</param>
        public void UnloadSceneBundle(SceneBundle _bundle, UnloadSceneOptions _options = UnloadSceneOptions.None) {
            unloadingBundles.Add(new Pair<SceneBundle, UnloadSceneOptions>(_bundle, _options));

            // Create a new unloading state if none.
            // Unloading will only start when the state ask for it.
            if (!unloadingState.IsActive()) {
                unloadingState = GameState.CreateState(unloadingStateType);
            }
        }

        private IEnumerator PerformUnloading() {
            // Wait while a loading operation is in process.
            while (state != LoadingState.Inactive) {
                yield return null;
            }

            // Initialization.
            SetLoadingState(LoadingState.Unloading);
            behaviour.OnStartUnloading();

            // Unload bundles.
            for (int i = 0; i < unloadingBundles.Count; i++) {
                var _pair = unloadingBundles[i];
                SceneBundle _bundle = _pair.First;

                // Before unloading the bundle, call the associated callback on all registered objects,
                // and wait for all processors to be ready.
                // This can be used to perform additional operations before destroying an object.
                foreach (var item in unloadingCallbacks) {
                    item.OnPreUnloadBundle(_bundle);
                }

                while (IsAnyProcessorActive()) {
                    yield return null;
                }

                var _operation = _bundle.UnloadAsync(_pair.Second);
                behaviour.OnUnloadBundle(_operation, i, loadingBundles.Count);

                yield return _operation;
            }

            unloadingBundles.Clear();
            yield return FreeMemory();

            // Wait for the behaviour to be ready.
            while (behaviour.CompleteUnloading()) {
                yield return null;
            }

            SetLoadingState(LoadingState.Complete);

            // Terminates state.
            unloadingState.RemoveState();
        }
        #endregion

        #region Utility
        /// <summary>
        /// Indicates if any <see cref="ILoadingProcessor"/> is currently processing or not.
        /// </summary>
        public bool IsAnyProcessorActive() {
            return processors.Exists(b => b.IsProcessing);
        }

        /// <summary>
        /// Sets the loading state of the object, and perform all related operations.
        /// </summary>
        private void SetLoadingState(LoadingState _state) {
            state = _state;

            foreach (ILoadingCallbackReceiver _callback in loadingCallbacks) {
                _callback.OnLoadingState(_state);
            }
        }

        /// <summary>
        /// Free some memory space.
        /// <br/> Called once unloading operations have been complete.
        /// </summary>
        private AsyncOperation FreeMemory() {
            GC.Collect();
            return Resources.UnloadUnusedAssets();
        }
        #endregion
    }
}
