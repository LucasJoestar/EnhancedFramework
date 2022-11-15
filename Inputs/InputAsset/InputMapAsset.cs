// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

#if INPUT_SYSTEM_PACKAGE
using EnhancedEditor;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

[assembly: InternalsVisibleTo("EnhancedFramework.Editor")]
namespace EnhancedFramework.Input {
    /// <summary>
    /// <see cref="ScriptableObject"/> asset used to reference an <see cref="InputAction"/>.
    /// </summary>
    public class InputMapAsset : ScriptableObject {
        #region Global Members
        [Section("Input Map Asset")]

        [SerializeField, Enhanced, ReadOnly] private InputActionMap map = null;

        /// <summary>
        /// The <see cref="InputActionMap"/> associated with this asset.
        /// </summary>
        public InputActionMap Map {
            get { return map; }
        }

        /// <summary>
        /// Is this input map currently enabled or not?
        /// </summary>
        public bool IsEnabled {
            get { return map.enabled; }
        }
        #endregion

        #region Initialization
        internal void Initialize(InputActionMap _map) {
            map = _map;
        }

        internal void Initialize(InputActionAsset _asset, bool _enable) {
            map = _asset.FindActionMap(map.id);

            if (_enable) {
                Enable();
            }
        }
        #endregion

        #region Input
        /// <summary>
        /// Enables this input map.
        /// </summary>
        public void Enable() {
            map.Enable();
        }

        /// <summary>
        /// Disables this input map.
        /// </summary>
        public void Disable() {
            map.Disable();
        }
        #endregion
    }
}
#endif
