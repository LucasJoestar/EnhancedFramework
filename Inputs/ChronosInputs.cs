// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedFramework.Core;
using EnhancedFramework.Core.GameStates;
using UnityEngine;

namespace EnhancedFramework.Inputs {
    /// <summary>
    /// <see cref="ChronosManager"/>-related input utility class.
    /// </summary>
    [ScriptGizmos(false, true)]
    [ScriptingDefineSymbol("CHRONOS_INPUT", "Chronos runtime inputs")]
    [AddComponentMenu(FrameworkUtility.MenuPath + "Chronos/Chronos Inputs Controller"), DisallowMultipleComponent]
    public class ChronosInputs : EnhancedBehaviour {
        public override UpdateRegistration UpdateRegistration => base.UpdateRegistration | UpdateRegistration.Init;

        #region Global Members
        [Section("Chronos Input")]

        [SerializeField, Enhanced, Required] private InputActionEnhancedAsset pause = null;

        [Space(10f)]

        [SerializeField, Enhanced, Required] private InputActionEnhancedAsset increaseChronos   = null;
        [SerializeField, Enhanced, Required] private InputActionEnhancedAsset decreaseChronos   = null;
        [SerializeField, Enhanced, Required] private InputActionEnhancedAsset resetChronos      = null;
        #endregion

        #region Enhanced Behaviour
        protected override void OnInit() {
            base.OnInit();

            #if CHRONOS_INPUT
            if (pause.IsValid()) {
                pause.OnPerformed += (InputActionEnhancedAsset _) => GameState.ToggleState(ChronosManager.Instance.PauseStateType);
            }

            if (increaseChronos.IsValid()) {
                increaseChronos.OnPerformed += (InputActionEnhancedAsset _) => ChronosStepper.Increase();
            }

            if (decreaseChronos.IsValid()) {
                decreaseChronos.OnPerformed += (InputActionEnhancedAsset _) => ChronosStepper.Decrease();
            }

            if (resetChronos.IsValid()) {
                resetChronos.OnPerformed    += (InputActionEnhancedAsset _) => ChronosStepper.Reset();
            }
            #endif
        }
        #endregion
    }
}
