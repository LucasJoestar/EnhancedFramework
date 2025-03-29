// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using System;
using UnityEngine;

namespace EnhancedFramework.Core {
    /// <summary>
    /// <see cref="EnhancedBehaviour"/>-related <see cref="IPoolableObject"/>.
    /// </summary>
    [ScriptGizmos(false, true)]
    [AddComponentMenu(FrameworkUtility.MenuPath + "Utility/Poolable Object"), DisallowMultipleComponent]
    public class EnhancedPoolableObject : EnhancedBehaviour, IPoolableObject {
        #region Poolable
        private IObjectPool pool = null;
        private bool isFromPool  = false;
        private int lifeCount    = 0;

        /// <summary>
        /// Whether this object is from a pool or not.
        /// </summary>
        public bool IsFromPool {
            get { return isFromPool; }
        }

        /// <summary>
        /// Total count of time this object has been used as an instance.
        /// </summary>
        public int LifeCount {
            get { return lifeCount; }
        }

        /// <summary>
        /// Called whenever this object is deactivated (either destroyed or sent to the pool).
        /// </summary>
        public Action<GameObject> OnDeactivated = null;

        // -------------------------------------------
        // Poolable Object
        // -------------------------------------------

        /// <inheritdoc cref="IPoolableObject.OnCreated"/>
        public virtual void OnCreated(IObjectPool _pool) {
            isFromPool = true;
            pool = _pool;
        }

        /// <inheritdoc cref="IPoolableObject.OnRemovedFromPool"/>
        public virtual void OnRemovedFromPool() {
            lifeCount++;
            SetActive(true);
        }

        /// <inheritdoc cref="IPoolableObject.OnSentToPool"/>
        public virtual void OnSentToPool() {
            SetActive(false);
            OnDeactivation();
        }

        // -------------------------------------------
        // Mono Behaviour
        // -------------------------------------------

        protected virtual void OnDestroy() {
            OnDeactivation();
        }

        // -------------------------------------------
        // Callbacks
        // -------------------------------------------

        /// <summary>
        /// Activates / deactivates this object.
        /// </summary>
        /// <param name="_isActive">Whether to activate or deactive this object.</param>
        protected virtual void SetActive(bool _isActive) {
            gameObject.SetActive(_isActive);
        }

        private void OnDeactivation() {
            OnDeactivated?.Invoke(gameObject);
            OnDeactivated = null;
        }
        #endregion

        #region Utility
        /// <summary>
        /// Get this object associated <see cref="IObjectPool"/>.
        /// </summary>
        /// <param name="_pool"><see cref="IObjectPool"/> associated with this object (null if none).</param>
        /// <returns>True if this object has a valid pool associated with it, false otherwise.</returns>
        public bool GetPool(out IObjectPool _pool) {
            _pool = pool;
            return isFromPool;
        }

        /// <summary>
        /// Release this instance and send it back to the pool.
        /// </summary>
        public virtual void Release() {
            if (GetPool(out IObjectPool _pool)) {
                _pool.ReleasePoolInstance(this);
                return;
            }

            DestroyPoolInstance();
        }

        /// <summary>
        /// Destroys this pool instance.
        /// </summary>
        protected virtual void DestroyPoolInstance() {
            Destroy(gameObject);
        }
        #endregion
    }
}
