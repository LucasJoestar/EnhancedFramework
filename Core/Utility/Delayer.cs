// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
// 
// Notes:
// 
// ================================================================================== //

using System;
using UnityEngine;

using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EnhancedFramework.Core {
    /// <summary>
    /// Utility class used to dynamically delay a specific method call.
    /// </summary>
    #if UNITY_EDITOR
    [InitializeOnLoad]
    #endif
    public class Delayer : IPermanentUpdate {
        /// <summary>
        /// Delayed call struct wrapper.
        /// </summary>
        private struct DelayedCall {
            #region Global Members
            public int ID;

            public float Delay;
            public Action Callback;
            public bool UseUnscaledTime;

            // -----------------------

            public DelayedCall(int _id, float _delay, Action _callback, bool _useUnscaledTime = false) {
                ID = _id;
                Delay = _delay;
                Callback = _callback;
                UseUnscaledTime = _useUnscaledTime;
            }
            #endregion

            #region Behaviour
            public bool Update() {
                Delay -= GetDeltaTime();

                if (Delay <= 0f) {
                    Invoke();
                    return true;
                }

                return false;
            }

            private float GetDeltaTime() {
                #if UNITY_EDITOR
                if (!Application.isPlaying) {
                    // Editor behaviour.
                    return EditorDeltaTime;
                }
                #endif

                // Runtime.
                return UseUnscaledTime
                      ? Time.unscaledDeltaTime
                      : Time.deltaTime;
            }

            public void Invoke() {
                Callback?.Invoke();
            }
            #endregion
        }

        #region Global Members
        public Object LogObject {
            get { return GameManager.Instance; }
        }

        /// <summary>
        /// Static singleton instance.
        /// </summary>
        private static readonly Delayer instance = new Delayer();

        /// <summary>
        /// All currently active delayed calls.
        /// </summary>
        private static readonly EnhancedCollection<DelayedCall> calls = new EnhancedCollection<DelayedCall>();

        // -------------------------------------------
        // Editor Delta Time
        // -------------------------------------------

        #if UNITY_EDITOR
        private static double lastUpdateTime = 0d;

        private static float EditorDeltaTime {
            get { return (float)(EditorApplication.timeSinceStartup - lastUpdateTime); }
        }
        #endif

        // -------------------------------------------
        // Initialization
        // -------------------------------------------

        // Called after the first scene Awake.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void Initialize() {
            UpdateManager.Instance.Register(instance, UpdateRegistration.Permanent);
        }

        #if UNITY_EDITOR
        // Editor constructor.
        static Delayer() {
            EditorApplication.update += EditorUpdate;
        }
        
        private static void EditorUpdate() {
            if (!Application.isPlaying) {
                instance.Update();

                // Editor deltaTime.
                lastUpdateTime = EditorApplication.timeSinceStartup;
            }
        }
        #endif
        #endregion

        #region Delayer
        public const int DefaultCallID = 0;

        // -----------------------

        /// <inheritdoc cref="Call(int, float, Action, bool)"/>
        public static void Call(float _delay, Action _callback, bool _useUnscaledTime = false) {
            Call(DefaultCallID, _delay, _callback, _useUnscaledTime);
        }

        /// <summary>
        /// Calls a given callback after a certain delay.
        /// </summary>
        /// <param name="_id">The id of this delayed call. You can use the same id to cancel it using <see cref="Cancel(int)"/></param>
        /// <param name="_delay">The time (in seconds) to wait for this callback to be called.</param>
        /// <param name="_callback">The delayed callback delegate.</param>
        /// <param name="_useUnscaledTime">If true, this callback delay will not be affected by the game time scale.
        /// <br/> Has no effect in edit mode.</param>
        public static void Call(int _id, float _delay, Action _callback, bool _useUnscaledTime = false) {
            calls.Add(new DelayedCall(_id, _delay, _callback, _useUnscaledTime));
        }

        /// <summary>
        /// Cancels a previously registered delayed call.
        /// </summary>
        /// <param name="_id">The id of the call to cancel.</param>
        /// <returns>True if the call with the given id could be found and successfully canceled, false otherwise.</returns>
        public static bool Cancel(int _id) {
            if ((_id == DefaultCallID) || (_id == currentID)) {
                return false;
            }

            int _index = calls.FindIndex(p => p.ID == _id);
            if (_index != -1) {
                calls.RemoveAt(_index);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Cancels all previously registered call matching a specific id.
        /// </summary>
        /// <param name="_id">Id to cancel the associated calls.</param>
        public static void CancelAll(int _id) {
            for (int i = calls.Count; i-- > 0;) {
                if (calls[i].ID == _id) {
                    calls.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Completes a previously registered delayed call.
        /// </summary>
        /// <param name="_id">The id of the call to complete.</param>
        /// <returns>True if the call with the given id could be found and successfully completed, false otherwise.</returns>
        public static bool Complete(int _id) {
            if ((_id == DefaultCallID) || (_id == currentID)) {
                return false;
            }

            int _index = calls.FindIndex(p => p.ID == _id);

            if (_index != -1) {
                calls[_index].Invoke();
                calls.RemoveAt(_index);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Completes all previously registered call matching a specific id.
        /// </summary>
        /// <param name="_id">Id to complete the associated calls.</param>
        public static void CompleteAll(int _id) {
            for (int i = 0; i < calls.Count; i++) {

                if (calls[i].ID == _id) {
                    calls[i].Invoke();
                    calls.RemoveAt(i);

                    i--;
                }
            }
        }
        #endregion

        #region Behaviour
        private static int currentID = DefaultCallID;

        // -----------------------

        public void Update() {
            for (int i = 0; i < calls.Count; i++) {
                var _call = calls[i];
                currentID = _call.ID;

                if (_call.Update()) {
                    calls.RemoveAt(i);
                    i--;
                } else {
                    calls[i] = _call;
                }
            }

            currentID = DefaultCallID;
        }
        #endregion
    }
}
