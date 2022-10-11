// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedFramework.Core;
using System;
using UnityEngine;

#if DOTWEEN_ENABLED
using DG.Tweening;
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
        [Enhanced, Range(0f, 1f)] public Vector2 FadeAlpha = new Vector2(0f, 1f);

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

            Tween.DoKill(false);
            Tween = Group.DOFade(_alpha, _duration).SetEase(_ease).SetUpdate(UseUnscaledTime);

            if (_onComplete != null) {
                Tween.onComplete = new TweenCallback(_onComplete);
            }
        }
        #endregion
    }
    #endif
}
