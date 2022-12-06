// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedFramework.Core;
using System;
using System.Collections;
using UnityEngine;

#if DOTWEEN_ENABLED
using DG.Tweening;
#endif

#if UNITY_EDITOR
using UnityEditor;

#if EDITOR_COROUTINE_ENABLED
using Unity.EditorCoroutines.Editor;
#endif
#endif

using Range = EnhancedEditor.RangeAttribute;

namespace EnhancedFramework.UI {
    /// <summary>
    /// Class wrapper for a fading <see cref="CanvasGroup"/> instance.
    /// <para/> Fading is performed instantly. For a tweening behaviour, please use <see cref="TweeningFadingGroup"/>.
    /// </summary>
    [Serializable]
    public class FadingGroup : IFadingObject {
        #region Global Members
        /// <summary>
        /// This object <see cref="CanvasGroup"/>.
        /// </summary>
        [Enhanced, Required] public CanvasGroup Group = null;

        /// <summary>
        /// The fading target alpha of this group: first value when fading out, second when fading in.
        /// </summary>
        [Tooltip("The fading target alpha of this group: first value when fading out, second when fading in")]
        [Enhanced, MinMax(0f, 1f)] public Vector2 FadeAlpha = new Vector2(0f, 1f);

        // -----------------------

        public bool IsVisible {
            get { return Group.alpha == FadeAlpha.y; }
        }
        #endregion

        #region Behaviour
        public virtual void Show(Action _onComplete = null) {
            Group.alpha = FadeAlpha.y;

            _onComplete?.Invoke();
        }

        public virtual void Hide(Action _onComplete = null) {
            Group.alpha = FadeAlpha.x;

            _onComplete?.Invoke();
        }

        public virtual void FadeInOut(float _duration, Action _onAfterFadeIn = null, Action _onBeforeFadeOut = null) {
            Show(OnShow);

            // ----- Local Methods ----- \\

            void OnShow() {
                _onAfterFadeIn?.Invoke();
                WaitForFadeInOut(_duration, _onBeforeFadeOut);
            }
        }

        public virtual void Fade(FadingMode _mode, float _inOutWaitDuration = .5f, Action _onComplete = null) {
            switch (_mode) {
                case FadingMode.Show:
                    Show(_onComplete);
                    break;

                case FadingMode.Hide:
                    Hide(_onComplete);
                    break;

                case FadingMode.FadeInOut:
                    FadeInOut(_inOutWaitDuration, _onComplete, null);
                    break;

                case FadingMode.None:
                default:
                    break;
            }
        }

        public virtual void Invert(Action _onComplete = null) {
            SetVisibility(!IsVisible, _onComplete);
        }

        public virtual void SetVisibility(bool _isVisible, Action _onComplete = null) {
            if (_isVisible) {
                Show(_onComplete);
            } else {
                Hide(_onComplete);
            }
        }

        // -----------------------

        protected virtual void WaitForFadeInOut(float _duration, Action _onBeforeFadeOut) {
            GameManager.Instance.StartCoroutine(DoFadeInOutWait(_duration, _onBeforeFadeOut));
        }

        protected void OnFadeInOutWaitCompleted(Action _onBeforeFadeOut) {
            _onBeforeFadeOut?.Invoke();
            Hide();
        }

        private IEnumerator DoFadeInOutWait(float _duration, Action _onBeforeFadeOut) {
            yield return new WaitForSecondsRealtime(_duration);
            OnFadeInOutWaitCompleted(_onBeforeFadeOut);
        }
        #endregion
    }

    /// <summary>
    /// Special <see cref="FadingGroup"/>, using an intermediary <see cref="IFadingObject"/> as a transition.
    /// <br/> Makes the transition group perform a fade in, set this group visibility, then fades out the transition group.
    /// </summary>
    [Serializable]
    public class TransitionFadingGroup : FadingGroup {
        #region Global Members
        [Space(10f)]

