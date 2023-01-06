// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
//  Based on the InputAction property drawer
//
//      https://github.com/needle-mirror/com.unity.inputsystem/blob/master/InputSystem/Editor/PropertyDrawers/InputActionDrawer.cs
//      https://github.com/needle-mirror/com.unity.inputsystem/blob/master/InputSystem/Editor/PropertyDrawers/InputActionDrawerBase.cs
//
// ================================================================================== //

#if INPUT_SYSTEM_PACKAGE
using EnhancedEditor.Editor;
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Editor;

namespace EnhancedFramework.Editor {
    /// <summary>
    /// Custom <see cref="InputAction"/> drawer, as the default one won't be drawn.
    /// </summary>
    [CustomPropertyDrawer(typeof(InputAction), true)]
    public class InputActionPropertyDrawer : EnhancedPropertyEditor {
        #region Drawer Content
        private static readonly Type inputActionDrawerType      = typeof(InputParameterEditor).Assembly.GetType("UnityEngine.InputSystem.Editor.InputActionDrawer");
        private static readonly MethodInfo getPropertyHeight    = inputActionDrawerType.GetMethod("GetPropertyHeight", BindingFlags.Public | BindingFlags.Instance);
        private static readonly MethodInfo onGUI                = inputActionDrawerType.GetMethod("OnGUI", BindingFlags.Public | BindingFlags.Instance);

        private static readonly object[] getPropertyHeightParameters    = new object[2];
        private static readonly object[] onGUIParameters                = new object[3];

        private object drawer = null;

        // -----------------------

        protected override float OnEnhancedGUI(Rect _position, SerializedProperty _property, GUIContent _label) {
            if (drawer == null) {
                drawer = Activator.CreateInstance(inputActionDrawerType);
            }

            // Get Property Height.
            getPropertyHeightParameters[0] = _property;
            getPropertyHeightParameters[1] = _label;

            try {
                _position.height = (float)getPropertyHeight.Invoke(drawer, getPropertyHeightParameters);

                // On GUI.
                onGUIParameters[0] = _position;
                onGUIParameters[1] = _property;
                onGUIParameters[2] = _label;

                onGUI.Invoke(drawer, onGUIParameters);
            } catch (Exception) { }

            return _position.height;
        }
        #endregion
    }
}
#endif
