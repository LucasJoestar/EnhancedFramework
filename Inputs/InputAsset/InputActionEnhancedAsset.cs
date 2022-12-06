// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

[assembly: InternalsVisibleTo("EnhancedFramework.Editor")]
namespace EnhancedFramework.Input {
    /// <summary>
    /// <see cref="ScriptableObject"/> asset used to reference an input action.
    /// </summary>
    public class InputActionEnhancedAsset : BaseInputActionEnhancedAsset {
        #region Global Members
        [Section("Input Asset")]

        [SerializeField, Enhanced, ReadOnly] private InputMapEnhancedAsset map  = null;
        [SerializeField, Enhanced, ReadOnly] internal InputAction input = null;

        /// <summary>
        /// The <see cref="InputMapEnhancedAsset"/> associated with this input.
        /// </summary>
        public InputMapEnhancedAsset Map {
            get { return map; }
        }

        public override bool IsEnabled {
            get { return input.enabled; }
        }

        // -----------------------

        /// <summary>
        /// Called when this input starts performing.
        /// </summary>
        public event Action<InputActionEnhancedAsset> OnStarted   = null;

        /// <summary>
        /// Called when this input peform action has been canceled.
        /// </summary>
        public event Action<InputActionEnhancedAsset> OnCanceled  = null;

        /// <summary>
        /// Called when this input has been fully performed.
        /// </summary>
        public event Action<InputActionEnhancedAsset> OnPerformed = null;
        #endregion

        #region Initialization
        /// <summary>
        /// Editor generator called method.
        /// </summary>
        internal void Initialize(InputAction _input) {
            input = _input;
        }

        /// <summary>
        /// <see cref="InputManager"/> runtime called method.
        /// </summary>
        internal void Initialize(InputActionAsset _asset) {
            input = _asset.FindAction(input.id);

            input.started   += (InputAction.CallbackContext _context) => OnStarted?.Invoke(this);
            input.canceled  += (InputAction.CallbackContext _context) => OnCanceled?.Invoke(this);
            input.performed += (InputAction.CallbackContext _context) => OnPerformed?.Invoke(this);
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

        #region Utility
        public override void Enable() {
            input.Enable();
        }

        public override void Disable() {
            input.Disable();
        }

        // -----------------------

        /// <summary>
        /// Editor generator called method.
        /// </summary>
        internal bool SetMap(InputMapEnhancedAsset _map) {
            if (_map.Contains(this)) {
                map = _map;
                return true;
            }

            return false;
        }
        #endregion
    }
}
