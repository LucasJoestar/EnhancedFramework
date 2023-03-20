// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

#if ENABLE_INPUT_SYSTEM
#define NEW_INPUT_SYSTEM
#endif

using EnhancedEditor;
using EnhancedFramework.Core;
using UnityEngine;

#if NEW_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace EnhancedFramework.Inputs {
    /// <summary>
    /// Base class for any Enhanced input <see cref="ScriptableObject"/> asset.
    /// </summary>
    [CreateAssetMenu(fileName = FileName, menuName = InputEnhancedAsset.MenuPath + "Database", order = InputEnhancedAsset.MenuOrder)]
    public class InputDatabase : EnhancedScriptableObject {
        public const string FileName = "IPD_InputDatabase";

        #region Global Members
        #if NEW_INPUT_SYSTEM
        [Section("Input Database")]
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
