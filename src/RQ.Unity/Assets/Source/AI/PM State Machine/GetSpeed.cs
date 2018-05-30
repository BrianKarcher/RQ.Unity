//using PM = HutongGames.PlayMaker;
//using HutongGames.PlayMaker;
//using RQ.Animation.BasicAction.Action;
//using RQ.Entity.Components;
//using RQ.Physics.Components;

//namespace Assets.Source.AI.PM_State_Machine
//{
//    [ActionCategory("RQ")]
//    [PM.Tooltip("Gets the 2d Speed of a Game Object and stores it in a Float Variable.")]
//    public class GetSpeed : FsmStateAction
//    {
//        [RequiredField]
//        [UIHint(UIHint.Variable)]
//        [PM.Tooltip("The speed, or in technical terms: velocity magnitude")]
//        public FsmFloat storeResult;

//        [PM.Tooltip("Repeat every frame.")]
//        public bool everyFrame;

//        private GetSpeedAtom _getSpeedAtom;
//        private IComponentRepository _entity;

//        public override void Reset()
//        {
//            //gameObject = null;
//            storeResult = null;
//            everyFrame = false;
//        }

//        public override void OnEnter()
//        {
//            _getSpeedAtom = new GetSpeedAtom();
//            var rqSM = Owner.GetComponent<PlayMakerStateMachineComponent>();
//            _entity = rqSM.GetComponentRepository();
//            _getSpeedAtom.Start(_entity);
//            DoGetSpeed();

//            if (!everyFrame)
//            {
//                Finish();
//            }
//        }

//        public override void OnUpdate()
//        {
//            DoGetSpeed();
//        }

//        void DoGetSpeed()
//        {
//            if (storeResult.IsNone) return;
//            _getSpeedAtom.OnUpdate();
//            storeResult.Value = _getSpeedAtom.Speed;
//        }
//    }
//}
