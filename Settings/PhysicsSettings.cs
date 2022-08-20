// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using UnityEngine;

using Min = EnhancedEditor.MinAttribute;
using Range = EnhancedEditor.RangeAttribute;

namespace EnhancedFramework.Settings {
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "STT_PhysicsSettings", menuName = GameSettings.MenuPath + "Physics", order = GameSettings.MenuOrder)]
	public class PhysicsSettings : ScriptableObject {
        /// <summary>
        /// Game used global Physics settings.
        /// </summary>
        public static PhysicsSettings I;

        #region Settings
        [Section("Physics Settings")]

        [Enhanced] public float Gravity = -9.81f;
        [Enhanced, Max(0f)] public float MaxGravity = -25f;

        [Space]

        [Enhanced, Range(.1f, 90f)] public float GroundAngle    = 30f;
        [Enhanced, Min(0f)] public float ClimbHeight      = .2f;
        [Enhanced, Min(0f)] public float SnapHeight       = .2f;

        [Space]

        [Enhanced, Min(0f)] public float SteepSlopeRequiredMovement = 20f;
        [Enhanced, Min(0f)] public float SteepSlopeRequiredForce    = 10f;

        [HorizontalLine(SuperColor.Sapphire)]

        [Enhanced, Range(0f, 1f)] public float OnGroundedForceMultiplier = .55f;

        [Space]

        [Enhanced, Min(0f)] public float GroundDecelerationForce    = 17f;
        [Enhanced, Min(0f)] public float AirDecelerationForce       = 5f;
        #endregion
    }
}
