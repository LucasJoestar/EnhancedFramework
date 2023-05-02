// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using System;
using System.Collections;
using System.Collections.Generic;

namespace EnhancedFramework.Core {
    /// <summary>
    /// <see cref="TriggerInfo"/>-related wrapper for a single object.
    /// </summary>
    public struct TriggerInfoHandler : IHandler<TriggerInfo> {
        #region Global Members
        private Handler<TriggerInfo> handler;

        // -----------------------

        public int ID {
            get { return handler.ID; }
        }

        public bool IsValid {
            get { return GetHandle(out _); }
        }

        /// <inheritdoc cref="TriggerInfo.Count"/>
        public int ActorCount {
            get { return GetHandle(out TriggerInfo _info) ? _info.Count : 0; }
        }

        // -------------------------------------------
        // Constructor(s)
        // -------------------------------------------

        /// <inheritdoc cref="TriggerInfoHandler(TriggerInfo, int)"/>
        public TriggerInfoHandler(TriggerInfo _info) {
            handler = new Handler<TriggerInfo>(_info);
        }

        /// <param name="_info"><see cref="TriggerInfo"/> to handle.</param>
        /// <param name="_id">ID of the associated operation.</param>
        /// <inheritdoc cref="TriggerInfoHandler"/>
        public TriggerInfoHandler(TriggerInfo _info, int _id) {
            handler = new Handler<TriggerInfo>(_info, _id);
        }
        #endregion

        #region Utility
        /// <inheritdoc cref="IHandler{T}.GetHandle(out T)"/>
        public bool GetHandle(out TriggerInfo _info) {
            return handler.GetHandle(out _info) && _info.IsValid;
        }

        /// <inheritdoc cref="TriggerInfo.RegisterActor(ITrigger, ITriggerActor)"/>
        public TriggerInfoHandler RegisterActor(ITrigger _trigger, ITriggerActor _actor) {

            TriggerInfoHandler _handler = this;

            if (!GetHandle(out TriggerInfo _info)) {
                _info = TriggerInfo.None;
                _handler = new TriggerInfoHandler(_info);
            }

            TriggerInfo _newInfo = _info.RegisterActor(_trigger, _actor);

            if (_info != _newInfo) {
                _handler = new TriggerInfoHandler(_newInfo);
            }

            return _handler;
        }

        /// <inheritdoc cref="TriggerInfo.UnregisterActor(ITriggerActor)"/>
        public TriggerInfoHandler UnregisterActor(ITriggerActor _actor) {

            if (GetHandle(out TriggerInfo _info)) {
                _info.UnregisterActor(_actor);
            }

            return this;
        }

        /// <inheritdoc cref="TriggerInfo.Release"/>
        public TriggerInfoHandler Release() {

            if (GetHandle(out TriggerInfo _info)) {
                _info.Release();
            }

            return default;
        }
        #endregion
    }

    /// <summary>
    /// Wrapper for an <see cref="ITrigger"/> state infos.
    /// </summary>
    [Serializable]
    public class TriggerInfo : IHandle, IPoolableObject, IEnumerable<ITriggerActor> {
        #region Global Members
        private int id = 0;

        /// <summary>
        /// Default instance of this class.
        /// </summary>
        public static TriggerInfo None = new TriggerInfo();

        /// <summary>
        /// This trigger instance.
        /// </summary>
        public ITrigger Trigger = null;

        /// <summary>
        /// Alls current actors within this trigger.
        /// </summary>
        public Set<ITriggerActor> Actors = new Set<ITriggerActor>();

        // -----------------------

        /// <inheritdoc cref="IHandle.ID"/>
        public int ID {
            get { return id; }
        }

        /// <summary>
        /// Indicates whether this object is valid or not.
        /// </summary>
        public bool IsValid {
            get { return this != None; }
        }

        /// <summary>
        /// The total amount of actors within this trigger.
        /// </summary>
        public virtual int Count {
            get { return Actors.Count; }
        }

        // -------------------------------------------
        // Constructor(s)
        // -------------------------------------------

        /// <inheritdoc cref="TriggerInfo"/>
        internal protected TriggerInfo() { }
        #endregion

