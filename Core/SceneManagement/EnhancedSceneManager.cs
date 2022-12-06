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

using Object = UnityEngine.Object;

namespace EnhancedFramework.Core {
    /// <summary>
    /// All <see cref="EnhancedSceneManager"/> loading-related possible states.
    /// </summary>
    public enum LoadingState {
        Inactive                = 0,
        Request,
        Prepare,
        Start,
        Loading                 = 20,
        Unloading,
        FreeMemory,
        WaitForInitialization,
        Ready,
        Complete                = 99,
    }

    /// <summary>
    /// All <see cref="EnhancedSceneManager"/> unloading-related possible states.
    /// </summary>
    public enum UnloadingState {
        Inactive                = 0,
        Request,
        Prepare,
        Start,
        Unloading               = 20,
        FreeMemory,
        WaitForInitialization,
        Ready,
        Complete                = 99
    }

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
        /// This processor <see cref="Object"/> used for log.
        /// </summary>
        Object LogObject { get; }

        /// <summary>
        /// Indicates if the object has been fully initialized or is still processing.
        /// </summary>
        bool IsProcessing { get; }
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

        public override void OnStopLoading() {
            base.OnStopLoading();

            // Unpause when exiting the loading state.
            if (pauseGameOnLoading) {
                ChronosManager.Instance.RemoveOverride(chronosID);
            }
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
        public override UpdateRegistration UpdateRegistration => base.UpdateRegistration | UpdateRegistration.Init;

        #region Global Members
        public const int DefaultOperationPriority = 0;

        [Section("Enhanced Scene Manager")]

        [SerializeField]
        private SerializedType<ILoadingState> loadingStateType      = new SerializedType<ILoadingState>(SerializedTypeConstraint.None, typeof(DefaultLoadingGameState));

        [SerializeField]
        private SerializedType<IUnloadingState> unloadingStateType  = new SerializedType<IUnloadingState>(SerializedTypeConstraint.None, typeof(DefaultUnloadingGameState));

        [Space(10f)]

        [SerializeField, Enhanced, Required, Tooltip("The first scene to load when starting the game")] private SceneBundle firstScene = null;

        [Space(10f)]

        [SerializeField, Enhanced, ReadOnly] private LoadingState loadingState      = LoadingState.Inactive;
        [SerializeField, Enhanced, ReadOnly] private UnloadingState unloadingState  = UnloadingState.Inactive;

        [Space(10f)]

        [SerializeField, Enhanced, ReadOnly] private Set<SceneBundle> loadedBundles = new Set<SceneBundle>();

        [Space(10f), HorizontalLine(SuperColor.Grey, 1f), Space(10f)]

        [SerializeField, Enhanced, Block]
        private PolymorphValue<SceneManagerBehaviour> behaviour = new PolymorphValue<SceneManagerBehaviour>(SerializedTypeConstraint.None,
                                                                                                            typeof(DefaultSceneManagerBehaviour), "Behaviour");

        // -----------------------

        private readonly List<ILoadingProcessor> processors = new List<ILoadingProcessor>();

        // -------------------------------------------
        // Accessors
        // -------------------------------------------

        /// <summary>
        /// The current <see cref="Core.LoadingState"/> of this scene manager.
        /// </summary>
        public LoadingState LoadingState {
            get { return loadingState; }
        }

        /// <summary>
        /// The current <see cref="Core.UnloadingState"/> of this scene manager.
        /// </summary>
        public UnloadingState UnloadingState {
            get { return unloadingState; }
        }

        /// <summary>
        /// This scene manager <see cref="SceneManagerBehaviour"/> instance.
        /// <br/> Use this to call your own specific behaviours.
        /// </summary>
        public SceneManagerBehaviour Behaviour {
            get { return behaviour.Value; }
        }

        // -------------------------------------------
        // Events
        // -------------------------------------------

        /// <summary>
        /// Called when starting a new loading operation.
        /// </summary>
        public static event Action OnStartLoading   = null;

        /// <summary>
        /// Called when stoping the current loading operation.
        /// </summary>
        public static event Action OnStopLoading    = null;

        /// <summary>
        /// Called when starting a new unloading operation.
        /// </summary>
        public static event Action OnStartUnloading = null;

        /// <summary>
        /// Called when stoping the current unloading operation.
        /// </summary>
        public static event Action OnStopUnloading  = null;

        /// <summary>
        /// Called when the state for the current loading operation changes.
        /// </summary>
        public static event Action<LoadingState> OnLoadingState     = null;

        /// <summary>
        /// Called when the state for the current unloading operation changes.
        /// </summary>
        public static event Action<UnloadingState> OnUnloadingState = null;

        /// <summary>
        /// Called whenever after a <see cref="SceneBundle"/> has been loaded.
        /// </summary>
        public static event Action<SceneBundle> OnPostLoadBundle    = null;

        /// <summary>
        /// Called whenever before unloading a <see cref="SceneBundle"/>.
        /// </summary>
        public static event Action<SceneBundle> OnPreUnloadBundle   = null;
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
                BuildSceneDatabase _database = BuildSceneDatabase.Database;

                for (int i = 0; i < _database.SceneBundleCount; i++) {
                    SceneBundle _bundle = _database.GetSceneBundleAt(i);

                    // Simulate scene loading; a scene cannot be loaded twice anyway.
                    if (_bundle.IsLoaded) {
                        LoadSceneBundle(_bundle, LoadSceneMode.Additive);
                    }
                }

                // Default loading.
                if (loadingBundles.Count == 0) {
                    DoPerformLoading();
                }
            }
            #else
            LoadFirstScene();
            #endif

