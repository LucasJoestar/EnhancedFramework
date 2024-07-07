// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

#if DOTWEEN_ENABLED
#define DOTWEEN
#endif

using EnhancedEditor;
using System;
using UnityEngine;

#if DOTWEEN
using DG.Tweening;
using System.Collections.Generic;
#endif

#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

using Range = EnhancedEditor.RangeAttribute;

namespace EnhancedFramework.Core {
    /// <summary>
    /// <see cref="Animator"/>-related parameter configuration asset.
    /// </summary>
    [CreateAssetMenu(fileName = "ANIP_AnimatorParameter", menuName = FrameworkUtility.MenuPath + "Animation/Animator Parameter", order = FrameworkUtility.MenuOrder)]
    #pragma warning disable
    public sealed class EnhancedAnimatorParameter : EnhancedScriptableObject {
        #region Animator Wrapper
        /// <summary>
        /// Wrapper for a single <see cref="UnityEngine.Animator"/> operations.
        /// </summary>
        public sealed class AnimatorWrapper {
            private const float DelayValue = .001f;

            /// <summary>
            /// Animator wrapped in this object.
            /// </summary>
            public readonly Animator Animator = null;

            private DelayHandler delay = default;
            private TweenHandler tween = default;

            private float tweenTarget = 0f;

            /// <summary>
            /// True this animator is not affected by the game time scale, false otherwise.
            /// </summary>
            public bool RealTime {
                get { return Animator.updateMode == AnimatorUpdateMode.UnscaledTime; }
            }

            // -------------------------------------------
            // Constructor(s)
            // -------------------------------------------

            public AnimatorWrapper(Animator _animator) {
                Animator = _animator;
            }

            // -------------------------------------------
            // Utility
            // -------------------------------------------

            /// <inheritdoc cref="Delayer.Call(float, Action, bool)"/>
            public void Delay(Action _callback) {

                delay.Cancel();
                delay = Delayer.Call(DelayValue, _callback, RealTime);
            }

            #if DOTWEEN
            /// <inheritdoc cref="Tweener.Tween(float, float, Action{float}, float, Ease, bool, Action{bool})"/>
            public void Tween(float _from, float _to, Action<float> _setter, float _duration, Ease _ease) {

                if (tween.IsValid && (tweenTarget == _to)) {
                    return;
                }

                tween.Stop();

                tweenTarget = _to;
                tween = Tweener.Tween(_from, _to, _setter, _duration, _ease, RealTime, null);
            }
            #endif

            /// <inheritdoc cref="Tweener.Tween(float, float, Action{float}, float, AnimationCurve, bool, Action{bool})"/>
            public void Tween(float _from, float _to, Action<float> _setter, float _duration, AnimationCurve _curve) {

                if (tween.IsValid && (tweenTarget == _to)) {
                    return;
                }

                tween.Stop();

                tweenTarget = _to;
                tween = Tweener.Tween(_from, _to, _setter, _duration, _curve, RealTime, null);
            }

            /// <summary>
            /// Clears this wrapper content and stops its operations.
            /// </summary>
            public void Clear() {

                delay.Cancel();
                tween.Stop();
            }
        }
        #endregion

        #region Global Members
        [Section("Animator Parameter")]

        [Tooltip("Identifier name of this parameter")]
        [SerializeField, Enhanced, DisplayName("Name")] protected string parameterName = "Parameter";

        [Tooltip("Type of this parameter")]
        [SerializeField] private AnimatorControllerParameterType type = AnimatorControllerParameterType.Trigger;

        [Space(10f)]

        [Tooltip("Default value of this parameter")]
        [SerializeField, Enhanced, ShowIf(nameof(IsFloat)), DisplayName("Default")] private float defaultFloat  = 0f;

        [Tooltip("Default value of this parameter")]
        [SerializeField, Enhanced, ShowIf(nameof(IsBool)),  DisplayName("Default")] private bool defaultBool    = false;

        [Tooltip("Default value of this parameter")]
        [SerializeField, Enhanced, ShowIf(nameof(IsInt)),   DisplayName("Default")] private int defaultInt      = 0;

        [Space(10f), HorizontalLine(SuperColor.Grey, 1f), Space(10f)]

        [Tooltip("If true, parameter value update will be delayed to the next frame")]
        [SerializeField] private bool delayUpdate = false;

        [Space(5f)]

        [Tooltip("Parameter update tween duration (in seconds)")]
        [SerializeField, Enhanced, ShowIf(nameof(CanUseTween)), Range(0f, 10f)] private float updateValueDuration = 0f;

