// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace EnhancedFramework.Core {
    /// <summary>
    /// Utility <see cref="Component"/> using the attribute <see cref="SelectionBaseAttribute"/>.
    /// </summary>
    [ScriptGizmos(false, true)]
    [AddComponentMenu(FrameworkUtility.MenuPath + "Utility/Selection Base"), SelectionBase, DisallowMultipleComponent]
    public sealed class SelectionBaseBehaviour : EnhancedBehaviour { }
}
