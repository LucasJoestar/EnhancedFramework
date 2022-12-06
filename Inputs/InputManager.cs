// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedFramework.Core;
using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityInputActionAsset = UnityEngine.InputSystem.InputActionAsset;

namespace EnhancedFramework.Input {
    /// <summary>
    /// Input-managing singleton class used for the whole game.
    /// </summary>
    public class InputManager : EnhancedSingleton<InputManager> {
        public override UpdateRegistration UpdateRegistration => base.UpdateRegistration | UpdateRegistration.Init;

        #region Global Members
        [Section("Input Manager")]

        [SerializeField, Enhanced, Required] private UnityInputActionAsset inputAsset = null;

        [Space(10f)]

        [SerializeField] private InputActionEnhancedAsset[] inputs = new InputActionEnhancedAsset[] { };
        [SerializeField] private InputMapEnhancedAsset[] maps = new InputMapEnhancedAsset[] { };

        // -----------------------

        /// <summary>
        /// The total amount of <see cref="Input.InputActionEnhancedAsset"/> in the game.
        /// </summary>
        public int InputCount {
            get { return inputs.Length; }
        }

        /// <summary>
        /// The total amount of <see cref="InputMapEnhancedAsset"/> in the game.
        /// </summary>
        public int InputMapCount {
            get { return maps.Length; }
        }
        #endregion

        #region Enhanced Behaviour
        protected override void OnInit() {
            base.OnInit();

            // Inputs are not serialize with a consistant reference.
            // So use the asset to retrieve their target input within.
            foreach (InputActionEnhancedAsset _input in inputs) {
                _input.Initialize(inputAsset);
            }

            foreach (InputMapEnhancedAsset _map in maps) {
                _map.Initialize(inputAsset);
            }
        }
        #endregion

        #region Utility
        /// <summary>
        /// Get the <see cref="InputActionEnhancedAsset"/> at a specific index.
        /// <para/>
        /// Use <see cref="InputCount"/> to get the total amount of input actions.
        /// </summary>
        /// <param name="_index">The index to get the input at.</param>
        /// <returns>The <see cref="InputActionEnhancedAsset"/> at the specified index.</returns>
        public InputActionEnhancedAsset GetInputAAt(int _index) {
            return inputs[_index];
        }

        /// <summary>
        /// Get the <see cref="InputMapEnhancedAsset"/> at a specific index.
        /// <para/>
        /// Use <see cref="InputMapCount"/> to get the total amount of input action maps.
        /// </summary>
        /// <param name="_index">The index to get the input map at.</param>
        /// <returns>The <see cref="InputMapEnhancedAsset"/> at the specified index.</returns>
        public InputMapEnhancedAsset GetInputMapAt(int _index) {
            return maps[_index];
        }
        #endregion

        #region Editor Tool
        #if UNITY_EDITOR
        [ContextMenu("Get Inputs", false, 10)]
        private void GetInputs() {
            string[] _paths = Array.ConvertAll(AssetDatabase.FindAssets($"t:{typeof(InputActionEnhancedAsset).Name}"), AssetDatabase.GUIDToAssetPath);
            inputs = Array.ConvertAll(_paths, AssetDatabase.LoadAssetAtPath<InputActionEnhancedAsset>);

            _paths = Array.ConvertAll(AssetDatabase.FindAssets($"t:{typeof(InputMapEnhancedAsset).Name}"), AssetDatabase.GUIDToAssetPath);
            maps = Array.ConvertAll(_paths, AssetDatabase.LoadAssetAtPath<InputMapEnhancedAsset>);

            EditorUtility.SetDirty(this);
        }
        #endif
        #endregion
    }
}
