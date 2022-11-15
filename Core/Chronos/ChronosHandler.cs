// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

#if DOTWEEN_ENABLED
using DG.Tweening;
using DG.Tweening.Core;
using EnhancedEditor;
using UnityEngine;

namespace EnhancedFramework.Core {
    /// <summary>
    /// Manages the chronos of all associated <see cref="EnhancedBehaviour"/> and <see cref="Animator"/>.
    /// </summary>
    [DisallowMultipleComponent]
    public class ChronosHandler : MonoBehaviour {
        #region Chronos Alteration
        /// <summary>
        /// Chronos alteration wrapper class.
        /// </summary>
        private struct ChronosAlteration {
            public readonly Tween Tween;
            public readonly int ID;

            // -----------------------

            public ChronosAlteration(int _id, EaseCurveTween<float> _easeCurve, DOGetter<float> _getter, DOSetter<float> _setter, TweenCallback _onComplete) {
                Tween = _easeCurve.To(_getter, _setter).SetUpdate(true).OnComplete(_onComplete).Pause();
                ID = _id;
            }

            // -----------------------

            public void Pause() {
                Tween.Pause();
            }

            public void Play() {
                Tween.Play();
            }

            public void Complete() {
                Tween.Complete(true);
            }
        }
        #endregion

        #region Global Members
        [Section("Chronos Handler")]

        [SerializeField, Enhanced, ReadOnly] private float chronos = ChronosManager.ChronosDefaultValue;
        [SerializeField, Enhanced, ReadOnly] private int chronosID = 0;

        /// <summary>
        /// The global chronos value of this handler.
        /// </summary>
        public float Chronos {
            get { return chronos; }
        }

        // -----------------------

        [SerializeField] private EnhancedBehaviour[] behaviours = new EnhancedBehaviour[0];
        [SerializeField] private Animator[] animators           = new Animator[0];

        // -----------------------

        private BufferV<ChronosAlteration> chronosBuffer = new BufferV<ChronosAlteration>();
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
        /// Pushes a new chronos alteration into the buffer.
        /// <br/> The chronos alteration is used to modify an object time perception.
        /// </summary>
        /// <param name="_id">The unique id used for this handler alteration.</param>
        /// <param name="_alteration">The ateration to apply on this <see cref="ChronosHandler"/> associated objects.</param>
        /// <param name="_priority">The priority of this alteration.</param>
        public void PushChronos(int _id, EaseCurveTween<float> _alteration, int _priority) {
            ChronosAlteration _chronos = new ChronosAlteration(_id, _alteration, GetChronos, SetChronos, () => OnChronosComplete(_id));
            chronosBuffer.Set(_id, new Pair<ChronosAlteration, int>(_chronos, _priority));

            RefreshChronos();
        }

        /// <summary>
        /// Pops the chronos associated with a specific id from the buffer.
        /// </summary>
        /// <param name="_id">The id to pop the associated alteration.</param>
        public void PopChronos(int _id) {
            chronosBuffer.Pop(_id);
            RefreshChronos();
        }

        /// <summary>
        /// Freezes this handler and set its chronos to zero.
        /// </summary>
        /// <param name="_id">The unique id used to freeze this handler.</param>
        /// <param name="_priority">The priority of this alteration.</param>
        /// <param name="_duration">Freeze duration (in seconds).</param>
        /// <param name="_delay">Freeze delay (in seconds).</param>
        public void Freeze(int _id, int _priority, float _duration = 0f, float _delay = 0f) {
            CurveTween<float> _chronos = new CurveTween<float>(0f, _duration, _delay, freezeCurve);
            PushChronos(_id, _chronos, _priority);
        }

        /// <summary>
        /// Clears and resets this handler chronos back to default.
        /// </summary>
        public void ClearChronos() {
            for (int i = chronosBuffer.Count; i-- > 0;) {
                chronosBuffer[i].Second.First.Complete();
            }

            SetChronos(ChronosManager.ChronosDefaultValue);
        }

        // -----------------------

        private float GetChronos() {
            return chronos;
        }

        private void SetChronos(float _chronos) {
            chronos = _chronos;

            foreach (EnhancedBehaviour _behaviour in behaviours) {
                _behaviour.Chronos = _chronos;
            }

            foreach (Animator _animator in animators) {
                _animator.speed = _chronos;
            }
        }

        private void OnChronosComplete(int _id) {
            chronosBuffer.Remove(_id);
            RefreshChronos();
        }

        // -----------------------

        private void RefreshChronos() {
            // Reset state.
            if (chronosBuffer.Count == 0) {
                chronos = ChronosManager.ChronosDefaultValue;
                chronosID = 0;

                return;
            }

            ChronosAlteration _chronos = chronosBuffer.Value;
            if (chronosBuffer.TryGetValue(chronosID, out Pair<ChronosAlteration, int> _previous) && (_chronos.ID != _previous.First.ID)) {
                _previous.First.Pause();
                _chronos.Play();

                chronosID = _chronos.ID;
            }
        }
        #endregion
    }
}
#endif
