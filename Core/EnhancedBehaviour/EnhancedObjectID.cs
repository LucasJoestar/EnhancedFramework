// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
//  Based on Unity editor struct GlobalObjectId:
//      https://docs.unity3d.com/ScriptReference/GlobalObjectId.html
//
// ================================================================================== //

using EnhancedEditor;
using System;
using UnityEngine;

using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
using SceneAsset = EnhancedEditor.SceneAsset;
#endif

namespace EnhancedFramework.Core {
    /// <summary>
    /// Enhanced type used as a persistent id for an object.
    /// </summary>
    [Serializable]
    public class EnhancedObjectID {
        /// <summary>
        /// Identifier of an <see cref="EnhancedObjectID"/> object type.
        /// </summary>
        public enum Type {
            // Unity.
            Null            = 0,
            ImportedAsset   = 1,
            SceneObject     = 2,
            SourceAsset     = 3,

            /// <summary>
            /// Dynamically created object.
            /// </summary>
            [Separator(SeparatorPosition.Top)]
            DynamicObject   = 11,

            /// <summary>
            /// Unknown <see cref="Object"/> type.
            /// </summary>
            Unknown         = 99,
        }

        #region Global Members
        [SerializeField] private Type type          = Type.Null;
        [SerializeField] private string assetGUID   = string.Empty;

        [SerializeField] private ulong objectID = 0L;
        [SerializeField] private ulong prefabID = 0L;

        /// <summary>
        /// Get this id associated asset guid.
        /// </summary>
        public string AssetGUID {
            get { return assetGUID; }
        }

        /// <summary>
        /// Indicates if this id is valid.
        /// </summary>
        public bool IsValid {
            get { return (type != Type.Null) && (objectID != 0L) && !string.IsNullOrEmpty(assetGUID); }
        }

        // -----------------------

        /// <summary>
        /// Default null <see cref="EnhancedObjectID"/>.
        /// </summary>
        public static EnhancedObjectID Default {
            get {
                return new EnhancedObjectID() {
                    type = Type.Null,
                    assetGUID = string.Empty,
                    objectID = 0L,
                    prefabID = 0L
                };
            }
        }

        // -------------------------------------------
        // Constructor(s)
        // -------------------------------------------

        /// <param name="_object"><see cref="Object"/> to create an id for.</param>
        /// <inheritdoc cref="EnhancedObjectID()"/>
        public EnhancedObjectID(Object _object) {
            #if UNITY_EDITOR
            // Editor setup.
            if (!Application.isPlaying) {
                GlobalObjectId _globalID = GlobalObjectId.GetGlobalObjectIdSlow(_object);

                type = (Type)_globalID.identifierType;
                assetGUID = _globalID.assetGUID.ToString();

                objectID = _globalID.targetObjectId;
                prefabID = _globalID.targetPrefabId;

                return;
            }
            #endif

            // Runtime setup.
            switch (_object) {
                case ScriptableObject _scriptableObject:
                    type = Type.SourceAsset;
                    break;

                case GameObject _gameObject:
                case Component _component:
                    type = Type.SceneObject;
                    break;

                default:
                    type = Type.Unknown;
                    break;
            }

            assetGUID = string.Empty;
            prefabID = 0L;

            objectID = Guid.NewGuid().ToString().GetLongStableHashCode();
        }

        /// <inheritdoc cref="EnhancedObjectID"/>
        public EnhancedObjectID() {
            type = Type.DynamicObject;

            assetGUID = string.Empty;
            prefabID = 0L;

            objectID = Guid.NewGuid().ToString().GetLongStableHashCode();
        }
        #endregion

        #region Operator
        public static bool operator ==(EnhancedObjectID a, EnhancedObjectID b) {
            if (!ReferenceEquals(a, null)) {
                return a.Equals(b);
            }

            return ReferenceEquals(b, null);
        }

        public static bool operator !=(EnhancedObjectID a, EnhancedObjectID b) {
            return !(a == b);
        }

        public override bool Equals(object _object) {
            if (_object is EnhancedObjectID _id) {
                return Equals(_id);
            }

            return base.Equals(_object);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public override string ToString() {
            return $"{(int)type}-{assetGUID}-{objectID}.{prefabID}";
        }
        #endregion

        #region Utility
        /// <summary>
        /// Intializes this object id for a scene object.
        /// </summary>
        internal void InitSceneObject() {

            switch (type) {

                case Type.Null:
                case Type.ImportedAsset:
                case Type.SourceAsset:
                case Type.Unknown:

                    // Generate new id for instantiated or non initialized objects.
                    type = Type.SceneObject;
                    objectID = Guid.NewGuid().ToString().GetLongStableHashCode();

                    break;

                case Type.SceneObject:
                case Type.DynamicObject:
                default:
                    break;
            }
        }

        /// <summary>
        /// Copies the values of another <see cref="EnhancedObjectID"/>.
        /// </summary>
        /// <param name="_id"><see cref="EnhancedObjectID"/> to copy values.</param>
        public void Copy(EnhancedObjectID _id) {
            type = _id.type;
            assetGUID = _id.assetGUID;
            objectID = _id.objectID;
            prefabID = _id.prefabID;
        }

        /// <summary>
        /// Compares this id with another one.
        /// </summary>
        /// <param name="_id">ID to compare with this one.</param>
        /// <returns>True if both ids are equal, false otherwise.</returns>
        public bool Equals(EnhancedObjectID _id) {
            return !ReferenceEquals(_id, null) &&
                   (type == _id.type) &&
                   (objectID == _id.objectID) &&
                   (prefabID == _id.prefabID) &&
                    assetGUID.Equals(_id.assetGUID, StringComparison.Ordinal);
        }

        /// <summary>
        /// Get the <see cref="SceneAsset"/> associated with this id.
        /// </summary>
        /// <param name="_scene"><see cref="SceneAsset"/> associated with this id.</param>
        /// <returns>True if a scene could be found, false otherwise.</returns>
        public bool GetScene(out SceneAsset _scene) {
            if (string.IsNullOrEmpty(assetGUID)) {
                _scene = null;
                return false;
            }

            _scene = new SceneAsset(assetGUID);
            return _scene.IsValid;
        }

        /// <summary>
        /// Tries to parse a given <see cref="string"/> into an <see cref="EnhancedObjectID"/> instance.
        /// </summary>
        /// <param name="_id"><see cref="string"/> id representation to parse.</param>
        /// <param name="_objectID">Parsed id instance.</param>
        /// <returns>True if the parse operation could be successfully performed, false otherwise.</returns>
        public static bool TryParse(string _id, out EnhancedObjectID _objectID) {
            string[] _components = _id.Split('-');
            if ((_components.Length == 3) && int.TryParse(_components[0], out int _type)) {

                string[] _ids = _components[2].Split('.');
                if ((_ids.Length == 2) && ulong.TryParse(_ids[0], out ulong _objID) && ulong.TryParse(_ids[1], out ulong _prefabID)) {

                    _objectID = new EnhancedObjectID() {
                        type = (Type)_type,
                        assetGUID = _components[1],
                        objectID = _objID,
                        prefabID = _prefabID
                    };

                    return true;
                }
            }

            _objectID = null;
            return false;
        }
        #endregion
    }
}
