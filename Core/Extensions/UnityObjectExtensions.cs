// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using UnityEngine;

namespace EnhancedFramework.Core {
    /// <summary>
    /// Multiple extension methods related to the <see cref="Object"/> class.
    /// </summary>
	public static class UnityObjectExtensions {
        #region Extensions
        /// <summary>
        /// Indicates if this object is null using the ReferenceEquals method.
        /// <br/> Might not work properly if the object has been destroyed.
        /// </summary>
        /// <param name="_unityObject">Object to check.</param>
		public static bool IsNull(this Object _unityObject) {
            return ReferenceEquals(_unityObject, null);
        }

        /// <summary>
        /// Indicates if this object is not null using the ReferenceEquals method.
        /// <br/> Might not work properly if the object has been destroyed.
        /// </summary>
        /// <param name="_unityObject">Object to check.</param>
        public static bool IsValid(this Object _unityObject) {
            return !_unityObject.IsNull();
        }
        #endregion
    }
}
