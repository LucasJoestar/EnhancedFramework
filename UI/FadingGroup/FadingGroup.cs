// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

#if DOTWEEN_ENABLED
#define TWEENING
#endif

using EnhancedEditor;
using EnhancedFramework.Core;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

#if TWEENING
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
        #region Controller Event
        /// <summary>
        /// Lists all different <see cref="FadingGroupController"/> events to call.
        /// </summary>
        public enum ControllerEvent {
            ShowStarted     = 1 << 0,
            ShowPerformed   = 1 << 1,
            ShowCompleted   = 1 << 2,

            HideStarted     = 1 << 3,
            HidePerformed   = 1 << 4,
            HideCompleted   = 1 << 5,

            CompleteShow = ShowPerformed    | ShowCompleted,
            CompleteHide = HidePerformed   | HideCompleted,

            FullShow = ShowStarted | ShowPerformed  | ShowCompleted,
            FullHide = HideStarted | HidePerformed | CompleteHide,
        }
        #endregion

        #region State
        /// <summary>
        /// State used for a group event.
        /// </summary>
        public enum StateEvent {
            None        = 0,

            Show        = 1,
            Hide        = 2,

            ShowInstant = 3,
            HideInstant = 4,
        }
        #endregion

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

            [Tooltip("Sends informations to a Fading Group Controller when updating state")]
            Controller      = 1 << 30,

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
        /// This object <see cref="FadingGroupController"/>.
        /// </summary>
        [Enhanced, ShowIf("UseController"), Required] public FadingGroupController Controller = null;

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

        [Space(10f)]

        [Tooltip("If true, disables all Selectable components in children when this group is not interactable")]
        [Enhanced, ShowIf("IsInteractable")] public bool EnableSelectable = false;

        [Tooltip("Object to first select when visible")]
        [Enhanced, ShowIf("UseSelectable"), Required] public Selectable Selectable = null;
        [Enhanced, ShowIf("UseSelectable"), ReadOnly] public Selectable ActiveSelectable = null;

        // -----------------------

        public virtual bool IsVisible {
            get { return Group.alpha == FadeAlpha.y; }
        }

        public virtual float ShowDuration {
            get { return 0f; }
        }

        public virtual float HideDuration {
            get { return 0f; }
        }

        /// <summary>
        /// If true, use the default implementation to call controller events.
        /// </summary>
        public virtual bool UseDefaultControllerEvent {
            get { return true; }
        }

        /// <summary>
        /// If true, automatically enable/disable the associated canvas on visibility toggle.
        /// </summary>
        public bool UseCanvas {
            get { return HasParameter(Parameters.Canvas); }
        }

        /// <summary>
        /// If true, automatically sends events to a controller when updating this group state.
        /// </summary>
        public bool UseController {
            get { return HasParameter(Parameters.Controller); }
        }

        /// <summary>
        /// If true, automatically enable/disable the associated group interactability on visibility toggle.
        /// </summary>
        public bool IsInteractable {
            get { return HasParameter(Parameters.Interactable); }
        }

        /// <summary>
        /// If true, fading will not be affected by time scale.
        /// </summary>
        public bool RealTime {
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
        private static readonly List<Selectable> selectableBuffer = new List<Selectable>();
        protected DelayHandler delay = default;

        // -------------------------------------------
        // General
        // -------------------------------------------

        public virtual void Show(Action _onComplete = null) {
            CancelCurrentFade();
            Group.alpha = FadeAlpha.y;

            ToggleCanvas(true);
            CallControllerEvent(ControllerEvent.FullShow, true);

            OnSetVisibility(true, false);
            _onComplete?.Invoke();
        }

        public virtual void Hide(Action _onComplete = null) {
            CancelCurrentFade();
            Group.alpha = FadeAlpha.x;

            ToggleCanvas(false);
            CallControllerEvent(ControllerEvent.FullHide, true);

            OnSetVisibility(false, false);
            _onComplete?.Invoke();
        }

        public virtual void FadeInOut(float _duration, Action _onAfterFadeIn = null, Action _onBeforeFadeOut = null, Action _onComplete = null) {
            Show(OnShow);

            // ----- Local Methods ----- \\

            void OnShow() {
                _onAfterFadeIn?.Invoke();
                delay = Delayer.Call(_duration, OnWaitComplete, RealTime);
            }

            void OnWaitComplete() {
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

        // -------------------------------------------
        // Utility
        // -------------------------------------------

        public virtual void Evaluate(float _time, bool _show) {
            float _value = (_time == 0f) ? 0f : 1f;
            SetFadeValue(_value, _show);
        }

        public virtual void SetFadeValue(float _value, bool _show) {
            if (Group.alpha == _value) {
                return;
            }

            if (_value == FadeAlpha.x) {
                Hide();
            } else if (_value == FadeAlpha.y) {
                Show();
            } else {

                // Set alpha.
                CancelCurrentFade();
                Group.alpha = _value;

                ToggleCanvas(true);
            }
        }

        protected virtual void CancelCurrentFade() {
            delay.Cancel();
        }

        protected void ToggleCanvas(bool _select = false) {
            EnableCanvas(Group.alpha != FadeAlpha.x, _select);
            SetInteractable(Group.alpha != FadeAlpha.x);
        }

        public void EnableCanvas(bool _isVisible, bool _select) {
            if (UseCanvas) {
                Canvas.enabled = _isVisible;
            }

            if (UseSelectable) {

                if (_isVisible && _select) {

                    // Selection.
                    ActiveSelectable.Select();

                } else if (!_isVisible) {

                    // Reset to default.
                    ActiveSelectable = Selectable;
                }
            }
        }

        public void SetInteractable(bool _isInteractable) {

            if (IsInteractable && (Group.interactable != _isInteractable)) {
                Group.interactable = _isInteractable;

                // Disable all children selectable.
                if (EnableSelectable) {

                    Group.GetComponentsInChildren(selectableBuffer);

                    foreach (Selectable _selectable in selectableBuffer) {
                        _selectable.enabled = _isInteractable;
                    }
                }
            }
        }

        public void OnDisabled() {
            CallControllerEvent(ControllerEvent.FullHide, false);
        }

        protected virtual void CallControllerEvent(ControllerEvent _event, bool _isDefault = false) {
            if (!UseController || (_isDefault && !UseDefaultControllerEvent)) {
                return;
            }

            #if UNITY_EDITOR
            if (!Application.isPlaying && !Controller.ExecuteInEditMode) {
                return;
            }
            #endif

            // Call event.
            if (HasEvent(ControllerEvent.ShowStarted)) {
                Controller.OnShowStarted(this);
            }

            if (HasEvent(ControllerEvent.ShowPerformed)) {
                Controller.OnShowPerformed(this);
            }

            if (HasEvent(ControllerEvent.ShowCompleted)) {
                Controller.OnShowCompleted(this);
            }

            if (HasEvent(ControllerEvent.HideStarted)) {
                Controller.OnHideStarted(this);
            }

            if (HasEvent(ControllerEvent.HidePerformed)) {
                Controller.OnHidePerformed(this);
            }

            if (HasEvent(ControllerEvent.HideCompleted)) {
                Controller.OnHideCompleted(this);
            }

            // ----- Local Method ----- \\

            bool HasEvent(ControllerEvent _eventType) {
                return _event.HasFlag(_eventType);
            }
        }

        protected void OnSetVisibility(bool _visible, bool _instant) {

            // Effects.
            for (int i = 0; i < effects.Count; i++) {

                FadingGroupEffect _effect = effects[i];
                _effect.OnSetVisibility(_visible, _instant);
            }
        }
        #endregion

        #region Effect
        private readonly List<FadingGroupEffect> effects = new List<FadingGroupEffect>();

        // -----------------------

        /// <summary>
        /// Registers a specific <see cref="FadingGroupEffect"/> for this group.
        /// </summary>
        /// <param name="_effect"><see cref="FadingGroupEffect"/> to register.</param>
        public void RegisterEffect(FadingGroupEffect _effect) {
            effects.Add(_effect);
        }

        /// <summary>
        /// Unregisters a specific <see cref="FadingGroupEffect"/> from this group.
        /// </summary>
        /// <param name="_effect"><see cref="FadingGroupEffect"/> to unregister.</param>
        public void UnregisterEffect(FadingGroupEffect _effect) {
            effects.Remove(_effect);
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
    public abstract class FadingObjectTransitionFadingGroup : FadingGroup {
        #region Global Members
        [Space(10f)]

        [Tooltip("Duration before fading out the transition group when showing this group")]
        [Enhanced, Range(0f, 5f)] public float ShowDelay = .5f;

        [Tooltip("Duration before fading out the transition group when hiding this group")]
        [Enhanced, Range(0f, 5f)] public float HideDelay = .5f;

        [Space(10f)]

        [Tooltip("If true, only the transition group fade in will be affected by the instant fade parameter")]
        public bool InstantOnlyAffectFadeIn = false;

        // -----------------------

        public override bool IsVisible {
            get { return base.IsVisible && !TransitionGroup.IsVisible; }
        }

        public override float ShowDuration {
            get { return TransitionGroup.ShowDuration + ShowDelay + TransitionGroup.HideDuration; }
        }

        public override float HideDuration {
            get { return TransitionGroup.ShowDuration + HideDelay + TransitionGroup.HideDuration; }
        }

        public override bool UseDefaultControllerEvent {
            get { return false; }
        }

        /// <summary>
        /// The <see cref="IFadingObject"/> used for this group transitions.
        /// </summary>
        public abstract IFadingObject TransitionGroup { get; }
        #endregion

        #region Behaviour
        public override void Show(Action _onComplete = null) {

            if (IsVisible) {
                OnComplete();
                return;
            }

            CallControllerEvent(ControllerEvent.ShowStarted);
            TransitionGroup.FadeInOut(ShowDelay, OnTransitionFaded, null, OnComplete);

            // ----- Local Method ----- \\

            void OnTransitionFaded() {
                base.Show(OnShowed);
            }

            void OnShowed() {
                CallControllerEvent(ControllerEvent.ShowPerformed);
            }

            void OnComplete() {
                CallControllerEvent(ControllerEvent.ShowCompleted);
                _onComplete?.Invoke();
            }
        }

        public override void Hide(Action _onComplete = null) {

            if (!IsVisible) {
                OnComplete();
                return;
            }

            CallControllerEvent(ControllerEvent.HideStarted);
            TransitionGroup.FadeInOut(HideDelay, OnTransitionFaded, null, OnComplete);

            // ----- Local Method ----- \\

            void OnTransitionFaded() {
                base.Hide(OnHided);
            }

            void OnHided() {
                CallControllerEvent(ControllerEvent.HidePerformed);
            }

            void OnComplete() {
                CallControllerEvent(ControllerEvent.HideCompleted);
                _onComplete?.Invoke();
            }
        }

        public override void Show(bool _isInstant, Action _onComplete = null) {
            if (_isInstant && !InstantOnlyAffectFadeIn) {

                base.Show(_onComplete);
                CallControllerEvent(ControllerEvent.FullShow);

            } else {
                TransitionGroup.Show(_isInstant);
                Show(_onComplete);
            }
        }

        public override void Hide(bool _isInstant, Action _onComplete = null) {
            if (_isInstant && !InstantOnlyAffectFadeIn) {

                base.Hide(_onComplete);
                CallControllerEvent(ControllerEvent.FullHide);

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
            CallControllerEvent(ControllerEvent.ShowStarted);
            CancelCurrentFade();

            TransitionGroup.Show(OnFaded);

            // ----- Local Methods ----- \\

            void OnFaded() {
                base.Show();
                CallControllerEvent(ControllerEvent.ShowPerformed);

                _onFaded?.Invoke();

                delay = Delayer.Call(ShowDelay, OnWaitComplete, RealTime);
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
            CallControllerEvent(ControllerEvent.HideStarted);
            CancelCurrentFade();

            TransitionGroup.Show(OnFaded);

            // ----- Local Methods ----- \\

            void OnFaded() {
                base.Hide();
                CallControllerEvent(ControllerEvent.HidePerformed);

                _onFaded?.Invoke();

                delay = Delayer.Call(ShowDelay, OnWaitComplete, RealTime);
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
            TransitionGroup.Hide(OnComplete);

            // ----- Local Method ----- \\

            void OnComplete() {

                CallControllerEvent(IsVisible ? ControllerEvent.ShowCompleted : ControllerEvent.HideCompleted);
                _onComplete?.Invoke();
            }
        }
        #endregion
    }

    /// <summary>
    /// <see cref="FadingObjectTransitionFadingGroup"/> that can used any <see cref="IFadingObject"/> as a transition.
    /// </summary>
    [Serializable]
    public class TransitionFadingGroup : FadingObjectTransitionFadingGroup {
        #region Global Members
        [Tooltip("The FadingGroup to use as a transition: performs a fade in, set this group visibility, then fades out")]
        [SerializeField] private SerializedInterface<IFadingObject> transitionGroup = new SerializedInterface<IFadingObject>();

        // -----------------------

        public override IFadingObject TransitionGroup {
            get { return transitionGroup.Interface;  }
        }
        #endregion
    }

    /// <summary>
    /// Class wrapper for a fading <see cref="CanvasGroup"/> instance, using tweening.
    /// </summary>
    [Serializable]
    public class TweeningFadingGroup : FadingGroup {
        #region Global Members
        [Space(10f)]

        [SerializeField, Enhanced, Range(0f, 10f)] private float fadeInDuration = .5f;
        #if TWEENING
        [SerializeField] private Ease fadeInEase = Ease.OutSine;

        [Space(5f)]
        #endif

        [SerializeField, Enhanced, Range(0f, 10f)] private float fadeOutDuration = .5f;
        #if TWEENING
        [SerializeField] private Ease fadeOutEase = Ease.InSine;
        #endif

        // -----------------------

        public override bool UseDefaultControllerEvent {
            get { return false; }
        }

        public override float ShowDuration {
            get { return fadeInDuration; }
        }

        public override float HideDuration {
            get { return fadeOutDuration; }
        }
        #endregion

        #region Behaviour
        private TweenHandler tween = default;
        private float targetAlpha = 0f;

        // -------------------------------------------
        // General
        // -------------------------------------------

        public override void Show(Action _onComplete = null) {
            #if TWEENING
            Fade(FadeAlpha.y, fadeInDuration, fadeInEase, _onComplete);
            #else
            Fade(FadeAlpha.y, fadeInDuration, _onComplete);
            #endif

            OnSetVisibility(true, false);
        }

        public override void Hide(Action _onComplete = null) {
            #if TWEENING
            Fade(FadeAlpha.x, fadeOutDuration, fadeOutEase, _onComplete);
            #else
            Fade(FadeAlpha.x, fadeOutDuration, _onComplete);
            #endif

            OnSetVisibility(false, false);
        }

        public override void Show(bool _isInstant, Action _onComplete = null) {
            if (_isInstant) {
                Fade(FadeAlpha.y, _onComplete);
                OnSetVisibility(true, true);
            } else {
                Show(_onComplete);
            }
        }

        public override void Hide(bool _isInstant, Action _onComplete = null) {
            if (_isInstant) {
                Fade(FadeAlpha.x, _onComplete);
                OnSetVisibility(false, true);
            } else {
                Hide(_onComplete);
            }
        }

        // -------------------------------------------
        // Utility
        // -------------------------------------------

        public override void Evaluate(float _time, bool _show) {
            float _value;

            if (_show) {
                _value = (fadeInDuration != 0f) ? (_time / fadeInDuration) : 0f;
            } else {
                _value = (fadeOutDuration != 0f) ? (_time / fadeOutDuration) : 0f;
            }

            SetFadeValue(Mathf.Clamp(_value, 0f, 1f), _show);
        }

        public override void SetFadeValue(float _value, bool _show) {
            CancelCurrentFade();
            float _alpha;

            #if TWEENING
            if (_show) {
                _alpha = DOVirtual.EasedValue(0f, 1f, _value, fadeInEase);
            } else {
                _alpha = DOVirtual.EasedValue(1f, 0f, _value, fadeOutEase);
            }
            #else
            _alpha = _value;
            #endif

            if (Group.alpha == _alpha) {
                return;
            }

            bool _visible = IsVisible;

            Group.alpha = _alpha;
            ToggleCanvas((_value != FadeAlpha.x) && !_visible);
        }

        protected override void CancelCurrentFade() {
            base.CancelCurrentFade();

            // Stop without complete.
            tween.Stop();
        }

        // -------------------------------------------
        // Tween
        // -------------------------------------------

        #if TWEENING
        /// <param name="_duration">Fade duration (in seconds).</param>
        /// <param name="_ease">Fade ease.</param>
        /// <inheritdoc cref="Show(Action)"/>
        public void Show(float _duration, Ease _ease, Action _onComplete = null) {
            Fade(FadeAlpha.y, _duration, _ease, _onComplete);
            OnSetVisibility(true, false);
        }

        /// <param name="_duration">Fade duration (in seconds).</param>
        /// <param name="_ease">Fade ease.</param>
        /// <inheritdoc cref="Hide(Action)"/>
        public void Hide(float _duration, Ease _ease, Action _onComplete = null) {
            Fade(FadeAlpha.x, _duration, _ease, _onComplete);
            OnSetVisibility(false, false);
        }
        #endif

        // -------------------------------------------
        // Fade
        // -------------------------------------------

        private void Fade(float _alpha, Action _onComplete) {
            CancelCurrentFade();
            Group.alpha = _alpha;

            ToggleCanvas(_alpha == FadeAlpha.y);
            CallControllerEvent(IsVisible ? ControllerEvent.FullShow : ControllerEvent.FullHide);

            _onComplete?.Invoke();
        }

        private void Fade(float _alpha, float _duration, Action _onComplete) {
            DoFade(_alpha, CreateTween, _onComplete);

            // ----- Local Method ----- \\

            TweenHandler CreateTween(Action<float> _setter, Action<bool> _onStopped) {
                return Core.Tweener.Tween(Group.alpha, _alpha, _setter, _duration, RealTime, _onStopped);
            }
        }

        #if TWEENING
        private void Fade(float _alpha, float _duration, Ease _ease, Action _onComplete) {
            DoFade(_alpha, CreateTween, _onComplete);

            // ----- Local Method ----- \\

            TweenHandler CreateTween(Action<float> _setter, Action<bool> _onStopped) {
                return Core.Tweener.Tween(Group.alpha, _alpha, _setter, _duration, _ease, RealTime, _onStopped);
            }
        }
        #endif

        // -----------------------

        private void DoFade(float _alpha, Func<Action<float>, Action<bool>, TweenHandler> _tweener, Action _onComplete) {

            // Register callback if targetting to same alpha.
            if ((targetAlpha == _alpha) && tween.GetHandle(out EnhancedTween _tween)) {

                _tween.OnStopped += (b) => _onComplete?.Invoke();
                return;
            }

            CancelCurrentFade();

            EnableCanvas(true, _alpha != FadeAlpha.x);
            SetInteractable(true);

            if (Group.alpha == _alpha) {
                OnStopped();
                return;
            }

            CallControllerEvent((_alpha == FadeAlpha.y) ? ControllerEvent.ShowStarted : ControllerEvent.HideStarted);

            // Tween.
            targetAlpha = _alpha;
            tween = _tweener(Set, OnStopped);

            // ----- Local Method ----- \\

            void Set(float _value) {
                Group.alpha = _value;
            }

            void OnStopped(bool _completed = false) {

                ToggleCanvas(false);
                CallControllerEvent(IsVisible ? ControllerEvent.CompleteShow : ControllerEvent.CompleteHide);

                _onComplete?.Invoke();
            }
        }
        #endregion
    }
}
