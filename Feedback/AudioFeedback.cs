// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

#if DOTWEEN_ENABLED
using EnhancedEditor;
using UnityEngine;

namespace EnhancedFramework.Core {
	/// <summary>
	/// <see cref="EnhancedAssetFeedback"/> used to play various <see cref="AudioClip"/>.
	/// </summary>
	[CreateAssetMenu(fileName = FilePrefix + "Audio", menuName = MenuPath + "Audio", order = MenuOrder)]
	public class AudioAssetFeedback : EnhancedAssetFeedback {
		#region Global Members
		[Section("Audio Feedback")]

		[Enhanced, Required] public AudioClip Clip = null;
        #endregion

        #region Behaviour

        #endregion
    }
}
#endif
