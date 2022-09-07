// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace EnhancedFramework.Core {
	/// <summary>
	/// <see cref="EnhancedAssetFeedback"/> used to play various <see cref="AudioClip"/>.
	/// </summary>
	[CreateAssetMenu(fileName = FilePrefix + "Audio", menuName = MenuPath + "Audio", order = 100)]
	public class AudioAssetFeedback : EnhancedAssetFeedback {
		#region Global Members
		[Section("Audio Feedback")]

		[Enhanced, Required] public AudioClip Clip = null;
        #endregion

        #region Behaviour

        #endregion
    }
}
