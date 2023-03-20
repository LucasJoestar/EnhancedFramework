// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
// 
// Notes:
// 
// ================================================================================== //

using EnhancedEditor;
using System;
using System.Diagnostics;
using UnityEngine;

using Object = UnityEngine.Object;

namespace EnhancedFramework.Core {
    #region Update Interfaces & Registration
    // -------------------------------------------
    // Update Interfaces & Registration
    // -------------------------------------------

    [Flags]
    public enum UpdateRegistration {
        Early       = 1 << 0,
        Input       = 1 << 2,
        Update      = 1 << 9,
        Dynamic     = 1 << 12,
        Movable     = 1 << 15,
        Late        = 1 << 19,

        Permanent   = 1 << 20,

        Init        = 1 << 30,
        Play        = 1 << 31,
    }

    public interface IBaseUpdate    { Object LogObject { get; } }

    public interface IEarlyUpdate       : IBaseUpdate { void Update(); }
    public interface IInputUpdate       : IBaseUpdate { void Update(); }
    public interface IUpdate            : IBaseUpdate { void Update(); }
    public interface IDynamicUpdate     : IBaseUpdate { void Update(); }
    public interface IMovableUpdate     : IBaseUpdate { void Update(); }
    public interface ILateUpdate        : IBaseUpdate { void Update(); }
    public interface IPermanentUpdate   : IBaseUpdate { void Update(); }

    public interface IInitUpdate        : IBaseUpdate {
        bool IsInitialized { get;
            #if CSHARP_8_0_OR_NEWER
            internal
            #endif
            set; }

        void Init();
    }

    public interface IPlayUpdate        : IBaseUpdate {
        bool IsPlaying {
            get;
            #if CSHARP_8_0_OR_NEWER
            internal
            #endif
            set;
        }

        void Play();
    }
    #endregion

    /// <summary>
    /// Special instance managing all enhanced behaviour updates from a single object.
    /// </summary>
    [ScriptGizmos(false, true)]
    [DefaultExecutionOrder(-100)]
    [AddComponentMenu(FrameworkUtility.MenuPath + "Manager/Update Manager"), DisallowMultipleComponent]
    public class UpdateManager : EnhancedSingleton<UpdateManager>, ILoadingProcessor {
        public override UpdateRegistration UpdateRegistration => UpdateRegistration.Init;

        #region Global Members
        [Section("Update Manager")]

        [Tooltip("When true, suspends all regular object updates while in loading (except for IInitUpdate and IPermanentUpdate)")]
        [SerializeField] private bool suspendOnLoading = true;

        // -----------------------

        // True while any object is waiting for its initialization.
        public bool IsProcessing {
            get { return initUpdates.Count != 0; }
        }

        /// <summary>
        /// When true, suspends all regular object updates, except for <see cref="IInitUpdate"/> and <see cref="IPermanentUpdate"/>.
        /// </summary>
        public bool IsSuspended { get; internal set; } = false;

        // -----------------------

        private readonly EnhancedCollection<IEarlyUpdate> earlyUpdates          = new EnhancedCollection<IEarlyUpdate>();
        private readonly EnhancedCollection<IInputUpdate> inputUpdates          = new EnhancedCollection<IInputUpdate>(1);
        private readonly EnhancedCollection<IDynamicUpdate> dynamicUpdates      = new EnhancedCollection<IDynamicUpdate>(5);
        private readonly EnhancedCollection<IUpdate> updates                    = new EnhancedCollection<IUpdate>(10);
        private readonly EnhancedCollection<IMovableUpdate> movableUpdates      = new EnhancedCollection<IMovableUpdate>(10);
        private readonly EnhancedCollection<ILateUpdate> lateUpdates            = new EnhancedCollection<ILateUpdate>();
        private readonly EnhancedCollection<IPermanentUpdate> permanentUpdates  = new EnhancedCollection<IPermanentUpdate>();

        private readonly PairCollection<IInitUpdate, UpdateRegistration> initUpdates    = new PairCollection<IInitUpdate, UpdateRegistration>(10);
        private readonly PairCollection<IPlayUpdate, UpdateRegistration> playUpdates    = new PairCollection<IPlayUpdate, UpdateRegistration>();
        #endregion

