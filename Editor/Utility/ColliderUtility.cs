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

            Vector3 _scale = _transform.lossyScale;
            _bounds.center -= _transform.position;
            _bounds.extents = _transform.rotation * _bounds.extents;

            if (!_scale.IsNull()) {
                _bounds.center  = _bounds.center.Divide(_scale);
                _bounds.extents = _bounds.extents.Divide(_scale);
            }

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
