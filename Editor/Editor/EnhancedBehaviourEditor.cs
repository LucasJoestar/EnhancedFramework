// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor.Editor;
using EnhancedFramework.Core;
using UnityEditor;

namespace EnhancedFramework.Editor {
    /// <summary>
    /// Custom <see cref="EnhancedBehaviour"/> editor.
    /// </summary>
    [CustomEditor(typeof(EnhancedBehaviour), true), CanEditMultipleObjects]
    public class EnhancedBehaviourEditor : UnityObjectEditor {
        #region Editor GUI
        protected virtual void OnSceneGUI() {

            // Behaviour handles.
            EnhancedBehaviour _behaviour = target as EnhancedBehaviour;
            _behaviour.OnDrawHandles();
        }
        #endregion
    }
}
