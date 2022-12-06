// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedFramework.UI;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;

namespace EnhancedFramework.Timeline {
    /// <summary>
    /// Manages the visibility of a <see cref="FadingGroupBehaviour"/> for the duration of the clip.
    /// </summary>
    [DisplayName("Fading Group/Fading Group")]
    public class FadingGroupEnhancedPlayableAsset : EnhancedPlayableAsset<FadingObjectEnhancedPlayableBehaviour> {
        #region Global Members
        [Space(10f)]

        public ExposedReference<FadingGroupBehaviour> FadingGroup = new ExposedReference<FadingGroupBehaviour>();
        #endregion

        #region Behaviour
        public override Playable CreatePlayable(PlayableGraph _graph, GameObject _owner) {
            Template.fadingGroup = FadingGroup.Resolve(_graph.GetResolver());
            return base.CreatePlayable(_graph, _owner);
        }
        #endregion
    }
}
