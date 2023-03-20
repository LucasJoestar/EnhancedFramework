// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnhancedFramework.Core {
    /// <summary>
    /// <see cref="Component"/> wrapper for a <see cref="FlagValueGroup"/>.
    /// </summary>
    [ScriptGizmos(false, true)]
    [AddComponentMenu(FrameworkUtility.MenuPath + "Flag/Flag Group")]
    public class FlagGroupBehaviour : EnhancedBehaviour, IEnumerable<Flag> {
        public override UpdateRegistration UpdateRegistration => base.UpdateRegistration | UpdateRegistration.Init;

        #region Global Members
        [Section("Flag Group")]

        public FlagValueGroup Flags = new FlagValueGroup();

        [Tooltip("Set the flag values on initialization")]
        [SerializeField] private bool setFlagsOnInit = false;

        // -----------------------

        /// <summary>
        /// The total amount of flags in this object.
        /// </summary>
        public int Count {
            get { return Flags.Count; }
        }
        #endregion

        #region Enhanced Behaviour
        protected override void OnInit() {
            base.OnInit();

            if (setFlagsOnInit) {
                SetValues();
            }
        }
        #endregion

        #region Operator
        public static implicit operator FlagValueGroup(FlagGroupBehaviour _behaviour) {
            return _behaviour.Flags;
        }

        public Flag this[int _index] {
            get { return Flags[_index]; }
        }
        #endregion

        #region IEnumerable
        public IEnumerator<Flag> GetEnumerator() {
            for (int i = 0; i < Count; i++) {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        #endregion

        #region Management
        /// <inheritdoc cref="FlagGroup.AddFlag(Flag)"/>
        public void AddFlag(Flag _flag) {
            Flags.AddFlag(_flag);
        }

        /// <inheritdoc cref="FlagGroup.RemoveFlag(Flag)"/>
        public void RemoveFlag(Flag _flag) {
            Flags.RemoveFlag(_flag);
        }

        /// <inheritdoc cref="FlagGroup.RemoveFlagAt(int)"/>
        public void RemoveFlagAt(int _index) {
            Flags.RemoveFlagAt(_index);
        }

        /// <inheritdoc cref="FlagGroup.ContainFlag(Flag, out int)"/>
        public bool ContainFlag(Flag _flag, out int _index) {
            return Flags.ContainFlag(_flag, out _index);
        }
        #endregion

        #region Utility
        /// <inheritdoc cref="FlagValueGroup.IsValid"/>
        [Button(ActivationMode.Always, SuperColor.Green)]
        public bool IsValid() {
            bool _value = Flags.IsValid();

            #if DEVELOPMENT
            this.Log($"{name.Bold()} flags valid: {_value.ToString().Bold()}");
            #endif

            return _value;
        }

        /// <inheritdoc cref="FlagValueGroup.SetValues"/>
        [Button(ActivationMode.Play, SuperColor.Orange)]
        public void SetValues() {
            Flags.SetValues();
        }
        #endregion
    }
}
