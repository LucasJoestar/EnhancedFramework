// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using System;
using UnityEngine;

namespace EnhancedFramework.Inputs {
    /// <summary>
    /// Multiple <see cref="SingleInputActionEnhancedAsset"/>-related asset,
    /// <br/> being performed when all specified inputs are being performed together.
    /// </summary>
    [CreateAssetMenu(fileName = FilePrefix + "MultiInputAction", menuName = MenuPath + "Multi Action", order = MenuOrder)]
    public class MultipleInputActionEnhancedAsset : InputActionEnhancedAsset {
        public const string FilePrefix  = "IPX_";

        #region Global Members
        [Space(10f)]

        [SerializeField] private SingleInputActionEnhancedAsset[] inputs = new SingleInputActionEnhancedAsset[] { };

        // -----------------------

        public override bool IsEnabled {
            get { return Array.TrueForAll(inputs, i => i.IsEnabled); }
            set {
                if (value == IsEnabled) {
                    return;
                }

                if (value) {
                    Enable();
                } else {
                    Disable();
                }
            }
        }
        #endregion

        #region Initialization
        protected override void OnInit(InputDatabase _database) {
            // Event setup.
            foreach (SingleInputActionEnhancedAsset _input in inputs) {
                _input.OnStarted += (InputActionEnhancedAsset _) => CallOnStarted();
            }

            foreach (SingleInputActionEnhancedAsset _input in inputs) {
                _input.OnCanceled += (InputActionEnhancedAsset _) => CallOnCanceled();
            }

            foreach (SingleInputActionEnhancedAsset _input in inputs) {
                _input.OnPerformed += OnInputPerformed;
            }

            // ----- Local Methods ----- \\

            void OnInputPerformed(InputActionEnhancedAsset _input) {
                if (Performed()) {
                    CallOnPerformed();
                }
            }
        }
        #endregion

        #region Input
        public override bool Performed() {
            bool _isPerformed = false;

            foreach (SingleInputActionEnhancedAsset _input in inputs) {
                if (_input.Performed()) {
                    _isPerformed = true;
                    continue;
                }

                if (!_input.Holding()) {
                    return false;
                }
            }

            return _isPerformed;
        }

        public override bool Pressed() {
            bool _isPressed = false;

            foreach (SingleInputActionEnhancedAsset _input in inputs) {
                if (_input.Pressed()) {
                    _isPressed = true;
                    continue;
                }

                if (!_input.Performed() && !_input.Holding()) {
                    return false;
                }
            }

            return _isPressed;
        }

        public override bool Holding() {
            return Array.TrueForAll(inputs, i => i.Holding());
        }

        // -----------------------

        public override float GetAxis() {
            this.LogError($"{GetType().Name} value cannot be read as an axis");
            return 0f;
        }

        public override Vector2 GetVector2Axis() {
            this.LogError($"{GetType().Name} value cannot be read as a Vector2");
            return Vector2.zero;
        }

        public override Vector3 GetVector3Axis() {
            this.LogError($"{GetType().Name} value cannot be read as a Vector3");
            return Vector3.zero;
        }
        #endregion

        #region Utility
        public override void Enable() {
            foreach (SingleInputActionEnhancedAsset _input in inputs) {
                _input.Enable();
            }
        }

        public override void Disable() {
            foreach (SingleInputActionEnhancedAsset _input in inputs) {
                _input.Disable();
            }
        }
        #endregion
    }
}
