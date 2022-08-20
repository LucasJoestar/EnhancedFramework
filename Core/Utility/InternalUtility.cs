// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("EnhancedFramework.Editor")]
namespace EnhancedFramework.Core {
    /// <summary>
    /// <see cref="EnhancedFramework"/>-related internal utility class.
    /// </summary>
	internal class InternalUtility {
        #region Global Members
        /// <summary>
        /// Menu item path used for <see cref="EnhancedFramework"/> utilities.
        /// </summary>
        public const string MenuItemPath = "Tools/Enhanced Framework/";
        #endregion
    }
}
