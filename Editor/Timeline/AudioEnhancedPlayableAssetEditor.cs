// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework-Conversations ===== //
// 
// Notes:
//
// ================================================================================================ //

#if TIMELINE_ENABLED
using EnhancedFramework.Timeline;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;

using Object = UnityEngine.Object;

namespace EnhancedFramework.Editor {
    /// <summary>
    /// Custom <see cref="AudioEnhancedPlayableAsset"/> editor.
    /// <br/> Displays an waveform preview of the audio clips.
    /// </summary>
    [CustomTimelineEditor(typeof(AudioEnhancedPlayableAsset))]
    public class AudioEnhancedPlayableAssetEditor : EnhancedPlayableAssetEditor {
        #region Editor
        private const string NoAudioErrorMessage    = "No Audio assigned";

        // -------------------------------------------
        // Reflection
        // -------------------------------------------

        private const BindingFlags StaticFlags      = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        private const BindingFlags InstanceFlags    = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private static readonly Type waveformPreviewType        = typeof(EditorWindow).Assembly.GetType("UnityEditor.WaveformPreview");
        private static readonly Type waveformPreviewFactoryType = typeof(EditorWindow).Assembly.GetType("UnityEditor.WaveformPreviewFactory");

        private static readonly Type timelineWindowType         = typeof(ClipEditor).Assembly.GetType("UnityEditor.Timeline.TimelineWindow");
        private static readonly Type windowStateType            = typeof(ClipEditor).Assembly.GetType("UnityEditor.Timeline.WindowState");

        private static readonly Type directorStylesType         = typeof(ClipEditor).Assembly.GetType("UnityEditor.Timeline.DirectorStyles");
        private static readonly Type directorNamedColorType     = typeof(ClipEditor).Assembly.GetType("UnityEngine.Timeline.DirectorNamedColor");

        private static readonly PropertyInfo timelineWindowInstanceField        = timelineWindowType.GetProperty("instance", StaticFlags);
        private static readonly PropertyInfo timelineWindowStateField           = timelineWindowType.GetProperty("state", InstanceFlags);
        private static readonly PropertyInfo windowStateShowAudioWaveformField  = windowStateType.GetProperty("showAudioWaveform", InstanceFlags);

        private static readonly PropertyInfo directorStylesInstanceProperty     = directorStylesType.GetProperty("Instance", StaticFlags);
        private static readonly PropertyInfo directorStylesCustomSkinProperty   = directorStylesType.GetProperty("customSkin", InstanceFlags);
        private static readonly FieldInfo directorNamedColorAudioWaveformField  = directorNamedColorType.GetField("colorAudioWaveform", InstanceFlags);

        private static readonly PropertyInfo waveformPreviewBackgroundColorProperty = waveformPreviewType.GetProperty("backgroundColor", InstanceFlags);
        private static readonly PropertyInfo waveformPreviewWaveColorProperty       = waveformPreviewType.GetProperty("waveColor", InstanceFlags);
        private static readonly PropertyInfo waveformPreviewLoopingProperty         = waveformPreviewType.GetProperty("looping", InstanceFlags);
        private static readonly FieldInfo waveformPreviewPresentedObjectField       = waveformPreviewType.GetField("presentedObject", InstanceFlags);

        private static readonly MethodInfo waveformPreviewApplyModificationsMethod  = waveformPreviewType.GetMethod("ApplyModifications", InstanceFlags);
        private static readonly MethodInfo waveformPreviewOptimizeForSizeMethod     = waveformPreviewType.GetMethod("OptimizeForSize", InstanceFlags);
        private static readonly MethodInfo waveformPreviewSetTimeInfoMethod         = waveformPreviewType.GetMethod("SetTimeInfo", InstanceFlags);
        private static readonly MethodInfo waveformPreviewRenderMethod              = waveformPreviewType.GetMethod("Render", InstanceFlags);

        private static readonly MethodInfo waveformPreviewFactoryCreateMethod       = waveformPreviewFactoryType.GetMethod("Create", StaticFlags);

