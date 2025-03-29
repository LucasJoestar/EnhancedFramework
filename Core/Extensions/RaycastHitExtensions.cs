// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using System.Collections.Generic;
using UnityEngine;

namespace EnhancedFramework.Core {
    /// <summary>
    /// Multiple extension methods related to the <see cref="RaycastHit"/> struct.
    /// </summary>
    public static class RaycastHitExtensions {
        #region Content
        private static readonly List<Material> materialBuffer = new List<Material>();

        // -----------------------

        /// <summary>
        /// Get the shared <see cref="Material"/> from the <see cref="Mesh"/> hit by this <see cref="RaycastHit"/>.
        /// </summary>
        /// <param name="_hit"><see cref="RaycastHit"/> to get the associated hit shared <see cref="Material"/> from.</param>
        /// <param name="_material">Shared <see cref="Material"/> hit by this raycast.</param>
        /// <returns>True if the hit shared <see cref="Material"/> could be successfully found, false otherwise.</returns>
        public static bool GetSharedMaterial(this RaycastHit _hit, out Material _material) {

            if (GetRendererInfos(_hit, out Renderer _renderer, out int _materialIndex)) {

                List<Material> _buffer = materialBuffer;
                _renderer.GetSharedMaterials(_buffer);

                if (_materialIndex < _buffer.Count) {
                    _material = _buffer[_materialIndex];
                    return true;
                }
            }

            _material = null;
            return false;
        }

        /// <summary>
        /// Get the <see cref="Material"/> from the <see cref="Mesh"/> hit by this <see cref="RaycastHit"/>.
        /// </summary>
        /// <param name="_hit"><see cref="RaycastHit"/> to get the associated hit <see cref="Material"/> from.</param>
        /// <param name="_material"><see cref="Material"/> hit by this raycast.</param>
        /// <returns>True if the hit <see cref="Material"/> could be successfully found, false otherwise.</returns>
        public static bool GetMaterial(this RaycastHit _hit, out Material _material) {

            if (GetRendererInfos(_hit, out Renderer _renderer, out int _materialIndex)) {

                List<Material> _buffer = materialBuffer;
                _renderer.GetMaterials(_buffer);

                if (_buffer.Count < _materialIndex) {
                    _material = _buffer[_materialIndex];
                    return true;
                }
            }

            _material = null;
            return false;
        }

        /// <summary>
        /// Get informations about this <see cref="RaycastHit"/> hit renderer.
        /// </summary>
        /// <param name="_hit"><see cref="RaycastHit"/> to get the associated hit renderer infos.</param>
        /// <param name="_renderer"><see cref="Renderer"/> hit by this raycast (null if none).</param>
        /// <param name="_materialIndex">Index of the hit material index.</param>
        /// <returns>True if this hit renderer infos could be successfully found, false otherwise.</returns>
        public static bool GetRendererInfos(this RaycastHit _hit, out Renderer _renderer, out int _materialIndex) {

            Collider _collider = _hit.collider;

            if (_collider is MeshCollider _meshCollider) {
                if (_meshCollider.convex) {
                    Debug.LogWarning("MeshCollider must be non-convex to get the hit associated triangle index");
                    _materialIndex = 0;
                } else {
                    Mesh _mesh = _meshCollider.sharedMesh;
                    _materialIndex = _mesh.GetSubMeshIndex(_hit.triangleIndex);
                }
            } else {
                _materialIndex = 0;
            }

            if ((_materialIndex != -1) && _collider.TryGetComponent(out _renderer)) {
                return true;
            }

            _renderer = null;
            return false;
        }
        #endregion
    }
}
