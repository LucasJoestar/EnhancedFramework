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

namespace EnhancedFramework.Inputs {
    /// <summary>
    /// Input-managing singleton class used for the whole game.
    /// </summary>
    [ScriptGizmos(false, true)]
    [DefaultExecutionOrder(-970)]
    [AddComponentMenu(FrameworkUtility.MenuPath + "Input/Input Manager"), DisallowMultipleComponent]
    public class InputManager : EnhancedSingleton<InputManager> {
        public override UpdateRegistration UpdateRegistration => base.UpdateRegistration | UpdateRegistration.Init;

        #region Global Members
        [Section("Input Manager")]

        [SerializeField, Enhanced, Required] private InputDatabase database = null;

        [Space(10f)]

        [SerializeField] private InputActionEnhancedAsset[] inputs  = new InputActionEnhancedAsset[] { };
        [SerializeField] private InputMapEnhancedAsset[] maps       = new InputMapEnhancedAsset[] { };

        // -----------------------

        /// <summary>
        /// The total amount of <see cref="InputActionEnhancedAsset"/> in the game.
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

            // Inputs may not be serialized with a consistant reference,
            // so use the database to retrieve their target input within.
            foreach (InputActionEnhancedAsset _input in inputs) {
                _input.Initialize(database);
            }

            foreach (InputMapEnhancedAsset _map in maps) {
                _map.Initialize(database);
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
        public InputActionEnhancedAsset GetInputAt(int _index) {
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
        /// <summary>
        /// Editor utility, retrieving all inputs assets from the project.
        /// </summary>
        [ContextMenu("Get Inputs", false, 10)]
        private void GetInputs() {
            if (AssetDatabase.FindAssets($"t:{typeof(InputDatabase).Name}").SafeFirst(out string _path)) {
                database = AssetDatabase.LoadAssetAtPath<InputDatabase>(AssetDatabase.GUIDToAssetPath(_path));
            }

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
