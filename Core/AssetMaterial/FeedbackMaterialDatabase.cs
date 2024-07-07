// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using UnityEngine;

namespace EnhancedFramework.Core {
    /// <summary>
    /// <see cref="EnhancedAssetFeedback"/> related <see cref="AssetMaterialDatabase{T}"/>.
    /// </summary>
    [CreateAssetMenu(fileName = MenuPrefix + "FeedbackMaterialDatabase", menuName = MenuPath + "Feedback", order = MenuOrder)]
    public sealed class FeedbackMaterialDatabase : AssetMaterialDatabase<EnhancedAssetFeedback> { }
}
