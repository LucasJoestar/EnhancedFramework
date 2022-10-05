// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor.Editor;
using EnhancedFramework.GameStates;
using UnityEditor;
using UnityEngine;

namespace EnhancedFramework.Editor {
    /// <summary>
    /// Custom <see cref="GameState"/> drawer.
    /// </summary>
    [CustomPropertyDrawer(typeof(GameState), true)]
    public class GameStatePropertyDrawer : EnhancedPropertyEditor {
        #region Drawer Content
        protected override float OnEnhancedGUI(Rect _position, SerializedProperty _property, GUIContent _label) {
            string _name = EnhancedEditorUtility.GetSerializedPropertyValueTypeName(_property);

            EditorGUI.LabelField(_position, _label, EnhancedEditorGUIUtility.GetLabelGUI(string.IsNullOrEmpty(_name) ? "[None]" : _name));
            return _position.height;
        }
        #endregion
    }
}
