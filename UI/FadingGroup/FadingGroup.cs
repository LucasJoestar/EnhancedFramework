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
using UnityEngine.UI;

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
        #region Flags
        /// <summary>
        /// Contains a variety of parameters for this group.
        /// </summary>
        [Flags]
        public enum Parameters {
            None = 0,

            [Tooltip("Enables/disables the associated canvas on visibility toggle")]
            Canvas          = 1 << 0,

            [Tooltip("Enables/disables the group interactability on visibility toggle")]
            Interactable    = 1 << 1,

            [Tooltip("Selects a specific Selectable once visible")]
            Selectable      = 1 << 2,

            [Tooltip("Fading won't be affected by time scale")]
            UnscaledTime    = 1 << 31,
        }
        #endregion

        #region Global Members
        /// <summary>
        /// This object <see cref="UnityEngine.Canvas"/>.
        /// </summary>
        [Enhanced, ShowIf("UseCanvas"), Required] public Canvas Canvas = null;

        /// <summary>
        /// This object <see cref="CanvasGroup"/>.
        /// </summary>
        [Enhanced, Required] public CanvasGroup Group = null;

        /// <summary>
        /// The fading target alpha of this group: first value when fading out, second when fading in.
        /// </summary>
        [Tooltip("The fading target alpha of this group: first value when fading out, second when fading in")]
        [Enhanced, MinMax(0f, 1f)] public Vector2 FadeAlpha = new Vector2(0f, 1f);

        /// <summary>
        /// Parameters of this group.
        /// </summary>
        [Tooltip("Parameters of this group")]
        [Enhanced, DisplayName("Parameters")] public Parameters GroupParameters = Parameters.Interactable | Parameters.UnscaledTime;

        [Tooltip("Object to first select when visible")]
        [Enhanced, ShowIf("UseSelectable"), Required] public Selectable Selectable = null;

        // -----------------------

        public virtual bool IsVisible {
            get { return Group.alpha == FadeAlpha.y; }
        }

        /// <summary>
        /// If true, automatically enable/disable the associated canvas on visibility toggle.
        /// </summary>
        public bool UseCanvas {
            get { return HasParameter(Parameters.Canvas); }
        }

        /// <summary>
        /// If true, automatically enable/disable the associated group interactability on visibility toggle.
        /// </summary>
        public bool IsInteractable {
            get { return HasParameter(Parameters.UnscaledTime); }
        }

        /// <summary>
        /// If true, fading will not be affected by time scale.
        /// </summary>
        public bool UseUnscaledTime {
            get { return HasParameter(Parameters.UnscaledTime); }
        }

        /// <summary>
        /// Get if this group uses a specific selectable or not.
        /// </summary>
        public bool UseSelectable {
            get { return HasParameter(Parameters.Selectable); }
        }
        #endregion

        #region Behaviour
        private const int DefaultGUID = 0;
        protected int guid = DefaultGUID;

        // -------------------------------------------
        // General
        // -------------------------------------------

        public virtual void Show(Action _onComplete = null) {
            CancelCurrentFade();

            Group.alpha = FadeAlpha.y;
            ToggleCanvas();

            _onComplete?.Invoke();
        }

        public virtual void Hide(Action _onComplete = null) {
            CancelCurrentFade();

            Group.alpha = FadeAlpha.x;
            ToggleCanvas();

            _onComplete?.Invoke();
        }

        public virtual void FadeInOut(float _duration, Action _onAfterFadeIn = null, Action _onBeforeFadeOut = null, Action _onComplete = null) {
            Show(OnShow);

            // ----- Local Methods ----- \\

            void OnShow() {
                _onAfterFadeIn?.Invoke();

                guid = EnhancedUtility.GenerateGUID();
                Delayer.Call(guid, _duration, OnWaitComplete, UseUnscaledTime);
            }

            void OnWaitComplete() {
                guid = DefaultGUID;

                _onBeforeFadeOut?.Invoke();
                Hide(_onComplete);
            }
        }

        public virtual void Fade(FadingMode _mode, Action _onComplete = null, float _inOutWaitDuration = .5f) {
            switch (_mode) {
                case FadingMode.Show:
                    Show(_onComplete);
                    break;

                case FadingMode.Hide:
                    Hide(_onComplete);
                    break;

                case FadingMode.FadeInOut:
                    FadeInOut(_inOutWaitDuration, null, null, _onComplete);
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

        protected virtual void CancelCurrentFade() {
            if (guid != DefaultGUID) {
                Delayer.Cancel(guid);
                guid = DefaultGUID;
            }
        }

        protected void ToggleCanvas() {
            EnableCanvas(Group.alpha != FadeAlpha.x);
            SetInteractable(Group.alpha != FadeAlpha.x);
        }

        protected void EnableCanvas(bool _isVisible) {
            if (UseCanvas) {
                Canvas.enabled = _isVisible;
            }

            if (_isVisible && UseSelectable) {
                Selectable.Select();
            }
        }

        protected void SetInteractable(bool _isInteractable) {
            if (IsInteractable) {
                Group.interactable = _isInteractable;
            }
        }

        // -------------------------------------------
        // Instant
        // -------------------------------------------

        public virtual void Show(bool _isInstant, Action _onComplete = null) {
            Show(_onComplete);
        }

        public virtual void Hide(bool _isInstant, Action _onComplete = null) {
            Hide(_onComplete);
        }

        public virtual void Fade(FadingMode _mode, bool _isInstant, Action _onComplete = null, float _inOutWaitDuration = .5f) {
            switch (_mode) {
                case FadingMode.Show:
                    Show(_isInstant, _onComplete);
                    break;

                case FadingMode.Hide:
                    Hide(_isInstant, _onComplete);
                    break;

                case FadingMode.FadeInOut:
                    if (_isInstant) {
                        Show(_isInstant, _onComplete);
                        Hide(_isInstant, _onComplete);
                    } else {
                        FadeInOut(_inOutWaitDuration, null, null, _onComplete);
                    }
                    break;

                case FadingMode.None:
                default:
                    break;
            }
        }

        public virtual void Invert(bool _isInstant, Action _onComplete = null) {
            SetVisibility(!IsVisible, _isInstant, _onComplete);
        }

        public virtual void SetVisibility(bool _isVisible, bool _isInstant, Action _onComplete = null) {
            if (_isVisible) {
                Show(_isInstant, _onComplete);
            } else {
                Hide(_isInstant, _onComplete);
            }
        }
        #endregion

        #region Utility
        /// <summary>
        /// Get if this group has a specific parameter enabled.
        /// </summary>
        /// <param name="_parameter">Parameter to check.</param>
        /// <returns>True if this group has the parameter enabled, false otherwise.</returns>
        public bool HasParameter(Parameters _parameter) {
            return GroupParameters.HasFlag(_parameter);
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
        [SerializeField] private SerializedInterface<IFadingObject> transitionGroup = new SerializedInterface<IFadingObject>();

        [Tooltip("Duration before fading out the transition group when showing this group")]
        [Enhanced, Range(0f, 5f)] public float ShowDuration = .5f;

        [Tooltip("Duration before fading out the transition group when hiding this group")]
        [Enhanced, Range(0f, 5f)] public float HideDuration = .5f;

        [Space(10f)]

        [Tooltip("If true, only the transition group fade in will be affected by the instant fade parameter")]
        public bool InstantOnlyAffectFadeIn = true;

        // -----------------------

        /// <summary>
        /// The <see cref="IFadingObject"/> used for this group transitions.
        /// </summary>
        public IFadingObject TransitionGroup {
            get { return transitionGroup.Interface; }
        }
        #endregion

        #region Behaviour
        public override void Show(Action _onComplete = null) {
            TransitionGroup.FadeInOut(ShowDuration, OnTransitionFaded);

            // ----- Local Method ----- \\

            void OnTransitionFaded() {
                base.Show(_onComplete);
            }
        }

        public override void Hide(Action _onComplete = null) {
            TransitionGroup.FadeInOut(HideDuration, OnTransitionFaded);

            // ----- Local Method ----- \\

            void OnTransitionFaded() {
                base.Hide(_onComplete);
            }
        }

        public override void Show(bool _isInstant, Action _onComplete = null) {
            if (_isInstant && !InstantOnlyAffectFadeIn) {
                base.Show(_onComplete);
            } else {
                TransitionGroup.Show(_isInstant);
                Show(_onComplete);
            }
        }

        public override void Hide(bool _isInstant, Action _onComplete = null) {
            if (_isInstant && !InstantOnlyAffectFadeIn) {
                base.Hide(_onComplete);
            } else {
                TransitionGroup.Show(_isInstant);
                Hide(_onComplete);
            }
        }

        // -----------------------

        /// <summary>
        /// Fades in the transition group, then show this group, and wait for the show duration.
        /// <br/> Does not fade the transition group out.
        /// </summary>
        /// <param name="_onFaded">Called once the transition group has faded.</param>
        /// <param name="_onComplete">Called after waiting for this group show duration.</param>
        public void StartFadeIn(Action _onFaded = null, Action _onComplete = null) {
            CancelCurrentFade();
            TransitionGroup.Show(OnFaded);

            // ----- Local Methods ----- \\

            void OnFaded() {
                base.Show();
                _onFaded?.Invoke();

                guid = EnhancedUtility.GenerateGUID();
                Delayer.Call(guid, ShowDuration, OnWaitComplete, UseUnscaledTime);
            }

            void OnWaitComplete() {
                _onComplete?.Invoke();
            }
        }

        /// <summary>
        /// Fades in the transition group, then hide this group, and wait for the hide duration.
        /// <br/> Does not fade the transition group out.
        /// </summary>
        /// <param name="_onFaded">Called once the transition group has faded.</param>
        /// <param name="_onComplete">Called after waiting for this group hide duration.</param>
        public void StartFadeOut(Action _onFaded = null, Action _onComplete = null) {
            CancelCurrentFade();
            TransitionGroup.Show(OnFaded);

            // ----- Local Methods ----- \\

            void OnFaded() {
                base.Hide();
                _onFaded?.Invoke();

                guid = EnhancedUtility.GenerateGUID();
                Delayer.Call(guid, ShowDuration, OnWaitComplete, UseUnscaledTime);
            }

            void OnWaitComplete() {
                _onComplete?.Invoke();
            }
        }

        /// <summary>
        /// Fades out the transition group.
        /// </summary>
        /// <param name="_onComplete">Called once fading has been completed.</param>
        public void CompleteFade(Action _onComplete = null) {
            TransitionGroup.Hide(_onComplete);
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
        #endregion

        #region Behaviour
        public Tween Tween = null;

        #if UNITY_EDITOR && EDITOR_COROUTINE_ENABLED
        private EditorCoroutine coroutine = null;
        #endif

        // -------------------------------------------
        // General
        // -------------------------------------------

        public override void Show(Action _onComplete = null) {
            CancelCurrentFade();
            Fade(FadeAlpha.y, fadeInDuration, fadeInEase, _onComplete);
        }

        public override void Hide(Action _onComplete = null) {
            CancelCurrentFade();
            Fade(FadeAlpha.x, fadeOutDuration, fadeOutEase, _onComplete);
        }

        public override void Show(bool _isInstant, Action _onComplete = null) {
            if (_isInstant) {
                Fade(FadeAlpha.y, _onComplete);
            } else {
                Show(_onComplete);
            }
        }

        public override void Hide(bool _isInstant, Action _onComplete = null) {
            if (_isInstant) {
                Fade(FadeAlpha.x, _onComplete);
            } else {
                Hide(_onComplete);
            }
        }

        protected override void CancelCurrentFade() {
            base.CancelCurrentFade();

            #if UNITY_EDITOR && EDITOR_COROUTINE_ENABLED
            if (!Application.isPlaying && (coroutine != null)) {
                EditorCoroutineUtility.StopCoroutine(coroutine);
            }
            #endif

            Tween = Tween.DoKill(false);
        }

        // -------------------------------------------
        // Fade
        // -------------------------------------------

        private void Fade(float _alpha, Action _onComplete) {
            CancelCurrentFade();

            Group.alpha = _alpha;
            ToggleCanvas();

            _onComplete?.Invoke();
        }

        private void Fade(float _alpha, float _duration, Ease _ease, Action _onComplete) {
            CancelCurrentFade();
            EnableCanvas(true);
            SetInteractable(true);

            if (Group.alpha == _alpha) {
                OnComplete();
                return;
            }
            
            // Editor fade.
            #if UNITY_EDITOR
            if (!Application.isPlaying) {

                #if EDITOR_COROUTINE_ENABLED
                coroutine = EditorCoroutineUtility.StartCoroutine(DoFade(_alpha, _duration, _ease, _onComplete), this);
                #else
                Group.alpha = _alpha;
                #endif

                return;
            }
            #endif

            Tween = Group.DOFade(_alpha, _duration).SetEase(_ease).SetUpdate(UseUnscaledTime)
                         .SetRecyclable(true).SetAutoKill(true).OnKill(OnComplete);

            // ----- Local Method ----- \\

            void OnComplete() {
                Tween = null;

                ToggleCanvas();
                _onComplete?.Invoke();
            }
        }

        #if UNITY_EDITOR
        private IEnumerator DoFade(float _alpha, float _duration, Ease _ease, Action _onComplete) {
            if (_duration == 0f) {
                Group.alpha = _alpha;
            } else {
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

            ToggleCanvas();
            _onComplete?.Invoke();
        }
        #endif
        #endregion
    }
    #endif
}
