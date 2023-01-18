// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedFramework.Core;
using System;
using UnityEngine;

namespace EnhancedFramework.Inputs {
    /// <summary>
    /// Axis <see cref="SingleInputActionEnhancedAsset"/>-related asset,
    /// <br/> always using the last pressed input as the winning side.
    /// </summary>
    [CreateAssetMenu(fileName = FilePrefix + "AxisInputAction", menuName = MenuPath + "Axis Action", order = MenuOrder)]
    public class AxisInputActionEnhancedAsset : InputActionEnhancedAsset {
        public const string FilePrefix  = "IPA_";

        #region Global Members
        [Space(10f)]

        [SerializeField, Enhanced, Required] private SingleInputActionEnhancedAsset positive = null;
        [SerializeField, Enhanced, Required] private SingleInputActionEnhancedAsset negative = null;

        [Space(10f)]

        [SerializeField, Enhanced, ReadOnly] private bool isPositive = true;

        // -----------------------

        public override bool IsEnabled {
            get { return positive.IsEnabled && negative.IsEnabled; }
        }
        #endregion

        #region Initialization
        protected override void OnInit(InputDatabase _database) {
            // Event setup.
            positive.OnStarted += OnInputStarted;
            negative.OnStarted += OnInputStarted;

            positive.OnCanceled += OnInputCanceled;
            negative.OnCanceled += OnInputCanceled;

            positive.OnPerformed += OnInputPerformed;
            negative.OnPerformed += OnInputPerformed;

            // ----- Local Methods ----- \\

            void OnInputStarted(InputActionEnhancedAsset _input) {
                if (_input == UpdateActiveInput()) {
                    CallOnStarted();
                }
            }

            void OnInputCanceled(InputActionEnhancedAsset _input) {
                if (_input == UpdateActiveInput()) {
                    CallOnCanceled();
                }
            }

            void OnInputPerformed(InputActionEnhancedAsset _input) {
                if (_input == UpdateActiveInput()) {
                    CallOnPerformed();
                }
            }
        }
        #endregion

        #region Side Comparison
        private float positiveValue = 0f;
        private float negativeValue = 0f;

        // -----------------------

        private SingleInputActionEnhancedAsset UpdateActiveInput() {
            float _positive = positive.GetAxis();
            float _negative = negative.GetAxis();

            SingleInputActionEnhancedAsset _active = isPositive ? positive : negative;

            // If input values have not changed, there is no need for update.
            if ((_positive == positiveValue) && (_negative == negativeValue)) {
                return _active;
            }

            bool _isPositiveIncrease = _positive > positiveValue;
            bool _isNegativeIncrease = _negative > negativeValue;

            if (!_isPositiveIncrease && !_isNegativeIncrease) {
                // Released active input.
                if (isPositive && (_positive == 0f)) {
                    _active = negative;
                    isPositive = false;
                } else if (!isPositive && (_negative == 0f)) {
                    _active = positive;
                    isPositive = true;
                }
            } else if (_isPositiveIncrease && !_isNegativeIncrease && !isPositive) {
                // Positive input increase.
                _active = positive;
                isPositive = true;
            } else if (_isNegativeIncrease && !_isPositiveIncrease && isPositive) {
                // Negative input increase.
                _active = negative;
                isPositive = false;
            }

            positiveValue = _positive;
            negativeValue = _negative;

            return _active;
        }
        #endregion

        #region Input
        public override bool Performed() {
            return UpdateActiveInput().Performed();
        }

        public override bool Pressed() {
            return UpdateActiveInput().Pressed();
        }

        public override bool Holding() {
            return UpdateActiveInput().Holding();
        }

        // -----------------------

        public override float GetAxis() {
            return UpdateActiveInput().GetAxis() * isPositive.Signf();
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
            positive.Enable();
            negative.Enable();
        }

        public override void Disable() {
            positive.Disable();
            negative.Disable();
        }
        #endregion
    }
}
