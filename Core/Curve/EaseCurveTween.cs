// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

#if DOTWEEN_ENABLED
using DG.Tweening;
using DG.Tweening.Core;
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
    public abstract class EaseCurveTween<T> {
        #region Content
        /// <summary>
        /// Target value of the tween.
        /// </summary>
        public T Value = default;

        /// <summary>
        /// Duration of the curve.
        /// </summary>
        [Enhanced, Min(.001f)] public float Duration = 1f;
        [Enhanced, Min(0f)] public float Delay = 0f;

        // -----------------------

        protected EaseCurveTween() { }

        protected EaseCurveTween(T _value, float _duration, float _delay) {
            Value = _value;
            Duration = _duration;
            Delay = _delay;
        }

        // -----------------------

        public abstract Tween SetEase(Tween _tween);
        #endregion

        #region Transform
        public Tween Move(Transform _transform, bool snapping = false) {
            if (Value is Vector3 _value) {
                return SetEase(_transform.DOMove(_value, Duration, snapping).SetDelay(Delay));
            }

            throw new InvalidTweenException("\"Move\" action can only be performed on Vector3 type tween objects.");
        }

        public Tween Rotate(Transform _transform, RotateMode _mode = RotateMode.Fast) {
            if (Value is Vector3 _euler) {
                return SetEase(_transform.DORotate(_euler, Duration, _mode).SetDelay(Delay));
            }

            throw new InvalidTweenException("\"Rotate\" action can only be performed on Vector3 type tween objects.");
        }
        
        public Tween RotateQuaternion(Transform _transform) {
            if (Value is Quaternion _quaternion) {
                return SetEase(_transform.DORotateQuaternion(_quaternion, Duration).SetDelay(Delay));
            }

            throw new InvalidTweenException("\"Rotate Quaternion\" action can only be performed on Quaternion type tween objects.");
        }

        public Tween Scale(Transform _transform) {
            if (Value is Vector3 _scale) {
                return SetEase(_transform.DOScale(_scale, Duration).SetDelay(Delay));
            } else if (Value is float _scalef) {
                return SetEase(_transform.DOScale(_scalef, Duration).SetDelay(Delay));
            }

            throw new InvalidTweenException("\"Move\" action can only be performed on Vector3 type tween objects.");
        }
        #endregion

        #region Vector
        public Tween To(DOGetter<Vector2> _getter, DOSetter<Vector2> _setter) {
            if (Value is Vector2 _value) {
                return SetEase(DOTween.To(_getter, _setter, _value, Duration).SetDelay(Delay));
            }

            throw new InvalidTweenException("\"Move\" action can only be performed on Vector3 type tween objects.");
        }

        public Tween To(DOGetter<Vector3> _getter, DOSetter<Vector3> _setter) {
            if (Value is Vector3 _value) {
                return SetEase(DOTween.To(_getter, _setter, _value, Duration).SetDelay(Delay));
            }

            throw new InvalidTweenException("\"Move\" action can only be performed on Vector3 type tween objects.");
        }
        #endregion

        #region Float
        public Tween To(DOGetter<float> _getter, DOSetter<float> _setter) {
            if (Value is float _value) {
                return SetEase(DOTween.To(_getter, _setter, _value, Duration).SetDelay(Delay));
            }

            throw new InvalidTweenException("\"Move\" action can only be performed on Vector3 type tween objects.");
        }
        #endregion
    }

    /// <summary>
    /// Wrapper utility class for an ease curve type tween.
    /// </summary>
    [Serializable]
    public class EaseTween<T> : EaseCurveTween<T> {
        #region Content
        /// <summary>
        /// Ease used to evaluate this tween.
        /// </summary>
        public Ease Ease = Ease.OutSine;

        // -----------------------

        public EaseTween() { }

        public EaseTween(T _value, float _duration, float _delay, Ease _ease) : base(_value, _duration, _delay) {
            Ease = _ease;
        }

        // -----------------------

        public override Tween SetEase(Tween _tween) {
            return _tween.SetEase(Ease);
        }
        #endregion
    }

    /// <summary>
    /// Wrapper utility class for an animation curve type tween.
    /// </summary>
    [Serializable]
    public class CurveTween<T> : EaseCurveTween<T> {
        #region Content
        /// <summary>
        /// Animation curve used to evaluate this tween.
        /// </summary>
        [Enhanced, EnhancedCurve(0f, 0f, 1f, 1f, SuperColor.Lime)] public AnimationCurve Curve = AnimationCurve.Constant(0f, 1f, 0f);

        // -----------------------

        public CurveTween() { }

        public CurveTween(T _value, float _duration, float _delay, AnimationCurve _curve) : base(_value, _duration, _delay) {
            Curve = _curve;
        }

        // -----------------------

        public override Tween SetEase(Tween _tween) {
            return _tween.SetEase(Curve);
        }
        #endregion
    }

    #region Exception
    /// <summary>
    /// Exception for wrong-typed <see cref="EaseCurveTween{T}"/> when requesting to perform a tween.
    /// </summary>
    public class InvalidTweenException : Exception {
        public InvalidTweenException() : base() { }

        public InvalidTweenException(string _message) : base(_message) { }

        public InvalidTweenException(string _message, Exception _innerException) : base(_message, _innerException) { }
    }
    #endregion
}
#endif
