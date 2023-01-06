// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

#if ENABLE_INPUT_SYSTEM
#define NEW_INPUT_SYSTEM
#endif

using EnhancedEditor;
using UnityEngine;

#if NEW_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace EnhancedFramework.Input {
    /// <summary>
    /// Base class for any Enhanced input <see cref="ScriptableObject"/> asset.
    /// </summary>
    [CreateAssetMenu(fileName = FileName, menuName = InputEnhancedAsset.MenuPath + "Database", order = InputEnhancedAsset.MenuOrder)]
    public class InputDatabase : ScriptableObject {
        public const string FileName = "IPD_InputDatabase";

        #region Global Members
        [Section("Input Database")]

        #if NEW_INPUT_SYSTEM
        [SerializeField, Enhanced, Required] private InputActionAsset database = null;

        /// <summary>
        /// The new input system database object.
        /// </summary>
        public InputActionAsset ActionDatabase {
            get { return database; }
            internal set { database = value; }
        }
        #endif
        #endregion
    }
}
