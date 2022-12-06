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
    /// Manages the visibility of the <see cref="ScreenFadingGroup"/> for the duration of the clip.
    /// </summary>
    [DisplayName("Screen Effect/Screen Fading")]
    public class ScreenFadingEnhancedPlayableAsset : EnhancedPlayableAsset<ScreenFadingEnhancedPlayableBehaviour> { }

    /// <summary>
    /// <see cref="ScreenFadingEnhancedPlayableAsset"/>-related <see cref="PlayableBehaviour"/>.
    /// </summary>
    [Serializable]
    public class ScreenFadingEnhancedPlayableBehaviour : FadingObjectEnhancedPlayableBehaviour {
        #region Global Members
        protected override IFadingObject FadingObject {
            get { return ScreenFadingGroup.Instance; }
        }
        #endregion
    }
}
#endif
