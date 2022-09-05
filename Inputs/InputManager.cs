// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

#if INPUT_SYSTEM_PACKAGE
using EnhancedEditor;
using EnhancedFramework.Core;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EnhancedFramework.Input {
    /// <summary>
    /// Input-managing singleton class used for the whole game.
    /// </summary>
    public class InputManager : EnhancedSingleton<InputManager>, IObserver<InputControl> {
        #region Global Members
        [Section("Input Manager")]

        [SerializeField, Enhanced, Required] private InputActionAsset map = null;
        [SerializeField] private InputAsset[] inputs = new InputAsset[] { };

        // -----------------------

        private IDisposable subscriber = null;
        #endregion

        #region Enhanced Behaviour
        private void Start() {
            map.Enable();

            foreach (InputAsset _input in inputs) {
                _input.Initialize(map);
            }

            // Causes errors in the 2021 LTS version. Waiting for a fix from Unity.
            //subscriber = InputSystem.onAnyButtonPress.Subscribe(this);
        }
        #endregion

        #region Input Observer
        void IObserver<InputControl>.OnCompleted() {
            this.LogWarning("Input any key subscriber completed!");
            //Unsubscribe();
        }

        void IObserver<InputControl>.OnError(Exception _exception) {
            this.LogException(_exception);
        }

        void IObserver<InputControl>.OnNext(InputControl value) {
            this.Log($"Input any key triggered: {value}");
        }

        private void Unsubscribe() {
            subscriber.Dispose();
        }
        #endregion

        #region Utility
        private bool isAnyButtonPressed = false;

        // -----------------------

        /// <summary>
        /// Currently not working utility method.
        /// <br/> Waiting for a fix from Unity.
        /// </summary>
        public bool AnyKey() {
            return isAnyButtonPressed;
        }
        #endregion

        #region Editor Tool
        #if UNITY_EDITOR
        [ContextMenu("Get Inputs", false, 10)]
        private void GetInputs() {
            string[] _paths = Array.ConvertAll(AssetDatabase.FindAssets($"t:{typeof(InputAsset).Name}"), AssetDatabase.GUIDToAssetPath);
            inputs = Array.ConvertAll(_paths, AssetDatabase.LoadAssetAtPath<InputAsset>);

            EditorUtility.SetDirty(this);
        }
        #endif
        #endregion
    }
}
#endif