        #if DOTWEEN
        [Tooltip("Parameter update evaluation ease")]
        [SerializeField, Enhanced, ShowIf(nameof(CanUseTween))] private Ease updateEase = Ease.Linear;
        #else
        [Tooltip("Parameter update evaluation curve")]
        [SerializeField, Enhanced, ShowIf(nameof(CanUseTween))] private AnimationCurve updateCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        #endif

        // -----------------------

        [NonSerialized] protected int hash = 0;

        /// <summary>
        /// Name hash of this parameter.
        /// </summary>
        public int Hash {
            get {
                if (hash == 0) {

                    hash = Animator.StringToHash(parameterName);
                    this.LogErrorMessage("Parameter hash value was not correctly configured");
                }

                return hash;
            }
        }

        /// <summary>
        /// Whether this parameter value can be updated using a tween or not.
        /// </summary>
        public bool CanUseTween {
            get { return IsInt || IsFloat; }
        }

        /// <summary>
        /// Whether this parameter is of <see cref="bool"/> type or not.
        /// </summary>
        public bool IsBool {
            get { return type == AnimatorControllerParameterType.Bool; }
        }

        /// <summary>
        /// Whether this parameter is of <see cref="int"/> type or not.
        /// </summary>
        public bool IsInt {
            get { return type == AnimatorControllerParameterType.Int; }
        }

        /// <summary>
        /// Whether this parameter is of <see cref="float"/> type or not.
        /// </summary>
        public bool IsFloat {
            get { return type == AnimatorControllerParameterType.Float; }
        }

        /// <summary>
        /// Whether this parameter is of trigger type or not.
        /// </summary>
        public bool IsTrigger {
            get { return type == AnimatorControllerParameterType.Trigger; }
        }
        #endregion

        #region Initialization
        private readonly EnhancedCollection<AnimatorWrapper> animators = new EnhancedCollection<AnimatorWrapper>();

        // -----------------------

        /// <summary>
        /// Initializes this parameter.
        /// </summary>
        public void Initialize(Animator _animator) {
            hash = Animator.StringToHash(parameterName);
        }

        // -------------------------------------------
        // Registration
        // -------------------------------------------

        /// <summary>
        /// Registers a specific runtime <see cref="Animator"/> on this parameter.
        /// </summary>
        /// <param name="_animator"><see cref="Animator"/> to register.</param>
        public void Register(Animator _animator) {
            animators.Add(new AnimatorWrapper(_animator));
        }

        /// <summary>
        /// Unregisters a specific runtime <see cref="Animator"/> from this parameter.
        /// </summary>
        /// <param name="_animator"><see cref="Animator"/> to unregister.</param>
        public void Unregister(Animator _animator) {

            List<AnimatorWrapper> _animatorsSpan = animators.collection;
            int _count = _animatorsSpan.Count;

            for (int i = 0; i < _count; i++) {
                
                if (_animatorsSpan[i].Animator == _animator) {

                    _animatorsSpan.RemoveAt(i);
                    return;
                }
            }
        }
        #endregion

        #region Parameter
        // -------------------------------------------
        // Bool
        // -------------------------------------------

        /// <inheritdoc cref="EnhancedAnimatorController.SetBool(Animator, int, bool)"/>
        public void SetBool(Animator _animator, bool _value) {

            if (!IsBool) {

                this.LogErrorMessage($"Parameter is not of type {typeof(bool).Name.Bold()}   ({type})");
                return;
            }

            UpdateValue(_animator, Update);

            // ----- Local Method ----- \\

            void Update() {
                _animator.SetBool(Hash, _value);
            }
        }

        // -------------------------------------------
        // Int
        // -------------------------------------------

        /// <inheritdoc cref="EnhancedAnimatorController.SetInt(Animator, int, int)"/>
        public void SetInt(Animator _animator, int _value) {

            if (!IsInt) {

                this.LogErrorMessage($"Parameter is not of type {typeof(int).Name.Bold()}   ({type})");
                return;
            }

            float _from = _animator.GetInteger(Hash);
            TweenValue(_animator, _from, _value, SetValue);

            // ----- Local Method ----- \\

            void SetValue(float _value) {
                _animator.SetInteger(Hash, Mathf.RoundToInt(_value));
            }
        }

        // -------------------------------------------
        // Float
        // -------------------------------------------

        /// <inheritdoc cref="EnhancedAnimatorController.SetFloat(Animator, int, float)"/>
        public void SetFloat(Animator _animator, float _value) {

            if (!IsFloat) {

                this.LogErrorMessage($"Parameter is not of type {typeof(float).Name.Bold()}   ({type})");
                return;
            }

            float _from = _animator.GetFloat(Hash);
            TweenValue(_animator, _from, _value, SetValue);

            // ----- Local Method ----- \\

            void SetValue(float _value) {
                _animator.SetFloat(Hash, _value);
            }
        }

