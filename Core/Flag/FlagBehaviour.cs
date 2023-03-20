// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace EnhancedFramework.Core {
    /// <summary>
    /// <see cref="Component"/> wrapper for a <see cref="FlagReference"/>.
    /// </summary>
    [ScriptGizmos(false, true)]
    [AddComponentMenu(FrameworkUtility.MenuPath + "Flag/Flag")]
    public class FlagBehaviour : EnhancedBehaviour {
        #region Global Members
        [Section("Flag")]

        public FlagReference Flag = new FlagReference();
        #endregion

        #region Operator
        public static implicit operator Flag(FlagBehaviour _flag) {
            return _flag.Flag;
        }

        public static implicit operator bool(FlagBehaviour _flag) {
            return _flag.Flag;
        }

        public static implicit operator int(FlagBehaviour _flag) {
            return _flag.Flag;
        }

        public override string ToString() {
            return Flag.ToString();
        }
        #endregion

        #region Utility
        /// <inheritdoc cref="FlagReference.SetFlag(EnhancedEditor.Flag)"/>
        public void SetFlag(Flag _flag) {
            Flag.SetFlag(_flag);
        }

        /// <inheritdoc cref="FlagReference.GetValue"/>
        [Button(ActivationMode.Always, SuperColor.Green)]
        public bool GetValue() {
            bool _value = Flag.GetValue();

            #if DEVELOPMENT
            this.Log($"{Flag.Flag.Name.Bold()} flag value: {_value.ToString().Bold()}");
            #endif

            return _value;
        }

        /// <inheritdoc cref="FlagReference.SetValue(bool)"/>
        [Button(ActivationMode.Play, SuperColor.Orange)]
        public void SetValue(bool _value) {
            Flag.SetValue(_value);
        }
        #endregion
    }
}
