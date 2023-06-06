// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

#if DOTWEEN_ENABLED
using DG.Tweening;
using EnhancedEditor;
using EnhancedFramework.Core;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace EnhancedFramework.UI {
    /// <summary>
    /// Blinks an array <see cref="Graphic"/> color.
    /// </summary>
    [ScriptGizmos(false, true)]
    [AddComponentMenu(FrameworkUtility.MenuPath + "UI/Effect/Blink UI Effect")]
    public class BlinkUIEffect : EnhancedSelectableEffect {
        #region Blink
        [Serializable]
        public struct BlinkEffect {
            public Color Color;

            public float Duration;
            public Ease Ease;
        }
        #endregion

        #region Global Members
        [Section("Blink Effect")]

        [Tooltip("All graphics affected by this effect")]
        [SerializeField] private Graphic[] graphics = new Graphic[0];

        [Tooltip("If true, the blink animation will not be affected by the game time scale")]
        [SerializeField] private bool realTime = false;

        [Space(10f)]

        [SerializeField] private BlinkEffect blink = new BlinkEffect();
        [SerializeField] private EnumValues<SelectableState, bool> enableBlink = new EnumValues<SelectableState, bool>();
        #endregion

        #region Behaviour
        private Sequence sequence = null;

        // -----------------------

        public override void OnSelectionState(EnhancedSelectable _selectable, SelectableState _state, bool _instant) {

            // Security.
            if (graphics.Length == 0) {
                return;
            }

            bool _enabled = enableBlink[_state];

            // Stop.
            if (!_enabled && sequence.IsActive()) {

                sequence.DoKill();
                return;
            }

            // Blink.
            if (_enabled && !sequence.IsActive()) {

                foreach (Graphic _graphic in graphics) {
                    sequence.Join(_graphic.DOColor(blink.Color, blink.Duration));
                }

                sequence.SetEase(blink.Ease).SetUpdate(realTime).SetRecyclable(true).SetAutoKill(false).OnKill(OnKilled);
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
