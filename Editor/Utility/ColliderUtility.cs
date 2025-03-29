// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

#if UNITY_2020_3 || UNITY_2021_3 || UNITY_2022_2_OR_NEWER
#define FIND_OBJECT_BY_TYPE
#endif

using EnhancedEditor;
using EnhancedEditor.Editor;
using EnhancedFramework.Core;
using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

using Object = UnityEngine.Object;

namespace EnhancedFramework.Editor {
    /// <summary>
    /// Contains multiple <see cref="Collider"/>-utility menus and methods.
    /// </summary>
    public static class ColliderUtility {
        #region Content
        private const string ReplaceUndoName = "Replace ProBuilder Collider Script";
        private static readonly Type colliderBehaviourType = Type.GetType("UnityEngine.ProBuilder.ColliderBehaviour, Unity.ProBuilder");

        // -----------------------

        /// <summary>
        /// Context menu separator.
        /// </summary>
        [MenuItem("CONTEXT/Collider/")]
        private static void Separator(MenuCommand _) { }

        /// <summary>
        /// Setups this collider so that it encapsulates any child <see cref="MeshRenderer"/>.
        /// </summary>
        [MenuItem("CONTEXT/Collider/Setup")]
        public static void Setup(MenuCommand _menu) {
            if (_menu.context is not Collider _collider) {
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
                _center  = _center.Divide(_scale);
                _extents = _extents.Divide(_scale);
            }

            _bounds.center  = _center;
            _bounds.extents = _extents.Abs();

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
                    _box.size   = _bounds.size;
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
            if (_menu.context is not Collider _collider) {
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

        // -------------------------------------------
        // Toolbar
        // -------------------------------------------

        /// <summary>
        /// Shows all <see cref="ColliderVisibilityBehaviour"/> in the loaded scene(s).
        /// </summary>
        [MenuItem(FrameworkUtility.MenuItemPath + "Collider/Show %#&1", false, FrameworkUtility.MenuOrder)]
        public static void ShowCollider() {
            GetComponents<ColliderVisibilityBehaviour>(Enable);

            // ----- Local Method ----- \\

            static void Enable(ColliderVisibilityBehaviour _collider) {

                _collider.Enable();
                EditorUtility.SetDirty(_collider.gameObject);
            }
        }

        /// <summary>
        /// Gides all <see cref="ColliderVisibilityBehaviour"/> in the loaded scene(s).
        /// </summary>
        [MenuItem(FrameworkUtility.MenuItemPath + "Collider/Hide %#&2", false, FrameworkUtility.MenuOrder)]
        public static void HideCollider() {
            GetComponents<ColliderVisibilityBehaviour>(Disable);

            // ----- Local Method ----- \\

            static void Disable(ColliderVisibilityBehaviour _collider) {

                _collider.Disable();
                EditorUtility.SetDirty(_collider.gameObject);
            }
        }

        /// <summary>
        /// Replaces all ProBuilder ColliderBehaviour scripts by a <see cref="ColliderVisibilityBehaviour"/>.
        /// </summary>
        [MenuItem(FrameworkUtility.MenuItemPath + "Collider/Replace Pro Builder Collider Scripts", false, FrameworkUtility.MenuOrder + 100)]
        public static void ReplaceProBuilderColliderScripts() {

            if (colliderBehaviourType == null)
                return;

            int _count = 0;
            GetComponents(colliderBehaviourType, Replace);

            Debug.Log($"Replaced Component(s) => {_count}");

            // ----- Local Method ----- \\

            void Replace(Component _component) {

                if (PrefabUtility.IsPartOfPrefabInstance(_component) || PrefabUtility.IsAddedComponentOverride(_component) || PrefabUtility.IsPartOfImmutablePrefab(_component))
                    return;

                GameObject _object = _component.gameObject;

                Undo.RecordObject(_object, ReplaceUndoName);
                Undo.DestroyObjectImmediate(_component);
                
                _component = Undo.AddComponent<ColliderVisibilityBehaviour>(_object);

                while (ComponentUtility.MoveComponentUp(_component)) { }

                _count++;
                EditorUtility.SetDirty(_object);
            }
        }

        /// <summary>
        /// Detects all off-centered colliders in the loaded scene(s).
        /// </summary>
        [MenuItem(FrameworkUtility.MenuItemPath + "Collider/Detect All Off-centered Colliders", false, FrameworkUtility.MenuOrder - 100)]
        public static void DetectAllOffCenteredColliders() {
            GetComponents<CapsuleCollider>(OnCapsuleDetected);
            GetComponents<SphereCollider>(OnSphereDetected);
            GetComponents<BoxCollider>(OnBoxDetected);

            // ----- Local Methods ----- \\

            void OnCapsuleDetected(CapsuleCollider _capsule) {
                if (_capsule.center != Vector3.zero) {
                    _capsule.LogMessage("Off-centered Capsule => " + _capsule.name);
                }
            }

            void OnSphereDetected(SphereCollider _sphere) {
                if (_sphere.center != Vector3.zero) {
                    _sphere.LogMessage("Off-centered Sphere => " + _sphere.name);
                }
            }

            void OnBoxDetected(BoxCollider _box) {
                if (_box.center != Vector3.zero) {
                    _box.LogMessage("Off-centered Box => " + _box.name);
                }
            }
        }

        /// <summary>
        /// Adjust the center of all colliders in the loaded scene(s).
        /// </summary>
        [MenuItem(FrameworkUtility.MenuItemPath + "Collider/Adjust All Colliders Center", false, FrameworkUtility.MenuOrder - 100)]
        public static void AdjustAllCollidersCenter() {
            GetComponents<CapsuleCollider>(AdjustCapsule);
            GetComponents<SphereCollider>(AdjustSphere);
            GetComponents<BoxCollider>(AdjustBox);

            // ----- Local Methods ----- \\

            void AdjustCapsule(CapsuleCollider _capsule) {
                if (_capsule.center != Vector3.zero) {
                    SceneDesignerUtility.AdjustCapsuleCenter(_capsule);
                    _capsule.LogMessage("Adjust Capsule => " + _capsule.name);
                }
            }

            void AdjustSphere(SphereCollider _sphere) {
                if (_sphere.center != Vector3.zero) {
                    SceneDesignerUtility.AdjustSphereCenter(_sphere);
                    _sphere.LogMessage("Adjust Sphere => " + _sphere.name);
                }
            }

            void AdjustBox(BoxCollider _box) {
                if (_box.center != Vector3.zero) {
                    SceneDesignerUtility.AdjustBoxCenter(_box);
                    _box.LogMessage("Adjust Box => " + _box.name);
                }
            }
        }

        // -----------------------

        private static void GetComponents<T>(Action<T> _onAction) where T : Component {
            GetComponents(typeof(T), (c) => _onAction?.Invoke(c as T));
        }

        private static void GetComponents(Type _type, Action<Component> _onAction) {

            Component[] _components;

            #if FIND_OBJECT_BY_TYPE
            _components = Object.FindObjectsByType(_type, FindObjectsInactive.Include, FindObjectsSortMode.None) as Component[];
            #else
            _components = Object.FindObjectsOfType<Component>(true);
            #endif

            foreach (Component _component in _components) {
                _onAction(_component);
            }
        }
        #endregion
    }
}
