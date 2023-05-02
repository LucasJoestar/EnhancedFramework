// ===== Enhanced Editor - https://github.com/LucasJoestar/EnhancedEditor ===== //
// 
// Notes:
//
// ============================================================================ //

using EnhancedEditor;
using EnhancedFramework.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EnhancedFramework.UI {
    /// <summary>
    /// Utility <see cref="Component"/> used to move a UI panel on drag.
    /// </summary>
    [ScriptGizmos(false, true)]
    [AddComponentMenu(FrameworkUtility.MenuPath + "UI/Miscs/Draggable Panel"), DisallowMultipleComponent]
    public class DraggablePanel : EnhancedBehaviour, IPointerDownHandler, IDragHandler {
        public override UpdateRegistration UpdateRegistration => base.UpdateRegistration | UpdateRegistration.Init;

        #region Global Members
        [Section("Resizable Panel")]

        [SerializeField, Enhanced, Required] private RectTransform rectTransform = null;

        [Space(10f)]

        [SerializeField, Enhanced, ReadOnly] private Vector2 initialPosition = Vector2.zero;
        #endregion

        #region Enhanced Behaviour
        protected override void OnInit() {
            base.OnInit();

            initialPosition = GetPosition();
        }

        // -------------------------------------------
        // Editor
        // -------------------------------------------

        #if UNITY_EDITOR
        protected override void OnValidate() {
            base.OnValidate();

            // RectTransform reference initialization.
            if (!rectTransform) {
                rectTransform = transform.parent.GetComponent<RectTransform>();
            }
        }
        #endif
        #endregion

        #region Event
        private Vector2 previousPointerPosition = Vector2.zero;
        private Vector2 currentPointerPosition  = Vector2.zero;

        private Vector2 panelPosition           = Vector2.zero;

        // -----------------------

        void IPointerDownHandler.OnPointerDown(PointerEventData _data) {
            previousPointerPosition = _data.position;
            panelPosition = GetPosition();
        }

        void IDragHandler.OnDrag(PointerEventData _data) {
            currentPointerPosition = _data.position;

            Vector2 _positionValue = currentPointerPosition - previousPointerPosition;
            SetPosition(panelPosition + new Vector2(_positionValue.x, _positionValue.y));
        }
        #endregion

        #region Utility
        /// <summary>
        /// Get the current position of this panel.
        /// </summary>
        /// <returns>This panel current position.</returns>
        public Vector2 GetPosition() {
            return rectTransform.position;
        }

        /// <summary>
        /// Set the position of this panel.
        /// </summary>
        /// <param name="_position">The new position of this panel.</param>
        public void SetPosition(Vector2 _position) {
            rectTransform.position = _position;
        }

        /// <summary>
        /// Resets the position of this panel.
        /// </summary>
        [ContextMenu("Reset Position", false, 10)]
        public void ResetPosition() {
            SetPosition(initialPosition);
        }
        #endregion
    }
}
