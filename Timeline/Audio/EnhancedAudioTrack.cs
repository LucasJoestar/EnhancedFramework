// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

using DisplayName = System.ComponentModel.DisplayNameAttribute;

namespace EnhancedFramework.Timeline {
    /// <summary>
    /// <see cref="TrackAsset"/> class for every <see cref="IAudioEnhancedPlayableAsset"/>.
    /// </summary>
    [TrackColor(1f, .459f, .094f)] // Pumpkin - Differentiate this track from the built-in Audio track.
    [TrackClipType(typeof(IAudioEnhancedPlayableAsset))]
    [TrackBindingType(typeof(AudioSource), TrackBindingFlags.AllowCreateComponent)]
    [DisplayName("Enhanced Framework/Audio Track")]
    public class EnhancedAudioTrack : AudioTrack {
        #region Behaviour
        public override Playable CreateTrackMixer(PlayableGraph _graph, GameObject _go, int _inputCount) {

            // Usually, we can assign the bound object of this track to its clips here.
            // Unfortunately, the base AudioTrack we inherit from calls neither this method, nor CreatePlayable.
            // Because of this, the bound object assignation is made in the ClipEditor class.

            return base.CreateTrackMixer(_graph, _go, _inputCount);
        }
        #endregion
    }
}
