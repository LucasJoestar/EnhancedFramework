// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace EnhancedFramework.Core {
    /// <summary>
    /// <see cref="FootstepPlayer"/>-related <see cref="EnhancedAnimationEvent"/>.
    /// <para/>
    /// Can be used as a parameter on an animation event to trigger various footstep feedbacks.
    /// </summary>
    [CreateAssetMenu(fileName = "ANFT_FootstepEvent", menuName = FrameworkUtility.MenuPath + "Animation Event/Footstep", order = FrameworkUtility.MenuOrder)]
    public sealed class FootstepAnimationEvent : EnhancedAnimationEvent<FootstepPlayer> {
        #region Global Members
        [Section("Footstep Animation Event")]

        [Tooltip("The foot used to play this footstep event\nYou should create and use one for the left, and another one for the right")]
        [SerializeField] private Foot foot = Foot.Right;

        // -----------------------

        /// <summary>
        /// The foot used to play this footstep event.
        /// </summary>
        public Foot Foot {
            get { return foot; }
        }
        #endregion

        #region Event
        protected override void OnInvoke(FootstepPlayer _behaviour) {
            _behaviour.OnAnimationEvent(foot);
        }
        #endregion
    }
}
