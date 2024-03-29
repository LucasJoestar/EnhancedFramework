// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

#if DOTWEEN_ENABLED
#define TWEENER
#endif

using EnhancedEditor;
using EnhancedFramework.Core;
using System;
using UnityEngine;
using UnityEngine.UI;

#if TWEENER
using DG.Tweening;
#endif

namespace EnhancedFramework.UI {
    /// <summary>
    /// Fades a specific <see cref="Graphic"/> color.
    /// </summary>
    [ScriptGizmos(false, true)]
    [AddComponentMenu(FrameworkUtility.MenuPath + "UI/Effect/Color UI Effect"), DisallowMultipleComponent]
    #pragma warning disable
    public class ColorUIEffect : EnhancedSelectableEffect {
        #region Color
        [Serializable]
        public struct ColorFade {
            public Color Color;

            #if TWEENER
            public float Duration;
            public Ease Ease;
            #endif
        }
        #endregion

        #region Global Members
        [Section("Color Effect")]

        [SerializeField] private Graphic graphic = null;
        [SerializeField] private bool useRealTime = false;

        [Space(5f)]

        [SerializeField] private EnumValues<SelectableState, ColorFade> color = new EnumValues<SelectableState, ColorFade>();
        #endregion

        #region Behaviour
        #if TWEENER
        private Tween tween = null;
        #endif

        // -----------------------

        public override void OnSelectionState(EnhancedSelectable _selectable, SelectableState _state, bool _instant) {
            #if TWEENER
            // Fade.
            tween.DoKill();

            var _color = color[_state];
            tween = graphic.DOColor(_color.Color, _color.Duration).SetEase(_color.Ease)
                           .SetUpdate(useRealTime).SetRecyclable(true).SetAutoKill(true).OnKill(OnKilled);

            // ----- Local Methods ----- \\

            void OnKilled() {
                tween = null;
            }
            #else
            // Instant color.
            graphic.color = color[_state].Color;
            #endif
        }
        #endregion
    }
}
