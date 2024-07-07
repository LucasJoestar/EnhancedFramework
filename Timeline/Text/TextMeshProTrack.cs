// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

#if TEXT_MESH_PRO_PACKAGE
using TMPro;
using UnityEngine.Timeline;

using DisplayName = System.ComponentModel.DisplayNameAttribute;

namespace EnhancedFramework.Timeline {
    /// <summary>
    /// <see cref="TrackAsset"/> class for every <see cref="ITextMeshProPlayableAsset"/>.
    /// </summary>
    [TrackColor(.059f, .322f, .729f)] // Sapphire
    [TrackClipType(typeof(ITextMeshProPlayableAsset))]
    [TrackBindingType(typeof(TextMeshProUGUI), TrackBindingFlags.None)]
    [DisplayName("Enhanced Framework/Text Mesh Pro Track")]
    public sealed class TextMeshProTrack : EnhancedTrack { }
}
#endif
