// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EnhancedFramework.Core {
    /// <summary>
    /// <see cref="CrossSceneObject"/> reference wrapper.
    /// </summary>
    /// <typeparam name="T">The referencing object <see cref="Component"/> type to load.</typeparam>
    [Serializable]
    public class CrossSceneReference<T> where T : Component {
        #region Global Members
        /// <summary>
        /// The guid of the referencing <see cref="CrossSceneObject"/>.
        /// </summary>
        public int GUID = 0;

        #if UNITY_EDITOR
        /// <summary>
        /// The guid of the scene the referenced object is from.
        /// </summary>
        [SerializeField] internal string sceneGUID = string.Empty;
        [SerializeField] internal bool isSceneLoaded = false;
        #endif

        // -----------------------

        /// <inheritdoc cref="CrossSceneReference{T}(int)"/>
        public CrossSceneReference() { }

        /// <param name="_reference">The referenced object instance (must not be null).</param>
        /// <inheritdoc cref="CrossSceneReference{T}"/>
        public CrossSceneReference(CrossSceneObject _reference) {
            GUID = _reference.GUID;

            #if UNITY_EDITOR
            sceneGUID = AssetDatabase.AssetPathToGUID(_reference.gameObject.scene.path);
            #endif
        }
        #endregion

        #region Operator
        public static implicit operator T(CrossSceneReference<T> _reference) {
            return _reference.GetReference(out T _ref) ? _ref : null;
        }

        public static implicit operator CrossSceneReference<T>(T _component) {
            return new CrossSceneReference<T>(_component.gameObject.AddComponentIfNone<CrossSceneObject>());
        }
        #endregion

        #region Utility
        /// <summary>
        /// Get this object reference <see cref="Component"/> instance.
        /// </summary>
        /// <param name="_reference">This class referencing <see cref="Component"/>.</param>
        /// <returns>True if the associated reference <see cref="Component"/> could be found, false otherwise.</returns>
        public bool GetReference(out T _reference) {
            if (CrossSceneReferenceManager.Instance.GetReference(GUID, out CrossSceneObject _ref) && _ref.TryGetComponent(out _reference)) {
                return true;
            }

            _reference = null;
            return false;
        }
        #endregion
    }
}
