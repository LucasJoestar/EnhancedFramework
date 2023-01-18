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
    /// <see cref="FsmStateAction"/> used to fade the <see cref="ScreenFadingGroup"/>.
    /// </summary>
    [Tooltip("Fades the game screen.")]
    [ActionCategory("FadingGroup")]
    public class ScreenFade : FadingObjectFade {
        #region Global Members
        public override IFadingObject FadingObject {
            get { return ScreenFadingGroup.Instance; }
        }
        #endregion
    }
}
