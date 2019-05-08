using UnityEngine;
using WellFired;
using RQ.Model;
using RQ.Entity.Components;
using RQ.Messaging;
using RQ.Controller.SequencerEvents;

namespace RQ.SequencerEvents
{
    [WellFired.USequencerFriendlyName("Play Actions")]
    [WellFired.USequencerEvent("RQ/Play Actions")]
    [WellFired.USequencerEventHideDuration()]
    public class PlayActionsEvent : USEventBase, IPlayActionsEvent
    {
        private IComponentRepository _entity;
        [SerializeField]
        private bool _runOnGameController = false;
        //public void Awake()
        //{
        //    Log.Info("StartConversation Awaked");
        //}
        //public bool fireInEditor = false;

        //public string DialogName;
        //public Transform gameManager;
        //private GameManager _gameManagerScript;

        public void Awake()
        {
            
            //var actions = GetComponents<IAction>();
            //if (actions == null)
            //    return;
            //foreach (var action in actions)
            //{
            //    action.SetComponentRepository(_entity);
            //    action.Init();
            //}
        }

        public override void FireEvent()
        {
            if (_runOnGameController)
            {
                MessageDispatcher.Instance.DispatchMsg(0f, string.Empty, "Game Controller",
                    Enums.Telegrams.GetObject, null, (gm) => _entity = (IComponentRepository)gm);
            }
            else
            {
                _entity = AffectedObject.GetComponent<IComponentRepository>();
            }
                
            //Log.Info("StartConversation event fired in " + this.name + ".");
            
            var actions = GetComponents<IAction>();
            // @todo implement this with the new dialog system
            //_gameManagerScript = gameManager.GetComponent<GameManager>();
            //_gameManagerScript.StartDialog (DialogName);
            //trigger.TryStartConversation(trigger.actor);
            if (actions == null)
                return;
            foreach (var action in actions)
            {
                if (!action.isActiveAndEnabled)
                    continue;
                //GameController.Instance;
                action.SetComponentRepository(_entity);
                action.InitAction();
                action.Act(null);
            }
        }

        public override void ProcessEvent(float deltaTime)
        {

        }
    }
}