            // ----- Local Methods ----- \\

            void LoadFirstScene() {
                // Get this object scene bundle.
                if (SceneBundle.GetSceneBundle(gameObject.scene, out SceneBundle _bundle)) {
                    loadedBundles.Add(_bundle);
                }

                if (firstScene.IsValid()) {
                    Instance.LoadSceneBundle(firstScene, LoadSceneMode.Additive);
                }
            }
        }
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
        #endregion

        #region Loading
        private const float StartLoadDelay      = .2f;
        private const float LoadBundleInterval  = .01f;
        private const float LoadCompletionDelay = .25f;

        private readonly PairCollection<SceneBundle, LoadSceneMode> loadingBundles = new PairCollection<SceneBundle, LoadSceneMode>();

        private Coroutine loadingCoroutine = null;
        private GameState loadingGameState = null;

        // -------------------------------------------
        // Requests
        // -------------------------------------------

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
            if (!loadingGameState.IsActive()) {

                SetLoadingState(LoadingState.Request);
                loadingGameState = GameState.CreateState(loadingStateType);
            }
        }

        // -------------------------------------------
        // Game State Callbacks
        // -------------------------------------------

        /// <summary>
        /// /!\ Should only be called from a <see cref="LoadingGameState{T}"/> /!\
        /// <para/>
        /// Starts loading all previously requested <see cref="SceneBundle"/>.
        /// </summary>
        public void StartLoading() {
            #if DEVELOPMENT
            // This case should never happen. If it does, log an error.
            if (loadingCoroutine != null) {
                LogMessage("Starting a new loading operation while another is still running\nStoping the previous coroutine", LogType.Error);
                StopLoading();
            }
            #endif

            // Start loading.
            loadingCoroutine = StartCoroutine(PerformLoading());
        }

        /// <summary>
        /// /!\ Should only be called from a <see cref="LoadingGameState{T}"/> /!\
        /// <para/>
        /// Stops all active loading operation(s).
        /// </summary>
        public void StopLoading() {
            switch (loadingState) {
                // Ignore when inactive.
                case LoadingState.Inactive:
                    return;

                // Valid.
                case LoadingState.Complete:
                    break;

                // Prematurely stopped.
                default:
                    LogMessage("Loading prematurely canceled before completion", LogType.Warning);
                    StopCoroutine(loadingCoroutine);

                    Behaviour.OnCancelLoading();
                    break;
            }

            SetLoadingState(LoadingState.Inactive);
            Behaviour.OnStopLoading();

            OnStopLoading?.Invoke();
            loadingCoroutine = null;
        }

        // -------------------------------------------
        // Loading
        // -------------------------------------------

        private IEnumerator PerformLoading() {
            // Preparation.
            Behaviour.PrepareLoading(loadingBundles);
            SetLoadingState(LoadingState.Prepare);
            
            // Wait for all pending operations to be compelete.
            while ((unloadingState != UnloadingState.Inactive) || !Behaviour.StartLoading) {
                yield return null;
            }

            // Initialization.
            Behaviour.OnStartLoading();
            OnStartLoading?.Invoke();

            SetLoadingState(LoadingState.Start);
            yield return null;

            // Terminates all gameplay states.
            GameStateManager.Instance.PopNonPersistentStates();

            yield return new WaitForSecondsRealtime(StartLoadDelay);

            SetLoadingState(LoadingState.Loading);

            // Cache the yield instruction to avoid garbage.
            var _interval = new WaitForSecondsRealtime(LoadBundleInterval);

            for (int i = 0; i < loadingBundles.Count; i++) {
                var _pair = loadingBundles[i];
                SceneBundle _toLoad = _pair.First;
                LoadSceneMode _mode = _pair.Second;

                Behaviour.OnPreLoadBundle(_toLoad, _mode);

                // When wanting to load a bundle all alone,
                // simply unload all scenes except for core, then load it additively.
                if (_mode == LoadSceneMode.Single) {
                    SetLoadingState(LoadingState.Unloading);

                    foreach (SceneBundle _toUnload in loadedBundles) {
                        if (!_toUnload.IsCoreBundle()) {
                            UnloadSceneBundle(_toUnload, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
                        }
                    }

                    // Wait for unloading completion.
                    while (unloadingState != UnloadingState.Inactive) {
                        yield return null;
                    }

                    SetLoadingState(LoadingState.Loading);
                }

                // Loading operation.
                var _operation = _toLoad.LoadAsync(LoadSceneMode.Additive);
                Behaviour.OnLoadBundle(_operation, i, loadingBundles.Count);

                yield return _operation;
                yield return _interval;

                loadedBundles.Add(_toLoad);
                OnPostLoadBundle?.Invoke(_toLoad);
            }

            loadingBundles.Clear();

            // Free memory.
            SetLoadingState(LoadingState.FreeMemory);
            yield return FreeMemory();

            SetLoadingState(LoadingState.WaitForInitialization);

            // Once all scenes have been loaded, wait before checking initialized behaviours,
            // as some may not be awaken yet.
            yield return new WaitForSecondsRealtime(LoadCompletionDelay);

            // Wait for all behaviours to be fully loaded and initialized.
            while (IsAnyProcessorActive()) {
                yield return null;
            }

            SetLoadingState(LoadingState.Ready);
            Behaviour.OnLoadingReady();

            // Before completion, make sure the loading state is the current active one,
            // and let the behaviour complete its requested operations (like a press for any key).
            while (!loadingGameState.IsCurrentState || !Behaviour.CompleteLoading) {
                yield return null;
            }

            SetLoadingState(LoadingState.Complete);

            // Terminates the state.
            loadingGameState.RemoveState();
        }

        /// <summary>
        /// Sets the loading state of the object, and perform all related operations.
        /// </summary>
        private void SetLoadingState(LoadingState _state) {
            loadingState = _state;
            Behaviour.OnLoadingState(_state);

            OnLoadingState?.Invoke(_state);
        }
        #endregion

        #region Unloading
        private const float StartUnloadDelay        = .1f;
        private const float UnloadBundleInterval    = .01f;
        private const float UnloadCompletionDelay   = .05f;

        private readonly PairCollection<SceneBundle, UnloadSceneOptions> unloadingBundles = new PairCollection<SceneBundle, UnloadSceneOptions>();

        private Coroutine unloadingCoroutine = null;
        private GameState unloadingGameState = null;

        // -------------------------------------------
        // Requests
        // -------------------------------------------

        /// <summary>
        /// Requests to unload a specific <see cref="SceneBundle"/>.
        /// </summary>
        /// <param name="_bundle">The <see cref="SceneBundle"/> to unload.</param>
        /// <param name="_options">The options used to unload the bundle.</param>
        public void UnloadSceneBundle(SceneBundle _bundle, UnloadSceneOptions _options = UnloadSceneOptions.None) {
            unloadingBundles.Add(new Pair<SceneBundle, UnloadSceneOptions>(_bundle, _options));
            DoPerformUnloading();
        }

        private void DoPerformUnloading() {
            // Create a new unloading state if none.
            // Unloading will only start when the state ask for it.
            if (!unloadingGameState.IsActive()) {

                SetUnloadingState(UnloadingState.Request);
                unloadingGameState = GameState.CreateState(unloadingStateType);
            }
        }

        // -------------------------------------------
        // Game State Callbacks
        // -------------------------------------------

        /// <summary>
        /// /!\ Should only be called from a <see cref="UnloadingGameState{T}"/> /!\
        /// <para/>
        /// Starts unloading all previously requested <see cref="SceneBundle"/>.
        /// </summary>
        public void StartUnloading() {
            #if DEVELOPMENT
            // This case should never happen. If it does, log an error.
            if (unloadingCoroutine != null) {
                LogMessage("Starting a new unloading operation while another is still running\nStoping the previous coroutine", LogType.Error);
                StopUnloading();
            }
            #endif

            // Start unloading.
            unloadingCoroutine = StartCoroutine(PerformUnloading());
        }

        /// <summary>
        /// /!\ Should only be called from a <see cref="UnloadingGameState{T}"/> /!\
        /// <para/>
        /// Stops all active unloading operation(s).
        /// </summary>
        public void StopUnloading() {
            switch (unloadingState) {
                // Ignore when inactive.
                case UnloadingState.Inactive:
                    return;

                // Valid.
                case UnloadingState.Complete:
                    break;

                // Prematurely stopped.
                default:
                    LogMessage("Unloading prematurely canceled before completion", LogType.Warning);
                    StopCoroutine(unloadingCoroutine);

                    Behaviour.OnCancelUnloading();
                    break;
            }

            SetUnloadingState(UnloadingState.Inactive);
            Behaviour.OnStopUnloading();

            OnStopUnloading?.Invoke();
            unloadingCoroutine = null;
        }

        // -------------------------------------------
        // Unloading
        // -------------------------------------------

        private IEnumerator PerformUnloading() {
            // Preparation.
            Behaviour.PrepareUnloading(unloadingBundles);
            SetUnloadingState(UnloadingState.Prepare);

            // Wait while a loading operation is in process.
            while ((loadingState == LoadingState.Loading) || !Behaviour.StartUnloading) {
                yield return null;
            }

            // Initialization.
            Behaviour.OnStartUnloading();
            OnStartUnloading?.Invoke();

            SetUnloadingState(UnloadingState.Start);
            yield return new WaitForSecondsRealtime(StartUnloadDelay);

            SetUnloadingState(UnloadingState.Unloading);

            // Cache the yield instruction to avoid garbage.
            var _interval = new WaitForSecondsRealtime(UnloadBundleInterval);

            // Unload bundles.
            for (int i = 0; i < unloadingBundles.Count; i++) {
                var _pair = unloadingBundles[i];
                SceneBundle _bundle = _pair.First;
                UnloadSceneOptions _options = _pair.Second;

                // Before unloading the bundle, call the associated event
                // and wait for all processors to be ready.
                // This can be used to perform additional operations before destroying an object.
                Behaviour.OnPreUnloadBundle(_bundle, _options);
                OnPreUnloadBundle?.Invoke(_bundle);

                while (IsAnyProcessorActive()) {
                    yield return null;
                }

                // Unloading operation.
                var _operation = _bundle.UnloadAsync(_options);
                Behaviour.OnUnloadBundle(_operation, i, loadingBundles.Count);

                yield return _operation;
                yield return _interval;

                loadedBundles.Remove(_bundle);
                OnPostLoadBundle?.Invoke(_bundle);
            }

            unloadingBundles.Clear();

            // Free memory.
            SetUnloadingState(UnloadingState.FreeMemory);
            yield return FreeMemory();

            yield return new WaitForSecondsRealtime(UnloadCompletionDelay);

            SetUnloadingState(UnloadingState.Ready);
            Behaviour.OnUnloadingReady();

            // Wait for the behaviour to complete its associated operations.
            while (!Behaviour.CompleteUnloading) {
                yield return null;
            }

            SetUnloadingState(UnloadingState.Complete);

            // Terminates the state.
            unloadingGameState.RemoveState();
        }

        /// <summary>
        /// Sets the unloading state of the object, and perform all related operations.
        /// </summary>
        private void SetUnloadingState(UnloadingState _state) {
            unloadingState = _state;
            Behaviour.OnUnloadingState(_state);

            OnUnloadingState?.Invoke(_state);
        }
        #endregion

        #region Utility
        /// <summary>
        /// Indicates if any <see cref="ILoadingProcessor"/> is currently processing or not.
        /// </summary>
        public bool IsAnyProcessorActive() {
            int _index = processors.FindIndex(b => b.IsProcessing);

            if (_index != -1) {
                //this.Log("Waiting for " + processors[_index].LogObject.name);
                return true;
            }

            return false;
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
