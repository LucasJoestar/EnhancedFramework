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
        Early           = 1 << 0,
        Input           = 1 << 2,
        Update          = 1 << 9,
        Dynamic         = 1 << 12,
        Movable         = 1 << 15,
        Late            = 1 << 19,

        Stable       = 1 << 20,
        EarlyStable  = 1 << 21,

        Init            = 1 << 30,
        Play            = 1 << 31,
    }

    public interface IBaseUpdate {
        Object LogObject { get; }
        int InstanceID { get; }
    }

    public interface IEarlyUpdate   : IBaseUpdate { void Update(); }
    public interface IInputUpdate   : IBaseUpdate { void Update(); }
    public interface IUpdate        : IBaseUpdate { void Update(); }
    public interface IDynamicUpdate : IBaseUpdate { void Update(); }
    public interface IMovableUpdate : IBaseUpdate { void Update(); }
    public interface ILateUpdate    : IBaseUpdate { void Update(); }
    public interface IStableUpdate  : IBaseUpdate { void Update(); }

    public interface IInitUpdate    : IBaseUpdate {
        bool IsInitialized { get;
            #if CSHARP_8_0_OR_NEWER
            internal
            #endif
            set; }

        void Init();
    }

    public interface IPlayUpdate    : IBaseUpdate {
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
    [DefaultExecutionOrder(-999)]
    [AddComponentMenu(FrameworkUtility.MenuPath + "General/Update Manager"), DisallowMultipleComponent]
    public class UpdateManager : EnhancedSingleton<UpdateManager>, ILoadingProcessor {
        public override UpdateRegistration UpdateRegistration => UpdateRegistration.Init;

        #region Global Members
        [Section("Update Manager")]

        [Tooltip("If true, suspends all regular object updates while in loading (except for IInitUpdate and IPermanentUpdate)")]
        [SerializeField] private bool suspendOnLoading = true;

        // -----------------------

        // True while any object is waiting for its initialization.
        public bool IsProcessing {
            get { return initUpdates.Count != 0; }
        }

        /// <summary>
        /// When true, suspends all regular object updates, except for <see cref="IInitUpdate"/> and <see cref="IStableUpdate"/>.
        /// </summary>
        public bool IsSuspended { get; internal set; } = false;

        // -----------------------

        private readonly EnhancedCollection<IEarlyUpdate> earlyUpdates      = new EnhancedCollection<IEarlyUpdate>();
        private readonly EnhancedCollection<IInputUpdate> inputUpdates      = new EnhancedCollection<IInputUpdate>(1);
        private readonly EnhancedCollection<IDynamicUpdate> dynamicUpdates  = new EnhancedCollection<IDynamicUpdate>(5);
        private readonly EnhancedCollection<IUpdate> updates                = new EnhancedCollection<IUpdate>(10);
        private readonly EnhancedCollection<IMovableUpdate> movableUpdates  = new EnhancedCollection<IMovableUpdate>(10);
        private readonly EnhancedCollection<ILateUpdate> lateUpdates        = new EnhancedCollection<ILateUpdate>();

        private readonly EnhancedCollection<IStableUpdate> stableUpdates    = new EnhancedCollection<IStableUpdate>();

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

                if ((_object is IInitUpdate _init) && !_init.IsInitialized) {
                    initUpdates.Set(_init, _registration);
                    return;
                }
            }

            if (_registration.HasFlag(UpdateRegistration.Play)) {

                if ((_object is IPlayUpdate _play) && !_play.IsPlaying) {
                    playUpdates.Set(_play, _registration);
                    return;
                }
            }

            // Regular updates.
            DoRegister(earlyUpdates,      UpdateRegistration.Early);
            DoRegister(inputUpdates,      UpdateRegistration.Input);
            DoRegister(updates,           UpdateRegistration.Update);
            DoRegister(dynamicUpdates,    UpdateRegistration.Dynamic);
            DoRegister(movableUpdates,    UpdateRegistration.Movable);
            DoRegister(lateUpdates,       UpdateRegistration.Late);

            DoRegister(stableUpdates,     UpdateRegistration.Stable);

            // ----- Local Methods ----- \\

            void DoRegister<U>(EnhancedCollection<U> _list, UpdateRegistration _flag) {

                if (_registration.HasFlag(_flag) && (_object is U _update)) {
                    _list.Add(_update);
                }
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

                if ((_object is IInitUpdate _init) && !_init.IsInitialized) {
                    initUpdates.Remove(_init);
                    return;
                }
            }

            if (_registration.HasFlag(UpdateRegistration.Play)) {

                if ((_object is IPlayUpdate _play) && !_play.IsPlaying) {
                    playUpdates.Remove(_play);
                    return;
                }
            }

            // Regular updates.
            DoUnregister(earlyUpdates,        UpdateRegistration.Early);
            DoUnregister(inputUpdates,        UpdateRegistration.Input);
            DoUnregister(updates,             UpdateRegistration.Update);
            DoUnregister(dynamicUpdates,      UpdateRegistration.Dynamic);
            DoUnregister(movableUpdates,      UpdateRegistration.Movable);
            DoUnregister(lateUpdates,         UpdateRegistration.Late);

            DoUnregister(stableUpdates,       UpdateRegistration.Stable);

            // ----- Local Methods ----- \\

            void DoUnregister<U>(EnhancedCollection<U> _list, UpdateRegistration _flag) where U : class {

                if (_registration.HasFlag(_flag) && (_object is U _update)) {
                    _list.Remove(_update);
                }
            }
        }
        #endregion

        #region Enhanced Behaviour
        private const long InitWatcherMaxDuration = 100; // In milliseconds, so about 0,1 second.
        private readonly Stopwatch initWatcher = new Stopwatch();

        #if DEVELOPMENT
        private double loadingDuration = 0d;
        private bool isLoading = false;
        #endif

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

                #if DEVELOPMENT
                isLoading = true;
                #endif
            }

            void OnStopLoading() {

                // Resume execution after loading.
                if (suspendOnLoading) {
                    IsSuspended = false;
                }

                #if DEVELOPMENT
                this.LogMessage($"Loading Duration - {loadingDuration.ToString("0.#########").Bold()} seconds");

                isLoading = false;
                loadingDuration = 0d;
                #endif
            }
        }

        private void Update() {
            initWatcher.Restart();

            // Initializations.
            while ((initWatcher.ElapsedMilliseconds < InitWatcherMaxDuration) && initUpdates.SafeFirst(out var _pair)) {

                IInitUpdate _init = _pair.First;

                CallUpdate(_init.Init, _init);
                _init.IsInitialized = true;

                // Once the object is initialized, register its other updates.
                initUpdates.RemoveFirst();

                Register<IBaseUpdate>(_init, _pair.Second);
            }

            #if DEVELOPMENT
            // Calcul loading duration.
            if (isLoading) {
                loadingDuration += initWatcher.Elapsed.TotalSeconds;
            }
            #endif

            // Only perform initializations and permanent updates while suspended (useful when loading a scene).
            if (IsSuspended) {

                UpdatePermanents();
                return;
            }

            // Play.
            while ((initWatcher.ElapsedMilliseconds < InitWatcherMaxDuration) && playUpdates.SafeFirst(out var _pair)) {

                IPlayUpdate _play = _pair.First;

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

                for (int i = stableUpdates.Count; i-- > 0;) {

                    var _update = stableUpdates[i];
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

        #region Logger
        public override Color GetLogMessageColor(LogType _type) {
            return SuperColor.Cyan.Get();
        }
        #endregion
    }
}