        // -------------------------------------------
        // Trigger
        // -------------------------------------------

        /// <inheritdoc cref="EnhancedAnimatorController.SetTrigger(Animator, int)"/>
        public void SetTrigger(Animator _animator) {

            if (!IsTrigger) {

                this.LogErrorMessage($"Parameter is not of type Trigger   ({type})");
                return;
            }

            UpdateValue(_animator, Update);

            // ----- Local Method ----- \\

            void Update() {
                _animator.SetTrigger(Hash);
            }
        }

        /// <inheritdoc cref="EnhancedAnimatorController.ResetTrigger(Animator, int)"/>
        public void ResetTrigger(Animator _animator) {

            if (!IsTrigger) {

                this.LogErrorMessage($"Parameter is not of type Trigger   ({type})");
                return;
            }

            UpdateValue(_animator, Update);

            // ----- Local Method ----- \\

            void Update() {
                _animator.ResetTrigger(Hash);
            }
        }
        #endregion

        #region Utility
        /// <summary>
        /// Manages a specific update call.
        /// </summary>
        /// <param name="_animator"><see cref="Animator"/> associated with this call.</param>
        /// <param name="_callback">Update callback.</param>
        private void UpdateValue(Animator _animator, Action _callback) {

            if (delayUpdate && GetWrapperIndex(_animator, out int _index)) {

                animators[_index].Delay(_callback);
                return;
            }

            _callback?.Invoke();
        }

        /// <summary>
        /// Manages a specific update call.
        /// </summary>
        /// <param name="_animator"><see cref="Animator"/> associated with this call.</param>
        /// <param name="_callback">Update callback.</param>
        private void TweenValue(Animator _animator, float _from, float _to, Action<float> _setter) {

            // Instant.
            if (!GetWrapperIndex(_animator, out int _index)) {

                _setter?.Invoke(_to);
                return;
            }

            if (delayUpdate) {

                animators[_index].Delay(OnUpdate);
                return;
            }

            OnUpdate();

            // ----- Local Method ----- \\

            void OnUpdate() {

                // Instant.
                if (updateValueDuration == 0f) {

                    _setter?.Invoke(_to);
                    return;
                }
                
                #if DOTWEEN
                animators[_index].Tween(_from, _to, _setter, updateValueDuration, updateEase);
                #else
                animators[_index].Tween(_from, _to, _setter, updateValueDuration, updateCurve);
                #endif
            }
        }

        /// <summary>
        /// Get the index of the wrapper associated with a specific <see cref="Animator"/>.
        /// </summary>
        /// <param name="_animator"><see cref="Animator"/> to get the associated wrapper index.</param>
        /// <param name="_index">Index of this animator wrapper.</param>
        /// <returns>True if an associated wrapper could be found for this animator, false otherwise.</returns>
        private bool GetWrapperIndex(Animator _animator, out int _index) {

            List<AnimatorWrapper> _animatorsSpan = animators.collection;
            int _count = _animatorsSpan.Count;

            for (int i = 0; i < _count; i++) {

                if (_animatorsSpan[i].Animator == _animator) {

                    _index = i;
                    return true;
                }
            }

            _index = -1;
            return false;
        }

        #if UNITY_EDITOR
        // -------------------------------------------
        // Editor
        // -------------------------------------------

        /// <summary>
        /// Creates and setups this parameter in an <see cref="AnimatorController"/>.
        /// </summary>
        /// <param name="_animator"><see cref="AnimatorController"/> on which to create this parameter.</param>
        /// <param name="_parameters">All parameters of this animator.</param>
        internal void Create(AnimatorController _animator, ref AnimatorControllerParameter[] _parameters) {

            // Parameter.
            if (!FindParameterByName(_parameters, parameterName, out AnimatorControllerParameter _parameter)) {

                _animator.AddParameter(parameterName, type);
                _parameters = _animator.parameters;

                if (!FindParameterByName(_parameters, parameterName, out _parameter)) {
                    return;
                }
            }

            // Setup.
            _parameter.type = type;

            _parameter.defaultBool  = defaultBool;
            _parameter.defaultFloat = defaultFloat;
            _parameter.defaultInt   = defaultInt;
        }

        // -----------------------

        private bool FindParameterByName(AnimatorControllerParameter[] _parameters, string _name, out AnimatorControllerParameter _parameter) {

            for (int i = 0; i < _parameters.Length; i++) {

                _parameter = _parameters[i];
                if (_parameter.name.Equals(_name, System.StringComparison.Ordinal)) {
                    return true;
                }
            }

            _parameter = null;
            return false;
        }
        #endif
        #endregion
    }
}
