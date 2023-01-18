// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedFramework.Core;
using EnhancedFramework.UI;
using HutongGames.PlayMaker;
using UnityEngine;

using Tooltip = HutongGames.PlayMaker.TooltipAttribute;

namespace EnhancedFramework.PlayMaker {
    /// <summary>
    /// <see cref="FsmStateAction"/> used to fade a <see cref="FadingObjectBehaviour"/>.
    /// </summary>
    [Tooltip("Fades a Fading Group Behaviour.")]
    [ActionCategory("FadingGroup")]
    public class FadingGroupBehaviourFade : FadingObjectFade {
        #region Global Members
        // -------------------------------------------
        // Variable
        // -------------------------------------------

        [Tooltip("The Fading Group Behaviour to fade"), DisplayOrder(0)]
        [RequiredField, ObjectType(typeof(FadingObjectBehaviour))]
        public FsmObject FadingGroup = null;

        // -----------------------

        public override IFadingObject FadingObject {
            get { return FadingGroup.Value as FadingObjectBehaviour; }
        }
        #endregion

        #region Behaviour
        public override void Reset() {
            base.Reset();

            FadingGroup = null;
        }
        #endregion
    }
}
