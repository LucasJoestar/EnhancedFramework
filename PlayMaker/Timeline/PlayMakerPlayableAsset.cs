// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedFramework.Timeline;
using UnityEngine;
using UnityEngine.Playables;

namespace EnhancedFramework.PlayMaker.Timeline {
    /// <summary>
    /// Base non-generic PlayMaker <see cref="PlayableAsset"/> class.
    /// </summary>
    public abstract class PlayMakerPlayableAsset : PlayableAsset { }

    /// <summary>
    /// Base generic class for every PlayMaker <see cref="PlayableAsset"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="EnhancedPlayableBehaviour"/> playable for this asset.</typeparam>
    public class PlayMakerPlayableAsset<T> : PlayMakerPlayableAsset where T : EnhancedPlayableBehaviour, new() {
        #region Global Members
        /// <summary>
        /// The <see cref="EnhancedPlayableBehaviour"/> template of this asset.
        /// </summary>
        [Enhanced, Block] public T Template = default;
        #endregion

        #region Behaviour
        public override Playable CreatePlayable(PlayableGraph _graph, GameObject _owner) {
            var _playable = ScriptPlayable<T>.Create(_graph, Template);
            return _playable;
        }
        #endregion
    }
}
