// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedFramework.Core;
using UnityEditor;
using UnityEngine;

namespace EnhancedFramework.Editor {
    /// <summary>
    /// Contains multiple <see cref="Collider"/>-utility menus and methods.
    /// </summary>
    public class ColliderUtility {
        #region Content
        /// <summary>
        /// Context menu separator.
        /// </summary>
        /// <param name="_"></param>
        [MenuItem("CONTEXT/Collider/")]
        private static void Separator(MenuCommand _) { }

        /// <summary>
        /// Setups this collider so that it encapsulates any child <see cref="MeshRenderer"/>.
        /// </summary>
        [MenuItem("CONTEXT/Collider/Setup")]
        public static void Setup(MenuCommand _menu) {
            if (!(_menu.context is Collider _collider)) {
                return;
            }

            Transform _transform = _collider.transform;
            Quaternion _rotation = _transform.rotation;

            _transform.rotation = Quaternion.identity;

            Bounds _bounds = new Bounds(_transform.position, Vector3.zero);

            foreach (MeshRenderer _renderer in _collider.GetComponentsInChildren<MeshRenderer>()) {
                if (_renderer.enabled) {
                    _bounds.Encapsulate(_renderer.bounds);
                }
            }

            foreach (SkinnedMeshRenderer _renderer in _collider.GetComponentsInChildren<SkinnedMeshRenderer>()) {
                if (_renderer.enabled) {
                    _bounds.Encapsulate(_renderer.bounds);
                }
            }

            Vector3 _scale      = _transform.lossyScale;
            Vector3 _center     = _bounds.center - _transform.position;
            Vector3 _extents    = _bounds.extents;

            if (!_scale.IsNull()) {

                _center     = _center.Divide(_scale);
                _extents    = _extents.Divide(_scale);
            }

            _bounds.center  = _center;
            _bounds.extents = new Vector3(Mathf.Abs(_extents.x), Mathf.Abs(_extents.y), Mathf.Abs(_extents.z));

            _transform.rotation = _rotation;

            Undo.RecordObject(_collider, "Setup Collider");

            switch (_collider) {
                case CapsuleCollider _caspule:
                    _caspule.center = _bounds.center;
                    _caspule.radius = _bounds.extents.Max();
                    break;

                case SphereCollider _sphere:
                    _sphere.center = _bounds.center;
                    _sphere.radius = _bounds.extents.Max();
                    break;

                case BoxCollider _box:
                    _box.center = _bounds.center;
                    _box.size = _bounds.size;
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Adjust the position of this collider,
        /// so that its y position is at the ground point.
        /// </summary>
        [MenuItem("CONTEXT/Collider/Adjust Position")]
        public static void AdjustPosition(MenuCommand _menu) {
            if (!(_menu.context is Collider _collider)) {
                return;
            }

            Undo.RecordObject(_collider, "Adjust Collider");

            switch (_collider) {
                case CapsuleCollider _caspule:
                    _caspule.center = _caspule.center.SetY(_caspule.height / 2f);
                    break;

                case SphereCollider _sphere:
                    _sphere.center = _sphere.center.SetY(_sphere.radius);
                    break;

                case BoxCollider _box:
                    _box.center = _box.center.SetY(_box.size.y / 2f);
                    break;

                default:
                    break;
            }
        }
        #endregion
    }
}
