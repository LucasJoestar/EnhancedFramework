// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using System;
using System.Diagnostics;

using Object = UnityEngine.Object;

namespace EnhancedFramework.Core {
    #region Update Interfaces & Registration
    // -------------------------------------------
    // Update Interfaces & Registration
    // -------------------------------------------

    [Flags]
    public enum UpdateRegistration {
        Early   = 1 << 0,
        Input   = 1 << 1,
        Update  = 1 << 2,
        Dynamic = 1 << 3,
        Movable = 1 << 4,
        Late    = 1 << 5,

        Init    = 1 << 31
    }

    public interface IBaseUpdate    { Object GetLogObject { get; } }

    public interface IEarlyUpdate   : IBaseUpdate { void Update(); }
    public interface IInputUpdate   : IBaseUpdate { void Update(); }
    public interface IUpdate        : IBaseUpdate { void Update(); }
    public interface IDynamicUpdate : IBaseUpdate { void Update(); }
    public interface IMovableUpdate : IBaseUpdate { void Update(); }
    public interface ILateUpdate    : IBaseUpdate { void Update(); }

    public interface IInitUpdate    : IBaseUpdate {
        bool IsInitialized { get;
            #if CSHARP_8_0_OR_NEWER
            internal
            #endif
            set; }

        void Init();
    }
    #endregion

    /// <summary>
    /// Special instance managing all enhanced behaviour updates from a single object.
    /// </summary>
    public class UpdateManager : EnhancedSingleton<UpdateManager> {
        #region Global Members
        public bool IsSuspended { get; private set; } = false;

        // -----------------------

        private Stamp<IEarlyUpdate> earlyUpdates        = new Stamp<IEarlyUpdate>();
        private Stamp<IInputUpdate> inputUpdates        = new Stamp<IInputUpdate>(1);
        private Stamp<IDynamicUpdate> dynamicUpdates    = new Stamp<IDynamicUpdate>(5);
        private Stamp<IUpdate> updates                  = new Stamp<IUpdate>(10);
        private Stamp<IMovableUpdate> movableUpdates    = new Stamp<IMovableUpdate>(10);
        private Stamp<ILateUpdate> lateUpdates          = new Stamp<ILateUpdate>();

        private Stamp<Pair<IInitUpdate, UpdateRegistration>> initUpdates          = new Stamp<Pair<IInitUpdate, UpdateRegistration>>(10);
        #endregion

        #region Global Registration
        /// <summary>
        /// Registers an object on defined updates.
        /// </summary>
        /// <typeparam name="T">Object type to register.</typeparam>
        /// <param name="_object">Object to be registered on update(s).</param>
        /// <param name="_registration">Defined update registration (can use multiple).</param>
        public void Register<T>(T _object, UpdateRegistration _registration) {
            // Prevent registering on update before init.
            if ((_registration & UpdateRegistration.Init) != 0) {
                IInitUpdate _init = _object as IInitUpdate;

                if (!_init.IsInitialized) {
                    initUpdates.Add(new Pair<IInitUpdate, UpdateRegistration>(_init, _registration));
                    return;
                }
            }

            if ((_registration & UpdateRegistration.Early) != 0) {
                earlyUpdates.Add((IEarlyUpdate)_object);
            }

            if ((_registration & UpdateRegistration.Input) != 0) {
                inputUpdates.Add((IInputUpdate)_object);
            }

            if ((_registration & UpdateRegistration.Update) != 0) {
                updates.Add((IUpdate)_object);
            }

            if ((_registration & UpdateRegistration.Dynamic) != 0) {
                dynamicUpdates.Add((IDynamicUpdate)_object);
            }

            if ((_registration & UpdateRegistration.Movable) != 0) {
                movableUpdates.Add((IMovableUpdate)_object);
            }

            if ((_registration & UpdateRegistration.Late) != 0) {
                lateUpdates.Add((ILateUpdate)_object);
            }
        }

        /// <summary>
        /// Unregisters an object from defined updates.
        /// </summary>
        /// <typeparam name="T">Object type to unregister.</typeparam>
        /// <param name="_object">Object to be unregistered from update(s).</param>
        /// <param name="_registration">Defined update unregistration (can use multiple).</param>
        public void Unregister<T>(T _object, UpdateRegistration _registration) {
            // Non-init object unregistration.
            if ((_registration & UpdateRegistration.Init) != 0) {
                IInitUpdate _init = _object as IInitUpdate;

                if (!_init.IsInitialized) {
                    int _index = Array.FindIndex(initUpdates.Array, (p) => p.First == _init);
                    initUpdates.RemoveAt(_index);

                    return;
                }
            }

            if ((_registration & UpdateRegistration.Early) != 0) {
                earlyUpdates.Remove((IEarlyUpdate)_object);
            }

            if ((_registration & UpdateRegistration.Input) != 0) {
                inputUpdates.Remove((IInputUpdate)_object);
            }

            if ((_registration & UpdateRegistration.Update) != 0) {
                updates.Remove((IUpdate)_object);
            }

            if ((_registration & UpdateRegistration.Dynamic) != 0) {
                dynamicUpdates.Remove((IDynamicUpdate)_object);
            }

            if ((_registration & UpdateRegistration.Movable) != 0) {
                movableUpdates.Remove((IMovableUpdate)_object);
            }

            if ((_registration & UpdateRegistration.Late) != 0) {
                lateUpdates.Remove((ILateUpdate)_object);
            }
        }
        #endregion

        #region Enhanced Behaviour
        private const long InitWatcherMaxDuration = 100; // 0,1 second.
        private readonly Stopwatch initWatcher = new Stopwatch();

        // -----------------------

        private void Update() {
            // Initializations.
            initWatcher.Restart();

            while ((initUpdates.Count > 0) && (initWatcher.ElapsedMilliseconds < InitWatcherMaxDuration)) {
                var _pair = initUpdates.First();
                var _init = _pair.First;

                CallUpdate(_init.Init, _init);
                _init.IsInitialized = true;

                // Once the object is initialized, register its other updates.
                initUpdates.RemoveAt(0);

                Register<IBaseUpdate>(_init, _pair.Second);
            }

            // Only perform initialization while suspended (useful when loading a scene).
            if (IsSuspended) {
                return;
            }

            int i;

            for (i = earlyUpdates.Count; i-- > 0;) {
                var _update = earlyUpdates[i];
                CallUpdate(_update.Update, _update);
            }

            for (i = inputUpdates.Count; i-- > 0;) {
                var _update = inputUpdates[i];
                CallUpdate(_update.Update, _update);
            }

            for (i = dynamicUpdates.Count; i-- > 0;) {
                var _update = dynamicUpdates[i];
                CallUpdate(_update.Update, _update);
            }

            for (i = updates.Count; i-- > 0;) {
                var _update = updates[i];
                CallUpdate(_update.Update, _update);
            }

            for (i = movableUpdates.Count; i-- > 0;) {
                var _update = movableUpdates[i];
                CallUpdate(_update.Update, _update);
            }

            for (i = lateUpdates.Count; i-- > 0;) {
                var _update = lateUpdates[i];
                CallUpdate(_update.Update, _update);
            }

            // ----- Local Method ----- \\

            void CallUpdate(Action _update, IBaseUpdate _base) {
                try {
                    _update();
                } catch (Exception _exception) {
                    // Log the exception on error.
                    Object _object = _base.GetLogObject;
                    if (_object != null) {
                        _object = this;
                    }

                    _object.LogException(_exception);
                }
            }
        }
        #endregion
    }
}
