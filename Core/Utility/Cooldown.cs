// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using System;
using UnityEngine;

namespace EnhancedFramework.Core {
    /// <summary>
    /// Utility class for an easy-to use realtime cooldown.
    /// </summary>
    [Serializable]
    public class Cooldown {
        #region Global Members
        /// <summary>
        /// Duration of the cooldown, that is the time (in seconds) it takes to be fully reloaded.
        /// </summary>
        public float Duration = 0f;

        protected float time = 0f;

        /// <summary>
        /// Is this cooldown valid again?
        /// </summary>
        public virtual bool IsValid {
            get { return (Time.time - time) > Duration; }
        }

        // -----------------------

        /// <param name="_duration"><inheritdoc cref="Duration" path="/summary"/></param>
        /// <inheritdoc cref="Cooldown"/>
        public Cooldown(float _duration) {
            Duration = _duration;
        }
        #endregion

        #region Operator
        public static implicit operator bool(Cooldown _cooldown) {
            return _cooldown.IsValid;
        }
        #endregion

        #region Behaviour
        /// <summary>
        /// Reloads this cooldown using its current duration.
        /// </summary>
        public virtual void Reload() {
            time = Time.time;
        }
        #endregion
    }

    /// <summary>
    /// Utility class for an easy-to use unscaled-time cooldown.
    /// </summary>
    [Serializable]
    public class UnscaledCooldown : Cooldown {
        #region Global Members
        public override bool IsValid {
            get { return (Time.unscaledTime - time) > Duration; }
        }

        // -----------------------

        /// <summary>
        /// <inheritdoc cref="UnscaledCooldown"/>
        /// </summary>
        /// <inheritdoc cref="Cooldown(float)"/>
        public UnscaledCooldown(float _duration) : base(_duration) { }
        #endregion

        #region Behaviour
        public override void Reload() {
            time = Time.unscaledTime;
        }
        #endregion
    }

    /// <summary>
    /// Utility class for an easy-to use manual update cooldown.
    /// </summary>
    [Serializable]
    public class ManualCooldown : Cooldown {
        #region Global Members
        public override bool IsValid {
            get { return time > Duration; }
        }

        // -----------------------

        /// <summary>
        /// <inheritdoc cref="ManualCooldown"/>
        /// </summary>
        /// <inheritdoc cref="Cooldown(float)"/>
        public ManualCooldown(float _duration) : base(_duration) { }
        #endregion

        #region Behaviour
        /// <summary>
        /// Updates this cooldown value.
        /// </summary>
        /// <param name="_deltaTime">Time increase.</param>
        public void Update(float _deltaTime) {
            time += _deltaTime;
        }

        public override void Reload() {
            time = 0f;
        }
        #endregion
    }
}
