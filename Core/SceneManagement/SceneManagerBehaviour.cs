// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using System;
using UnityEngine.SceneManagement;

namespace EnhancedFramework.Core {
    /// <summary>
    /// Base class to inherit you own <see cref="EnhancedSceneManager"/> behaviours from.
    /// <br/> Receives callbacks on loading, and can be used to perform additional operations.
    /// </summary>
    [Serializable]
    public abstract class SceneManagerBehaviour {
        #region Loading
        /// <summary>
        /// Holds the start of the current loading while false.
        /// </summary>
        public virtual bool StartLoading {
            get { return true; }
        }

        /// <summary>
        /// Holds the completion of the current loading while false.
        /// </summary>
        public virtual bool CompleteLoading {
            get { return true; }
        }

        // -------------------------------------------
        // General
        // -------------------------------------------

        /// <summary>
        /// Prepare a new loading operation.
        /// </summary>
        /// <param name="_loadingBundles">All <see cref="SceneBundle"/> to load, with their associated <see cref="LoadSceneMode"/>.</param>
        public virtual void PrepareLoading(in PairCollection<SceneBundle, LoadSceneMode> _loadingBundles) { }

        /// <summary>
        /// Called when starting the current loading operation.
        /// </summary>
        public virtual void OnStartLoading() { }

        /// <summary>
        /// Called whenever the current loading state of the <see cref="EnhancedSceneManager"/> changes.
        /// </summary>
        /// <param name="_state">Current loading step operation.</param>
        public virtual void OnLoadingState(LoadingState _state) { }

        // -------------------------------------------
        // Scene Bundle
        // -------------------------------------------

        /// <summary>
        /// Called just before starting a new <see cref="SceneBundle"/> loading operation.
        /// </summary>
        /// <param name="_bundle">The <see cref="SceneBundle"/> to load.</param>
        /// <param name="_mode"><see cref="LoadSceneMode"/> used to load this bundle.</param>
        public virtual void OnPreLoadBundle(SceneBundle _bundle, LoadSceneMode _mode) { }

        /// <summary>
        /// Called when starting a new <see cref="SceneBundle"/> loading operation.
        /// </summary>
        /// <param name="_operation">The current loading operation.</param>
        /// <param name="_index">The index of the current loading operation.</param>
        /// <param name="_count">The total count of all loading operations to come.</param>
        public virtual void OnLoadBundle(LoadBundleAsyncOperation _operation, int _index, int _count) { }

        // -------------------------------------------
        // Completion
        // -------------------------------------------

        /// <summary>
        /// Called once the current loading operation is ready to be completed.
        /// <br/> Use <see cref="CompleteLoading"/> to indicate when the operation can be completed.
        /// </summary>
        public virtual void OnLoadingReady() { }

        /// <summary>
        /// Called when the current loading operation has been canceled.
        /// </summary>
        public virtual void OnCancelLoading() { }

        /// <summary>
        /// Called when the current loading operation has stopped.
        /// </summary>
        public virtual void OnStopLoading() { }
        #endregion

        #region Unloading
        /// <summary>
        /// Holds the start of the current unloading while false.
        /// </summary>
        public virtual bool StartUnloading {
            get { return true; }
        }

        /// <summary>
        /// Holds the completion of the current unloading while false.
        /// </summary>
        public virtual bool CompleteUnloading {
            get { return true; }
        }

        // -------------------------------------------
        // General
        // -------------------------------------------

        /// <summary>
        /// Prepare a new unloading operation.
        /// </summary>
        /// <param name="_unloadingBundles">All <see cref="SceneBundle"/> to unload, with their associated <see cref="UnloadSceneOptions"/>.</param>
        public virtual void PrepareUnloading(in PairCollection<SceneBundle, UnloadSceneOptions> _unloadingBundles) { }

        /// <summary>
        /// Called when starting the current unloading operation.
        /// </summary>
        public virtual void OnStartUnloading() { }

        /// <summary>
        /// Called whenever the current unloading state of the <see cref="EnhancedSceneManager"/> changes.
        /// </summary>
        /// <param name="_state">Current unloading step operation.</param>
        public virtual void OnUnloadingState(UnloadingState _state) { }

        // -------------------------------------------
        // Scene Bundle
        // -------------------------------------------

        /// <summary>
        /// Called just before starting a new <see cref="SceneBundle"/> unloading operation.
        /// </summary>
        /// <param name="_bundle">The <see cref="SceneBundle"/> to unload.</param>
        /// <param name="_mode"><see cref="UnloadSceneOptions"/> used to unload this bundle.</param>
        public virtual void OnPreUnloadBundle(SceneBundle _bundle, UnloadSceneOptions _mode) { }

        /// <summary>
        /// Called when starting a new <see cref="SceneBundle"/> unloading operation.
        /// </summary>
        /// <param name="_operation">The current unloading operation.</param>
        /// <param name="_index">The index of the current unloading operation.</param>
        /// <param name="_count">The total count of all unloading operations to come.</param>
        public virtual void OnUnloadBundle(UnloadBundleAsyncOperation _operation, int _index, int _count) { }

        // -------------------------------------------
        // Completion
        // -------------------------------------------

        /// <summary>
        /// Called once the current unloading operation is ready to be completed.
        /// <br/> Use <see cref="CompleteUnloading"/> to indicate when the operation can be completed.
        /// </summary>
        public virtual void OnUnloadingReady() { }

        /// <summary>
        /// Called when the current unloading operation has been canceled.
        /// </summary>
        public virtual void OnCancelUnloading() { }

        /// <summary>
        /// Called when the current unloading operation has stopped.
        /// </summary>
        public virtual void OnStopUnloading() { }
        #endregion
    }
}
