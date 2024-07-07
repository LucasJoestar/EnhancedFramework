// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace EnhancedFramework.Core {
    /// <summary>
    /// <see cref="Component"/> wrapper for a <see cref="FlagValueGroup"/>.
    /// </summary>
    [ScriptGizmos(false, true)]
    [AddComponentMenu(FrameworkUtility.MenuPath + "Flag/Flag Group")]
    public sealed class FlagGroupBehaviour : EnhancedBehaviour {
        #region Global Members
        [Section("Flag Group")]

        public FlagValueGroup Flags = new FlagValueGroup();

        // -----------------------

        /// <summary>
        /// The total amount of flag in this object group.
        /// </summary>
        public int Count {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return Flags.Count; }
        }
        #endregion

        #region Operator
        public static implicit operator FlagValueGroup(FlagGroupBehaviour _behaviour) {
            return _behaviour.Flags;
        }

        public Flag this[int _index] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return Flags[_index]; }
        }
        #endregion

        #region Management
        /// <inheritdoc cref="FlagGroup.AddFlag(Flag)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddFlag(Flag _flag) {
            Flags.AddFlag(_flag);
        }

        /// <inheritdoc cref="FlagGroup.RemoveFlag(Flag)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveFlag(Flag _flag) {
            Flags.RemoveFlag(_flag);
        }

        /// <inheritdoc cref="FlagGroup.RemoveFlagAt(int)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveFlagAt(int _index) {
            Flags.RemoveFlagAt(_index);
        }

        /// <inheritdoc cref="FlagGroup.ContainFlag(Flag, out int)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainFlag(Flag _flag, out int _index) {
            return Flags.ContainFlag(_flag, out _index);
        }
        #endregion

        #region Utility
        /// <inheritdoc cref="FlagValueGroup.Valid"/>
        [Button(ActivationMode.Always, SuperColor.Green, IsDrawnOnTop = false)]
        public bool GetValue() {
            bool _value = Flags.Valid;

            #if DEVELOPMENT
            this.Log($"{name.Bold()} flags valid: {_value.ToString().Bold()}");
            #endif

            return _value;
        }

        /// <inheritdoc cref="FlagValueGroup.SetValues"/>
        public void SetFlags() {
            Flags.SetValues();
        }

        /// <inheritdoc cref="FlagValueGroup.SetValues(bool)"/>
        [Button(ActivationMode.Play, SuperColor.Orange, IsDrawnOnTop = false)]
        public void SetFlags(bool _valid = true) {
            Flags.SetValues(_valid);
        }
        #endregion
    }
}
