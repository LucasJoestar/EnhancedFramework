// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using UnityEngine;
using UnityEngine.Playables;

namespace EnhancedFramework.Timeline {
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
    public abstract class EnhancedPlayableClip<T> : PlayableAsset where T : EnhancedPlayableBehaviour, new() {
		/// <summary>
		/// 
		/// </summary>
		public enum PlayMode {
			Editor,
			Play,
			Always
        }

        #region Global Members
        [SerializeField] public T Template = default;
		[SerializeField] protected PlayMode playMode = PlayMode.Play;
		#endregion

		#region Playable
		public override Playable CreatePlayable(PlayableGraph _graph, GameObject _owner) {
			if ((playMode != PlayMode.Always) && ((playMode == PlayMode.Play) != Application.isPlaying)) {
				return Playable.Null;
            }

			var playable = ScriptPlayable<T>.Create(_graph, Template);
			return playable;
		}
		#endregion
	}

	public class EnhancedPlayableBehaviour : PlayableBehaviour {
        public override void OnBehaviourPlay(Playable playable, FrameData info) {
            base.OnBehaviourPlay(playable, info);

			Debug.Log("Play");
        }

        public override void OnBehaviourPause(Playable playable, FrameData info) {
            base.OnBehaviourPause(playable, info);

			Debug.Log("Pause");
		}
    }
}
