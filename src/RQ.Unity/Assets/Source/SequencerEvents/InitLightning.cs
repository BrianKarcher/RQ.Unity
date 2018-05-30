using UnityEngine;
using System.Collections;
using RQ.AI;
using RQ.Entity.Common;
using RQ.Entity.UI;
using RQ.Model;
using RQ.Messaging;
namespace RQ
{
    /// <summary>
    /// A custom event that will load a level that is already setup in your build settings. 
    /// </summary>
    [WellFired.USequencerFriendlyName("Init Lightning")]
    [WellFired.USequencerEvent("RQ/HeavensJudgement/InitLightning")]
    [WellFired.USequencerEventHideDuration()]
    public class InitLightning : WellFired.USEventBase
    {
        //private IBaseGameEntity _sprite2;
        //public EntityUIBase Sprite;

        public Transform Prefab;

        public virtual void Start()
        {
            //if (Sprite == null)
            //{
            //    Sprite = base.AffectedObject.GetComponent<EntityUIBase>();
            //}
            //if (Sprite != null)
            //{
            //    _sprite2 = Sprite.GetEntity() as IBaseGameEntity;
            //}
        }
        //public bool fireInEditor = false;

        //public string Animation;


        public override void FireEvent()
        {
            //if (_sprite2 != null)
            //{
            //GameManager gameManager = _sprite2.gameManager;
            //ICameraClass cameraClass = GameController._instance.GetCamera();
            ICameraClass cameraClass = null;
            MessageDispatcher.Instance.DispatchMsg(0f, string.Empty, "Game Controller",
                Enums.Telegrams.GetCamera, null, i => cameraClass = i as ICameraClass);
            Rect viewport = cameraClass.GetViewport();

            // Trim the area that lightning strikes can hit
            //RectOffset offset = new RectOffset(.20f, .20f, -.20f, -.20f);
            //RectOffset j = new RectOffset(
            //Rect trimmedViewport = offset.Add(viewport);

            float xRand = Random.Range(viewport.x, viewport.x + viewport.width);
            float yRand = Random.Range(viewport.y, viewport.y + viewport.height);

            //_gameManagerScript = gameManager.GetComponent<GameManager>();
            //_gameManagerScript.StartDialog (DialogName);
            //_sprite2.SetAnimation(Animation);
            MessageDispatcher2.Instance.DispatchMsg("CreateLightningEffect", 0f, string.Empty, "Game Controller", null);
            Instantiate(Prefab, new Vector3(xRand, yRand, -0.8f), this.transform.rotation);
            //}
        }

        public override void ProcessEvent(float deltaTime)
        {

        }
    }
}