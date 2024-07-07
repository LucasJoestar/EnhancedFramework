// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace EnhancedFramework.Core {
    /// <summary>
    /// Utility <see cref="Component"/> used to represent a two-dimensional sprite in a three-dimensional space.
    /// <br/> In short, makes the sprite always look in direction of the main <see cref="Camera"/>.
    /// </summary>
    [ScriptGizmos(false, true)]
    [AddComponentMenu(FrameworkUtility.MenuPath + "Utility/Billboard Sprite"), DisallowMultipleComponent]
    public sealed class BillboardSprite : EnhancedBehaviour, ILateUpdate {
        public override UpdateRegistration UpdateRegistration => base.UpdateRegistration | UpdateRegistration.Late;

        #region Global Members
        [Section("Billboard Sprite")]

        [Tooltip("Axis to lock and not face the main Camera")]
        [SerializeField] private AxisConstraints lockedAxis = AxisConstraints.None;
        #endregion

        #region Enhanced Behaviour
        void ILateUpdate.Update() {

            if (!MainCameraBehaviour.GetMainCamera(out Camera _camera)) {
                return;
            }

            Vector3 _objectForward = transform.forward;
            Vector3 _forward = -_camera.transform.forward;

            // Constraints.
            if (lockedAxis.HasFlagUnsafe(AxisConstraints.X)) {
                _forward.x = _objectForward.x;
            }

            if (lockedAxis.HasFlagUnsafe(AxisConstraints.X)) {
                _forward.y = _objectForward.y;
            }

            if (lockedAxis.HasFlagUnsafe(AxisConstraints.X)) {
                _forward.z = _objectForward.z;
            }

            transform.forward = _forward;
        }
        #endregion
    }
}
