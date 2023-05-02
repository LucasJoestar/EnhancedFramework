// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedFramework.Core.GameStates;
using System;
using UnityEngine;

namespace EnhancedFramework.Core {
    /// <summary>
    /// <see cref="ChronosModifier"/>-related wrapper for a single chronos operation.
    /// </summary>
    public struct ChronosHandler : IHandler<ChronosModifier> {
        #region Global Members
        private Handler<ChronosModifier> handler;

        // -----------------------

        public int ID {
            get { return handler.ID; }
        }

        public bool IsValid {
            get { return GetHandle(out _); }
        }

        // -------------------------------------------
        // Constructor(s)
        // -------------------------------------------

        /// <inheritdoc cref="ChronosHandler(ChronosModifier, int)"/>
        public ChronosHandler(ChronosModifier _chronos) {
            handler = new Handler<ChronosModifier>(_chronos);
        }

        /// <param name="_chronos"><see cref="ChronosModifier"/> to handle.</param>
        /// <param name="_id">ID of the associated call operation.</param>
        /// <inheritdoc cref="ChronosHandler"/>
        public ChronosHandler(ChronosModifier _chronos, int _id) {
            handler = new Handler<ChronosModifier>(_chronos, _id);
        }
        #endregion

        #region Utility
        /// <inheritdoc cref="IHandler{T}.GetHandle(out T)"/>
        public bool GetHandle(out ChronosModifier _chronos) {
            return handler.GetHandle(out _chronos) && (_chronos.ModifierType != ChronosModifier.Type.None);
        }

        /// <summary>
        /// Removes this handler associated chronos modifier.
        /// </summary>
        public bool Remove() {
            if (GetHandle(out ChronosModifier _chronos)) {
                _chronos.Remove();
                return true;
            }

            return false;
        }
        #endregion
    }

    /// <summary>
    /// Utility class used to modify the game chronos both during editor and runtime.
    /// </summary>
    [Serializable]
    public class ChronosModifier : IHandle, IPoolableObject, IComparable<ChronosModifier> {
        #region Type
        /// <summary>
        /// References all different types of modifiers.
        /// </summary>
        public enum Type {
            None        = 0,
            Override    = 1,
            Coefficient = 2,
        }
        #endregion

        #region Global Members
        private int id = 0;
        private Type type = Type.None;

        private float chronos = 0f;

        // -----------------------

        /// <inheritdoc cref="IHandle.ID"/>
        public int ID {
            get { return id; }
        }

        /// <summary>
        /// Chronos modifier value.
        /// </summary>
        public float Chronos {
            get { return chronos; }
        }

        /// <summary>
        /// The type of this chronos modifier.
        /// </summary>
        public Type ModifierType {
            get { return type; }
        }

        // -------------------------------------------
        // Constructor(s)
        // -------------------------------------------

        /// <inheritdoc cref="ChronosModifier(float)"/>
        internal ChronosModifier() { }

        /// <summary>
        /// Prevents from instanciating new instances without using the <see cref="ChronosManager"/> class.
        /// </summary>
        internal ChronosModifier(float _chronos) {
            chronos = _chronos;
        }
        #endregion

        #region Comparer
        int IComparable<ChronosModifier>.CompareTo(ChronosModifier _other) {
            return id.CompareTo(_other.id);
        }
        #endregion

        #region Chronos
        private DelayHandler delay = default;

        // -----------------------

        /// <inheritdoc cref="ApplyChronos(int, float, Type, float, bool, Action)"/>
        internal ChronosHandler ApplyChronos(int _id, float _chronos, Type _type) {
            // Setup.
            chronos = _chronos;

            SetType(_type);

            delay.Cancel();
            id = _id;

            return new ChronosHandler(this, id);
        }

        /// <summary>
        /// Initializes this object for a new chronos modifier.
        /// </summary>
        /// <param name="_id">Id of this modifier.</param>
        /// <param name="_chronos">Chronos override value.</param>
        /// <param name="_type">Chronos modifier type..</param>
        /// <param name="_duration">Duration of this override (in seconds).</param>
        /// <param name="_realTime">If true, the given duration will not be affected by the game time scale.</param>
        /// <param name="_onComplete">Delegate to be called once the override is removed.</param>
        /// <returns><see cref="ChronosHandler"/> of this chronos modifier.</returns>
        internal ChronosHandler ApplyChronos(int _id, float _chronos, Type _type, float _duration, bool _realTime, Action _onComplete) {
            ChronosHandler _handler = ApplyChronos(_id, _chronos, _type);
            delay = Delayer.Call(_duration, RemoveChronos, _realTime);

            return _handler;

            // ----- Local Method ----- \\

            void RemoveChronos() {

                Remove();
                _onComplete?.Invoke();
            }
        }

