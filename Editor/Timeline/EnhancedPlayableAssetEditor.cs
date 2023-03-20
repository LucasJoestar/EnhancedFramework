// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework-Conversations ===== //
// 
// Notes:
//
// ================================================================================================ //

#if TIMELINE_ENABLED
using EnhancedFramework.Timeline;
using UnityEditor.Timeline;
using UnityEngine.Timeline;

namespace EnhancedFramework.Editor {
    /// <summary>
    /// Custom <see cref="EnhancedPlayableAsset"/> editor.
    /// </summary>
    [CustomTimelineEditor(typeof(EnhancedPlayableAsset))]
    public class EnhancedPlayableAssetEditor : ClipEditor {
        #region Utility
        public override void OnCreate(TimelineClip _clip, TrackAsset _track, TimelineClip _clonedFrom) {
            base.OnCreate(_clip, _track, _clonedFrom);

            // TrackAsset callback OnCreateClip in called before assigning the clip default values and references.
            // So use the editor callback instead.

            if (_clip.asset is EnhancedPlayableAsset _asset) {
                _asset.OnCreated(_clip);
            }
        }
        #endregion
    }
}
#endif
