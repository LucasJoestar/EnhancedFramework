// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

[assembly: InternalsVisibleTo("EnhancedFramework.Editor")]
namespace EnhancedFramework.Input {
    /// <summary>
    /// <see cref="ScriptableObject"/> asset used to reference an <see cref="InputAction"/>.
    /// </summary>
    public class InputMapEnhancedAsset : InputEnhancedAsset {
        #region Global Members
        [Section("Input Map Asset")]

        [SerializeField, Enhanced, ReadOnly] internal InputActionMap map = null;
        [SerializeField] private bool enableOnInit = true;

        // -----------------------

        public override bool IsEnabled {
            get { return map.enabled; }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Editor generator called method.
        /// </summary>
        internal void Initialize(InputActionMap _map) {
            map = _map;
        }

        /// <summary>
        /// <see cref="InputManager"/> runtime called method.
        /// </summary>
        internal void Initialize(InputActionAsset _asset) {
            map = _asset.FindActionMap(map.id);

            if (enableOnInit) {
                Enable();
            }
        }
        #endregion

        #region Utility
        public override void Enable() {
            map.Enable();
        }

        public override void Disable() {
            map.Disable();
        }

        // -----------------------

        /// <summary>
        /// Get if this map contains a specific <see cref="InputActionEnhancedAsset"/>.
        /// </summary>
        /// <param name="_asset">The asset to check.</param>
        /// <returns>True if this map contain the given <see cref="InputActionEnhancedAsset"/>, false otherwise.</returns>
        public bool Contains(InputActionEnhancedAsset _asset) {
            return map.Contains(_asset.input);
        }
        #endregion
    }
}
