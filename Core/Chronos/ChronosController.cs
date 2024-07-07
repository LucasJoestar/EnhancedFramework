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
    [ScriptGizmos(false, true)]
    [AddComponentMenu(FrameworkUtility.MenuPath + "Utility/Chronos Controller"), DisallowMultipleComponent]
    public sealed class ChronosController : EnhancedBehaviour {
        #region Chronos Alteration
        /// <summary>
        /// Chronos alteration wrapper class.
        /// </summary>
        private readonly struct ChronosAlteration {
            public readonly Tween Tween;
            public readonly int ID;

            // -------------------------------------------
            // Constructor(s)
            // -------------------------------------------

            public ChronosAlteration(int _id, EaseCurveTween<float> _easeCurve, DOGetter<float> _getter, DOSetter<float> _setter, TweenCallback _onComplete) {
                Tween = _easeCurve.To(_getter, _setter).SetUpdate(true).OnKill(_onComplete)
                                  .SetRecyclable(true).SetAutoKill(true).Pause();
                ID = _id;
            }

            // -------------------------------------------
            // Utility
            // -------------------------------------------

            public readonly void Pause() {
                Tween.Pause();
            }

            public readonly void Play() {
                Tween.Play();
            }

            public readonly void Complete() {
                Tween.DoKill(true);
            }
        }
        #endregion

        #region Global Members
        [Section("Chronos Manipulator")]

        [SerializeField, Enhanced, ReadOnly] private float handlerChronos = ChronosManager.ChronosDefaultValue;
        [SerializeField, Enhanced, ReadOnly] private int chronosID = 0;

        /// <summary>
        /// The global chronos value of this handler.
        /// </summary>
        public float HandlerChronos {
            get { return handlerChronos; }
        }

        // -----------------------

        [SerializeField] private EnhancedBehaviour[] behaviours = new EnhancedBehaviour[0];
        [SerializeField] private Animator[] animators           = new Animator[0];

        // -----------------------

        private readonly BufferV<ChronosAlteration> chronosBuffer = new BufferV<ChronosAlteration>();
        #endregion

        #region Enhanced Behaviour
        #if UNITY_EDITOR
        protected override void OnValidate() {
            base.OnValidate();

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

        private DOGetter<float> chronosGetter = null;
        private DOSetter<float> chronosSetter = null;

        // -----------------------

        /// <summary>
        /// Pushes a new chronos alteration into the buffer.
        /// <br/> The chronos alteration is used to modify an object time perception.
        /// </summary>
        /// <param name="_id">The unique id used for this handler alteration.</param>
        /// <param name="_alteration">The ateration to apply on this <see cref="ChronosController"/> associated objects.</param>
        /// <param name="_priority">The priority of this alteration.</param>
        public void PushChronos(int _id, EaseCurveTween<float> _alteration, int _priority) {

            if (chronosGetter == null) {
                chronosGetter = GetChronos;
                chronosSetter = SetChronos;
            }

            ChronosAlteration _chronos = new ChronosAlteration(_id, _alteration, chronosGetter, chronosSetter, () => OnChronosComplete(_id));
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
            var _chronosSpan = chronosBuffer.collection;

            for (int i = _chronosSpan.Count; i-- > 0;) {
                _chronosSpan[i].Second.First.Complete();
            }

            SetChronos(ChronosManager.ChronosDefaultValue);
        }

        // -----------------------

        private float GetChronos() {
            return handlerChronos;
        }

        private void SetChronos(float _chronos) {
            handlerChronos = _chronos;

            for (int i = 0; i < behaviours.Length; i++) {
                behaviours[i].Chronos = _chronos;
            }

            for (int i = 0; i < animators.Length; i++) {
                animators[i].speed = _chronos;
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
                handlerChronos = ChronosManager.ChronosDefaultValue;
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
