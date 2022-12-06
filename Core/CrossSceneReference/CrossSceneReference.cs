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

using Object = UnityEngine.Object;

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
        public int GUID = CrossSceneReferenceUtility.NullGUID;

        #if UNITY_EDITOR
        /// <summary>
        /// The guid of the scene the referenced object is from.
        /// </summary>
        [SerializeField] internal string sceneGUID = string.Empty;
        [SerializeField] internal bool isSceneLoaded = false;
        #endif

        // -----------------------

        /// <inheritdoc cref="CrossSceneReference{T}"/>
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
            return _reference.GetReference();
        }

        public static implicit operator CrossSceneReference<T>(T _component) {
            return new CrossSceneReference<T>(_component.gameObject.AddComponentIfNone<CrossSceneObject>());
        }
        #endregion

        #region Utility
        /// <summary>
        /// <inheritdoc cref="GetReference(out T)" path="/summary"/>
        /// </summary>
        /// <returns><inheritdoc cref="GetReference(out T)" path="/param[@name='_reference']"/></returns>
        /// <exception cref="MissingCrossCeneReferenceException"></exception>
        public T GetReference() {
            if (GetReference(out T _reference)) {
                return _reference;
            }

            throw new MissingCrossCeneReferenceException(GUID.ToString());
        }

        /// <summary>
        /// Get this object reference <see cref="Component"/> instance.
        /// </summary>
        /// <param name="_reference">This class referencing <see cref="Component"/>.</param>
        /// <returns>True if the associated reference <see cref="Component"/> could be found, false otherwise.</returns>
        public bool GetReference(out T _reference) {
            if ((GUID != CrossSceneReferenceUtility.NullGUID) && GetReference(out CrossSceneObject _ref) && _ref.TryGetComponent(out _reference)) {
                return true;
            }

            _reference = null;
            return false;

            // ----- Local Method ----- \\

            bool GetReference(out CrossSceneObject _reference) {
                #if UNITY_EDITOR
                if (!Application.isPlaying) {
                    CrossSceneObject[] _objects = Object.FindObjectsOfType<CrossSceneObject>();

                    foreach (CrossSceneObject _object in _objects) {
                        if (_object.GUID == GUID) {
                            _reference = _object;
                            return true;
                        }
                    }

                    _reference = null;
                    return false;
                }
                #endif

                return CrossSceneReferenceManager.Instance.GetReference(GUID, out _reference);
            }
        }
        #endregion
    }

    /// <summary>
    /// <see cref="CrossSceneReference{T}"/>-related utility class.
    /// </summary>
    public static class CrossSceneReferenceUtility {
        #region Global Members
        /// <summary>
        /// The guid value used for null reference.
        /// </summary>
        public const int NullGUID = 0;
        #endregion
    }

    /// <summary>
    /// Exception raised when a <see cref="CrossSceneReference{T}"/> could not be retrieved.
    /// </summary>
    public class MissingCrossCeneReferenceException : Exception {
        #region Global Members
        public const string MessageFormat = "Could not retrieve the cross scene reference with GUID \'{0}\'";

        // -----------------------

        /// <inheritdoc cref="MissingCrossCeneReferenceException(string, Exception)"/>
        public MissingCrossCeneReferenceException() : base(string.Format(MessageFormat, "[Uknown]")) { }

        /// <inheritdoc cref="MissingCrossCeneReferenceException(string, Exception)"/>
        public MissingCrossCeneReferenceException(string _guid) : base(string.Format(MessageFormat, _guid)) { }

        /// <param name="_guid">Guid of the missing cross scene object.</param>
        /// <inheritdoc cref="MissingCrossCeneReferenceException"/>
        public MissingCrossCeneReferenceException(string _guid, Exception _innerException) : base(string.Format(MessageFormat, _guid), _innerException) { }
        #endregion
    }
}
