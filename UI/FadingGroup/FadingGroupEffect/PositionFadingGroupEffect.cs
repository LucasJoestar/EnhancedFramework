// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

#if DOTWEEN_ENABLED
using DG.Tweening;
using EnhancedEditor;
using EnhancedFramework.Core;
using UnityEngine;

namespace EnhancedFramework.UI {
    /// <summary>
    /// <see cref="FadingGroupEffect"/> moving the position of a <see cref="RectTransform"/> according to a source <see cref="FadingGroup"/> visibility.
    /// </summary>
    [ScriptGizmos(false, true)]
    [AddComponentMenu(MenuPath + "Position - Fading Group Effect")]
    public sealed class PositionFadingGroupEffect : FadingGroupEffect {
        #region Global Members
        [Section("Position Effect"), PropertyOrder(0)]

        [Tooltip("RectTransform to move on group visibility")]
        [SerializeField, Enhanced, Required] private RectTransform rectTransform = null;

        [Space(10f), HorizontalLine(SuperColor.Grey, 1f), Space(10f), PropertyOrder(2)]

        [Tooltip("If true, animations not be affected by the game time scale")]
        [SerializeField] private bool realTime = false;

        [Space(10f)]

        [SerializeField] private EaseTween<Vector2> showPositionTween   = new EaseTween<Vector2>();
        [SerializeField] private EaseTween<Vector2> showAnchorMinTween  = new EaseTween<Vector2>();
        [SerializeField] private EaseTween<Vector2> showAnchorMaxTween  = new EaseTween<Vector2>();

        [Space(10f)]

        [SerializeField] private EaseTween<Vector2> hidePositionTween   = new EaseTween<Vector2>();
        [SerializeField] private EaseTween<Vector2> hideAnchorMinTween  = new EaseTween<Vector2>();
        [SerializeField] private EaseTween<Vector2> hideAnchorMaxTween  = new EaseTween<Vector2>();
        #endregion

        #region Enhanced Behaviour
        protected override void OnBehaviourDisabled() {
            base.OnBehaviourDisabled();

            // Stop.
            sequence.DoKill();
        }


        #if UNITY_EDITOR
        // -------------------------------------------
        // Editor
        // -------------------------------------------

        protected override void OnValidate() {
            
            // Reference.
            if (!rectTransform) {
                rectTransform = GetComponent<RectTransform>();
            }
        }
#endif
        #endregion

        #region Effect
        private TweenCallback onKilledCallback = null;
        private Sequence sequence = null;

        // -----------------------

        protected internal override void OnSetVisibility(bool _visible, bool _instant) {

            sequence.DoKill();
            sequence = DOTween.Sequence(); {

                if (_visible) {

                    // Show.
                    sequence.Join(showPositionTween.AnchorPosition(rectTransform, false));
                    sequence.Join(showAnchorMinTween.AnchorMin(rectTransform));
                    sequence.Join(showAnchorMaxTween.AnchorMax(rectTransform));

                } else {

                    // Hide.
                    sequence.Join(hidePositionTween.AnchorPosition(rectTransform, false));
                    sequence.Join(hideAnchorMinTween.AnchorMin(rectTransform));
                    sequence.Join(hideAnchorMaxTween.AnchorMax(rectTransform));
                }

                onKilledCallback ??= OnKilled;
                sequence.SetUpdate(realTime).SetRecyclable(true).SetAutoKill(true).OnKill(onKilledCallback);
            }

            if (_instant) {
                sequence.Complete(true);
            }

            // ----- Local Method ----- \\

            void OnKilled() {
                sequence = null;
            }
        }
        #endregion
    }
}
#endif