        // -----------------------

        private static readonly Dictionary<TimelineClip, object> previewCache = new Dictionary<TimelineClip, object>();
        private static ColorSpace colorSpace = ColorSpace.Uninitialized;

        // -------------------------------------------
        // Editor
        // -------------------------------------------

        public override ClipDrawOptions GetClipOptions(TimelineClip _timelineClip) {
            ClipDrawOptions _clipOptions = base.GetClipOptions(_timelineClip);
            AudioAssetClip _audioPlayableAsset = _timelineClip.asset as AudioAssetClip;

            if ((_audioPlayableAsset != null) && (_audioPlayableAsset.Audio == null)) {
                _clipOptions.errorText = NoAudioErrorMessage;
            }

            return _clipOptions;
        }

        public override void DrawBackground(TimelineClip _clip, ClipBackgroundRegion _region) {
            if (!ShowAudioWaveform()) {
                return;
            }

            Rect _rect = _region.position;
            if (_rect.width <= 0f) {
                return;
            }

            // Get target clip.
            AudioClip _audioClip = _clip.asset as AudioClip;
            if (_audioClip == null) {

                AudioAssetClip _audioPlayableAsset = _clip.asset as AudioAssetClip;
                if ((_audioPlayableAsset != null) && (_audioPlayableAsset.Audio != null)) {
                    _audioClip = _audioPlayableAsset.Audio.GetClip();
                }
            }

            if (_audioClip == null) {
                return;
            }

            Rect _quantizedRect = new Rect(Mathf.Ceil(_rect.x), Mathf.Ceil(_rect.y), Mathf.Ceil(_rect.width), Mathf.Ceil(_rect.height));

            if (QualitySettings.activeColorSpace != colorSpace) {
                colorSpace = QualitySettings.activeColorSpace;
                previewCache.Clear();
            }

            // Get cache preview.
            if (!previewCache.TryGetValue(_clip, out object _preview) || _audioClip != (waveformPreviewPresentedObjectField.GetValue(_preview) as Object)) {

                _preview = previewCache[_clip]
                         = waveformPreviewFactoryCreateMethod.Invoke(null, new object[] { (int)_quantizedRect.width, _audioClip });

                Color waveColour = GammaCorrect((Color)directorNamedColorAudioWaveformField.GetValue(directorStylesCustomSkinProperty.GetValue(directorStylesInstanceProperty.GetValue(null))));
                Color transparent = waveColour;
                transparent.a = 0;

                waveformPreviewBackgroundColorProperty.SetValue(_preview, transparent);
                waveformPreviewWaveColorProperty.SetValue(_preview, waveColour);
            }

            // Update cache values.
            waveformPreviewLoopingProperty.SetValue(_preview, (_clip.clipCaps & ClipCaps.Looping) != 0);
            waveformPreviewSetTimeInfoMethod.Invoke(_preview, new object[] { _region.startTime, _region.endTime - _region.startTime });
            waveformPreviewOptimizeForSizeMethod.Invoke(_preview, new object[] { _quantizedRect.size });

            // Repaint.
            if (Event.current.type == EventType.Repaint) {

                waveformPreviewApplyModificationsMethod.Invoke(_preview, null);
                waveformPreviewRenderMethod.Invoke(_preview, new object[] { _quantizedRect });
            }
        }

        // -------------------------------------------
        // Utility
        // -------------------------------------------

        private static Color GammaCorrect(Color color) {
            return (QualitySettings.activeColorSpace == ColorSpace.Linear) ? color.gamma : color;
        }

        private static bool ShowAudioWaveform() {
            var _timelineWindow = timelineWindowInstanceField.GetValue(null);
            if (_timelineWindow == null) {
                return false;
            }

            var _state = timelineWindowStateField.GetValue(_timelineWindow);
            if (_state == null) {
                return false;
            }

            return (bool)windowStateShowAudioWaveformField.GetValue(_state);
        }
        #endregion
    }
}
#endif
