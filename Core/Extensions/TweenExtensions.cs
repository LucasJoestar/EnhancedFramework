// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //


using EnhancedEditor;

#if DOTWEEN_ENABLED
using DG.Tweening;
using Extensions = DG.Tweening.TweenExtensions;
#endif

namespace EnhancedFramework.Core {
    /// <summary>
    /// Multiple extension methods related to the <see cref="Tween"/> class.
    /// </summary>
    [ScriptingDefineSymbol("DOTWEEN_ENABLED", "DoTween plugin extensions and classes")]
	public static class TweenExtensions {
        #region Extensions
        #if DOTWEEN_ENABLED
        /// <param name="_tween"><inheritdoc cref="Extensions.Complete(Tween, bool)" path="/param[@name='t']"/></param>
        /// <param name="_withCallbacks"><inheritdoc cref="Extensions.Complete(Tween, bool)" path="/param[@name='withCallbacks']"/></param>
        /// <returns>NULL tween value: use it to assign your tween variable with</returns>
        /// <inheritdoc cref="Extensions.Complete(Tween, bool)"/>
        public static Tween DoComplete(this Tween _tween, bool _withCallbacks = true) {
            if (_tween.IsActive()) {
                _tween.Complete(_withCallbacks);
            }

            return null;
        }

        /// <param name="_tween"><inheritdoc cref="Extensions.Kill(Tween, bool)" path="/param[@name='t']"/></param>
        /// <param name="_complete"><inheritdoc cref="Extensions.Kill(Tween, bool)" path="/param[@name='complete']"/></param>
        /// <returns>NULL tween value: use it to assign your tween variable with</returns>
        /// <inheritdoc cref="Extensions.Kill(Tween, bool)"/>
        public static Tween DoKill(this Tween _tween, bool _complete = false) {
            if (_tween.IsActive()) {
                _tween.Kill(_complete);
            }

            return null;
        }

        // -----------------------

        /// <param name="_sequence"><inheritdoc cref="Extensions.Complete(Tween, bool)" path="/param[@name='t']"/></param>
        /// <param name="_withCallbacks"><inheritdoc cref="Extensions.Complete(Tween, bool)" path="/param[@name='withCallbacks']"/></param>
        /// <returns>NULL sequence value: use it to assign your sequence variable with</returns>
        /// <inheritdoc cref="Extensions.Complete(Tween, bool)"/>
        public static Sequence DoComplete(this Sequence _sequence, bool _withCallbacks = true) {
            if (_sequence.IsActive()) {
                _sequence.Complete(_withCallbacks);
            }

            return null;
        }

        /// <param name="_sequence"><inheritdoc cref="Extensions.Kill(Tween, bool)" path="/param[@name='t']"/></param>
        /// <param name="_complete"><inheritdoc cref="Extensions.Kill(Tween, bool)" path="/param[@name='complete']"/></param>
        /// <returns>NULL sequence value: use it to assign your sequence variable with</returns>
        /// <inheritdoc cref="Extensions.Kill(Tween, bool)"/>
        public static Sequence DoKill(this Sequence _sequence, bool _complete = false) {
            if (_sequence.IsActive()) {
                _sequence.Kill(_complete);
            }

            return null;
        }
        #endif
        #endregion
    }
}
