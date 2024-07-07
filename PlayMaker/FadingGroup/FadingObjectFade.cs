// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedFramework.Core;
using HutongGames.PlayMaker;
using System;
using UnityEngine;

using Tooltip = HutongGames.PlayMaker.TooltipAttribute;

namespace EnhancedFramework.PlayMaker {
    /// <summary>
    /// Base abstract <see cref="FsmStateAction"/> used to fade an <see cref="IFadingObject"/>.
    /// </summary>
    public abstract class FadingObjectFade : FsmStateAction {
        #region Global Members
        // -------------------------------------------
        // Mode - Instant - Wait - Event
        // -------------------------------------------

        [Tooltip("Fading Mode used to fade the group.")]
        [RequiredField, ObjectType(typeof(FadingMode))]
        public FsmEnum FadingMode = null;

        [Tooltip("Whether to fade the group instantly or not.")]
        public FsmBool Instant = null;

        [Tooltip("Whether to fade the group instantly or not.")]
        [HideIf(nameof(HideWaitDuration))]
        public FsmFloat WaitDuration = null;

        [Tooltip("Event to send when fade is completed.")]
        public FsmEvent CompletedEvent;

        // -----------------------

        /// <summary>
        /// The <see cref="IFadingObject"/> to fade.
        /// </summary>
        public abstract IFadingObject FadingObject { get; }

        public bool HideWaitDuration() {
            return (((FadingMode)FadingMode.Value) != Core.FadingMode.FadeInOut) || Instant.Value;
        }
        #endregion

        #region Behaviour
        public override void Reset() {
            base.Reset();

            FadingMode      = null;
            Instant         = null;
            WaitDuration    = null;
            CompletedEvent  = null;
        }

        public override void OnEnter() {
            base.OnEnter();

            Fade();
            Finish();
        }

        // -----------------------

        private void Fade() {
            IFadingObject _fadingObject = FadingObject;

            if (_fadingObject != null) {
                PerformFade(_fadingObject, (FadingMode)FadingMode.Value, Instant.Value, OnComplete, WaitDuration.Value);
            }

            // ----- Local Method ----- \\

            void OnComplete() {
                Fsm.Event(CompletedEvent);
            }
        }

        protected virtual void PerformFade(IFadingObject _fadingObject, FadingMode _fadingMode, bool _instant, Action _onComplete, float _waitDuration) {
            _fadingObject.Fade((FadingMode)FadingMode.Value, Instant.Value, _onComplete, WaitDuration.Value);
        }
        #endregion
    }
}
