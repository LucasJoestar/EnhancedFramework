// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

#if !zENABLE_INPUT_SYSTEM
#define NEW_INPUT_SYSTEM
#endif

using EnhancedEditor;
using System.Runtime.CompilerServices;
using UnityEngine;

#if NEW_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[assembly: InternalsVisibleTo("EnhancedFramework.Editor")]
namespace EnhancedFramework.Input {
    /// <summary>
    /// <see cref="ScriptableObject"/> asset used to reference an input map.
    /// <br/> Not of any use when using the native input system.
    /// </summary>
    [CreateAssetMenu(fileName = FilePrefix + "InputMap", menuName = MenuPath + "Map", order = MenuOrder)]
    public class InputMapEnhancedAsset : InputEnhancedAsset {
        public const string FilePrefix  = "IPM_";

        #region Global Members
        #if NEW_INPUT_SYSTEM
        [SerializeField, Enhanced, ReadOnly] internal InputActionMap map = null;
        #endif

        // -----------------------

        public override bool IsEnabled {
            get {
                #if NEW_INPUT_SYSTEM
                return map.enabled;
                #else
                return false;
                #endif
            }
        }
        #endregion

        #region Initialization
        #if NEW_INPUT_SYSTEM
        /// <summary>
        /// Editor generator called method.
        /// </summary>
        internal void Initialize(InputActionMap _map) {
            map = _map;
        }
        #endif

        protected override void OnInit(InputDatabase _database) {
            #if NEW_INPUT_SYSTEM
            map = _database.ActionDatabase.FindActionMap(map.id);
            #endif
        }
        #endregion

        #region Utility
        public override void Enable() {
            #if NEW_INPUT_SYSTEM
            map.Enable();
            #endif
        }

        public override void Disable() {
            #if NEW_INPUT_SYSTEM
            map.Disable();
            #endif
        }

        // -----------------------

        /// <summary>
        /// Get if this map contains a specific <see cref="SingleInputActionEnhancedAsset"/>.
        /// </summary>
        /// <param name="_asset">The asset to check.</param>
        /// <returns>True if this map contain the given <see cref="SingleInputActionEnhancedAsset"/>, false otherwise.</returns>
        public bool Contains(SingleInputActionEnhancedAsset _asset) {
            #if NEW_INPUT_SYSTEM
            return map.Contains(_asset.input);
            #else
            return false;
            #endif
        }
        #endregion
    }
}
