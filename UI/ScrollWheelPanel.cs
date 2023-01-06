// ===== Enhanced Editor - https://github.com/LucasJoestar/EnhancedEditor ===== //
// 
// Notes:
//
// ============================================================================ //

using EnhancedEditor;
using EnhancedFramework.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using Range = EnhancedEditor.RangeAttribute;

namespace EnhancedFramework.UI {
    /// <summary>
    /// Utility <see cref="Component"/> used to scroll on a panel.
    /// </summary>
    public class ScrollWheelPanel : EnhancedBehaviour, IScrollHandler {
        #region Global Members
        [Section("Scroll Wheel Panel")]

        [SerializeField, Enhanced, Required] private Scrollbar scroll = null;
        [SerializeField, Enhanced, Range(0f, 1f)] private float sensitivity = .1f;
        #endregion

        #region Enhanced Behaviour
        #if UNITY_EDITOR
        private void OnValidate() {
            // Reference initialization.
            if (!scroll) {
                scroll = transform.GetComponentInChildren<Scrollbar>();
            }
        }
        #endif
        #endregion

        #region Event
        void IScrollHandler.OnScroll(PointerEventData _data) {
            scroll.value = Mathf.Clamp(scroll.value + (scroll.size * sensitivity * _data.scrollDelta.y), 0f, 1f);
            scroll.Rebuild(CanvasUpdate.Layout);
        }
        #endregion
    }
}
