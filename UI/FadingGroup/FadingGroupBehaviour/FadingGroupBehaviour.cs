// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedFramework.Core;
using System;
using UnityEngine;

namespace EnhancedFramework.UI {
    /// <summary>
    /// Base class for <see cref="EnhancedBehaviour"/>-encapsulated <see cref="FadingGroup"/>.
    /// <para/>
    /// Inherited by <see cref="FadingGroupBehaviour"/> and <see cref="TweeningFadingGroupBehaviour"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="FadingGroup"/> class type used by this object.</typeparam>
    public abstract class FadingGroupBehaviour<T> : FadingObjectBehaviour where T : FadingGroup, new() {
        #region Global Members
        [Section("Fading Group")]

        [SerializeField, Enhanced, Block] protected T group = default;

        [Space(10f)]

        [SerializeField] protected FadingMode initMode = FadingMode.Hide;

        // -----------------------

        public T Group {
            get { return group; }
        }

        public override IFadingObject FadingObject {
            get { return group; }
        }

        public override FadingMode InitMode {
            get { return initMode; }
        }
        #endregion

        #region Enhanced Behaviour
        protected override void OnBehaviourDisabled() {
            base.OnBehaviourDisabled();

            // Group callback.
            group.OnDisabled();
        }

        #if UNITY_EDITOR
        protected override void OnValidate() {
            base.OnValidate();

            // References.
            if (group == null) {
                group = Activator.CreateInstance<T>();
            }

            if (!group.Group) {
                group.Group = GetComponent<CanvasGroup>();
            }

            if (group.UseCanvas && !group.Canvas) {
                group.Canvas = GetComponent<Canvas>();
            }

            if (group.UseController && !group.Controller) {
                group.Controller = GetComponent<FadingGroupController>();
            }

            if (group.UseSelectable && !Application.isPlaying) {
                group.ActiveSelectable = group.Selectable;
            }
        }
        #endif
        #endregion

        #region Play Mode Data
        public override bool CanSavePlayModeData {
            get { return true; }
        }

        // -----------------------

        public override void SavePlayModeData(PlayModeEnhancedObjectData _data) {

            // Save as json.
            _data.Strings.Add(JsonUtility.ToJson(group));
        }

        public override void LoadPlayModeData(PlayModeEnhancedObjectData _data) {

            // Load from json.
            T _group = JsonUtility.FromJson<T>(_data.Strings[0]);

            _group.Canvas     = group.Canvas;
            _group.Controller = group.Controller;
            _group.Group      = group.Group;

            _group.Selectable       = group.Selectable;
            _group.ActiveSelectable = group.ActiveSelectable;

            group = _group;
        }
        #endregion
    }

    /// <summary>
    /// Ready-to-use <see cref="EnhancedBehaviour"/>-encapsulated <see cref="FadingGroup"/>.
    /// <br/> Use this to quickly implement instantly fading <see cref="CanvasGroup"/> objects.
    /// </summary>
    [ScriptGizmos(false, true)]
    [AddComponentMenu(FrameworkUtility.MenuPath + "UI/Fading Group/Fading Group"), DisallowMultipleComponent]
    public sealed class FadingGroupBehaviour : FadingGroupBehaviour<FadingGroup> { }
}
