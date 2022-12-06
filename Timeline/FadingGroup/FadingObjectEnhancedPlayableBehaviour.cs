// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedFramework.Core;
using System;
using UnityEngine;
using UnityEngine.Playables;

namespace EnhancedFramework.Timeline {
    /// <summary>
    /// Base <see cref="PlayableBehaviour"/> class for a <see cref="IFadingObject"/>.
    /// </summary>
    [Serializable]
    public class FadingObjectEnhancedPlayableBehaviour : EnhancedPlayableBehaviour {
        #region Global Members
        [Space(10f), HorizontalLine(SuperColor.Grey, 1f, 50f), Space(10f)]

        public bool StartVisibility = true;
        public bool EndVisibility   = false;

        // -----------------------

        internal IFadingObject fadingGroup = null;

        // -----------------------

        protected virtual IFadingObject FadingObject {
            get { return fadingGroup; }
        }
        #endregion

        #region Behaviour
        protected override void OnPlay(Playable _playable, FrameData _info) {
            base.OnPlay(_playable, _info);

            IFadingObject _object = FadingObject;
            if (_object != null) {
                _object.SetVisibility(StartVisibility);
            }
        }

        protected override void OnPause(Playable _playable, FrameData _info) {
            base.OnPause(_playable, _info);

            IFadingObject _object = FadingObject;
            if (_object != null) {
                _object.SetVisibility(EndVisibility);
            }
        }
        #endregion
    }
}
