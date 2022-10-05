// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using DG.Tweening;
using EnhancedEditor;
using System;
using System.Collections.Generic;
using UnityEngine;

using Min = EnhancedEditor.MinAttribute;

namespace EnhancedFramework.Core {
    /// <summary>
    /// Base class for ease/curve values.
    /// <br/> Prefer using <see cref="EaseValue"/>/<see cref="CurveValue"/> instead of this.
    /// </summary>
    [Serializable]
    public abstract class EaseCurveValue {
        #region Global Members
        /// <summary>
        /// Range of the curve (minimum and maximum values).
        /// </summary>
        public Vector2 Range = new Vector2(0f, 1f);

        /// <summary>
        /// Duration of the curve.
        /// </summary>
        [Enhanced, Min(.001f)] public float Duration = 1f;

        protected readonly Dictionary<int, float> timeWrapper = new Dictionary<int, float>();

        // -----------------------

        protected EaseCurveValue() { }

        protected EaseCurveValue(Vector2 _range, float _duration) {
            Range = _range;
            Duration = _duration;
        }
        #endregion

        #region Registration
        /// <summary>
        /// Registers a new id for this value, and create a specific wrapper for it.
        /// <br/> Registration must be performed before any use.
        /// </summary>
        /// <param name="_id">The new id to register.</param>
        public virtual void Register(int _id) {
            timeWrapper.Add(_id, 0f);
        }

        /// <summary>
        /// Unregisters a specific id from this value, along with its associated wrapper.
        /// <br/> Use this when a registered object is being destroyed.
        /// </summary>
        /// <param name="_id">The id to unregister.</param>
        public virtual void Unregister(int _id) {
            timeWrapper.Remove(_id);
        }
        #endregion

        #region Behaviour
        /// <summary>
        /// Continue evaluating this value by increasing its current time.
        /// </summary>
        /// <param name="_id"><inheritdoc cref="DoEvaluate(int, float)" path="/param[@name='_id']"/></param>
        /// <param name="_increase">Value time increase (in seconds).</param>
        /// <returns>This value at the current evaluation time.</returns>
        public float EvaluateContinue(int _id, float _increase) {
            float _time = Mathf.Clamp(timeWrapper[_id] + _increase, 0f, Duration);
            timeWrapper[_id] = _time;

            return DoEvaluate(_id, _time / Duration);
        }

        /// <summary>
        /// Evaluates this value at a specific time.
        /// </summary>
        /// <param name="_id"><inheritdoc cref="DoEvaluate(int, float)" path="/param[@name='_id']"/></param>
        /// <param name="_time">Time to evaluate this value at.</param>
        /// <returns>This value at the given time.</returns>
        public float Evaluate(int _id, float _time) {
            timeWrapper[_id] = _time;
            return DoEvaluate(_id, _time / Duration);
        }

        /// <summary>
        /// Evaluates this curve value at a specific percentage (from 0 to 1).
        /// </summary>
        /// <param name="_id"><inheritdoc cref="DoEvaluate(int, float)" path="/param[@name='_id']"/></param>
        /// <param name="_percent">Lifetime percentage to evaluate the curve at.</param>
        /// <returns>The curve value at the given percentage.</returns>
        public float EvaluatePercent(int _id, float _percent) {
            timeWrapper[_id] = Duration * _percent;
            return DoEvaluate(_id, _percent);
        }

        // -----------------------

        /// <summary>
        /// Use this to implement this value evaluation behaviour.
        /// </summary>
        /// <param name="_id">The identifier to retrieve the associated values from
        /// (use <see cref="Register(int)"/> to register a new id).</param>
        protected abstract float DoEvaluate(int _id, float _percent);
        #endregion

        #region Utility
        /// <summary>
        /// Get the current evaluation time ratio (between 0 and 1) of this value.
        /// </summary>
        /// <param name="_id"><inheritdoc cref="DoEvaluate(int, float)" path="/param[@name='_id']"/></param>
        public float GetTimeRatio(int _id) {
            return timeWrapper[_id] / Duration;
        }

        /// <summary>
        /// Resets this value back to its first value.
        /// </summary>
        /// <param name="_id"><inheritdoc cref="DoEvaluate(int, float)" path="/param[@name='_id']"/></param>
        /// <returns>This value first default value.</returns>
        public float Reset(int _id) {
            timeWrapper[_id] = 0f;
            return DoEvaluate(_id, 0f);
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

        protected override float DoEvaluate(int _id, float _percent) {
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

        protected override float DoEvaluate(int _id, float _percent) {
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
        #region Decrease Wrapper
        /// <summary>
        /// Wrapper for object instance decrease values.
        /// </summary>
        private class DecreaseWrapper {
            public float Duration = 0f;
            public float From = 0f;
            public float Time = 0f;

            // -----------------------

            public float GetTime(AnimationCurve _curve) {
                return DOVirtual.EasedValue(0f, From, Time / Duration, _curve);
            }
        }
        #endregion

        #region Global Members
        /// <summary>
        /// Duration of the decrease curve.
        /// </summary>
        [Space(10f), Enhanced, Min(.001f)] public float DecreaseDuration = .1f;

        /// <summary>
        /// Animation curve used to decrease this value current time.
        /// </summary>
        [Enhanced, EnhancedCurve(0f, 0f, 1f, 1f, SuperColor.Crimson)] public AnimationCurve DecreaseCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

        private readonly Dictionary<int, DecreaseWrapper> decreaseWrapper = new Dictionary<int, DecreaseWrapper>();

        // -----------------------

        public AdvancedCurveValue() { }

        public AdvancedCurveValue(Vector2 _range, float _duration, AnimationCurve _curve) : base(_range, _duration, _curve) { }
        #endregion

        #region Registration
        public override void Register(int _id) {
            base.Register(_id);

            decreaseWrapper.Add(_id, new DecreaseWrapper());
        }

        public override void Unregister(int _id) {
            base.Unregister(_id);

            decreaseWrapper.Remove(_id);
        }
        #endregion

        #region Behaviour
        protected override float DoEvaluate(int _id, float _percent) {
            float _value = base.DoEvaluate(_id, _percent);

            DecreaseWrapper _wrapper = decreaseWrapper[_id];

            // Reset decrease value timer on time increase.
            if (_wrapper.Time != 0f) {
                float _decreaseTime = _wrapper.GetTime(DecreaseCurve);

                if (GetTimeRatio(_id) > _decreaseTime) {
                    _wrapper.Time = 0f;
                }
            }

            return _value;
        }

        /// <summary>
        /// Decreases this value, using a specific decrease curve.
        /// </summary>
        /// <param name="_decrease">Value time decrease (in seconds).
        /// <br/> Must be positive.</param>
        /// <returns>The curve value with applied time decrease.</returns>
        public float Decrease(int _id, float _decrease) {
            DecreaseWrapper _wrapper = decreaseWrapper[_id];
            float _time = timeWrapper[_id];

            if (_wrapper.Time == 0f) {
                _wrapper.Duration = Mathf.Max(DecreaseDuration * GetTimeRatio(_id), .01f);
                _wrapper.From = _time;
            }

            _wrapper.Time = Mathf.Clamp(_wrapper.Time + _decrease, 0f, _wrapper.Duration);

            _time = _wrapper.GetTime(DecreaseCurve);
            timeWrapper[_id] = _time;

            return base.DoEvaluate(_id, _time / Duration);
        }
        #endregion
    }
}
