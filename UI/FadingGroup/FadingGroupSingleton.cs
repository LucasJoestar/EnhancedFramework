// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedFramework.Core;
using System;
using UnityEngine;

namespace EnhancedFramework.UI {
    /// <summary>
    /// Inherit from this to quickly implement <see cref="EnhancedSingleton{T}"/>-encapsulated <see cref="FadingGroup"/>.
    /// </summary>
    /// <typeparam name="T"><inheritdoc cref="EnhancedSingleton{T}" path="/typeparam[@name='T']"/></typeparam>
    /// <typeparam name="U"><inheritdoc cref="FadingGroupBehaviour{T}{T}" path="/typeparam[@name='T']"/>.</typeparam>
    public abstract class FadingGroupSingleton<T, U> : EnhancedSingleton<T>, IFadingObject where T : FadingGroupSingleton<T, U> where U : FadingGroup, new() {
        public override UpdateRegistration UpdateRegistration => base.UpdateRegistration | UpdateRegistration.Init;

        #region Global Members
        [Section("Fading Group")]

        [SerializeField, Enhanced, Block] protected U group = default;

        [Space(10f)]

        [SerializeField] protected FadingMode initMode = FadingMode.Hide;

        /// <inheritdoc cref="FadingGroup.Group"/>
        public CanvasGroup Group {
            get { return group.Group; }
        }

        // -----------------------

        public virtual bool IsVisible {
            get { return group.IsVisible; }
        }
        #endregion

        #region Enhanced Behaviour
        protected override void OnInit() {
            base.OnInit();

            group.Fade(initMode);
        }

        #if UNITY_EDITOR
        private void OnValidate() {
            // Initialization.
            if ((group != null) && !group.Group) {
                group.Group = GetComponent<CanvasGroup>();
            }
        }
        #endif
        #endregion

        #region Visiblity
        // -------------------------------------------
        // General
        // -------------------------------------------

        public virtual void Show(Action _onComplete = null) {
            group.Show(_onComplete);
        }

        public virtual void Hide(Action _onComplete = null) {
            group.Hide(_onComplete);
        }

        public virtual void FadeInOut(float _duration, Action _onAfterFadeIn = null, Action _onBeforeFadeOut = null, Action _onComplete = null) {
            group.FadeInOut(_duration, _onAfterFadeIn, _onBeforeFadeOut, _onComplete);
        }

        public virtual void Fade(FadingMode _mode, Action _onComplete = null, float _inOutWaitDuration = .5f) {
            group.Fade(_mode, _onComplete, _inOutWaitDuration);
        }

        public virtual void Invert(Action _onComplete = null) {
            group.Invert(_onComplete);
        }

        public virtual void SetVisibility(bool _isVisible, Action _onComplete = null) {
            group.SetVisibility(_isVisible, _onComplete);
        }

        // -------------------------------------------
        // Instant
        // -------------------------------------------

        public virtual void Show(bool _isInstant, Action _onComplete = null) {
            group.Show(_isInstant, _onComplete);
        }

        public virtual void Hide(bool _isInstant, Action _onComplete = null) {
            group.Hide(_isInstant, _onComplete);
        }

        public virtual void Fade(FadingMode _mode, bool _isInstant, Action _onComplete = null, float _inOutWaitDuration = .5f) {
            group.Fade(_mode, _isInstant, _onComplete, _inOutWaitDuration);
        }

        public virtual void Invert(bool _isInstant, Action _onComplete = null) {
            group.Invert(_isInstant, _onComplete);
        }

        public virtual void SetVisibility(bool _isVisible, bool _isInstant, Action _onComplete = null) {
            group.SetVisibility(_isVisible, _isInstant, _onComplete);
        }

        // -------------------------------------------
        // Inspector
        // -------------------------------------------

        [Button(SuperColor.Green)]
        protected void ShowGroup() {
            Show();
        }

        [Button(SuperColor.Crimson)]
        protected void HideGroup() {
            Hide();
        }
        #endregion
    }
}