        // -------------------------------------------
        // Behaviour
        // -------------------------------------------

        /// <summary>
        /// Removes this chronos modifer from the game.
        /// </summary>
        public void Remove() {

            // Ignore if inactive.
            if (type == Type.None) {
                return;
            }

            Type _type = type;
            SetType(Type.None);

            delay.Cancel();
            id = 0;

            ChronosManager.ReleaseChronos(this, _type);
        }
        #endregion

        #region Pool
        void IPoolableObject.OnCreated() { }

        void IPoolableObject.OnRemovedFromPool() { }

        void IPoolableObject.OnSentToPool() {

            // Make sure the chronos is not active.
            Remove();
        }
        #endregion

        #region Utility
        /// <summary>
        /// Sets the type of this object.
        /// </summary>
        /// <param name="_type">New type of this object.</param>
        private void SetType(Type _type) {
            type = _type;
        }
        #endregion
    }

    /// <summary>
    /// Game global chronos manager singleton instance.
    /// <br/> Manages the whole time scale of the game, with numerous multiplicators and overrides.
    /// </summary>
    [ScriptGizmos(false, true)]
    [DefaultExecutionOrder(-990)]
    [AddComponentMenu(FrameworkUtility.MenuPath + "Chronos/Chronos Manager"), DisallowMultipleComponent]
    public class ChronosManager : EnhancedSingleton<ChronosManager>, IObjectPoolManager<ChronosModifier>, IGameStateLifetimeCallback {
        public override UpdateRegistration UpdateRegistration => base.UpdateRegistration | UpdateRegistration.Init;

        #region Global Members
        public const float ChronosDefaultValue = 1f;

        [Section("Chronos Manager")]

        [SerializeField] private SerializedType<IPauseChronosState> pauseStateType = new SerializedType<IPauseChronosState>(SerializedTypeConstraint.None,
                                                                                                                            typeof(DefaultPauseChronosGameState));

        [Space(5f)]

        [SerializeField] private bool usePauseInterface = false;
        [SerializeField, Enhanced, ShowIf("usePauseInterface"), Required] private FadingObjectBehaviour pauseInterface = null;

        [Space(10f)]

        [SerializeField, Enhanced, ReadOnly, DisplayName("Chronos")] private float gameChronos = ChronosDefaultValue;
        [SerializeField, Enhanced, ReadOnly] private float coefficient = ChronosDefaultValue;

        /// <summary>
        /// The game global chronos value (applied on <see cref="Time.timeScale"/>).
        /// </summary>
        public float GameChronos {
            get { return gameChronos; }
        }

        /// <summary>
        /// <see cref="GameState"/> type used when pausing the game chronos.
        /// </summary>
        public Type PauseStateType {
            get { return pauseStateType.Type; }
        }
        #endregion

        #region Enhanced Behaviour
        private readonly int editorChronosID = EnhancedUtility.GenerateGUID();

        // -----------------------

        protected override void OnBehaviourEnabled() {
            base.OnBehaviourEnabled();

            // Override the default chronos behaviour to implement it as a coefficient.
            ChronosStepper.OnSetChronos = ApplyEditorChronos;
        }

        protected override void OnInit() {
            base.OnInit();

            // Initialization.
            pool.Initialize(this);
        }

        protected override void OnBehaviourDisabled() {
            base.OnBehaviourDisabled();

            ChronosStepper.OnSetChronos = (f) => Time.timeScale = f;
        }

        // -----------------------

        private void ApplyEditorChronos(float _chronos) {
            PushCoefficient(editorChronosID, _chronos);
        }
        #endregion

        #region Chronos Override
        private static readonly BufferR<ChronosModifier> chronosBuffers = new BufferR<ChronosModifier>(new ChronosModifier(1f));

        // -----------------------

        /// <inheritdoc cref="PushOverride(int, float, int, float, bool, Action)"/>
        public ChronosHandler PushOverride(int _id, float _chronosOverride, int _priority) {
            return PushOverride(_id, _priority, ApplyChronos);

            // ----- Local Method ----- \\

            ChronosHandler ApplyChronos(ChronosModifier _modifier) {
                return _modifier.ApplyChronos(_id, _chronosOverride, ChronosModifier.Type.Override);
            }
        }

