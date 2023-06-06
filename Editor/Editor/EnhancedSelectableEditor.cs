// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedFramework.UI;
using UnityEditor;
using UnityEditor.UI;

namespace EnhancedFramework.Editor {
    /// <summary>
    /// Custom <see cref="EnhancedSelectable"/> editor.
    /// </summary>
    [CustomEditor(typeof(EnhancedSelectable), true), CanEditMultipleObjects]
    public class EnhancedSelectableEditor : SelectableEditor {
        #region Editor GUI
        public override void OnInspectorGUI() {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("AutoSelectOnEnabled"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Effects"));

            EditorGUILayout.Space(5f);
            base.OnInspectorGUI();

            SerializedProperty _last = serializedObject.FindProperty("group");

            while (_last.NextVisible(false)) {
                EditorGUILayout.PropertyField(_last);
            }

            serializedObject.ApplyModifiedProperties();
        }
        #endregion
    }
}
