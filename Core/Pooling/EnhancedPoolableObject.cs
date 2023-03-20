// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace EnhancedFramework.Core {
    /// <summary>
    /// <see cref="EnhancedBehaviour"/>-related <see cref="IPoolableObject"/>.
    /// </summary>
    [ScriptGizmos(false, true)]
    [AddComponentMenu(FrameworkUtility.MenuPath + "Utility/Poolable Object"), DisallowMultipleComponent]
    public class EnhancedPoolableObject : EnhancedBehaviour, IPoolableObject {
        #region Poolable
        /// <inheritdoc cref="IPoolableObject.OnCreated"/>
        public virtual void OnCreated() { }

        /// <inheritdoc cref="IPoolableObject.OnRemovedFromPool"/>
        public virtual void OnRemovedFromPool() {
            SetActive(true);
        }

        /// <inheritdoc cref="IPoolableObject.OnSentToPool"/>
        public virtual void OnSentToPool() {
            SetActive(false);
        }

        // -----------------------

        /// <summary>
        /// Activates / deactivates this object.
        /// </summary>
        /// <param name="_isActive">Whether to activate or deactive this object.</param>
        protected virtual void SetActive(bool _isActive) {
            gameObject.SetActive(_isActive);
        }
        #endregion
    }
}
