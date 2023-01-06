// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedFramework.Core.GameStates;
using UnityEngine;

namespace EnhancedFramework.Core {
    /// <summary>
    /// Game global chronos manager singleton instance.
    /// <br/> Manages the whole time scale of the game, with numerous multiplicators and overrides.
    /// </summary>
    public class ChronosManager : EnhancedSingleton<ChronosManager>, IGameStateLifetimeCallback {
        public override UpdateRegistration UpdateRegistration => base.UpdateRegistration | UpdateRegistration.Init;

        #region Global Members
        public const float ChronosDefaultValue = 1f;

        [Section("Chronos Manager")]

        [SerializeField] private bool hasPauseInterface = false;
        [SerializeField, Enhanced, ShowIf("hasPauseInterface")] private SerializedInterface<IFadingObject> pauseInterface = null;

        [Space(10f)]

        [SerializeField, Enhanced, ReadOnly, DisplayName("Chronos")] private float gameChronos = ChronosDefaultValue;
        [SerializeField, Enhanced, ReadOnly] private float coefficient = ChronosDefaultValue;

        /// <summary>
        /// The game global chronos value (applied on <see cref="Time.timeScale"/>).
        /// </summary>
        public float GameChronos {
            get { return gameChronos; }
        }

        // -----------------------

        private readonly BufferV<float> chronosBuffer = new BufferV<float>(ChronosDefaultValue);
        private readonly PairCollection<int, float> coefficientBuffer = new PairCollection<int, float>();
        #endregion

        #region Enhanced Behaviour
        private readonly int editorChronosID = EnhancedUtility.GenerateGUID();

        // -----------------------

        protected override void OnInit() {
            base.OnInit();

            // Override the default chronos behaviour to implement it as a coefficient.
            ChronosStepper.OnSetChronos = ApplyEditorChronos;
        }

        private void OnDestroy() {
            ChronosStepper.OnSetChronos = (f) => Time.timeScale = f;
        }

        // -----------------------

        private void ApplyEditorChronos(float _chronos) {
            PushCoefficient(editorChronosID, _chronos);
        }
        #endregion

        #region Chronos Override
        /// <summary>
        /// Applies a new chronos override value on the game.
        /// <br/> The active override is always the one with the highest priority.
        /// </summary>
        /// <param name="_id">Id of this override. Use the same id to modify its value and remove it.</param>
        /// <param name="_chronosOverride">Global chronos override value.</param>
        /// <param name="_priority">Priority of this override. Only the one with the highest value will be active.</param>
        public void ApplyOverride(int _id, float _chronosOverride, int _priority) {
            float _chronos = chronosBuffer.Push(_id, new Pair<float, int>(_chronosOverride, _priority));
            RefreshChronos(_chronos);
        }

        /// <summary>
        /// Removes a previously applied chronos override value from the game.
        /// </summary>
        /// <param name="_id">Id of the override to remove (same as the one used to apply it).</param>
        public void RemoveOverride(int _id) {
            float _chronos = chronosBuffer.Pop(_id);
            RefreshChronos(_chronos);
        }

        // -----------------------

        private void RefreshChronos(float _chronos) {
            gameChronos = _chronos;
            Time.timeScale = Mathf.Min(99f, _chronos * coefficient);
        }
        #endregion

        #region Coefficient
        /// <summary>
        /// Pushes and applies a new chronos coefficient in buffer.
        /// <br/> Coefficients are only applied when no global override is active.
        /// </summary>
        /// <param name="_id">Id of this coefficient. Use the same id to modify its value and remove it.</param>
        /// <param name="_coefficient">The chronos coefficient value to apply.</param>
        public void PushCoefficient(int _id, float _coefficient) {
            coefficientBuffer.Set(_id, _coefficient);
            UpdateCoefficient();
        }

        /// <summary>
        /// Pops and removes an already pushed chronos coeffcient from the buffer.
        /// </summary>
        /// <param name="_id">Id of the coeffcient to remove (same as the one used to push it).</param>
        public void PopCoefficient(int _id) {
            coefficientBuffer.Remove(_id);
            UpdateCoefficient();
        }

        // -----------------------

        private void UpdateCoefficient() {
            float _coef = ChronosDefaultValue;

            foreach (var _pair in coefficientBuffer) {
                _coef *= _pair.Second;
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
                pauseState = GameState.CreateState<PauseChronosGameState>();
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

            if (hasPauseInterface) {
                pauseInterface.Interface.Show();
            }
        }

        void IGameStateLifetimeCallback.OnTerminate(GameState _state) {
            pauseState = null;

            if (hasPauseInterface) {
                pauseInterface.Interface.Hide();
            }
        }
        #endregion
    }
}
