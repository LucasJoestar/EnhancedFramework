// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedFramework.Core;
using EnhancedFramework.GameStates;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EnhancedFramework.SceneManagement {
    /// <summary>
    /// Base class to inherit your own scene manager from.
    /// <br/> Already contains a bunch of useful methods to dynamically load and unload both <see cref="SceneAsset"/> and <see cref="SceneBundle"/>.
    /// <para/>
    /// Inherit from this class to implement loading screen and various behaviours/informations during loading.
    /// </summary>
    /// <typeparam name="T">This class type, to be used as an <see cref="EnhancedSingleton{T}"/>.</typeparam>
    /// <typeparam name="U">The <see cref="GameState"/> to be applied when loading a scene.</typeparam>
    public abstract class EnhancedSceneManager<T, U> : EnhancedSingleton<T> where T : EnhancedSceneManager<T, U> where U : GameState, new() {
        #region Global Members
        public const int DefaultOperationPriority = 0;

        [Section("Enhanced Scene Manager")]

        [SerializeField, Enhanced, ReadOnly] protected int operationPriority = DefaultOperationPriority;

        /// <summary>
        /// Indicates if the game is currently loading a scene.
        /// </summary>
        public bool IsLoading => (loadingBundleOperations.Count != 0) || (loadingSceneOperations.Count != 0);

        /// <summary>
        /// Indicates if the game is currently loading a scene.
        /// </summary>
        public bool IsUnloading => (unloadingBundleOperations.Count != 0) || (unloadingSceneOperations.Count != 0);

        /// <summary>
        /// Indicates if the game is currently performing an asynchronous loading or unloading operation.
        /// </summary>
        public bool IsPerformingOperation => IsLoading || IsUnloading;

        // -----------------------

        protected U loadingState = null;

        // Operations are sorted in their respective priority order (execute from first to last).
        // Also useful for displaying the current operations progress.

        protected Stamp<LoadBundleAsyncOperation> loadingBundleOperations = new Stamp<LoadBundleAsyncOperation>(2);
        protected Stamp<UnloadBundleAsyncOperation> unloadingBundleOperations = new Stamp<UnloadBundleAsyncOperation>(2);

        protected Stamp<AsyncOperation> loadingSceneOperations = new Stamp<AsyncOperation>(2);
        protected Stamp<AsyncOperation> unloadingSceneOperations = new Stamp<AsyncOperation>(2);
        #endregion

        // --- Scene Bundles --- \\

        #region Bundle Unload Async
        /// <param name="_bundle">The <see cref="SceneBundle"/> the unload.</param>
        /// <inheritdoc cref="SceneBundle.UnloadAsync(UnloadSceneOptions)"/>
        public virtual UnloadBundleAsyncOperation UnloadSceneBundleAsync(SceneBundle _bundle, UnloadSceneOptions _options = UnloadSceneOptions.None) {
            UnloadBundleAsyncOperation _op = _bundle.UnloadAsync(_options);

            if (!_op.IsDone) {
                // Sort operations so that the first element is the currently active one.
                // If the priority is not modified outside this class, the stamp order should not be modified (first in, first out).
                unloadingBundleOperations.Add(_op, true);

                CreateLoadingState();

                _op.Priority = operationPriority--;
                _op.OnCompleted += OnSceneBundleUnloaded;
            }

            return _op;
        }

        // -----------------------

        protected virtual void OnSceneBundleUnloaded(UnloadBundleAsyncOperation _op) {
            _op.OnCompleted -= OnSceneBundleUnloaded;

            // If the stamp order was not modified, the elment to remove should be at the first index.
            unloadingBundleOperations.Remove(_op);
            OnOperationComplete();
        }
        #endregion

        #region Bundle Load Async
        /// <param name="_bundle"><inheritdoc cref="Doc(SceneAsset, SceneBundle)" path="/param[@name='_bundle']"/></param>
        /// <inheritdoc cref="SceneBundle.LoadAsync(LoadSceneMode)"/>
        public LoadBundleAsyncOperation LoadSceneBundleAsync(SceneBundle _bundle, LoadSceneMode _mode = LoadSceneMode.Additive) {
            return LoadSceneBundleAsync(_bundle, new LoadSceneParameters(_mode, LocalPhysicsMode.None));
        }

        /// <param name="_bundle"><inheritdoc cref="Doc(SceneAsset, SceneBundle)" path="/param[@name='_bundle']"/></param>
        /// <inheritdoc cref="SceneBundle.LoadAsync(LoadSceneParameters)"/>
        public virtual LoadBundleAsyncOperation LoadSceneBundleAsync(SceneBundle _bundle, LoadSceneParameters _parameters) {
            if (_bundle.IsLoaded) {
                // Do not load the bundle if it is already loaded.
                if (_parameters.loadSceneMode == LoadSceneMode.Additive) {
                    this.LogWarning($"The SceneBundle '{_bundle.name}' cannot be asynchronously additively loaded, because it is already loaded!");
                    return LoadBundleAsyncOperation.CompleteOperation;
                }

                // If wanting to reload the bundle, unload it first.
                UnloadSceneBundleAsync(_bundle, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
            }

            if (_parameters.loadSceneMode == LoadSceneMode.Single) {
                _parameters.loadSceneMode = LoadSceneMode.Additive;

                UnloadAllScenesExceptCore();
            }

            LoadBundleAsyncOperation _op = _bundle.LoadAsync(_parameters);

            if (!_op.IsDone) {
                // Sort operations so that the first element is the currently active one.
                // If the priority is not modified outside this class, the stamp order should not be modified (first in, first out).
                loadingBundleOperations.Add(_op, true);

                CreateLoadingState();

                _op.Priority = operationPriority--;
                _op.OnCompleted += OnSceneBundleLoaded;
            }

            return _op;
        }

        // -----------------------

        protected virtual void OnSceneBundleLoaded(LoadBundleAsyncOperation _op) {
            _op.OnCompleted -= OnSceneBundleLoaded;

            // If the stamp order was not modified, the elment to remove should be at the first index.
            loadingBundleOperations.Remove(_op);
            OnOperationComplete();
        }
        #endregion

        #region Bundle Load
        /// <param name="_bundle"><inheritdoc cref="Doc(SceneAsset, SceneBundle)" path="/param[@name='_bundle']"/></param>
        /// <inheritdoc cref="SceneBundle.LoadAsync(LoadSceneMode)"/>
        public void LoadSceneBundle(SceneBundle _bundle, LoadSceneMode _mode = LoadSceneMode.Additive) {
            LoadSceneBundle(_bundle, new LoadSceneParameters(_mode, LocalPhysicsMode.None));
        }

        /// <param name="_bundle"><inheritdoc cref="Doc(SceneAsset, SceneBundle)" path="/param[@name='_bundle']"/></param>
        /// <inheritdoc cref="SceneBundle.LoadAsync(LoadSceneParameters)"/>
        public virtual void LoadSceneBundle(SceneBundle _bundle, LoadSceneParameters _parameters) {
            if (_bundle.IsLoaded) {
                this.LogWarning($"The SceneBundle '{_bundle.name}' is already loaded! To properly reload it, please use 'LoadSceneBundleAsync' instead.");
                return;
            }

            if (_parameters.loadSceneMode == LoadSceneMode.Single) {
                _parameters.loadSceneMode = LoadSceneMode.Additive;

                UnloadAllScenesExceptCore();
            }

            _bundle.Load(_parameters);
        }
        #endregion

        // --- Scene Assets --- \\

        #region Asset Unload Async
        /// <param name="_scene">The <see cref="SceneAsset"/> to unload.</param>
        /// <inheritdoc cref="SceneAsset.UnloadAsync(out AsyncOperation, UnloadSceneOptions)"/>
        public virtual bool UnloadSceneAssetAsync(SceneAsset _scene, out AsyncOperation _operation, UnloadSceneOptions _options = UnloadSceneOptions.None) {
            if (!_scene.UnloadAsync(out _operation, _options)) {
                return false;
            }

            ManageUnloadingOperation(_operation);
            return true;
        }
        #endregion

        #region Scene Load Async
        /// <param name="_scene"><inheritdoc cref="Doc(SceneAsset, SceneBundle)" path="/param[@name='_scene']"/></param>
        /// <inheritdoc cref="SceneAsset.LoadAsync(out AsyncOperation, LoadSceneMode)"/>
        public bool LoadSceneAsync(SceneAsset _scene, out AsyncOperation _operation, LoadSceneMode _mode = LoadSceneMode.Additive) {
            return LoadSceneAsync(_scene, out _operation, _mode);
        }

        /// <param name="_scene"><inheritdoc cref="Doc(SceneAsset, SceneBundle)" path="/param[@name='_scene']"/></param>
        /// <inheritdoc cref="SceneAsset.LoadAsync(out AsyncOperation, LoadSceneParameters)"/>
        public virtual bool LoadSceneAsync(SceneAsset _scene, out AsyncOperation _operation, LoadSceneParameters _parameters) {
            if (_scene.IsLoaded) {
                // Do not load the scene if it is already loaded.
                if (_parameters.loadSceneMode == LoadSceneMode.Additive) {
                    this.LogWarning($"The Scene '{_scene.Name}' cannot be asynchronously additively loaded, because it is already loaded!");

                    _operation = null;
                    return false;
                }

                // If wanting to reload the asset, unload it first.
                if (!UnloadSceneAssetAsync(_scene, out _operation, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects)) {
                    _operation = null;
                    return false;
                }
            }

            if (_parameters.loadSceneMode == LoadSceneMode.Single) {
                _parameters.loadSceneMode = LoadSceneMode.Additive;

                UnloadAllScenesExceptCore();
            }

            if (!_scene.LoadAsync(out _operation, _parameters)) {
                return false;
            }

            if (!_operation.isDone) {
                // Sort operations so that the first element is the currently active one.
                // If the priority is not modified outside this class, the stamp order should not be modified (first in, first out).
                loadingSceneOperations.Add(_operation, true);

                CreateLoadingState();

                _operation.priority = operationPriority--;
                _operation.completed += OnSceneAssetLoaded;
            }

            return true;
        }

        // -----------------------

        protected virtual void OnSceneAssetLoaded(AsyncOperation _op) {
            _op.completed -= OnSceneAssetLoaded;

            // If the stamp order was not modified, the elment to remove should be at the first index.
            loadingSceneOperations.Remove(_op);
            OnOperationComplete();
        }
        #endregion

        #region Scene Load
        /// <param name="_scene"><inheritdoc cref="Doc(SceneAsset, SceneBundle)" path="/param[@name='_scene']"/></param>
        /// <inheritdoc cref="SceneAsset.UnloadAsync(out AsyncOperation, UnloadSceneOptions)"/>
        public Scene LoadScene(SceneAsset _scene, LoadSceneMode _mode = LoadSceneMode.Single) {
            return LoadScene(_scene, new LoadSceneParameters(_mode, LocalPhysicsMode.None));
        }

        /// <param name="_scene"><inheritdoc cref="Doc(SceneAsset, SceneBundle)" path="/param[@name='_scene']"/></param>
        /// <inheritdoc cref="SceneAsset.UnloadAsync(out AsyncOperation, LoadSceneParameters)"/>
        public virtual Scene LoadScene(SceneAsset _scene, LoadSceneParameters _parameters) {
            if (_scene.IsLoaded) {
                this.LogWarning($"The Scene '{_scene.Name}' is already loaded! To properly reload it, please use 'LoadSceneAssetAsync' instead.");
                return default;
            }

            if (_parameters.loadSceneMode == LoadSceneMode.Single) {
                _parameters.loadSceneMode = LoadSceneMode.Additive;

                UnloadAllScenesExceptCore();
            }

            return _scene.Load(_parameters);
        }
        #endregion

        // --- Scene --- \\

        #region Scene
        /// <summary>
        /// Unloads all loaded scenes except the core one.
        /// </summary>
        public void UnloadAllScenesExceptCore() {
            for (int i = SceneManager.sceneCount; i-- > 0;) {
                Scene _scene = SceneManager.GetSceneAt(i);

                if (!_scene.IsCoreScene()) {
                    UnloadScene(_scene);
                }
            }
        }

        protected virtual AsyncOperation UnloadScene(Scene _scene) {
            AsyncOperation _op = SceneManager.UnloadSceneAsync(_scene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
            ManageUnloadingOperation(_op);

            return _op;
        }

        protected virtual void ManageUnloadingOperation(AsyncOperation _operation) {
            if ((_operation == null) || _operation.isDone) {
                return;
            }

            unloadingSceneOperations.Add(_operation, true);
            CreateLoadingState();

            _operation.priority = operationPriority--;
            _operation.completed += OnSceneUnloaded;
        }

        // -----------------------

        protected virtual void OnSceneUnloaded(AsyncOperation _op) {
            _op.completed -= OnSceneUnloaded;

            // If the stamp order was not modified, the elment to remove should be at the first index.
            unloadingSceneOperations.Remove(_op);
            OnOperationComplete();
        }
        #endregion

        // --- Utility --- \\

        #region Utility
        /// <summary>
        /// Unloads unused assets and calls the garbage collector.
        /// </summary>
        public virtual void FreeMemorySpace() {
            Resources.UnloadUnusedAssets();
            GC.Collect();
        }

        protected virtual void OnOperationComplete() {
            if (!IsPerformingOperation) {
                operationPriority = DefaultOperationPriority;

                loadingState.RemoveState();
                loadingState = null;
            }
        }

        protected virtual void CreateLoadingState() {
            if (loadingState == null) {
                loadingState = GameState.CreateState<U>();
            }
        }
        #endregion

        #region Documentation
        #if UNITY_EDITOR
        /// <summary>
        /// Documentation only method.
        /// </summary>
        /// <param name="_scene">The <see cref="SceneAsset"/> to load.</param>
        /// <param name="_bundle">The <see cref="SceneBundle"/> to load.</param>
        private void Doc(SceneAsset _scene, SceneBundle _bundle) { }
        #endif
        #endregion
    }
}
