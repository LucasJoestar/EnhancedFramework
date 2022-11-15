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
        public virtual void OnSetupLoading(in PairCollection<SceneBundle, LoadSceneMode> _loadingBundles) { }

        public virtual void OnStartLoading() { }

        public virtual void OnPreLoadBundle(SceneBundle _bundle, LoadSceneMode _mode) { }

        public virtual void OnLoadBundle(LoadBundleAsyncOperation _operation, int _index, int _count) { }

        public virtual void OnStopLoading() { }

        public abstract bool CompleteLoading();
        #endregion

        #region Unloading
        public virtual void OnSetupUnloading(in PairCollection<SceneBundle, UnloadSceneOptions> _unloadingBundles) { }

        public virtual void OnStartUnloading() { }

        public virtual void OnUnloadBundle(UnloadBundleAsyncOperation _operation, int _index, int _count) { }

        public virtual void OnStopUnloading() { }

        public abstract bool CompleteUnloading();
        #endregion
    }
}
