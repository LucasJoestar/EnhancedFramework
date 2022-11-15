// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

#if INPUT_SYSTEM_PACKAGE
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
        [Section("Input Asset")]

        [SerializeField, Enhanced, ReadOnly] private InputMapAsset map = null;
        [SerializeField, Enhanced, ReadOnly] private InputAction input = null;

        /// <summary>
        /// The <see cref="InputMapAsset"/> associated with this input.
        /// </summary>
        public InputMapAsset Map {
            get { return map; }
        }

        /// <summary>
        /// The <see cref="InputAction"/> associated with this asset.
        /// </summary>
        public InputAction Input {
            get { return input; }
        }
        #endregion

        #region Initialization
        internal void Initialize(InputAction _input) {
            input = _input;
        }

        internal void Initialize(InputActionAsset _asset) {
            input = _asset.FindAction(input.id);
        }

        internal bool SetMap(InputMapAsset _map) {
            if (_map.Map.Contains(input)) {
                map = _map;
                return true;
            }

            return false;
        }
        #endregion

        #region Input
        public override bool Performed() {
            return input.WasPerformedThisFrame();
        }

        public override bool Pressed() {
            return input.WasPressedThisFrame();
        }

        public override bool Holding() {
            InputActionPhase _phase = input.phase;
            return (_phase != InputActionPhase.Waiting) && (_phase != InputActionPhase.Started) && (_phase != InputActionPhase.Performed);
        }

        // -----------------------

        public override float GetAxis() {
            return input.ReadValue<float>();
        }

        public override Vector2 GetVector2Axis() {
            return input.ReadValue<Vector2>();
        }

        public override Vector3 GetVector3Axis() {
            Vector2 _value = input.ReadValue<Vector2>();
            return new Vector3(_value.x, 0f, _value.y);
        }
        #endregion
    }
}
#endif
