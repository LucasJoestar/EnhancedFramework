// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EnhancedFramework.Core {
    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum CardinalDirection {
        [Separator(SeparatorPosition.Bottom)]
        None        = 0,

        North       = 1 << 0,
        South       = 1 << 1,
        Ease        = 1 << 2,
        West        = 1 << 3,

        NorthEast   = 1 << 4,
        NorthWest   = 1 << 5,
        SouthEast   = 1 << 6,
        SouthWest   = 1 << 7,
    }

    /// <summary>
    /// Base class to inherit your own triggers from.
    /// </summary>
    [ScriptGizmos(false, true)]
    [AddComponentMenu(FrameworkUtility.MenuPath + "Trigger/Level Trigger"), DisallowMultipleComponent]
    public class LevelTrigger : EnhancedTrigger {

        /// <summary>
        /// Wrapper for a <see cref="LevelTrigger"/> activation direction range.
        /// </summary>
        [Serializable]
        public class ActivationDirection {
            #region Global Members
            [Tooltip("Name identifier of this direction")]
            [SerializeField, Enhanced, FlagField(false), Duo("inverse", 95f)] private CardinalDirection identifier = CardinalDirection.None;

            [Tooltip("When true, inverses the direction angle forward vector")]
            [SerializeField, HideInInspector, Enhanced, Duo("color", 75f)] private bool inverse = false;

            #if UNITY_EDITOR
            [Tooltip("Previsualisation color of this direction")]
            [SerializeField, HideInInspector] internal Color color = Color.red;
            #endif

            [Tooltip("Angle detection range of this direction")]
            [SerializeField, Enhanced, MinMax(-180f, 180f)] private Vector2Int angle = new Vector2Int(-15, 15);

            // -----------------------

            /// <summary>
            /// Identifier of this direction.
            /// </summary>
            public CardinalDirection Identifier {
                get { return identifier; }
            }

            /// <summary>
            /// Angle detection range of this direction.
            /// </summary>
            public Vector2Int DirectionAngle {
                get {
                    Vector2Int _angle = angle;

                    if (inverse) {
                        _angle.x -= 180;
                        _angle.y -= 180;
                    }

                    return _angle;
                }
            }
            #endregion
        }

        /// <summary>
        /// Wrapper for a <see cref="LevelTrigger"/> <see cref="ITrigger"/> callback.
        /// </summary>
        [Serializable]
        public class Callback {
            #region Global Members
            [Tooltip("Callback object connected with this trigger")]
            [SerializeField] private SerializedInterface<ITrigger> trigger = null;

            [Tooltip("Requied trigger enter direction to call this callback")]
            [SerializeField] private CardinalDirection enter = CardinalDirection.None;

            [Tooltip("Requied trigger exit direction to call this callback")]
            [SerializeField] private CardinalDirection exit = CardinalDirection.None;

            /// <summary>
            /// <see cref="ITrigger"/> callback object.
            /// </summary>
            public ITrigger Trigger {
                get { return trigger.Interface; }
            }

            /// <summary>
            /// Requied trigger enter direction to call this callback.
            /// </summary>
            public CardinalDirection Enter {
                get { return enter; }
            }

            /// <summary>
            /// Requied trigger exit direction to call this callback.
            /// </summary>
            public CardinalDirection Exit {
                get { return exit; }
            }

            // -------------------------------------------
            // Constructor(s)
            // -------------------------------------------

            /// <param name="_trigger"><inheritdoc cref="Trigger" path="/summary"/></param>
            /// <param name="_enter"><inheritdoc cref="Enter" path="/summary"/></param>
            /// <param name="_exit"><inheritdoc cref="Exit" path="/summary"/></param>
            /// <inheritdoc cref="Callback"/>
            public Callback(ITrigger _trigger, CardinalDirection _enter, CardinalDirection _exit) {
                trigger = new SerializedInterface<ITrigger>(_trigger);
                enter = _enter;
                exit = _exit;
            }
            #endregion

            #region Trigger
            /// <param name="_direction">Identifier of the associated <see cref="LevelTrigger"/> entered direction.</param>
            /// <inheritdoc cref="ITrigger.OnEnterTrigger(Component)"/>
            public void OnEnterTrigger(Component _component, CardinalDirection _direction) {

                // Trigger activation.
                if ((enter != CardinalDirection.None) && !enter.HasFlag(_direction)) {
                    return;
                }

                trigger.Interface.OnEnterTrigger(_component);
            }

            /// <param name="_direction">Identifier of the associated <see cref="LevelTrigger"/> exit direction.</param>
            /// <inheritdoc cref="ITrigger.OnExitTrigger(Component)"/>
            public void OnExitTrigger(Component _component, CardinalDirection _direction) {

                // Trigger activation.
                if ((exit != CardinalDirection.None) && !exit.HasFlag(_direction)) {
                    return;
                }

                trigger.Interface.OnExitTrigger(_component);
            }
            #endregion
        }

        #region Global Members
        [Section("Level Trigger")]

        [Tooltip("Required tags on objects to interact with this Trigger")]
        [SerializeField] private TagGroup requiredTags = new TagGroup();

        [Space(5f)]

        [SerializeField] private BlockList<Callback> callbacks = new BlockList<Callback>();

        [Space(10f), HorizontalLine(SuperColor.Grey, 1f), Space(10f)]

        [SerializeField] internal BlockArray<ActivationDirection> directions = new BlockArray<ActivationDirection>();
        #endregion

        #region Enhanced Behaviour
        #if UNITY_EDITOR
        // -------------------------------------------
        // Editor
        // -------------------------------------------

        private const int RaycastInterval   = 2;
        private const float HandlesAlpha    = .5f;

        private static readonly RaycastHit[] hitBuffer = new RaycastHit[16];

        // -----------------------

        protected override void OnValidate() {
            base.OnValidate();

            if (!Application.isPlaying && (callbacks.Count == 0)) {
                GetObjectCallbacks();
            }
        }

        protected internal override void OnDrawHandles() {
            base.OnDrawHandles();

            Collider _collider = GetComponent<Collider>();

            if ((directions.Count == 0) || !_collider) {
                return;
            }

            Vector3 _center = _collider.bounds.center;
            List<Vector3> _area = new List<Vector3>();

            for (int i = directions.Count; i-- > 0;) {

                ActivationDirection _direction = directions[i];

                using (var _scope = EnhancedGUI.HandlesColor.Scope(_direction.color.SetAlpha(HandlesAlpha))) {

                    int _maxAngle = _direction.DirectionAngle.y;
                    float _angle = _direction.DirectionAngle.x;

                    _area.Clear();
                    _area.Add(_center);

                    while (true) {

                        Vector3 _position = GetPoint(_angle);
                        if (_position != _center) {
                            _area.Add(_position);
                        }

                        if ((int)_angle == _maxAngle) {
                            break;
                        }

                        _angle = Mathf.MoveTowards(_angle, _maxAngle, RaycastInterval);
                    }

                    _area.Add(_center);
                    Handles.DrawAAConvexPolygon(_area.ToArray());

                    // ----- Local Method ----- \\

                    Vector3 GetPoint(float _angle) {

                        Vector3 _direction = Quaternion.AngleAxis(_angle, _collider.transform.up) * _collider.transform.forward;
                        float _length = _collider.bounds.max.SetY(0f).sqrMagnitude;

                        int _count = Physics.RaycastNonAlloc(_center + _direction * _length, -_direction, hitBuffer, _length);
                        for (int i = 0; i < _count; i++) {

                            if (hitBuffer[i].collider == _collider) {
                                return hitBuffer[i].point;
                            }
                        }

                        return _center;
                    }
                }
            }
        }
#endif
        #endregion

        #region Registration
        /// <summary>
        /// Registers a specific <see cref="ITrigger"/> callback instance on this trigger.
        /// </summary>
        /// <param name="_callback"><see cref="ITrigger"/> instance to register.</param>
        /// <inheritdoc cref="Callback(ITrigger, CardinalDirection, CardinalDirection)"/>
        public void RegisterCallback(ITrigger _callback, CardinalDirection _enter = CardinalDirection.None, CardinalDirection _exit = CardinalDirection.None) {
            callbacks.Add(new Callback(_callback, _enter, _exit));
        }

        /// <summary>
        /// Unregisters a specific <see cref="ITrigger"/> callback instance from this trigger.
        /// </summary>
        /// <param name="_callback"><see cref="ITrigger"/> instance to unregister.</param>
        public void UnregisterCallback(ITrigger _callback) {

            int _index = callbacks.List.FindIndex(c => c.Trigger == _callback);
            if (_index != -1) {
                callbacks.List.RemoveAt(_index);
            }
        }

        // -----------------------

        /// <summary>
        /// Get trigger callbacks from this object and all its parents.
        /// </summary>
        [Button(SuperColor.Green, IsDrawnOnTop = false)]
        [ContextMenu("Get Callbacks", false, 100)]
        protected void GetObjectCallbacks() {

            foreach (ITrigger _trigger in GetComponentsInParent<ITrigger>()) {
                if (!ReferenceEquals(_trigger, this)) {
                    RegisterCallback(_trigger);
                }
            }
        }

        /// <summary>
        /// Removes trigger callbacks from this object and all its parents.
        /// </summary>
        [ContextMenu("Clear Callbacks", false, 100)]
        protected void ClearObjectCallbacks() {

            foreach (ITrigger _trigger in GetComponentsInParent<ITrigger>()) {
                if (!ReferenceEquals(_trigger, this)) {
                    UnregisterCallback(_trigger);
                }
            }
        }
        #endregion

        #region Trigger
        protected override void OnEnterTrigger(Component _component) {
            base.OnEnterTrigger(_component);

            // Callbacks.
            CardinalDirection _direction = CardinalDirection.None;

            foreach (Callback _callback in callbacks) {
                _callback.OnEnterTrigger(_component, _direction);
            }
        }

        protected override void OnExitTrigger(Component _component) {
            base.OnExitTrigger(_component);

            // Callbacks.
            CardinalDirection _direction = CardinalDirection.None;

            foreach (Callback _callback in callbacks) {
                _callback.OnExitTrigger(_component, _direction);
            }
        }

        // -----------------------

        protected override bool InteractWithTrigger(Component _component) {
            return base.InteractWithTrigger(_component) && _component.HasTags(requiredTags);
        }
        #endregion
    }
}
