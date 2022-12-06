// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

#if DOTWEEN_ENABLED
using EnhancedFramework.UI;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;

namespace EnhancedFramework.Timeline {
    /// <summary>
    /// Manages the visibility of a <see cref="TweeningFadingGroupEnhancedPlayableAsset"/> for the duration of the clip.
    /// </summary>
    [DisplayName("Fading Group/Tweening Fading Group")]
    public class TweeningFadingGroupEnhancedPlayableAsset : EnhancedPlayableAsset<FadingObjectEnhancedPlayableBehaviour> {
        #region Global Members
        [Space(10f)]

        public ExposedReference<TweeningFadingGroupBehaviour> FadingGroup = new ExposedReference<TweeningFadingGroupBehaviour>();
        #endregion

        #region Behaviour
        public override Playable CreatePlayable(PlayableGraph _graph, GameObject _owner) {
            Template.fadingGroup = FadingGroup.Resolve(_graph.GetResolver());
            return base.CreatePlayable(_graph, _owner);
        }
        #endregion
    }
}
#endif