        #region Global Registration
        /// <summary>
        /// Registers an object on defined updates.
        /// </summary>
        /// <typeparam name="T">Object type to register.</typeparam>
        /// <param name="_object">Object to be registered on update(s).</param>
        /// <param name="_registration">Defined update registration (can use multiple).</param>
        public void Register<T>(T _object, UpdateRegistration _registration) {
            // Prevent registering on update(s) before init and play.
            if (_registration.HasFlag(UpdateRegistration.Init)) {
                IInitUpdate _init = _object as IInitUpdate;

                if (!_init.IsInitialized) {
                    initUpdates.Set(_init, _registration);
                    return;
                }
            }

            if (_registration.HasFlag(UpdateRegistration.Play)) {
                IPlayUpdate _play = _object as IPlayUpdate;

                if (!_play.IsPlaying) {
                    playUpdates.Set(_play, _registration);
                    return;
                }
            }

            // Regular updates.
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

            if (_registration.HasFlag(UpdateRegistration.Permanent)) {
                permanentUpdates.Add((IPermanentUpdate)_object);
            }
        }

        /// <summary>
        /// Unregisters an object from defined updates.
        /// </summary>
        /// <typeparam name="T">Object type to unregister.</typeparam>
        /// <param name="_object">Object to be unregistered from update(s).</param>
        /// <param name="_registration">Defined update unregistration (can use multiple).</param>
        public void Unregister<T>(T _object, UpdateRegistration _registration) {
            // Non initialized and played objects unregistration.
            if (_registration.HasFlag(UpdateRegistration.Init)) {
                IInitUpdate _init = _object as IInitUpdate;

                if (!_init.IsInitialized) {
                    initUpdates.Remove(_init);
                    return;
                }
            }

            if (_registration.HasFlag(UpdateRegistration.Play)) {
                IPlayUpdate _play = _object as IPlayUpdate;

                if (!_play.IsPlaying) {
                    playUpdates.Remove(_play);
                    return;
                }
            }

            // Regular updates.
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

            if (_registration.HasFlag(UpdateRegistration.Permanent)) {
                permanentUpdates.Remove((IPermanentUpdate)_object);
            }
        }
        #endregion

        #region Enhanced Behaviour
        private const long InitWatcherMaxDuration = 100; // In milliseconds, so about 0,1 second.
        private readonly Stopwatch initWatcher = new Stopwatch();

        // -----------------------

        protected override void OnInit() {
            base.OnInit();

            EnhancedSceneManager.OnStartLoading += OnStartLoading;
            EnhancedSceneManager.OnStopLoading  += OnStopLoading;

            // ----- Local Methods ----- \\

            void OnStartLoading() {
                // Suspends updates when entering a new loading.
                if (suspendOnLoading) {
                    IsSuspended = true;
                }
            }

            void OnStopLoading() {
                // Resume execution after loading.
                if (suspendOnLoading) {
                    IsSuspended = false;
                }
            }
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

            // Only perform initializations and permanent updates while suspended (useful when loading a scene).
            if (IsSuspended) {
                UpdatePermanents();
                return;
            }

            // Play.
            while ((playUpdates.Count > 0) && (initWatcher.ElapsedMilliseconds < InitWatcherMaxDuration)) {
                var _pair = playUpdates.First();
                var _play = _pair.First;

                CallUpdate(_play.Play, _play);
                _play.IsPlaying = true;

                // Once the object is playing, register its other updates.
                playUpdates.RemoveFirst();

                Register<IBaseUpdate>(_play, _pair.Second);
            }

            // Perform inverse loops in case of the objects unregistrating themselves during the update.
            int i;

            UpdatePermanents();

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

            // ----- Local Methods ----- \\

            void UpdatePermanents() {
                for (int i = permanentUpdates.Count; i-- > 0;) {
                    var _update = permanentUpdates[i];
                    CallUpdate(_update.Update, _update);
                }
            }

            void CallUpdate(Action _update, IBaseUpdate _base) {
                try {
                    _update();
                } catch (Exception _exception) {
                    // Log the exception on error.
                    Object _object = _base.LogObject;
                    if (_object.IsNull()) {
                        _object = this;
                    }

                    _object.LogException(_exception);
                }
            }
        }
        #endregion
    }
}
