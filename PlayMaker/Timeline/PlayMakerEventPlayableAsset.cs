// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedFramework.Timeline;
using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;

using DisplayName = System.ComponentModel.DisplayNameAttribute;

namespace EnhancedFramework.PlayMaker.Timeline {
    /// <summary>
    /// Sends an event in a <see cref="PlayMakerFSM"/> on play.
    /// </summary>
    [DisplayName("PlayMaker/Event")]
    public class PlayMakerEventPlayableAsset : PlayMakerPlayableAsset<PlayMakerEventPlayableBehaviour> {
        #region Global Members
        [Space(10f)]

        public ExposedReference<PlayMakerFSM> FSM = new ExposedReference<PlayMakerFSM>();
        #endregion

        #region Behaviour
        public override Playable CreatePlayable(PlayableGraph _graph, GameObject _owner) {
            Template.fsm = FSM.Resolve(_graph.GetResolver());
            return base.CreatePlayable(_graph, _owner);
        }
        #endregion
    }

    /// <summary>
    /// <see cref="PlayMakerEventPlayableAsset"/>-related <see cref="PlayableBehaviour"/>.
    /// </summary>
    [Serializable]
    public class PlayMakerEventPlayableBehaviour : EnhancedPlayableBehaviour {
        #region Global Members
        [Space(10f), HorizontalLine(SuperColor.Grey, 1f, 50f), Space(10f)]

        public string EventName = "TimelineEvent";

        // -----------------------

        internal PlayMakerFSM fsm = null;
        #endregion

        #region Behaviour
        protected override void OnPlay(Playable _playable, FrameData _info) {
            base.OnPlay(_playable, _info);

            if (fsm != null) {
                fsm.SendEvent(EventName);
            }
        }
        #endregion
    }
}
