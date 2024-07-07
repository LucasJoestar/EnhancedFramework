// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using UnityEngine;

namespace EnhancedFramework.Core {
    /// <summary>
    /// <see cref="AudioAsset"/> related <see cref="AssetMaterialDatabase{T}"/>.
    /// </summary>
    [CreateAssetMenu(fileName = MenuPrefix + "AudioMaterialDatabase", menuName = MenuPath + "Audio", order = MenuOrder)]
    public sealed class AudioMaterialDatabase : AssetMaterialDatabase<AudioAsset> { }
}
