// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedEditor.Editor;
using EnhancedFramework.Core.GameStates;
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

    /// <summary>
    /// Custom <see cref="Pair{T, U}"/> (<see cref="GameState"/>/<see cref="int"/>) drawer,
    /// used to draw the state stack in the <see cref="GameStateManager"/> inspector.
    /// </summary>
    [CustomPropertyDrawer(typeof(Pair<Reference<GameState>, int>), true)]
    public class PairGameStatePriorityPropertyDrawer : EnhancedPropertyEditor {
        #region Drawer Content
        private static readonly GUIContent gameStateGUI = new GUIContent("Game State");
        private static readonly GUIContent priorityGUI = new GUIContent("Priority");

        // -----------------------

        protected override float OnEnhancedGUI(Rect _position, SerializedProperty _property, GUIContent _label) {
            _property.NextVisible(true);
            SerializedProperty _secondProperty = _property.Copy();
            _secondProperty.NextVisible(false);

            EnhancedEditorGUI.DuoField(_position, _property, gameStateGUI, _secondProperty, priorityGUI, 50f, out float _extraHeight);
            return _position.height + _extraHeight;
        }
        #endregion
    }
}
