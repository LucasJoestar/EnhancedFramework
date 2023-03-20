// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedFramework.Timeline;
using UnityEngine.Playables;

namespace EnhancedFramework.PlayMaker.Timeline {
    /// <summary>
    /// Base interface to inherit any <see cref="PlayMakerTrack"/> <see cref="PlayableAsset"/> from.
    /// </summary>
    public interface IPlayMakerPlayableAsset { }

    /// <summary>
    /// Base non-generic PlayMaker <see cref="PlayableAsset"/> class.
    /// </summary>
    public abstract class PlayMakerPlayableAsset : EnhancedPlayableAsset, IPlayMakerPlayableAsset { }

    /// <summary>
    /// Base generic class for every PlayMaker <see cref="PlayableAsset"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="EnhancedPlayableBehaviour"/> playable for this asset.</typeparam>
    public abstract class PlayMakerPlayableAsset<T> : EnhancedPlayableAsset<T, PlayMakerFSM>, IPlayMakerPlayableAsset
                                                      where T : EnhancedPlayableBehaviour<PlayMakerFSM>, new () { }
}
