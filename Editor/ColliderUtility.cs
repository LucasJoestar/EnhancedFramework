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
