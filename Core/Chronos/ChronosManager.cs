// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace EnhancedFramework.Core {
    /// <summary>
    /// Game global chronos manager singleton instance.
    /// <br/> Manages the whole time scale of the game, with numerous multiplicators and overrides.
    /// </summary>
    public class ChronosManager : EnhancedSingleton<ChronosManager> {
        public override UpdateRegistration UpdateRegistration => base.UpdateRegistration | UpdateRegistration.Init;

        #region Global Members
        public const float ChronosDefaultValue = 1f;

        [Section("Chronos Manager")]

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
            EnhancedEditor.Chronos.OnSetChronos = ApplyEditorChronos;
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
            Time.timeScale = gameChronos
                           = _chronos;
        }
        #endregion

        #region Coefficient
        private const int ChronosCoefficientPriority = -999;
        private readonly int chronosCoefficientID = EnhancedUtility.GenerateGUID();

        // -----------------------

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
            ApplyOverride(chronosCoefficientID, coefficient, ChronosCoefficientPriority);
        }
        #endregion
    }
}
