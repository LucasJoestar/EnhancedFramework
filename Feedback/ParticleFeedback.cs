// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace EnhancedFramework.Core {
	/// <summary>
	/// <see cref="EnhancedAssetFeedback"/> used to play various <see cref="ParticleSystem"/>.
	/// </summary>
	[CreateAssetMenu(fileName = FilePrefix + "Particle", menuName = MenuPath + "Particle", order = 100)]
	public class ParticleFeedback : EnhancedAssetFeedback {
		#region Global Members
		[Section("Particle Feedback")]

		[Enhanced, Required] public ParticleSystem Particle = null;
		#endregion

		#region Behaviour

		#endregion
	}
}