        /// <summary>
        /// Pushes a new chronos override in buffer.
        /// <br/> The active override is always the last pushed in buffer with the highest priority.
        /// </summary>
        /// <param name="_id">Id of this override. Use the same id to modify or pop its value.</param>
        /// <param name="_chronosOverride">Chronos override value.</param>
        /// <param name="_priority">Priority of this override. Only the one with the highest value will be active.</param>
        /// <param name="_duration">Duration of this override (in seconds).</param>
        /// <param name="_realTime">If true, the given duration will not be affected by the game time scale.</param>
        /// <param name="_onComplete">Delegate to be called once the override is removed.</param>
        /// <inheritdoc cref="ChronosModifier.ApplyChronos(int, float, ChronosModifier.Type, float, bool, Action)"/>
        public ChronosHandler PushOverride(int _id, float _chronosOverride, int _priority, float _duration, bool _realTime = true, Action _onComplete = null) {
            return PushOverride(_id, _priority, ApplyChronos);

            // ----- Local Method ----- \\

            ChronosHandler ApplyChronos(ChronosModifier _modifier) {
                return _modifier.ApplyChronos(_id, _chronosOverride, ChronosModifier.Type.Override, _duration, _realTime, _onComplete);
            }
        }

        /// <summary>
        /// Pops a previously pushed in buffer chronos override.
        /// </summary>
        /// <param name="_id">Id of the override to pop.</param>
        public void PopOverride(int _id) {
            if (chronosBuffers.Find((c) => c.First.ID == _id, out var _element)) {
                _element.First.Remove();
            }
        }

        // -----------------------

        private ChronosHandler PushOverride(int _id, int _priority, Func<ChronosModifier, ChronosHandler> _creator) {

            // Get modifier.
            ChronosModifier _modifier;

            if (chronosBuffers.Find((c) => c.First.ID == _id, out var _element)) {
                _modifier = _element.First;
            } else {
                _modifier = GetChronos();
            }

            // Apply.
            ChronosHandler _handler = _creator(_modifier);

            float _chronos = chronosBuffers.Push(_modifier, _priority).Chronos;
            RefreshChronos(_chronos);

            return _handler;
        }

        private void RefreshChronos(float _chronos) {
            gameChronos = _chronos;
            Time.timeScale = Mathf.Min(99f, _chronos * coefficient);
        }
        #endregion

        #region Coefficient
        private static readonly EnhancedCollection<ChronosModifier> coefficientBuffer = new EnhancedCollection<ChronosModifier>();

        // -----------------------

        /// <inheritdoc cref="PushCoefficient(int, float, float, bool, Action)"/>
        public ChronosHandler PushCoefficient(int _id, float _chronosOverride) {
            return PushCoefficient(_id, ApplyChronos);

            // ----- Local Method ----- \\

            ChronosHandler ApplyChronos(ChronosModifier _modifier) {
                return _modifier.ApplyChronos(_id, _chronosOverride, ChronosModifier.Type.Coefficient);
            }
        }

        /// <summary>
        /// Pushes a new chronos coefficient in buffer.
        /// <br/> Coefficients are only applied when no global override is active.
        /// </summary>
        /// <param name="_id">Id of this coefficient. Use the same id to modify or pop its value.</param>
        /// <param name="_chronosOverride">Chronos coefficient value.</param>
        /// <param name="_duration">Duration of this coefficient (in seconds).</param>
        /// <param name="_realTime">If true, the given duration will not be affected by the game time scale.</param>
        /// <param name="_onComplete">Delegate to be called once the coefficient is removed.</param>
        /// <inheritdoc cref="ChronosModifier.ApplyChronos(int, float, ChronosModifier.Type, float, bool, Action)"/>
        public ChronosHandler PushCoefficient(int _id, float _chronosOverride, float _duration, bool _realTime = true, Action _onComplete = null) {
            return PushCoefficient(_id, ApplyChronos);

            // ----- Local Method ----- \\

            ChronosHandler ApplyChronos(ChronosModifier _modifier) {
                return _modifier.ApplyChronos(_id, _chronosOverride, ChronosModifier.Type.Coefficient, _duration, _realTime, _onComplete);
            }
        }

