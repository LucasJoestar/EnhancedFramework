// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

using Range = EnhancedEditor.RangeAttribute;

namespace EnhancedFramework.Core {
    #region Forward Constraint
    /// <summary>
    /// Constraints for a forward movement.
    /// </summary>
    public enum ForwardConstraint {
        None     = 0,
        Forward  = 1,
        Backward = 2,
    }
    #endregion

    /// <summary>
    /// <see cref="Animator"/>-related state configuration asset.
    /// </summary>
    [CreateAssetMenu(fileName = "ANIS_AnimatorState", menuName = FrameworkUtility.MenuPath + "Animation/Animator State", order = FrameworkUtility.MenuOrder)]
    public class EnhancedAnimatorState : EnhancedScriptableObject {
        #region Global Members
        [Section("Animator State")]

        [Tooltip("Identifier name of this state")]
        [SerializeField, Enhanced, DisplayName("Name")] private string stateName = "State";

        [Tooltip("Animation to play on this state")]
        [SerializeField] private AnimationClip animation = null;
         
        [Space(5f)]

        [Tooltip("Speed coefficient of this animation")]
        [SerializeField, Enhanced, Range(-5f, 5f)] private float speed = 1f;

        [Tooltip("Animation cycle offset (in normalized time)")]
        [SerializeField, Enhanced, Range(-10f, 10f)] private float cycleOffset = 0f;

        [Space(5f)]

        [Tooltip("If true, plays this animation in mirror")]
        [SerializeField] private bool mirror = false;

        [Tooltip("If true, enables foot IK")]
        [SerializeField] private bool footIK = false;

        [Tooltip("If true, write animation default values on animated keys")]
        [SerializeField] private bool writeDefaults = true;

        [Space(10f), HorizontalLine(SuperColor.Grey, 1f), Space(10f)]

        [Tooltip("If true, allows the transit to this state while already playing")]
        [SerializeField] private bool canTransitToSelf = false;

        [Space(5f)]

        [Tooltip("All configured transitions in this state")]
        [SerializeField] private EnhancedCollection<EnhancedAnimatorStateTransition> transitions = new EnhancedCollection<EnhancedAnimatorStateTransition>();

        [Space(10f), HorizontalLine(SuperColor.Grey, 1f), Space(10f)]

        [Tooltip("Toggles this state root motion")]
        [SerializeField] private bool rootMotion = false;

        [Space(5f)]

        [Tooltip("Root motion forward movement constraint")]
        [SerializeField, Enhanced, ShowIf("rootMotion")] private ForwardConstraint motionForwardContraint = ForwardConstraint.None;

        [Tooltip("Root motion influence on the object position")]
        [SerializeField, Enhanced, ShowIf("rootMotion")] private AxisConstraints motionPositionConstraints = AxisConstraints.X | AxisConstraints.Z;

        [Tooltip("Root motion influence on the object rotation")]
        [SerializeField, Enhanced, ShowIf("rootMotion")] private AxisConstraints motionRotationConstraints = AxisConstraints.None;

        [Space(10f), HorizontalLine(SuperColor.Grey, 1f), Space(10f)]

        [Space(5f, order = 2), Title("Default Transition", order = 3), Space(5f, order = 4)]

        [SerializeField, Enhanced, Block] private EnhancedAnimatorTransition defaultTransition = new EnhancedAnimatorTransition();

        [Space(15f, order = 0), Title("Exit Transition", order = 1), Space(5f, order = 2)]

        [Tooltip("Mode used for this state default exit transition")]
        [SerializeField, Enhanced, DisplayName("Mode")] private ExitTransitionMode exitTransitionMode = ExitTransitionMode.Default;

        // -----------------------

        [Tooltip("Normalized time at which the exit transition take effect")]
        [SerializeField, Enhanced, ShowIf("HasExitTransition"), Range(0f, 10f)] private float exitTime = .9f;

        [Space(5f)]

        [Tooltip("Default exit transition duration")]
        [SerializeField, Enhanced, ShowIf("UseDefaultExitWithDuration"), Range(0f, 10f)] private float exitTransitionDuration = .5f;

        [Tooltip("Default exit transition destination state")]
        [SerializeField, Enhanced, ShowIf("UseCustomExit"), DisplayName("State")] private EnhancedAnimatorState exitState = null;

        [Space(10f)]

        [SerializeField, Enhanced, ShowIf("UseCustomExit"), Block] private EnhancedAnimatorTransitionSettings exitTransition = new EnhancedAnimatorTransitionSettings();

        // -----------------------

        private int hash = 0;
        private int layerIndex = 0;

        // -----------------------

