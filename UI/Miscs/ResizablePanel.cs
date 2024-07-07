// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedFramework.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EnhancedFramework.UI {
    /// <summary>
    /// Utility <see cref="Component"/> used to resize a UI panel on drag.
    /// </summary>
    [ScriptGizmos(false, true)]
    [AddComponentMenu(FrameworkUtility.MenuPath + "UI/Miscs/Resizable Panel"), DisallowMultipleComponent]
    public sealed class ResizablePanel : EnhancedBehaviour, IPointerDownHandler, IDragHandler {
        public override UpdateRegistration UpdateRegistration => base.UpdateRegistration | UpdateRegistration.Init;

        #region Global Members
        [Section("Resizable Panel")]

        [SerializeField, Enhanced, Required] private RectTransform rectTransform = null;

        [Space(5f)]

        [SerializeField] private Vector2 minSize = Vector3.zero;
        [SerializeField] private Vector2 maxSize = Vector3.zero;

        [Space(10f)]

        [SerializeField, Enhanced, ReadOnly] private Vector2 initialiSize = Vector2.zero;
        #endregion

        #region Enhanced Behaviour
        protected override void OnInit() {
            base.OnInit();

            initialiSize = SetSize(GetSize());
        }

        #if UNITY_EDITOR
        // -------------------------------------------
        // Editor
        // -------------------------------------------

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

        private Vector2 panelSize               = Vector2.zero;

        // -----------------------

        void IPointerDownHandler.OnPointerDown(PointerEventData _data) {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, _data.position, _data.pressEventCamera, out previousPointerPosition);
            panelSize = GetSize();
        }

        void IDragHandler.OnDrag(PointerEventData _data) {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, _data.position, _data.pressEventCamera, out currentPointerPosition);

            Vector2 _resizeValue = currentPointerPosition - previousPointerPosition;
            SetSize(panelSize + new Vector2(_resizeValue.x, -_resizeValue.y));
        }
        #endregion

        #region Utility
        /// <summary>
        /// Get the current size of this panel.
        /// </summary>
        /// <returns>This panel current size.</returns>
        public Vector2 GetSize() {
            return rectTransform.sizeDelta;
        }

        /// <summary>
        /// Set the size of this panel.
        /// </summary>
        /// <param name="_size">The new size of this panel.</param>
        /// <returns>This panel final size.</returns>
        public Vector2 SetSize(Vector2 _size) {
            _size = new Vector2(Mathf.Clamp(_size.x, minSize.x, maxSize.x), Mathf.Clamp(_size.y, minSize.y, maxSize.y));

            rectTransform.sizeDelta = _size;
            return _size;
        }

        /// <summary>
        /// Resets the size of this panel.
        /// </summary>
        [ContextMenu("Reset Size", false, 10)]
        public void ResetSize() {
            SetSize(initialiSize);
        }
        #endregion
    }
}
