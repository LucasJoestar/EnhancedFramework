// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using System;

namespace EnhancedFramework.Core {
    /// <summary>
    /// <see cref="IFadingObject"/>-related fading mode.
    /// </summary>
    public enum FadingMode {
        None = 0,

        Show        = 1,
        Hide        = 2,
        FadeInOut   = 3,
    }

    /// <summary>
    /// Base interface to inherit any fading object from.
    /// </summary>
    public interface IFadingObject {
        #region Content
        /// <summary>
        /// Indicates whether this object is currently visible or not.
        /// </summary>
        bool IsVisible { get; }

        /// <summary>
        /// Duration used to fade in this object.
        /// </summary>
        float ShowDuration { get; }

        /// <summary>
        /// Duration used to fade out this object.
        /// </summary>
        float HideDuration { get; }

        // -------------------------------------------
        // General
        // -------------------------------------------

        /// <summary>
        /// Fades in this object and show it.
        /// </summary>
        /// <param name="_onComplete">Called once this object fade is complete.</param>
        void Show(Action _onComplete = null);

        /// <summary>
        /// Fades out this object and hide it.
        /// </summary>
        /// <param name="_onComplete"><inheritdoc cref="Show(Action)" path="/param[@name='_onComplete']"/></param>
        void Hide(Action _onComplete = null);

        /// <summary>
        /// Fades in this object and show it, then wait for a given time, and fade out.
        /// </summary>
        /// <param name="_duration">The duration to wait before fading out.</param>
        /// <param name="_onAfterFadeIn">Called right after fading int.</param>
        /// <param name="_onBeforeFadeOut">Called right before fading out.</param>
        /// <param name="_onComplete"><inheritdoc cref="Show(Action)" path="/param[@name='_onComplete']"/></param>
        void FadeInOut(float _duration, Action _onAfterFadeIn = null, Action _onBeforeFadeOut = null, Action _onComplete = null);

        /// <summary>
        /// Fades this object according to a given <see cref="FadingMode"/>.
        /// </summary>
        /// <param name="_mode">The <see cref="FadingMode"/> used to fade this object.</param>
        /// <param name="_onComplete"><inheritdoc cref="Show(Action)" path="/param[@name='_onComplete']"/></param>
        /// <param name="_inOutWaitDuration">The duration to wait before fading out if using <see cref="FadingMode.FadeInOut"/>.</param>
        void Fade(FadingMode _mode, Action _onComplete = null, float _inOutWaitDuration = .5f);

        /// <summary>
        /// Inverts this object visibility (show it if hidden, hide if if visible).
        /// </summary>
        /// <param name="_onComplete"><inheritdoc cref="Show(Action)" path="/param[@name='_onComplete']"/></param>
        void Invert(Action _onComplete = null);

        /// <summary>
        /// Sets this object visibility and show/hide it accordingly.
        /// </summary>
        /// <param name="_isVisible">Should this object be visible?</param>
        /// <param name="_onComplete"><inheritdoc cref="Show(Action)" path="/param[@name='_onComplete']"/></param>
        void SetVisibility(bool _isVisible, Action _onComplete = null);

        // -------------------------------------------
        // Instant
        // -------------------------------------------

        /// <param name="_isInstant">If true, instantly fades this object.</param>
        /// <inheritdoc cref="Show(Action)"/>
        void Show(bool _isInstant, Action _onComplete = null);

        /// <param name="_isInstant"><inheritdoc cref="Show(bool, Action)" path="/param[@name='_isInstant']"/></param>
        /// <inheritdoc cref="Hide(Action)"/>
        void Hide(bool _isInstant, Action _onComplete = null);

        /// <param name="_isInstant"><inheritdoc cref="Show(bool, Action)" path="/param[@name='_isInstant']"/></param>
        /// <inheritdoc cref="Fade(FadingMode, Action, float)"/>
        void Fade(FadingMode _mode, bool _isInstant, Action _onComplete = null, float _inOutWaitDuration = .5f);

        /// <param name="_isInstant"><inheritdoc cref="Show(bool, Action)" path="/param[@name='_isInstant']"/></param>
        /// <inheritdoc cref="Invert(Action)"/>
        void Invert(bool _isInstant, Action _onComplete = null);

        /// <param name="_isInstant"><inheritdoc cref="Show(bool, Action)" path="/param[@name='_isInstant']"/></param>
        /// <inheritdoc cref="SetVisibility(bool, Action)"/>
        void SetVisibility(bool _isVisible, bool _isInstant, Action _onComplete = null);

        // -------------------------------------------
        // Utility
        // -------------------------------------------