        #region IEnumerable
        public IEnumerator<ITriggerActor> GetEnumerator() {
            for (int i = 0; i < Count; i++) {
                yield return Actors[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        #endregion

        #region Behaviour
        private static int lastID = 0;

        // -----------------------

        /// <summary>
        /// Registers a new actor within this trigger.
        /// </summary>
        /// <param name="_trigger"><see cref="ITrigger"/> instance to register this actor within.</param>
        /// <param name="_actor"><see cref="ITriggerActor"/> within this trigger.</param>
        /// <returns>This <see cref="TriggerInfo"/> instance</returns>
        public TriggerInfo RegisterActor(ITrigger _trigger, ITriggerActor _actor) {

            TriggerInfo _infos = this;

            if (!IsValid) {
                _infos = EnhancedTriggerInfoManager.GetInfos(_trigger);
            }

            _infos.Actors.AddInstance(_actor);
            return _infos;
        }

        /// <summary>
        /// Unregisters an actor from this trigger.
        /// </summary>
        /// <param name="_actor"><see cref="ITriggerActor"/> to unregister.</param>
        /// <returns>This <see cref="TriggerInfo"/> instance</returns>
        public TriggerInfo UnregisterActor(ITriggerActor _actor) {

            if (IsValid) {
                Actors.Remove(_actor);
            }

            return this;
        }

        /// <summary>
        /// Releases this <see cref="TriggerInfo"/> instance.
        /// </summary>
        /// <returns>This <see cref="TriggerInfo"/> instance</returns>
        public TriggerInfo Release() {

            if (IsValid) {
                EnhancedTriggerInfoManager.ReleaseInfos(this);
            }

            return None;
        }

        // -------------------------------------------
        // Core
        // -------------------------------------------

        /// <summary>
        /// Setupts this object for a specific <see cref="ITrigger"/> instance.
        /// </summary>
        /// <param name="_trigger">This object associated trigger instance.</param>
        internal TriggerInfoHandler Setup(ITrigger _trigger) {
            Reset();
            Trigger = _trigger;

            id = ++lastID;
            return new TriggerInfoHandler(this, id);
        }

        /// <summary>
        /// Resets this object infos.
        /// </summary>
        internal void Reset() {

            for (int i = Count; i-- > 0;) {
                Actors[i].ExitTrigger(Trigger);
            }

            Actors.Clear();
            Trigger = null;
        }
        #endregion

        #region Pool
        void IPoolableObject.OnCreated() { }

        void IPoolableObject.OnRemovedFromPool() { }

        void IPoolableObject.OnSentToPool() {
            Reset();
        }
        #endregion
    }

    /// <summary>
    /// <see cref="ITrigger"/>-related utility class.
    /// <br/> Use this for getting a <see cref="TriggerInfo"/> instance.
    /// </summary>
    public class EnhancedTriggerInfoManager : IObjectPoolManager<TriggerInfo> {
        #region Pool
        private const int PoolCapacity = 2;

        private static readonly ObjectPool<TriggerInfo> pool = new ObjectPool<TriggerInfo>(PoolCapacity);
        public static readonly EnhancedTriggerInfoManager Instance = new EnhancedTriggerInfoManager();

        /// <inheritdoc cref="EnhancedTriggerInfoManager"/>
        private EnhancedTriggerInfoManager() {

            // Pool initialization.
            pool.Initialize(this);
        }

        // -------------------------------------------
        // Management
        // -------------------------------------------

        /// <summary>
        /// Get a <see cref="TriggerInfo"/> instance from the pool.
        /// </summary>
        /// <inheritdoc cref="ObjectPool{T}.Get"/>
        public static TriggerInfo GetInfos(ITrigger _trigger) {
            TriggerInfo _infos = pool.Get();
            _infos.Setup(_trigger);

            return _infos;
        }

        /// <summary>
        /// Releases a specific <see cref="TriggerInfo"/> instance and sent it back to the pool.
        /// </summary>
        /// <inheritdoc cref="ObjectPool{T}.Release(T)"/>
        public static bool ReleaseInfos(TriggerInfo _instance) {
            return pool.Release(_instance);
        }

        /// <summary>
        /// Clears the <see cref="TriggerInfo"/> pool content.
        /// </summary>
        /// <inheritdoc cref="ObjectPool{T}.Clear(int)"/>
        public static void ClearPool(int _capacity = PoolCapacity) {
            pool.Clear(_capacity);
        }

        // -----------------------

        /// <inheritdoc cref="IObjectPoolManager{TriggerInfos}.CreateInstance"/>
        TriggerInfo IObjectPoolManager<TriggerInfo>.CreateInstance() {
            return new TriggerInfo();
        }

        /// <inheritdoc cref="IObjectPoolManager{TriggerInfos}.DestroyInstance(TriggerInfos)"/>
        void IObjectPoolManager<TriggerInfo>.DestroyInstance(TriggerInfo _instance) {
            // Cannot destroy the instance, so simply ignore the object and wait for the garbage collector to pick it up.
        }
        #endregion
    }
}
