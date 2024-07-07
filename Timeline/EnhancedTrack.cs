// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

using Object = UnityEngine.Object;

namespace EnhancedFramework.Timeline {
    /// <summary>
    /// Base <see cref="TrackAsset"/> class for every <see cref="EnhancedPlayableAsset"/>.
    /// </summary>
    public abstract class EnhancedTrack : TrackAsset {
        #region Behaviour
        public override Playable CreateTrackMixer(PlayableGraph _graph, GameObject _go, int _inputCount) {

            if (_go.TryGetComponent(out PlayableDirector _playable)) {

                // Get binding object.
                Object _binding = _playable.GetGenericBinding(this);
                IEnumerable<TimelineClip> _clips = GetClips();

                foreach (TimelineClip _clip in _clips) {
                    if (_clip.asset is IBindingPlayableAsset _asset) {
                        _asset.BindingObject = _binding;
                    }
                }
            }

            return base.CreateTrackMixer(_graph, _go, _inputCount);
        }
        #endregion
    }
}
