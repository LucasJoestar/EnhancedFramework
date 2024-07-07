// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor.Editor;
using EnhancedFramework.Core;
using UnityEditor;
using UnityEngine;

namespace EnhancedFramework.Editor {
    /// <summary>
    /// Custom <see cref="EnhancedCollection{T}"/> drawer.
    /// </summary>
    [CustomPropertyDrawer(typeof(EnhancedCollection<>), true)]
    public sealed class EnhancedCollectionPropertyDrawer : EnhancedPropertyEditor {
        #region Drawer Content
        protected override float OnEnhancedGUI(Rect _position, SerializedProperty _property, GUIContent _label) {
            _property = _property.FindPropertyRelative("collection");
            _position.height = EditorGUI.GetPropertyHeight(_property, true);

            EditorGUI.PropertyField(_position, _property, _label, true);
            return _position.height;
        }
        #endregion
    }
}
