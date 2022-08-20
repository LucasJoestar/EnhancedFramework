// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using DG.Tweening;
using EnhancedEditor;
using System;
using UnityEngine;

using Min = EnhancedEditor.MinAttribute;

namespace EnhancedFramework.Core {
    /// <summary>
    /// Base class for ease/curve values.
    /// <br/> Prefer using <see cref="EaseValue"/>/<see cref="CurveValue"/> instead of this.
    /// </summary>
    [Serializable]
    public abstract class EaseCurveValue {
        #region Content
        /// <summary>
        /// Current evaluation time of this value.
        /// </summary>
        [Enhanced, ReadOnly] public float CurrentTime = 0f;

        [Space(5f)]

        /// <summary>
        /// Range of the curve (minimum and maximum values).
        /// </summary>
        public Vector2 Range = new Vector2(0f, 1f);

        /// <summary>
        /// Duration of the curve.
        /// </summary>
        [Enhanced, Min(.001f)] public float Duration = 1f;

        /// <summary>
        /// Current evaluation time ratio (between 0 and 1) of this value.
        /// </summary>
        public float CurrentTimeRatio {
            get {
                return CurrentTime / Duration;
            }
        }

        // -----------------------

        protected EaseCurveValue() { }

        protected EaseCurveValue(Vector2 _range, float _duration) {
            Range = _range;
            Duration = _duration;
        }

        // -----------------------

        /// <summary>
        /// Continue evaluating this curve value by increasing its current time.
        /// </summary>
        /// <param name="_timeIncrease">Current curve value time increase (in seconds).</param>
        /// <returns>The curve value at the current evaluation time.</returns>
        public float EvaluateContinue(float _timeIncrease) {
            float _time = Mathf.Clamp(CurrentTime + _timeIncrease, 0f, Duration);
            return Evaluate(_time);
        }

        /// <summary>
        /// Evaluates this curve value at a specific time.
        /// </summary>
        /// <param name="_time">Time to evaluate the curve at.</param>
        /// <returns>The curve value at the given time.</returns>
        public float Evaluate(float _time) {
            return EvaluatePercent(_time / Duration);
        }

        /// <summary>
        /// Evaluates this curve value at a specific percentage (from 0 to 1).
        /// </summary>
        /// <param name="_percent">Lifetime percentage to evaluate the curve at.</param>
        /// <returns>The curve value at the given percentage.</returns>
        public abstract float EvaluatePercent(float _percent);

        /// <summary>
        /// Resets this curve back to its first value.
        /// </summary>
        /// <returns>First curve value.</returns>
        public float Reset() {
            return Evaluate(0f);
        }
        #endregion
    }

    /// <summary>
    /// Wrapper utility class for an ease curve type value.
    /// </summary>
    [Serializable]
    public class EaseValue : EaseCurveValue {
        #region Content
        /// <summary>
        /// Ease used to evaluate this value.
        /// </summary>
        public Ease Ease = Ease.OutSine;

        // -----------------------

        public EaseValue() { }

        public EaseValue(Vector2 _range, float _duration, Ease _ease) : base(_range, _duration) {
            Ease = _ease;
        }

        // -----------------------

        public override float EvaluatePercent(float _percent) {
            CurrentTime = Duration * _percent;
            return DOVirtual.EasedValue(Range.x, Range.y, _percent, Ease);
        }
        #endregion
    }

    /// <summary>
    /// Wrapper utility class for an animation curve type value.
    /// </summary>
    [Serializable]
    public class CurveValue : EaseCurveValue {
        #region Content
        /// <summary>
        /// Animation curve used to evaluate this value.
        /// </summary>
        [Enhanced, EnhancedCurve(0f, 0f, 1f, 1f, SuperColor.Lime)] public AnimationCurve Curve = AnimationCurve.Constant(0f, 1f, 0f);

        // -----------------------

        public CurveValue() { }

        public CurveValue(Vector2 _range, float _duration, AnimationCurve _curve) : base(_range, _duration) {
            Curve = _curve;
        }

        // -----------------------

        public override float EvaluatePercent(float _percent) {
            CurrentTime = Duration * _percent;
            return DOVirtual.EasedValue(Range.x, Range.y, _percent, Curve);
        }
        #endregion
    }

    /// <summary>
    /// <inheritdoc cref="CurveValue"/>
    /// <para/> Uses an additional curve to decrease its value.
    /// </summary>
    [Serializable]
    public class AdvancedCurveValue : CurveValue {
        #region Content
        /// <summary>
        /// Duration of the decrease curve.
        /// </summary>
        [Space(10f), Enhanced, Min(.001f)] public float DecreaseDuration = .1f;

        /// <summary>
        /// Animation curve used to decrease this value current time.
        /// </summary>
        [Enhanced, EnhancedCurve(0f, 0f, 1f, 1f, SuperColor.Crimson)] public AnimationCurve DecreaseCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

        private float decreaseDurationValue = 0f;
        private float decreaseTimeFrom = 0f;

        private float decreaseCurrentTime = 0f;

        // -----------------------

        public AdvancedCurveValue() { }

        public AdvancedCurveValue(Vector2 _range, float _duration, AnimationCurve _curve) : base(_range, _duration, _curve) { }

        // -----------------------

        public override float EvaluatePercent(float _percent) {
            float _time = CurrentTime;
            float _value = base.EvaluatePercent(_percent);

            // Reset decrease value timer on time increase.
            if (CurrentTime > _time) {
                decreaseCurrentTime = 0f;
            }

            return _value;
        }

        /// <summary>
        /// Decreases this value, using a specific decrease curve.
        /// </summary>
        /// <param name="_timeDecrease">Time used to decrease value (in seconds).
        /// <br/> Must be positive.</param>
        /// <returns>The curve value with applied time decrease.</returns>
        public float Decrease(float _timeDecrease) {
            if (decreaseCurrentTime == 0f) {
                decreaseDurationValue = (DecreaseDuration * CurrentTimeRatio) + .01f;
                decreaseTimeFrom = CurrentTime;
            }

            decreaseCurrentTime = Mathf.Clamp(decreaseCurrentTime + _timeDecrease, 0f, decreaseDurationValue);
            CurrentTime = DOVirtual.EasedValue(0f, decreaseTimeFrom, decreaseCurrentTime / decreaseDurationValue, DecreaseCurve);

            return Evaluate(CurrentTime);
        }
        #endregion
    }
}
