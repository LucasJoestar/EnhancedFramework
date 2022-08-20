// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using UnityEngine;

namespace EnhancedFramework.Core {
    /// <summary>
    /// Multiple extension methods related to the <see cref="AnimationCurve"/> class.
    /// </summary>
    public static class AnimationCurveExtensions {
        #region Content
        /// <summary>
        /// Get the total duration of a specific <see cref="AnimationCurve"/>.
        /// </summary>
        /// <param name="_curve"><see cref="AnimationCurve"/> to get duration.</param>
        /// <returns>This curve duration.</returns>
        public static float Duration(this AnimationCurve _curve) {
            return _curve[_curve.length - 1].time;
        }

        /// <summary>
        /// Get the first key value of this curve.
        /// </summary>
        /// <param name="_curve"><see cref="AnimationCurve"/> to get first value from.</param>
        /// <returns>First key value of this curve.</returns>
        public static float First(this AnimationCurve _curve) {
            return _curve[0].value;
        }

        /// <summary>
        /// Get the last key value of this curve.
        /// </summary>
        /// <param name="_curve"><see cref="AnimationCurve"/> to get last value from.</param>
        /// <returns>Last key value of this curve.</returns>
        public static float Last(this AnimationCurve _curve) {
            return _curve[_curve.length - 1].value;
        }
        #endregion
    }
}
