// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedFramework.Core;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace EnhancedFramework.Timeline {
    /// <summary>
    /// <see cref="TrackAsset"/> class for every <see cref="FadingObjectPlayableAsset"/>.
    /// </summary>
    [TrackColor(.855f, .568f, .0f)] // Harvest Gold
    [TrackClipType(typeof(IFadingObjectPlayableAsset))]
    [TrackBindingType(typeof(FadingObjectBehaviour), TrackBindingFlags.None)]
    [System.ComponentModel.DisplayName("Enhanced Framework/Fading Track")]
    public sealed class FadingObjectTrack : EnhancedTrack {
        #region Behaviour
        public override void GatherProperties(PlayableDirector _director, IPropertyCollector _driver) {
            base.GatherProperties(_director, _driver);

            Object _object = _director.GetGenericBinding(this);
            if ((_object == null) || (_object is not FadingObjectBehaviour _fadingObject)) {
                return;
            }

            // Register object properties.
            _driver.AddFromComponent(_fadingObject.gameObject, _fadingObject);

            CanvasGroup _group = _fadingObject.GetComponentInChildren<CanvasGroup>();
            if (_group.IsValid()) {
                _driver.AddFromComponent(_group.gameObject, _group);
            }

            Canvas _canvas = _fadingObject.GetComponentInParent<Canvas>();
            if (_canvas.IsValid()) {
                _driver.AddFromComponent(_canvas.gameObject, _canvas);
            }
        }
        #endregion
    }
}
