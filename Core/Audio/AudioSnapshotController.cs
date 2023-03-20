// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using UnityEngine;

using Range = EnhancedEditor.RangeAttribute;

namespace EnhancedFramework.Core {
    /// <summary>
    /// <see cref="AudioSnapshotAsset"/>-related <see cref="EnhancedBehaviour"/> controller.
    /// </summary>
    [AddComponentMenu(FrameworkUtility.MenuPath + "Audio/Snapshot Controller"), DisallowMultipleComponent]
    public class AudioSnapshotController : AudioWeightControllerBehaviour {
        #region Global Members
        [Section("Audio Snapshot Controller"), PropertyOrder(0)]

        [Tooltip("Snapshot wrapped in this object")]
        [SerializeField, Enhanced, Required] private AudioSnapshotAsset snaphsot = null;

        [Tooltip("Priority of this snapshot. Only the snapshot with the highest priority is active")]
        [SerializeField, Enhanced, ShowIf("overridePriority"), Range(0f, 99f)] private int priority = 0;

        [Tooltip("If true, overrides the default priority of this snaphsot")]
        [SerializeField] private bool overridePriority = false;

        // -----------------------

        /// <summary>
        /// Priority of this snapshot. Only the snapshot with the highest priority is active.
        /// </summary>
        public int Priority {
            get { return overridePriority ? priority : snaphsot.Priority; }
        }
        #endregion

        #region Behaviour
        protected override void OnActivation() {
            base.OnActivation();
        }

        protected override void OnDeactivation() {
            base.OnDeactivation();

            AudioManager.Instance.PopSnapshot(snaphsot, false);
        }

        // -----------------------

        protected override void SetWeight(float _weight) {
            base.SetWeight(_weight);

            AudioManager.Instance.PushSnapshot(snaphsot, weight, Priority, false);
        }
        #endregion
    }
}
