// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor.Editor;
using EnhancedFramework.Core.GameStates;
using UnityEditor;
using UnityEngine;

namespace EnhancedFramework.Editor {
    /// <summary>
    /// Custom <see cref="GameStateOverride"/> drawer.
    /// </summary>
    [CustomPropertyDrawer(typeof(GameStateOverride), true)]
    public sealed class GameStateOverridePropertyDrawer : EnhancedPropertyEditor {
        #region Drawer Content
        protected override float OnEnhancedGUI(Rect _position, SerializedProperty _property, GUIContent _label) {
            _position.height = EditorGUI.GetPropertyHeight(_property, true);
            EditorGUI.PropertyField(_position, _property, _label, true);

            return _position.height;
        }
        #endregion
    }
}
