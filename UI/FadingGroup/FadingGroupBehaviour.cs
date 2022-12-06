// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedFramework.Core;
using System;
using UnityEngine;
using UnityEngine.UI;

using Range = EnhancedEditor.RangeAttribute;

namespace EnhancedFramework.UI {
    /// <summary>
    /// Base class for <see cref="EnhancedBehaviour"/>-encapsulated <see cref="FadingGroup"/>.
    /// <para/>
    /// Inherited by <see cref="FadingGroupBehaviour"/> and <see cref="TweeningFadingGroupBehaviour"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="FadingGroup"/> class type used by this object.</typeparam>
    public abstract class FadingGroupBehaviour<T> : EnhancedBehaviour, IFadingObject where T : FadingGroup, new() {
        public override UpdateRegistration UpdateRegistration => base.UpdateRegistration | UpdateRegistration.Init;

        #region Global Members
        [Section("Fading Group")]

        [SerializeField, Enhanced, Block] protected T group = default;

        [Space(10f)]

        [SerializeField, Enhanced, Range(0f, 1f)] protected float startAlpha = 1f;

        /// <inheritdoc cref="FadingGroup.Group"/>
        public CanvasGroup Group {
            get { return group.Group; }
        }

        // -----------------------

        public bool IsVisible {
            get { return group.IsVisible; }
        }
        #endregion

        #region Enhanced Behaviour
        protected override void OnInit() {
            base.OnInit();

            group.Group.alpha = startAlpha;
        }

        #if UNITY_EDITOR
        private void OnValidate() {
            // Initialization.
            if ((group != null) && !group.Group) {
                group.Group = GetComponent<CanvasGroup>();

                if (group.Group) {
                    startAlpha = group.Group.alpha;
                }
            }
        }
        #endif
        #endregion

        #region Visiblity
        public virtual void Show(Action _onComplete = null) {
            group.Show(_onComplete);
        }

        public virtual void Hide(Action _onComplete = null) {
            group.Hide(_onComplete);
        }

        public virtual void FadeInOut(float _duration, Action _onAfterFadeIn = null, Action _onBeforeFadeOut = null) {
            group.FadeInOut(_duration, _onAfterFadeIn, _onBeforeFadeOut);
        }

        public virtual void Fade(FadingMode _mode, float _inOutWaitDuration = .5f, Action _onComplete = null) {
            group.Fade(_mode, _inOutWaitDuration, _onComplete);
        }

        public virtual void Invert(Action _onComplete = null) {
            group.Invert(_onComplete);
        }

        public virtual void SetVisibility(bool _isVisible, Action _onComplete = null) {
            group.SetVisibility(_isVisible, _onComplete);
        }

        // -----------------------

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

    /// <summary>
    /// Ready-to-use <see cref="EnhancedBehaviour"/>-encapsulated <see cref="FadingGroup"/>.
    /// <br/> Use this to quickly implement instantly fading <see cref="CanvasGroup"/> objects.
    /// </summary>
    public class FadingGroupBehaviour : FadingGroupBehaviour<FadingGroup> { }
}
