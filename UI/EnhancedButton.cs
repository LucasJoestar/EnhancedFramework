// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

#if TEXT_MESH_PRO_PACKAGE
using EnhancedEditor;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace EnhancedFramework.UI {
    /// <summary>
    /// Enhanced <see cref="Button"/> behaviour, being automatically selected on mouse hover.
    /// </summary>
    public class EnhancedButton : Button{
        #region Effects
        /// <summary>
        /// <see cref="TextMeshPro"/>-related additional button effects.
        /// </summary>
        [Serializable]
        public class TextEffect {
            public EnumValues<State, float> Size = new EnumValues<State, float>(1f);
            public EnumValues<State, Vector2> Offset = new EnumValues<State, Vector2>(Vector2.zero);
        }

        /// <summary>
        /// Enum referencing all <see cref="Button"/> possible states.
        /// </summary>
        public enum State {
            Normal,
            Highlighted,
            Pressed,
            Selected,
            Disabled
        }
        #endregion

        #region Global Members
        [Tooltip("Automatically selects this button whenever its GameObject gets enabled")]
        [SerializeField, HideInInspector] public bool AutoSelectOnEnabled = false;
        [SerializeField, HideInInspector] public bool UseTextEffects = false;

        [Space(10f)]

        [Tooltip("Additional TextMeshPro-related effects")]
        [SerializeField, Enhanced, ShowIf("UseTextEffects")] public TextEffect TextEffects = new TextEffect();

        // -----------------------

        [SerializeField, HideInInspector] private Color outlineColor = Color.white;
        #endregion

        #region Mono Behaviour
        protected override void OnEnable() {
            base.OnEnable();
            
            // Selection.
            if (AutoSelectOnEnabled) {
                Select();
            } else {
                OnDeselect(null);
            }
        }

        #if UNITY_EDITOR
        protected override void OnValidate() {
            base.OnValidate();

            // Serialize outline graphic on validate, as the value is not initialized yet on enable.
            if (!Application.isPlaying && (targetGraphic is TextMeshProUGUI _text)) {
                outlineColor = _text.outlineColor;
            }
        }
        #endif
        #endregion

        #region Selectable
        public override void OnPointerEnter(PointerEventData eventData) {
            base.OnPointerEnter(eventData);

            // Select on hover.
            Select();
        }

        public override void OnDeselect(BaseEventData eventData) {
            base.OnDeselect(eventData);

            // Simulate pointer exit.
            OnPointerExit(null);
        }

        protected override void DoStateTransition(SelectionState state, bool instant) {
            base.DoStateTransition(state, instant);

            #if UNITY_EDITOR
            if (!Application.isPlaying) {
                return;
            }
            #endif

            // Extra play animations.
            if (!gameObject.activeInHierarchy || !(targetGraphic is TextMeshProUGUI _text)) {
                return;
            }

            Color _color;
            float _size;
            Vector2 _offset;

            switch (state) {
                case SelectionState.Normal:
                    _color  = colors.normalColor;
                    _size   = TextEffects.Size[State.Normal];
                    _offset = TextEffects.Offset[State.Normal];
                    break;

                case SelectionState.Highlighted:
                    _color  = colors.highlightedColor;
                    _size   = TextEffects.Size[State.Highlighted];
                    _offset = TextEffects.Offset[State.Highlighted];
                    break;

                case SelectionState.Pressed:
                    _color  = colors.pressedColor;
                    _size   = TextEffects.Size[State.Pressed];
                    _offset = TextEffects.Offset[State.Pressed];
                    break;

                case SelectionState.Selected:
                    _color  = colors.selectedColor;
                    _size   = TextEffects.Size[State.Selected];
                    _offset = TextEffects.Offset[State.Selected];
                    break;

                case SelectionState.Disabled:
                    _color  = colors.disabledColor;
                    _size   = TextEffects.Size[State.Disabled];
                    _offset = TextEffects.Offset[State.Disabled];
                    break;

                default:
                    return;
            }

            // Text outline.
            if ((transition == Transition.ColorTint) && (_text.outlineWidth != 0f)) {
                _text.outlineColor = outlineColor * _color;
            }

            // Additional effects.
            if (UseTextEffects) {
                _text.fontSize = _size;
                _text.margin = new Vector4(_offset.x, _offset.y, _text.margin.y, _text.margin.z);
            }
        }
        #endregion
    }
}
#endif
