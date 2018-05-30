//using RQ.Animation;
//using RQ.Entity.Common;
//using RQ.Entity.UI;
//namespace RQ
//{
//    /// <summary>
//    /// A custom event that will load a level that is already setup in your build settings. 
//    /// </summary>
//    [WellFired.USequencerFriendlyName("Play Animation")]
//    [WellFired.USequencerEvent("RQ/Sprite/PlayAnimation")]
//    [WellFired.USequencerEventHideDuration()]
//    public class PlayAnimation : WellFired.USEventBase 
//    {
//        private AnimationComponent _sprite2;

//        //public virtual void Start()
//        //{
            
//        //}
//        //public bool fireInEditor = false;
		
//        public string Animation;
		
//        public override void FireEvent()
//        {
//            var _sprite2 = base.AffectedObject.GetComponent<AnimationComponent>();
//            //if (uiBase != null)
//            //    _sprite2 = (IBaseGameEntity) uiBase.GetEntity();
//            //_gameManagerScript = gameManager.GetComponent<GameManager>();
//            //_gameManagerScript.StartDialog (DialogName);
//            _sprite2.GetSpriteAnimator().Render(Animation);
//        }

//        public override void ProcessEvent(float deltaTime)
//        {

//        }
//    }
//}