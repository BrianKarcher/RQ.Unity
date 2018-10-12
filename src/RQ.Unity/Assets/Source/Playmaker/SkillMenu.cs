using PM = HutongGames.PlayMaker;
using HutongGames.PlayMaker;
using RQ.Animation.BasicAction.Action;
using RQ.Entity.Components;
using RQ.Physics.Components;
using RQ.AI.Action;
using UnityEngine;

namespace Assets.Source.AI.PM_State_Machine
{
    [ActionCategory("RQ.UI")]
    [PM.Tooltip("Skill Menu.")]
    public class SkillMenu : FsmStateAction
    {
        //[RequiredField]
        //[UIHint(UIHint.Variable)]
        //[PM.Tooltip("The speed, or in technical terms: velocity magnitude")]
        //public FsmFloat storeResult;

        //[PM.Tooltip("Repeat every frame.")]
        //public bool everyFrame;

        [UIHint(UIHint.Variable)]
        [PM.Tooltip("Fire when skill is selected.")]
        public FsmEvent IsComplete;

        public SkillMenuAtom _atom;
        private IComponentRepository _entity;
        private PhysicsComponent _physicsComponent;

        //public override void Reset()
        //{
        //    //gameObject = null;
        //    storeResult = null;
        //    everyFrame = false;
        //}

        public override void OnEnter()
        {
            var rqSM = Owner.GetComponent<PlayMakerStateMachineComponent>();
            _entity = rqSM.GetComponentRepository();
            _atom.Start(_entity);
        }

        public override void OnExit()
        {
            base.OnExit();
            _atom.End();
        }

        public override void OnUpdate()
        {
            Tick();
        }

        private void Tick()
        {
            //if (storeResult.IsNone) return;
            //_getSpeedAtom.OnUpdate();
            //storeResult.Value = _getSpeedAtom.Speed;
            var result = _atom.OnUpdate();
            if (result == RQ.AI.AtomActionResults.Success)
            {
                Finish();
                if (IsComplete != null)
                    Fsm.Event(IsComplete);
            }
        }
    }
}