        /// <summary>
        /// Evaluates this fading object at a specific time.
        /// </summary>
        /// <param name="_time">Time to evaluate this fade object at.</param>
        /// <param name="_show">Whether to show or hide this object.</param>
        void Evaluate(float _time, bool _show);

        /// <summary>
        /// Sets the fade value of this oject.
        /// </summary>
        /// <param name="_value">Normalized fade value (between 0 and 1).</param>
        /// <param name="_show">Whether to show or hide this object.</param>
        void SetFadeValue(float _value, bool _show);
        #endregion
    }

    /// <summary>
    /// Base non-generic class for <see cref="IFadingObject"/>-encapsulated <see cref="EnhancedBehaviour"/>.
    /// </summary>
    public abstract class FadingObjectBehaviour : EnhancedBehaviour, IFadingObject {
        public override UpdateRegistration UpdateRegistration => base.UpdateRegistration | UpdateRegistration.Init;

        #region Global Members
        /// <summary>
        /// <see cref="IFadingObject"/> of this behaviour.
        /// </summary>
        public abstract IFadingObject FadingObject { get; }

        /// <summary>
        /// <see cref="FadingMode"/> applied on this object initialization.
        /// </summary>
        public abstract FadingMode InitMode { get; }

        // -----------------------

        public virtual bool IsVisible {
            get { return FadingObject.IsVisible; }
        }

        public virtual float ShowDuration {
            get { return FadingObject.ShowDuration; }
        }

        public virtual float HideDuration {
            get { return FadingObject.HideDuration; }
        }

        // -------------------------------------------
        // Constructor(s)
        // -------------------------------------------

        /// <summary>
        /// Prevents from inheriting this class in other assemblies.
        /// </summary>
        internal protected FadingObjectBehaviour() { }
        #endregion

        #region Enhanced Behaviour
        protected override void OnInit() {
            base.OnInit();

            FadingObject.Fade(InitMode);
        }
        #endregion

        #region Visiblity
        // -------------------------------------------
        // General
        // -------------------------------------------

        public virtual void Show(Action _onComplete = null) {
            FadingObject.Show(_onComplete);
        }

        public virtual void Hide(Action _onComplete = null) {
            FadingObject.Hide(_onComplete);
        }

        public virtual void FadeInOut(float _duration, Action _onAfterFadeIn = null, Action _onBeforeFadeOut = null, Action _onComplete = null) {
            FadingObject.FadeInOut(_duration, _onAfterFadeIn, _onBeforeFadeOut, _onComplete);
        }

        public virtual void Fade(FadingMode _mode, Action _onComplete = null, float _inOutWaitDuration = .5f) {
            FadingObject.Fade(_mode, _onComplete, _inOutWaitDuration);
        }

        public virtual void Invert(Action _onComplete = null) {
            FadingObject.Invert(_onComplete);
        }

        public virtual void SetVisibility(bool _isVisible, Action _onComplete = null) {
            FadingObject.SetVisibility(_isVisible, _onComplete);
        }

        // -------------------------------------------
        // Instant
        // -------------------------------------------

        public virtual void Show(bool _isInstant, Action _onComplete = null) {
            FadingObject.Show(_isInstant, _onComplete);
        }

        public virtual void Hide(bool _isInstant, Action _onComplete = null) {
            FadingObject.Hide(_isInstant, _onComplete);
        }

        public virtual void Fade(FadingMode _mode, bool _isInstant, Action _onComplete = null, float _inOutWaitDuration = .5f) {
            FadingObject.Fade(_mode, _isInstant, _onComplete, _inOutWaitDuration);
        }

        public virtual void Invert(bool _isInstant, Action _onComplete = null) {
            FadingObject.Invert(_isInstant, _onComplete);
        }

        public virtual void SetVisibility(bool _isVisible, bool _isInstant, Action _onComplete = null) {
            FadingObject.SetVisibility(_isVisible, _isInstant, _onComplete);
        }

        // -------------------------------------------
        // Utility
        // -------------------------------------------

        public virtual void Evaluate(float _time, bool _show) {
            FadingObject.Evaluate(_time, _show);
        }

        public virtual void SetFadeValue(float _value, bool _show) {
            FadingObject.SetFadeValue(_value, _show);
        }

        // -------------------------------------------
        // Inspector
        // -------------------------------------------

        /// <summary>
        /// Shows this object on screen.
        /// </summary>
        [Button(SuperColor.Green)]
        public void ShowGroup() {
            Show();
        }

        /// <summary>
        /// Hides this object from screen.
        /// </summary>
        [Button(SuperColor.Crimson)]
        public void HideGroup() {
            Hide();
        }
        #endregion
    }
}
