// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using System;
using UnityEngine;

#if DOTWEEN_ENABLED
using DG.Tweening;
#endif

using Min = EnhancedEditor.MinAttribute;

namespace EnhancedFramework.Core {
    /// <summary>
    /// Base class for valueless ease/curve settings.
    /// <br/> Prefer using <see cref="EaseSettings"/>/<see cref="CurveSettings"/> instead of this.
    /// </summary>
    [Serializable]
    public abstract class EaseCurveSettings {
        #region Content
        [Tooltip("Animation duration, in seconds")]
        [Enhanced, Min(0f)] public float Duration = 1f;

        [Tooltip("Activation delay, in seconds")]
        [Enhanced, Min(0f)] public float Delay = 0f;
        #endregion
    }

    #if DOTWEEN_ENABLED
    /// <summary>
    /// Wrapper utility class for an <see cref="DG.Tweening.Ease"/> type settings.
    /// </summary>
    [Serializable]
    public class EaseSettings : EaseCurveSettings {
        #region Content
        [Tooltip("Evaluation ease preset")]
        public Ease Ease = Ease.OutSine;
        #endregion
    }
    #endif

    /// <summary>
    /// Wrapper utility class for an <see cref="AnimationCurve"/> type settings.
    /// </summary>
    [Serializable]
    public class CurveSettings : EaseCurveSettings {
        #region Content
        [Tooltip("Evaluation curve")]
        [Enhanced, EnhancedCurve(0f, 0f, 1f, 1f, SuperColor.Lime)] public AnimationCurve Ease = AnimationCurve.Constant(0f, 1f, 0f);
        #endregion
    }
}
