// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework-Conversations ===== //
// 
// Notes:
//
// ================================================================================================ //

#if TIMELINE_ENABLED
using EnhancedEditor;
using EnhancedFramework.Timeline;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine.Playables;
using UnityEngine;
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

        public override void GetSubTimelines(TimelineClip _clip, PlayableDirector _director, List<PlayableDirector> _subTimelines) {
            base.GetSubTimelines(_clip, _director, _subTimelines);

            // Use this callback to assign the Audio Source bound object reference to timeline clips.
            TrackAsset _track = _clip.GetParentTrack();
            Object _binding = _director.GetGenericBinding(_track);

            if ((_clip.asset is EnhancedPlayableAsset _asset) && _asset.SerializeBindingInComponent) {

                _director.gameObject.AddComponentIfNone<EnhancedPlayableBindingData>().RegisterData(_asset, _binding);
            }
        }
        #endregion
    }
}
#endif
