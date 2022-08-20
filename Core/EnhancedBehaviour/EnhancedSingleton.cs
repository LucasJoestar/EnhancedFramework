// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using System;

namespace EnhancedFramework.Core {
    /// <summary>
    /// Singleton instance class derived from <see cref="DreadfulBehaviour"/>.
    /// <br/> The object type must be set as this class type.
    /// <para/>
    /// Note that there should always be no more than one instance of this class at the same time.
    /// Every other instances will be automatically destroyed.
    /// <para/>
    /// </summary>
    /// <typeparam name="T">This object type.</typeparam>
    public abstract class EnhancedSingleton<T> : EnhancedBehaviour where T : EnhancedBehaviour {
        #region Singleton Instance
        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static T Instance { get; private set; } = null;

        /// <summary>
        /// Called whenever a new singleton instance of this class is assigned.
        /// </summary>
        public static event Action<EnhancedSingleton<T>> OnInstanceChanged = null;
        #endregion

        #region Enhanced Behaviour
        protected override void OnEnable() {
            base.OnEnable();

            if (Instance.IsValid()) {
                OnNonSingletonInstance();   // When a singleton instance already exist, call this method.
            } else {
                Instance = this as T;       // Set this object as singleton instance.
                OnInstanceChanged?.Invoke(this);
            }
        }

        protected override void OnDisable() {
            base.OnDisable();

            if ((Instance == this) && !GameManager.IsQuittingApplication) {
                Instance = null;
            }
        }

        // -----------------------

        /// <summary>
        /// Called when a new non-singleton instance of this object is enabled.
        /// <para/>
        /// By default, this base method call will log an error and destroy the object. Override its content to specify another behaviour.
        /// </summary>
        protected virtual void OnNonSingletonInstance() {
            this.LogError($"A Singleton Instance of \"{GetType().Name}\" already exist! Destroying object \"{name}\".");
            Destroy(this);
        }
        #endregion
    }
}
