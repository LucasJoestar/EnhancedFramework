// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedFramework.Timeline;
using UnityEngine.Timeline;

using DisplayName = System.ComponentModel.DisplayNameAttribute;

namespace EnhancedFramework.PlayMaker.Timeline {
    /// <summary>
    /// <see cref="TrackAsset"/> class for every <see cref="IPlayMakerPlayableAsset"/>.
    /// </summary>
    [TrackColor(.863f, .078f, .235f)] // Crimson
    [TrackClipType(typeof(IPlayMakerPlayableAsset))]
    [TrackBindingType(typeof(PlayMakerFSM), TrackBindingFlags.None)]
    [DisplayName("PlayMaker/PlayMaker Track")]
    public class PlayMakerTrack : EnhancedTrack { }
}