        /// <summary>
        /// Pops a previously pushed in buffer chronos coefficient.
        /// </summary>
        /// <param name="_id">Id of the coefficient to pop.</param>
        public void PopCoefficient(int _id) {
            if (coefficientBuffer.Find((c) => c.ID == _id, out var _chronos)) {
                _chronos.Remove();
            }
        }

        // -----------------------

        private ChronosHandler PushCoefficient(int _id, Func<ChronosModifier, ChronosHandler> _creator) {

            // Get modifier.
            ChronosModifier _modifier;

            if (coefficientBuffer.Find((c) => c.ID == _id, out var _element)) {
                _modifier = _element;
            } else {
                _modifier = GetChronos();
                coefficientBuffer.Add(_modifier);
            }

            // Apply.
            ChronosHandler _handler = _creator(_modifier);
            UpdateCoefficient();

            return _handler;
        }

        private void UpdateCoefficient() {
            float _coef = ChronosDefaultValue;

            foreach (var _pair in coefficientBuffer) {
                _coef *= _pair.Chronos;
            }

            coefficient = _coef;
            RefreshChronos(gameChronos);
        }
        #endregion

        #region Pause
        private GameState pauseState = null;

        // -----------------------

        /// <summary>
        /// Pauses the game and set its chronos to zero.
        /// </summary>
        [Button(ActivationMode.Play, SuperColor.Pumpkin)]
        public void Pause() {
            if (!pauseState.IsActive()) {
                pauseState = GameState.CreateState(pauseStateType);
            }
        }

        /// <summary>
        /// Resumes the game state and reset its chronos.
        /// </summary>
        [Button(ActivationMode.Play, SuperColor.Green)]
        public void Resume() {
            if (pauseState.IsActive()) {
                pauseState.RemoveState();
            }
        }

        // -----------------------

        void IGameStateLifetimeCallback.OnInit(GameState _state) {
            pauseState = _state;

            if (usePauseInterface) {
                pauseInterface.Show();
            }
        }

        void IGameStateLifetimeCallback.OnTerminate(GameState _state) {
            pauseState = null;

            if (usePauseInterface) {
                pauseInterface.Hide();
            }
        }
        #endregion

        #region Pool
        private static readonly ObjectPool<ChronosModifier> pool = new ObjectPool<ChronosModifier>(3);

        // -----------------------

        /// <summary>
        /// Get a <see cref="ChronosModifier"/> instance from the pool.
        /// </summary>
        /// <inheritdoc cref="ObjectPool{T}.Get"/>
        private static ChronosModifier GetChronos() {
            return pool.Get();
        }

        /// <summary>
        /// Releases a specific <see cref="ChronosModifier"/> instance and sent it back to the pool.
        /// </summary>
        /// <inheritdoc cref="ObjectPool{T}.Release(T)"/>
        internal static bool ReleaseChronos(ChronosModifier _chronos, ChronosModifier.Type _type) {

            switch (_type) {

                // Override.
                case ChronosModifier.Type.Override:
                    Instance.RefreshChronos(chronosBuffers.Pop(_chronos).Chronos);
                    break;

                // Coefficient.
                case ChronosModifier.Type.Coefficient:
                    coefficientBuffer.Remove(_chronos);
                    Instance.UpdateCoefficient();
                    break;

                // Ignore.
                case ChronosModifier.Type.None:
                default:
                    break;
            }

            return pool.Release(_chronos);
        }

        /// <summary>
        /// Clears the <see cref="ChronosModifier"/> pool content.
        /// </summary>
        /// <inheritdoc cref="ObjectPool{T}.Clear(int)"/>
        public static void ClearPool(int _capacity = 1) {
            pool.Clear(_capacity);
        }

        // -------------------------------------------
        // Manager
        // -------------------------------------------

        /// <inheritdoc cref="IObjectPoolManager{ChronosModifier}.CreateInstance"/>
        ChronosModifier IObjectPoolManager<ChronosModifier>.CreateInstance() {
            return new ChronosModifier();
        }

        /// <inheritdoc cref="IObjectPoolManager{ChronosModifier}.DestroyInstance(ChronosModifier)"/>
        void IObjectPoolManager<ChronosModifier>.DestroyInstance(ChronosModifier _call) {
            // Cannot destroy the instance, so simply ignore the object and wait for the garbage collector to pick it up.
        }
        #endregion
    }
}
