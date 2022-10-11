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
	/// <see cref="EnhancedAssetFeedback"/> used to change the color of an object.
	/// </summary>
	[CreateAssetMenu(fileName = FilePrefix + "Blink", menuName = MenuPath + "Blink", order = 100)]
	public class BlinkFeedback : EnhancedAssetFeedback {
		#region Global Members
		[Section("Blink Feedback")]

		public Color Color = SuperColor.Crimson.Get();
		#endregion

		#region Behaviour

		#endregion
	}
}
#endif