        [Tooltip("The FadingGroup to use as a transition: performs a fade in, set this group visibility, then fades out")]
        public SerializedInterface<IFadingObject> TransitionGroup = new SerializedInterface<IFadingObject>();

        [Tooltip("Duration before fading out the transition group when showing this group")]
        [Enhanced, Range(0f, 5f)] public float ShowDuration = .5f;

        [Tooltip("Duration before fading out the transition group when hiding this group")]
        [Enhanced, Range(0f, 5f)] public float HideDuration = .5f;
        #endregion

        #region Behaviour
        public override void Show(Action _onComplete = null) {
            TransitionGroup.Interface.FadeInOut(ShowDuration, OnTransitionFaded);

            // ----- Local Method ----- \\

            void OnTransitionFaded() {
                base.Show(_onComplete);
            }
        }

        public override void Hide(Action _onComplete = null) {
            TransitionGroup.Interface.FadeInOut(HideDuration, OnTransitionFaded);

            // ----- Local Method ----- \\

            void OnTransitionFaded() {
                base.Hide(_onComplete);
            }
        }
        #endregion
    }

    #if DOTWEEN_ENABLED
    /// <summary>
    /// Class wrapper for a fading <see cref="CanvasGroup"/> instance, using tweening.
    /// </summary>
    [Serializable]
    public class TweeningFadingGroup : FadingGroup {
        #region Global Members
        [Space(10f)]

        [Enhanced, Range(0f, 10f)] public float fadeInDuration = .5f;
        public Ease fadeInEase = Ease.OutSine;

        [Space(5f)]

        [Enhanced, Range(0f, 10f)] public float fadeOutDuration = .5f;
        public Ease fadeOutEase = Ease.InSine;

        [Space(10f)]

        public bool UseUnscaledTime = false;
        #endregion

        #region Behaviour
        public Tween Tween = null;

        // -----------------------

        public override void Show(Action _onComplete = null) {
            Fade(FadeAlpha.y, fadeInDuration, fadeInEase, _onComplete);
        }

        public override void Hide(Action _onComplete = null) {
            Fade(FadeAlpha.x, fadeOutDuration, fadeOutEase, _onComplete);
        }

        // -----------------------

        private void Fade(float _alpha, float _duration, Ease _ease, Action _onComplete) {
            if (Group.alpha == _alpha) {
                return;
            }
            
            // Editor fade.
            #if UNITY_EDITOR
            if (!Application.isPlaying) {

                #if EDITOR_COROUTINE_ENABLED
                EditorCoroutineUtility.StartCoroutine(DoFade(_alpha, _duration, _ease, _onComplete), this);
                #else
                Group.alpha = _alpha;
                #endif

                return;
            }
            #endif

            Tween.DoKill(false);
            Tween = Group.DOFade(_alpha, _duration).SetEase(_ease).SetUpdate(UseUnscaledTime);

            if (_onComplete != null) {
                Tween.onComplete = new TweenCallback(_onComplete);
            }
        }

        protected override void WaitForFadeInOut(float _duration, Action _onBeforeFadeOut) {
            Tween = DOVirtual.DelayedCall(_duration, () => OnFadeInOutWaitCompleted(_onBeforeFadeOut), UseUnscaledTime);
        }

        #if UNITY_EDITOR
        private IEnumerator DoFade(float _alpha, float _duration, Ease _ease, Action _onComplete) {
            if (_duration != 0f) {
                double _timer = EditorApplication.timeSinceStartup;
                float _from = Group.alpha;

                while (true) {
                    float _time = (float)(EditorApplication.timeSinceStartup - _timer);

                    // End loop.
                    if (_time > _duration) {
                        Group.alpha = _alpha;
                        break;
                    }

                    Group.alpha = DOVirtual.EasedValue(_from, _alpha, _time / _duration, _ease);
                    yield return null;
                }
            }

            _onComplete?.Invoke();
        }
        #endif
        #endregion
    }
    #endif
}
