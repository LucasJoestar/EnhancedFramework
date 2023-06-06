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
    /// <see cref="FsmStateAction"/> used to fade the <see cref="BlackBarsFadingGroup"/>.
    /// </summary>
    [Tooltip("Fades the screen Black Bars.")]
    [ActionCategory("FadingGroup")]
    public class BlackBarsFade : FadingObjectFade {
        #region Global Members
        public override IFadingObject FadingObject {
            get { return BlackBarsFadingGroup.Instance; }
        }
        #endregion
    }
}
