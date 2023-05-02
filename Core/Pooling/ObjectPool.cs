// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using System;
using UnityEngine;

namespace EnhancedFramework.Core {
	/// <summary>
	/// Base interface to inherit any poolable object from.
	/// </summary>
	public interface IPoolableObject {
        #region Content
        /// <summary>
        /// Called when this object is created.
        /// </summary>
        void OnCreated();

        /// <summary>
        /// Called when this element is removed from the pool.
        /// </summary>
        void OnRemovedFromPool();

        /// <summary>
        /// Called when this element is returned to the pool.
        /// </summary>
        void OnSentToPool();
        #endregion
    }

    /// <summary>
    /// <see cref="ObjectPool{T}"/>-related interface used to create and destroy instances.
    /// </summary>
    /// <typeparam name="T"><see cref="IPoolableObject"/> managed type.</typeparam>
    public interface IObjectPoolManager<T> where T : class, IPoolableObject {
        #region Content
        /// <summary>
        /// Creates a new object instance.
        /// </summary>
        /// <returns>Created object instance.</returns>
        T CreateInstance();

        /// <summary>
        /// Destroys a specific object instance.
        /// </summary>
        /// <param name="_instance">The instance to destroy.</param>
        void DestroyInstance(T _instance);
        #endregion
    }

    /// <summary>
    /// Generic pool system used to manage a specific <see cref="IPoolableObject"/> type.
    /// <para/>
    /// Keep in mind to initialize it as soon as possible using <see cref="Initialize(IObjectPoolManager{T})"/>.
    /// </summary>
    [Serializable]
    public sealed class ObjectPool<T> where T : class, IPoolableObject  {
        #region Global Members
        [SerializeField] private EnhancedCollection<T> pool = new EnhancedCollection<T>();
        private IObjectPoolManager<T> manager = null;

        // -------------------------------------------
        // Constructor(s)
        // -------------------------------------------

        /// <param name="_capacity">Initial capacity of the pool.</param>
        /// <inheritdoc cref="ObjectPool{T}"/>
        public ObjectPool(int _capacity = 1) {
            pool = new EnhancedCollection<T>(_capacity);
        }
        #endregion

        #region Initializer
        /// <summary>
        /// Initializes this pool.
        /// <br/> Keep in mind to always initialize it as soon as possible.
        /// </summary>
        /// <param name="_manager">Manager used to create and destroy pool object instances.</param>
        public void Initialize(IObjectPoolManager<T> _manager) {
            if (manager == _manager) {
                return;
            }

            manager = _manager;

            for (int i = pool.Count; i < pool.Capacity; i++) {
                T _instance = _manager.CreateInstance();
                pool.Add(_instance);

                _instance.OnCreated();
                _instance.OnSentToPool();
            }
        }
        #endregion

        #region Management
        /// <summary>
        /// Get an object instance from this pool.
        /// </summary>
        /// <returns>The object instance get from the pool.</returns>
        public T Get() {
            T _instance;

            if (pool.Count == 0) {
                _instance = manager.CreateInstance();
                _instance.OnCreated();
            } else {
                _instance = pool.Last();
                pool.RemoveLast();
            }           

            _instance.OnRemovedFromPool();
            return _instance;
        }

        /// <summary>
        /// Releases a specific instance and sent it to the pool.
        /// </summary>
        /// <param name="_instance">Object instance to release and send to the pool.</param>
        /// <returns>True if the instance could be successfully released and sent to the pool, false otherwise.</returns>
        public bool Release(T _instance) {
            // Ignore it if already in the pool.
            if (pool.IndexOf(_instance) != -1) {
                return false;
            }

            _instance.OnSentToPool();
            pool.Add(_instance);

            return true;
        }

        /// <summary>
        /// Clears this pool content and destroys all its instances.
        /// </summary>
        /// <param name="_capacity">New capacity of the pool.</param>
        public void Clear(int _capacity = 1) {
            // Destroy instances.
            for (int i = pool.Count; i-- > 0;) {
                T _object = pool[i];
                manager.DestroyInstance(_object);
            }

            // Reset capacity.
            pool.Clear();
            pool.Capacity = _capacity;
        }
        #endregion
    }
}
