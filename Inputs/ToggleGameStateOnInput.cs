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
    /// Utility component used to automatically push/remove
    /// <br/> a specific <see cref="GameState"/> from the stack whenever a specific input is being performed.
    /// </summary>
    [ScriptGizmos(false, true)]
    [AddComponentMenu(FrameworkUtility.MenuPath + "Input/Toggle Game State On Input")]
    public class ToggleGameStateOnInput : EnhancedBehaviour {
        #region Global Members
        [Section("Game State Input Toggle")]

        [SerializeField] private SerializedType<GameState> gameState = null;
        [SerializeField, Enhanced, Required] private InputActionEnhancedAsset toggleInput = null;
        #endregion

        #region Enhanced Behaviour
        protected override void OnBehaviourEnabled() {
            base.OnBehaviourEnabled();

            // Toggle the game state whenever the associated input is performed.
            toggleInput.OnPerformed += ToggleGameState;
            toggleInput.Enable();
        }

        protected override void OnBehaviourDisabled() {
            base.OnBehaviourDisabled();

            toggleInput.OnPerformed -= ToggleGameState;
            toggleInput.Disable();
        }

        // -----------------------

        private void ToggleGameState(InputActionEnhancedAsset _) {
            GameState.ToggleState(gameState);
        }
        #endregion
    }
}
