// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using System.ComponentModel;
using UnityEngine.Timeline;

namespace EnhancedFramework.PlayMaker.Timeline {
    /// <summary>
    /// <see cref="TrackAsset"/> class for every <see cref="PlayMakerPlayableAsset"/>.
    /// </summary>
    [TrackColor(.863f, .078f, .235f)] // Crimson
    [TrackClipType(typeof(PlayMakerPlayableAsset))]
    [DisplayName("PlayMaker/PlayMaker Track")]
    public class PlayMakerTrack : TrackAsset {  }
}
