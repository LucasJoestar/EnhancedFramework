// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
//  • Add dead zone value.
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedFramework.Core;
using System;
using UnityEngine;

namespace EnhancedFramework.Input {
    /// <summary>
    /// Axis <see cref="InputActionEnhancedAsset"/>-related asset,
    /// <br/> always using the last pressed input as the winning side.
    /// </summary>
    [Serializable]
    public class AxisInputActionEnhancedAsset : BaseInputActionEnhancedAsset {
        #region Global Members
        [Section("Enhanced Axis Input Action")]

        [SerializeField, Enhanced, Required] private InputActionEnhancedAsset positive = null;
        [SerializeField, Enhanced, Required] private InputActionEnhancedAsset negative = null;

        [Space(10f)]

        [SerializeField, Enhanced, ReadOnly] private bool isPositive = true;

        // -----------------------

        public override bool IsEnabled {
            get { return positive.IsEnabled && negative.IsEnabled; }
        }
        #endregion

        #region Side Comparison
        private float positiveValue = 0f;
        private float negativeValue = 0f;

        // -----------------------

        private InputActionEnhancedAsset UpdateActiveInput() {
            float _positive = positive.GetAxis();
            float _negative = negative.GetAxis();

            InputActionEnhancedAsset _active = isPositive ? positive : negative;

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
            this.LogError("Axis Input error! This asset cannot be used for getting a Vector2 value");
            return Vector2.zero;
        }

        public override Vector3 GetVector3Axis() {
            this.LogError("Axis Input error! This asset cannot be used for getting a Vector3 value");
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
