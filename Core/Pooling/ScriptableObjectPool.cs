// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace EnhancedFramework.Core {
    /// <summary>
    /// Base class for a <see cref="ScriptableObject"/> managing any <see cref="EnhancedPoolableObject"/> <see cref="ObjectPool{T}"/>.
    /// </summary>
    public abstract class ScriptableObjectPool<T> : EnhancedScriptableObject, IObjectPoolManager<T> where T : EnhancedPoolableObject  {
        #region Global Members
        [Section("Scriptable Pool")]

        [SerializeField, Enhanced, Required] private T template = null;

        [Space(10f), HorizontalLine(SuperColor.Grey, 1f), Space(10f)]

        [SerializeField, Enhanced, Block, ReadOnly] private ObjectPool<T> pool = new ObjectPool<T>();

        // -----------------------

        /// <summary>
        /// This scriptable managing <see cref="ObjectPool{T}"/>.
        /// </summary>
        public ObjectPool<T> Pool {
            get { return pool; }
        }
        #endregion

        #region Pool
        /// <summary>
        /// Get an instance of this scriptable managing <see cref="ObjectPool{T}"/>.
        /// </summary>
        /// <inheritdoc cref="ObjectPool{T}.Get"/>
        public T GetInstance() {
            pool.Initialize(this);
            return pool.Get();
        }

        /// <summary>
        /// Releases a specific instance and sent it back to this scriptable managing pool <see cref="ObjectPool{T}"/>.
        /// </summary>
        /// <inheritdoc cref="ObjectPool{T}.Release(T)"/>
        public bool ReleaseInstance(T _instance) {
            pool.Initialize(this);
            return pool.Release(_instance);
        }

        /// <summary>
        /// Clears this scriptable managing pool content and destroys all its instances.
        /// </summary>
        /// <inheritdoc cref="ObjectPool{T}.Clear(int)"/>
        public void ClearPool(int _capacity = 1) {
            pool.Clear(_capacity);
        }

        // -------------------------------------------
        // Manager
        // -------------------------------------------

        /// <inheritdoc cref="IObjectPoolManager{T}.CreateInstance"/>
        T IObjectPoolManager<T>.CreateInstance() {
            return Instantiate(template, Vector3.zero, Quaternion.identity);
        }

        /// <inheritdoc cref="IObjectPoolManager{T}.DestroyInstance(T)"/>
        void IObjectPoolManager<T>.DestroyInstance(T _instance) {
            Destroy(_instance.gameObject);
        }
        #endregion
    }
}
