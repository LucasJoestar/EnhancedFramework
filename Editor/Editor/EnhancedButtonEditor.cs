// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

#if TEXT_MESH_PRO_PACKAGE
using EnhancedFramework.UI;
using UnityEditor;
using UnityEditor.UI;

namespace EnhancedFramework.Editor {
    /// <summary>
    /// Custom <see cref="EnhancedButton"/> editor.
    /// </summary>
    [CustomEditor(typeof(EnhancedButton), true), CanEditMultipleObjects]
    public class EnhancedButtonEditor : ButtonEditor {
        #region Editor GUI
        public override void OnInspectorGUI() {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("AutoSelectOnEnabled"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("UseTextEffects"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("TextEffects"));

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space(10f);
            base.OnInspectorGUI();
        }
        #endregion
    }
}
#endif
