// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace EnhancedFramework.Settings {
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "STT_CollisionSettings", menuName = GameSettings.MenuPath + "Collision", order = GameSettings.MenuOrder)]
	public class CollisionSettings : ScriptableObject {
        /// <summary>
        /// Game used global Collision settings.
        /// </summary>
        public static CollisionSettings I;

        #region Settings
        [Section("Collision Settings")]

        public LayerMask PlayerCollisionMask = new LayerMask();
        #endregion
    }
}
