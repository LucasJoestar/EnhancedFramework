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

    public interface IBaseUpdate    { Object LogObject { get; } }

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
    public class UpdateManager : EnhancedSingleton<UpdateManager>, ILoadingProcessor {
        public override UpdateRegistration UpdateRegistration => UpdateRegistration.Init;

        #region Global Members
        // True while any object is waiting for its initialization.
        public bool IsProcessing {
            get { return initUpdates.Count != 0; }
        }

        public bool IsSuspended { get; private set; } = false;

        // -----------------------

        private readonly EnhancedCollection<IEarlyUpdate> earlyUpdates      = new EnhancedCollection<IEarlyUpdate>();
        private readonly EnhancedCollection<IInputUpdate> inputUpdates      = new EnhancedCollection<IInputUpdate>(1);
        private readonly EnhancedCollection<IDynamicUpdate> dynamicUpdates  = new EnhancedCollection<IDynamicUpdate>(5);
        private readonly EnhancedCollection<IUpdate> updates                = new EnhancedCollection<IUpdate>(10);
        private readonly EnhancedCollection<IMovableUpdate> movableUpdates  = new EnhancedCollection<IMovableUpdate>(10);
        private readonly EnhancedCollection<ILateUpdate> lateUpdates        = new EnhancedCollection<ILateUpdate>();

        private readonly EnhancedCollection<Pair<IInitUpdate, UpdateRegistration>> initUpdates = new EnhancedCollection<Pair<IInitUpdate, UpdateRegistration>>(10);
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
            if (_registration.HasFlag(UpdateRegistration.Init)) {
                IInitUpdate _init = _object as IInitUpdate;

                if (!_init.IsInitialized) {
                    initUpdates.Add(new Pair<IInitUpdate, UpdateRegistration>(_init, _registration));
                    return;
                }
            }

            if (_registration.HasFlag(UpdateRegistration.Early)) {
                earlyUpdates.Add((IEarlyUpdate)_object);
            }

            if (_registration.HasFlag(UpdateRegistration.Input)) {
                inputUpdates.Add((IInputUpdate)_object);
            }

            if (_registration.HasFlag(UpdateRegistration.Update)) {
                updates.Add((IUpdate)_object);
            }

            if (_registration.HasFlag(UpdateRegistration.Dynamic)) {
                dynamicUpdates.Add((IDynamicUpdate)_object);
            }

            if (_registration.HasFlag(UpdateRegistration.Movable)) {
                movableUpdates.Add((IMovableUpdate)_object);
            }

            if (_registration.HasFlag(UpdateRegistration.Late)) {
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
            if (_registration.HasFlag(UpdateRegistration.Init)) {
                IInitUpdate _init = _object as IInitUpdate;

                if (!_init.IsInitialized) {
                    int _index = initUpdates.FindIndex(i => i.First == _init);
                    initUpdates.RemoveAt(_index);

                    return;
                }
            }

            if (_registration.HasFlag(UpdateRegistration.Early)) {
                earlyUpdates.Remove((IEarlyUpdate)_object);
            }

            if (_registration.HasFlag(UpdateRegistration.Input)) {
                inputUpdates.Remove((IInputUpdate)_object);
            }

            if (_registration.HasFlag(UpdateRegistration.Update)) {
                updates.Remove((IUpdate)_object);
            }

            if (_registration.HasFlag(UpdateRegistration.Dynamic)) {
                dynamicUpdates.Remove((IDynamicUpdate)_object);
            }

            if (_registration.HasFlag(UpdateRegistration.Movable)) {
                movableUpdates.Remove((IMovableUpdate)_object);
            }

            if (_registration.HasFlag(UpdateRegistration.Late)) {
                lateUpdates.Remove((ILateUpdate)_object);
            }
        }
        #endregion

        #region Enhanced Behaviour
        private const long InitWatcherMaxDuration = 100; // In milliseconds, so about 0,1 second.
        private readonly Stopwatch initWatcher = new Stopwatch();

        // -----------------------

        protected override void OnInit() {
            base.OnInit();

            // Loading initialization registration.
            EnhancedSceneManager.Instance.RegisterProcessor(this);
        }

        private void Update() {
            initWatcher.Restart();

            // Initializations.
            while ((initUpdates.Count > 0) && (initWatcher.ElapsedMilliseconds < InitWatcherMaxDuration)) {
                var _pair = initUpdates.First();
                var _init = _pair.First;
                
                CallUpdate(_init.Init, _init);
                _init.IsInitialized = true;

                // Once the object is initialized, register its other updates.
                initUpdates.RemoveFirst();

                Register<IBaseUpdate>(_init, _pair.Second);
            }

            // Only perform initialization while suspended (useful when loading a scene).
            if (IsSuspended) {
                return;
            }

            // Perform inverse loops in case of the objects unregistrating themselves during the update.
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
                    Object _object = _base.LogObject;
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