        /// <summary>
        /// Simple name hash of this state.
        /// </summary>
        public int Hash {
            get {
                #if DEVELOPMENT
                if (hash == 0) {

                    hash = Animator.StringToHash(stateName);
                    this.LogWarning("State hash value was not correctly configured");
                }
                #endif

                return hash;
            }
        }

        /// <summary>
        /// The <see cref="AnimationClip"/> of this state.
        /// </summary>
        public AnimationClip Animation {
            get { return animation; }
        }

        /// <summary>
        /// Whether tho allow transitions to this state while already playing.
        /// </summary>
        public bool CanTransitToSelf {
            get { return canTransitToSelf; }
        }

        /// <summary>
        /// Whether root motion is active on this layer or not.
        /// </summary>
        public bool RootMotion {
            get { return rootMotion; }
        }

        /// <summary>
        /// Indicates if this state has a configured exit transition.
        /// </summary>
        public bool HasExitTransition {
            get { return exitTransitionMode != ExitTransitionMode.None; }
        }

        /// <summary>
        /// Indicates if this state exit transition is defined to <see cref="ExitTransitionMode.DefaultWithDuration"/>.
        /// </summary>
        public bool UseDefaultExitWithDuration {
            get { return exitTransitionMode == ExitTransitionMode.DefaultWithDuration; }
        }

        /// <summary>
        /// Indicates if this state exit transition is defined to <see cref="ExitTransitionMode.Custom"/>.
        /// </summary>
        public bool UseCustomExit {
            get { return exitTransitionMode == ExitTransitionMode.Custom; }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes this state.
        /// </summary>
        /// <param name="_layerIndex">Index of this state layer.</param>
        public void Initialize(int _layerIndex) {

            hash = Animator.StringToHash(stateName);
            layerIndex = _layerIndex;
        }
        #endregion

        #region Animation
        /// <inheritdoc cref="Play(Animator, int)"/>
        public void Play(Animator _animator) {
            Play(_animator, layerIndex);
        }

        /// <summary>
        /// Plays a transition from the current <see cref="Animator"/> state to this state.
        /// </summary>
        /// <param name="_animator"><see cref="Animator"/> on which to play the transition.</param>
        /// <param name="_layerIndex">Index of this state layer.</param>
        public void Play(Animator _animator, int _layerIndex) {

            int _currentHash = _animator.GetCurrentAnimatorStateInfo(_layerIndex).shortNameHash;
            int _nextHash    = _animator.GetNextAnimatorStateInfo(_layerIndex).shortNameHash;

            // Cancel transition if state is already playing, and not in transition / transitioning to self.
            if ((_currentHash == Hash) && ((_nextHash == 0) || (_nextHash == Hash))) {
                return;
            }

            EnhancedAnimatorTransition _transition = GetTransition();
            _transition.Play(_animator, this, _layerIndex);

            // ----- Local Method ----- \\

            EnhancedAnimatorTransition GetTransition() {

                foreach (EnhancedAnimatorStateTransition _state in transitions) {

                    if (_state.Contains(_currentHash, out EnhancedAnimatorTransition _transition) || _state.Contains(_nextHash, out _transition)) {
                        return _transition;
                    }
                }
                
                return defaultTransition;
            }
        }
        #endregion

        #region Motion
        /// <summary>
        /// Applies root motion on this state.
        /// </summary>
        /// <param name="_next"><see cref="Animator"/> next state.</param>
        /// <param name="_position">Root motion position velocity.</param>
        /// <param name="_rotation">Root motion rotation velocity.</param>
        public void ApplyMotion(EnhancedAnimatorState _next, ref Vector3 _position, ref Quaternion _rotation) {
            // Position.
            if (!HasPositionContraint(AxisConstraints.X)) {
                _position.x = 0f;
            }

            if (!HasPositionContraint(AxisConstraints.Y)) {
                _position.y = 0f;
            }

            if (!HasPositionContraint(AxisConstraints.Z)) {
                _position.z = 0f;
            }

            // Forward.
            _position = ApplyForwardConstraint(_position, motionForwardContraint);
            _position = ApplyForwardConstraint(_position, _next.motionForwardContraint);

            // Rotation.
            Vector3 _eulers = _rotation.eulerAngles;

            if (!HasRotationContraint(AxisConstraints.X)) {
                _eulers.x = 0f;
            }

            if (!HasRotationContraint(AxisConstraints.Y)) {
                _eulers.y = 0f;
            }

            if (!HasRotationContraint(AxisConstraints.Z)) {
                _eulers.z = 0f;
            }

            _rotation = Quaternion.Euler(_eulers);

            // ----- Local Method ----- \\

            Vector3 ApplyForwardConstraint(Vector3 _position, ForwardConstraint _constraint) {

                switch (_constraint) {

                    case ForwardConstraint.Forward:
                        _position.z = Mathf.Max(_position.z, 0f);
                        break;

                    case ForwardConstraint.Backward:
                        _position.z = Mathf.Min(_position.z, 0f);
                        break;

                    case ForwardConstraint.None:
                    default:
                        break;
                }

                return _position;
            }

            bool HasPositionContraint(AxisConstraints _constraint) {
                return motionPositionConstraints.HasFlag(_constraint) || _next.motionPositionConstraints.HasFlag(_constraint);
            }

            bool HasRotationContraint(AxisConstraints _constraint) {
                return motionRotationConstraints.HasFlag(_constraint) || _next.motionRotationConstraints.HasFlag(_constraint);
            }
        }
        #endregion

        #region Utility
#if UNITY_EDITOR
        /// <summary>
        /// Creates and setups this state in an <see cref="AnimatorController"/>, on a specific layer.
        /// </summary>
        /// <param name="_animator"><see cref="AnimatorController"/> on which to create this state.</param>
        /// <param name="_layerIndex">Index of the layer on which to create this state.</param>
        internal void Create(AnimatorController _animator, int _layerIndex, EnhancedCollection<AnimatorState> _states) {

            AnimationClip _animation = animation;

            if (!_states.Find(s => s.name == stateName, out AnimatorState _state)) {

                // An animation is required to create the state (otherwise, an error is thrown).
                if (_animation == null) {
                    _animation = AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets($"t:{typeof(AnimationClip).Name}")[0]));
                }

                _state = _animator.AddMotion(_animation, _layerIndex);
                _states.Add(_state);
            }

