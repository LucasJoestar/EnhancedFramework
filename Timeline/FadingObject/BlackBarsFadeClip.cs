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
    /// Manages the visibility of the <see cref="BlackBarsFadingGroup"/> for the duration of the clip.
    /// </summary>
    [DisplayName("Screen Effect/Black Bars")]
    public class BlackBarsFadeClip : FadingObjectFadePlayableAsset<BlackBarsFadeBehaviour> {
        #region Utility
        public override string ClipDefaultName {
            get { return "Black Bars"; }
        }
        #endregion
    }

    /// <summary>
    /// <see cref="BlackBarsFadeClip"/>-related <see cref="PlayableBehaviour"/>.
    /// </summary>
    [Serializable]
    public class BlackBarsFadeBehaviour : FadingObjectFadePlayableBehaviour {
        #region Global Members
        public override IFadingObject FadingObject {
            get { return BlackBarsFadingGroup.Instance; }
        }
        #endregion
    }
}
