// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

[assembly: InternalsVisibleTo("EnhancedFramework.Editor")]
namespace EnhancedFramework.Input {
    /// <summary>
    /// <see cref="ScriptableObject"/> asset used to reference an <see cref="InputAction"/>.
    /// </summary>
    public class InputAsset : BaseInputAsset {
        #region Global Members
        /// <summary>
        /// The <see cref="InputAction"/> associated with this asset.
        /// </summary>
        [field: SerializeField, Section("Input Asset"), Enhanced, ReadOnly]
        public InputAction Action { get; private set; } = null;
        #endregion

        #region Initialization
        internal void Initialize(InputAction _action) {
            Action = _action;
        }

        internal void Initialize(InputActionAsset _map) {
            Action = _map.FindAction(Action.id);
        }
        #endregion

        #region Input
        public override bool Performed() {
            return Action.WasPerformedThisFrame();
        }

        public override bool Pressed() {
            return Action.WasPressedThisFrame();
        }

        public override bool Holding() {
            InputActionPhase _phase = Action.phase;
            return (_phase != InputActionPhase.Waiting) && (_phase != InputActionPhase.Started) && (_phase != InputActionPhase.Performed);
        }

        // -----------------------

        public override float GetAxis() {
            return Action.ReadValue<float>();
        }

        public override Vector2 GetVector2Axis() {
            return Action.ReadValue<Vector2>();
        }

        public override Vector3 GetVector3Axis() {
            Vector2 _value = Action.ReadValue<Vector2>();
            return new Vector3(_value.x, 0f, _value.y);
        }
        #endregion
    }
}
