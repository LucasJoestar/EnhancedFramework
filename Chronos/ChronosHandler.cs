// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using DG.Tweening;
using DG.Tweening.Core;
using EnhancedEditor;
using EnhancedFramework.Core;
using UnityEngine;

namespace EnhancedFramework.Chronos {
    /// <summary>
    /// Manages the chronos of all associated <see cref="EnhancedBehaviour"/> and <see cref="Animator"/>.
    /// <para/>
    /// There should always be no more than one <see cref="ChronosHandler"/> per <see cref="GameObject"/>,
    /// with all its associated behaviours being linked.
    /// </summary>
    public class ChronosHandler : MonoBehaviour {
        #region Chronos Alteration
        /// <summary>
        /// Chronos alteration wrapper class.
        /// </summary>
        private struct ChronosAlteration {
            public EaseCurveTween<float> EaseCurve;
            public Tween Tween;

            // -----------------------

            public ChronosAlteration(EaseCurveTween<float> _easeCurve) {
                EaseCurve = _easeCurve;
                Tween = null;
            }

            // -----------------------

            public void Play(DOGetter<float> _getter, DOSetter<float> _setter, TweenCallback _onComplete) {
                Tween = EaseCurve.To(_getter, _setter).OnComplete(_onComplete);
            }

            public void Pause() {
                Tween.Pause();
            }

            public void Resume() {
                Tween.Play();
            }

            public void Complete() {
                Tween.Complete(true);
            }
        }
        #endregion

        #region Global Members
        [Section("Chronos Handler")]

        [SerializeField, Enhanced, ReadOnly] private float chronos = 1f;

        [Space(10f)]

        [SerializeField] private EnhancedBehaviour[] behaviours = new EnhancedBehaviour[] { };
        [SerializeField] private Animator[] animators           = new Animator[] { };

        // -----------------------

        private readonly Stamp<ChronosAlteration> chronosBuffer = new Stamp<ChronosAlteration>(2);
        #endregion

        #region Enhanced Behaviour
        #if UNITY_EDITOR
        private void OnValidate() {
            if (behaviours.Length == 0) {
                behaviours = GetComponentsInChildren<EnhancedBehaviour>();
            }

            if (animators.Length == 0) {
                animators = GetComponentsInChildren<Animator>();
            }
        }
        #endif
        #endregion

        #region Chronos
        private static readonly AnimationCurve freezeCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

        // -----------------------

        /// <summary>
        /// Push a new chronos iteration into the buffer.
        /// <br/> A chronos alteration is used to modify an object time perception.
        /// </summary>
        /// <param name="_alteration">Ateration to apply on this object.</param>
        public void PushChronos(EaseCurveTween<float> _alteration) {
            // Pause current alteration if one.
            if (chronosBuffer.SafeLast(out var _current)) {
                _current.Tween.Pause();
            }

            // Alter chronos using a new tween.
            ChronosAlteration _chronos = new ChronosAlteration(_alteration);

            chronosBuffer.Add(_chronos);
            _chronos.Play(GetChronos, SetChronos, OnChronosComplete);
        }

        /// <summary>
        /// Pop the last chronos iteration from the buffer and restore the previous one.
        /// </summary>
        public void PopChronos() {
            if (chronosBuffer.SafeLast(out var _current)) {
                _current.Complete();
            }
        }

        /// <summary>
        /// Freezes this object.
        /// </summary>
        /// <param name="_duration">Freeze duration. Use -1 for undertemined duration.</param>
        /// <param name="_delay">Freeze delay (in seconds).</param>
        public void Freeze(float _duration = 0f, float _delay = 0f) {
            CurveTween<float> _chronos = new CurveTween<float>(0f, _duration, _delay, freezeCurve);
            PushChronos(_chronos);
        }

        /// <summary>
        /// Clears and reset this object chronos back to 1.
        /// </summary>
        public void ClearChronos() {
            foreach (var _chronos in chronosBuffer) {
                _chronos.Complete();
            }

            SetChronos(1f);
        }

        // -----------------------

        private float GetChronos() => chronos;

        private void SetChronos(float _chronos) {
            chronos = _chronos;

            foreach (EnhancedBehaviour _behaviour in behaviours) {
                _behaviour.Chronos = _chronos;
            }

            foreach (Animator _animator in animators) {
                _animator.speed = _chronos;
            }
        }

        private void OnChronosComplete() {
            chronosBuffer.RemoveLast();

            // Resume last chronos.
            if (chronosBuffer.SafeLast(out var _chronos)) {
                _chronos.Resume();
            }
        }
        #endregion
	}
}
