// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedFramework.Core;
using EnhancedFramework.Core.GameStates;
using UnityEngine;

namespace EnhancedFramework.Input {
    /// <summary>
    /// Utility component used to automatically push/remove
    /// <br/> a specific <see cref="GameState"/> from the stack whenever a specific input is being performed.
    /// </summary>
    public class ToggleGameStateOnInput : EnhancedBehaviour {
        public override UpdateRegistration UpdateRegistration => base.UpdateRegistration | UpdateRegistration.Init;

        #region Global Members
        [Section("Game State Input Toggle")]

        [SerializeField] private SerializedType<GameState> gameState = null;
        [SerializeField, Enhanced, Required] private InputActionEnhancedAsset toggleInput = null;
        #endregion

        #region Enhanced Behaviour
        protected override void OnBehaviourEnabled() {
            base.OnBehaviourEnabled();

            toggleInput.Enable();
        }

        protected override void OnInit() {
            base.OnInit();

            // Toggle the game state whenever the associated input is performed.
            toggleInput.OnPerformed += ToggleGameState;
        }

        protected override void OnBehaviourDisabled() {
            base.OnBehaviourDisabled();

            toggleInput.Disable();
        }

        private void OnDestroy() {
            toggleInput.OnPerformed -= ToggleGameState;
        }

        // -----------------------

        private void ToggleGameState(InputActionEnhancedAsset _) {
            GameState.ToggleState(gameState);
        }
        #endregion
    }
}
