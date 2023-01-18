// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedFramework.Core;
using HutongGames.PlayMaker;
using UnityEngine;

using Tooltip = HutongGames.PlayMaker.TooltipAttribute;

namespace EnhancedFramework.PlayMaker {
    /// <summary>
    /// <see cref="FsmStateAction"/> used to compare the tags of a <see cref="UnityEngine.GameObject"/> and a <see cref="MultiTagsBehaviour"/>.
    /// </summary>
    [Tooltip("Compare the tags of a Game Object with a Multi Tags Behaviour Variable.")]
    [ActionCategory(ActionCategory.Logic)]
    public class GameObjectCompareMultiTags : FsmStateAction {
        #region Global Members
        // --------------------------------------------------------
        // Variable - Compare - True - False - Store - Every Frame
        // --------------------------------------------------------

        [Tooltip("The Game Object to compare the Tags.")]
        [RequiredField]
        public FsmOwnerDefault GameObject = null;

        [Tooltip("Tags to check for.")]
        [RequiredField, ObjectType(typeof(MultiTagsBehaviour))]
        public FsmObject MultiTags = null;

        [Tooltip("Event to send if the GameObject has all Tags.")]
        public FsmEvent TrueEvent;

        [Tooltip("Event to send if the GameObject does not have all Tags.")]
        public FsmEvent FalseEvent;

        [Tooltip("Store the result in a Bool variable.")]
        [UIHint(UIHint.Variable)]
        public FsmBool StoreResult;

        [Tooltip("Repeat every frame.")]
        public bool EveryFrame;
        #endregion

        #region Behaviour
        public override void Reset() {
            base.Reset();

            GameObject = null;
            MultiTags = null;
            TrueEvent = null;
            FalseEvent = null;
            StoreResult = null;
            EveryFrame = false;
        }

        public override void OnEnter() {
            base.OnEnter();

            CompareTags();

            if (!EveryFrame) {
                Finish();
            }
        }

        public override void OnUpdate() {
            base.OnUpdate();

            CompareTags();
        }

        // -----------------------

        private void CompareTags() {
            GameObject _gameObject = Fsm.GetOwnerDefaultTarget(GameObject);
            bool _hasTags = false;

            if (_gameObject.IsValid() && (MultiTags.Value is MultiTagsBehaviour _behaviour)) {
                _hasTags = _gameObject.GetTags().ContainsAll(_behaviour.Tags);
            }

            StoreResult.Value = _hasTags;
            Fsm.Event(_hasTags ? TrueEvent : FalseEvent);
        }
        #endregion
    }
}