            _state.writeDefaultValues   = writeDefaults;
            _state.cycleOffset          = cycleOffset;
            _state.iKOnFeet             = footIK;
            _state.mirror               = mirror;
            _state.speed                = speed;
            _state.name                 = stateName;
            _state.motion               = _animation;

            if (animation == null) {
                _state.motion = null;
            }

            EditorUtility.SetDirty(_state);
        }

        /// <summary>
        /// Creates and setups this state exit transition.
        /// </summary>
        /// <param name="_states">All states in the current layer.</param>
        /// <param name="_defaultState">Default state of the current layer.</param>
        /// <param name="_defaultTransition">Default transition used to exit the the current layer default state.</param>
        internal void CreateExitTransition(EnhancedCollection<AnimatorState> _states, EnhancedAnimatorState _defaultState,
                                           EnhancedAnimatorTransitionSettings _defaultTransition) {

            switch (exitTransitionMode) {

                // Default.
                case ExitTransitionMode.Default:
                    CreateTransition(_defaultState, _defaultTransition);
                    break;

                // Override duration.
                case ExitTransitionMode.DefaultWithDuration:

                    _defaultTransition.Duration = exitTransitionDuration;
                    CreateTransition(_defaultState, _defaultTransition);

                    break;

                // Custom.
                case ExitTransitionMode.Custom:
                    CreateTransition(exitState, exitTransition);
                    break;

                case ExitTransitionMode.None:
                default:
                    break;
            }

            // ----- Local Method ----- \\

            void CreateTransition(EnhancedAnimatorState _exitState, EnhancedAnimatorTransitionSettings _settings) {

                if (!_states.Find(s => s.name == stateName, out AnimatorState _state) || !_states.Find(s => s.name == _exitState.stateName, out AnimatorState _transitionState)) {
                    return;
                }

                AnimatorStateTransition[] _allTransitions = _state.transitions;
                if (!_allTransitions.Find(t => t.hasExitTime && (t.destinationState == _transitionState), out AnimatorStateTransition _transition)) {

                    // Create if don't exist.
                    // Overload taking a Transition does not saved it into the asset, so we don't use it.
                    _state.AddTransition(_transitionState, true);
                    _allTransitions = _state.transitions;

                    // Retrieve state.
                    if (!_allTransitions.Find(t => t.hasExitTime && (t.destinationState == _transitionState), out _transition)) {
                        return;
                    }
                }

                _transition.interruptionSource = TransitionInterruptionSource.None;
                _transition.orderedInterruption = false;
                _transition.canTransitionToSelf = false;
                _transition.isExit = false;
                _transition.mute = false;
                _transition.solo = false;
                _transition.hasExitTime = true;

                _transition.destinationState = _transitionState;
                _transition.exitTime = exitTime;

                _transition.duration = _settings.Duration;
                _transition.offset = _settings.Offset;
                _transition.hasFixedDuration = _settings.IsFixedDuration;

                EditorUtility.SetDirty(_transition);
                EditorUtility.SetDirty(_state);
            }
        }
        #endif
        #endregion
    }
}
