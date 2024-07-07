// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace EnhancedFramework.Core {
    /// <summary>
    /// <see cref="EnhancedBehaviour"/> wrapper used to control both an <see cref="EnhancedAnimatorController"/> and an <see cref="UnityEngine.Animator"/>.
    /// </summary>
    [ScriptGizmos(false, true)]
    [DefaultExecutionOrder(-10)]
    [RequireComponent(typeof(Animator))]
    [AddComponentMenu(FrameworkUtility.MenuPath + "Animation/Animator Handler"), DisallowMultipleComponent]
    public sealed class EnhancedAnimatorHandler : EnhancedBehaviour {
        public override UpdateRegistration UpdateRegistration => base.UpdateRegistration | UpdateRegistration.Init;

        #region Global Members
        [Section("Animator Handler")]

        [Tooltip("The controller associated with this animator")]
        [SerializeField, Enhanced, Required] private EnhancedAnimatorController controller = null;

        [Tooltip("The animator controlled in this handler")]
        [SerializeField, Enhanced, Required] private Animator animator = null;

        // -----------------------

        /// <summary>
        /// The <see cref="EnhancedAnimatorController"/> associated with this animator.
        /// </summary>
        public EnhancedAnimatorController Controller {
            get { return controller; }
            set { controller = value; }
        }

        /// <summary>
        /// The <see cref="UnityEngine.Animator"/> controlled in this handler.
        /// </summary>
        public Animator Animator {
            get { return animator; }
        }

        /// <inheritdoc cref="EnhancedAnimatorController.LayerCount"/>
        public int LayerCount {
            get { return controller.LayerCount; }
        }
        #endregion

        #region Enhanced Behaviour
        protected override void OnBehaviourEnabled() {
            base.OnBehaviourEnabled();

            // Registration.
            controller.Register(animator);
        }

        protected override void OnInit() {
            base.OnInit();

            controller.Initialize(animator);
        }

        protected override void OnBehaviourDisabled() {
            base.OnBehaviourDisabled();

            // Unregistration.
            controller.Unregister(animator);
        }

        #if UNITY_EDITOR
        // -------------------------------------------
        // Editor
        // -------------------------------------------

        protected override void OnValidate() {
            base.OnValidate();

            if (!animator) {
                animator = GetComponent<Animator>();
            }
        }
        #endif
        #endregion

        #region Animation
        /// <inheritdoc cref="EnhancedAnimatorController.Play(Animator, string, bool)"/>
        public bool Play(string _stateName, bool _instant = false) {
            return controller.Play(animator, _stateName, _instant);
        }

        /// <inheritdoc cref="EnhancedAnimatorController.Play(Animator, int, bool)"/>
        public bool Play(int _stateHash, bool _instant = false) {
            return controller.Play(animator, _stateHash, _instant);
        }

        /// <inheritdoc cref="EnhancedAnimatorController.Play(Animator, int, int, bool)"/>
        public bool Play(int _stateHash, int _layerIndex, bool _instant = false) {
            return controller.Play(animator, _stateHash, _layerIndex, _instant);
        }

        /// <inheritdoc cref="EnhancedAnimatorController.PlayDefault(Animator, int, bool)"/>
        public void PlayDefault(int _layerIndex, bool _instant = false) {
            controller.PlayDefault(animator, _layerIndex, _instant);
        }

        /// <inheritdoc cref="EnhancedAnimatorController.PlayDefault(Animator, int, float)"/>
        public void PlayDefault(int _layerIndex, float _transitionDuration) {
            controller.PlayDefault(animator, _layerIndex, _transitionDuration);
        }
        #endregion

        #region Parameter
        // -------------------------------------------
        // Bool
        // -------------------------------------------

        /// <inheritdoc cref="EnhancedAnimatorController.GetBool(Animator, string)"/>
        public bool GetBool(string _parameterName) {
            return controller.GetBool(animator, _parameterName);
        }

        /// <inheritdoc cref="EnhancedAnimatorController.GetBool(Animator, int)"/>
        public bool GetBool(int _parameterHash) {
            return controller.GetBool(animator, _parameterHash);
        }

        /// <inheritdoc cref="EnhancedAnimatorController.SetBool(Animator, string, bool)"/>
        public void SetBool(string _parameterName, bool _value) {
            controller.SetBool(animator, _parameterName, _value);
        }

        /// <inheritdoc cref="EnhancedAnimatorController.SetBool(Animator, int, bool)"/>
        public void SetBool(int _parameterHash, bool _value) {
            controller.SetBool(animator, _parameterHash, _value);
        }

        // -------------------------------------------
        // Int
        // -------------------------------------------

        /// <inheritdoc cref="EnhancedAnimatorController.GetInt(Animator, string)"/>
        public int GetInt(string _parameterName) {
            return controller.GetInt(animator, _parameterName);
        }

        /// <inheritdoc cref="EnhancedAnimatorController.GetInt(Animator, int)"/>
        public int GetInt(int _parameterHash) {
            return controller.GetInt(animator, _parameterHash);
        }

        /// <inheritdoc cref="EnhancedAnimatorController.SetInt(Animator, string, int)"/>
        public void SetInt(string _parameterName, int _value) {
            controller.SetInt(animator, _parameterName, _value);
        }

        /// <inheritdoc cref="EnhancedAnimatorController.SetInt(Animator, int, int)"/>
        public void SetInt(int _parameterHash, int _value) {
            controller.SetInt(animator, _parameterHash, _value);
        }

        // -------------------------------------------
        // Float
        // -------------------------------------------

        /// <inheritdoc cref="EnhancedAnimatorController.GetFloat(Animator, string)"/>
        public float GetFloat(string _parameterName) {
            return controller.GetFloat(animator, _parameterName);
        }

        /// <inheritdoc cref="EnhancedAnimatorController.GetFloat(Animator, int)"/>
        public float GetFloat(int _parameterHash) {
            return controller.GetFloat(animator, _parameterHash);
        }

        /// <inheritdoc cref="EnhancedAnimatorController.SetFloat(Animator, string, float)"/>
        public void SetFloat(string _parameterName, float _value) {
            controller.SetFloat(animator, _parameterName, _value);
        }

        /// <inheritdoc cref="EnhancedAnimatorController.SetFloat(Animator, int, float)"/>
        public void SetFloat(int _parameterHash, float _value) {
            controller.SetFloat(animator, _parameterHash, _value);
        }

        // -------------------------------------------
        // Trigger
        // -------------------------------------------

        /// <inheritdoc cref="EnhancedAnimatorController.SetTrigger(Animator, string)"/>
        public void SetTrigger(string _parameterName) {
            controller.SetTrigger(animator, _parameterName);
        }

        /// <inheritdoc cref="EnhancedAnimatorController.SetTrigger(Animator, int)"/>
        public void SetTrigger(int _parameterHash) {
            controller.SetTrigger(animator, _parameterHash);
        }

        /// <inheritdoc cref="EnhancedAnimatorController.ResetTrigger(Animator, string)"/>
        public void ResetTrigger(string _parameterName) {
            controller.ResetTrigger(animator, _parameterName);
        }

        /// <inheritdoc cref="EnhancedAnimatorController.ResetTrigger(Animator, int)"/>
        public void ResetTrigger(int _parameterHash) {
            controller.ResetTrigger(animator, _parameterHash);
        }
        #endregion
    }
}
