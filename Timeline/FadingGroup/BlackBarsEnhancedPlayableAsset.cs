// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

#if DOTWEEN_ENABLED
using EnhancedFramework.Core;
using EnhancedFramework.UI;
using System;
using System.ComponentModel;
using UnityEngine.Playables;

namespace EnhancedFramework.Timeline {
    /// <summary>
    /// Manages the visibility of the <see cref="BlackBarsFadingGroup"/> for the duration of the clip.
    /// </summary>
    [DisplayName("Screen Effect/Black Bars")]
    public class BlackBarsEnhancedPlayableAsset : EnhancedPlayableAsset<BlackBarsEnhancedPlayableBehaviour> { }

    /// <summary>
    /// <see cref="BlackBarsEnhancedPlayableAsset"/>-related <see cref="PlayableBehaviour"/>.
    /// </summary>
    [Serializable]
    public class BlackBarsEnhancedPlayableBehaviour : FadingObjectEnhancedPlayableBehaviour {
        #region Global Members
        protected override IFadingObject FadingObject {
            get { return BlackBarsFadingGroup.Instance; }
        }
        #endregion
    }
}
#endif
