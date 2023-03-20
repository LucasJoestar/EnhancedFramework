// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedFramework.Core;
using EnhancedFramework.UI;
using System;
using System.ComponentModel;
using UnityEngine.Playables;

namespace EnhancedFramework.Timeline {
    /// <summary>
    /// Manages the visibility of the <see cref="ScreenFadingGroup"/> for the duration of the clip.
    /// </summary>
    [DisplayName("Screen Effect/Screen Fade")]
    public class ScreenFadeClip : FadingObjectFadePlayableAsset<ScreenFadeBehaviour> {
        #region Utility
        public override string ClipDefaultName {
            get { return "Screen Fade"; }
        }
        #endregion
    }

    /// <summary>
    /// <see cref="ScreenFadeClip"/>-related <see cref="PlayableBehaviour"/>.
    /// </summary>
    [Serializable]
    public class ScreenFadeBehaviour : FadingObjectFadePlayableBehaviour {
        #region Global Members
        public override IFadingObject FadingObject {
            get { return ScreenFadingGroup.Instance; }
        }
        #endregion
    }
}
