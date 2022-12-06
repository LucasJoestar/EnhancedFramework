// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using System;
using UnityEngine;
using UnityEngine.Playables;

namespace EnhancedFramework.Timeline {
    /// <summary>
    /// Base Enhanced <see cref="PlayableBehaviour"/> class.
    /// </summary>
    [Serializable]
    public class EnhancedPlayableBehaviour : PlayableBehaviour {
        #region Global Members
        [Section("Enhanced Playable Behaviour")]

        [Tooltip("Indicates whether this behaviour should be executed in edit mode or not")]
        public bool ExecuteInEditMode = true;
		#endregion

		#region Behaviour
		private bool isPlaying = false;

		// -----------------------

		public override sealed void OnBehaviourPlay(Playable _playable, FrameData _info) {
			base.OnBehaviourPlay(_playable, _info);

			#if UNITY_EDITOR
			if (!ExecuteInEditMode && !Application.isPlaying) {
				return;
			}
			#endif

			// Do not play when already playing (required security).
			if (isPlaying) {
				return;
			}

			isPlaying = true;
			OnPlay(_playable, _info);
		}

		public override sealed void OnBehaviourPause(Playable _playable, FrameData _info) {
			base.OnBehaviourPause(_playable, _info);

			#if UNITY_EDITOR
			if (!ExecuteInEditMode && !Application.isPlaying) {
				return;
			}
			#endif

			// Don't pause while the behaviour is active.
			if (!isPlaying) {
				return;
			}

			isPlaying = false;
			OnPause(_playable, _info);
		}

		// -----------------------

		protected virtual void OnPlay(Playable _playable, FrameData _info) { }

		protected virtual void OnPause(Playable _playable, FrameData _info) { }
		#endregion
	}
}
