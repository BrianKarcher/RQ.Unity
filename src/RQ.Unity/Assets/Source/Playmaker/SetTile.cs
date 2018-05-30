using PM = HutongGames.PlayMaker;
using HutongGames.PlayMaker;
using RQ.Animation.BasicAction.Action;
using RQ.Entity.Components;
using RQ.Physics.Components;
using RQ.AI.Action;
using UnityEngine;

namespace Assets.Source.AI.PM_State_Machine
{
    [ActionCategory("RQ.Tilemap")]
    [PM.Tooltip("Sets the tile.")]
    public class SetTile : FsmStateAction
    {
        //[RequiredField]
        //[UIHint(UIHint.Variable)]
        //[PM.Tooltip("The speed, or in technical terms: velocity magnitude")]
        //public FsmFloat storeResult;

        //[PM.Tooltip("Repeat every frame.")]
        //public bool everyFrame;

        public SetTileAtom _atom;
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
            _physicsComponent = _entity.Components.GetComponent<PhysicsComponent>();
            _atom.Start(_entity);
            Vector2 startPosition;
            Vector2 leftPosition;
            Vector2 rightPosition;
            int x;
            int y;
            startPosition = (Vector2)_entity.transform.position;
            if (!_atom.GetTileAtPosition(startPosition, out x, out y))
            {
                Finish();
                return;
            }
            // Rolan is taller than he is wide, so extend out further up and down and less so left and right.
            if (Mathf.Abs(_physicsComponent.GetPhysicsData().Heading.x) > .5)
            {
                _atom.SetTile(x + (_physicsComponent.GetPhysicsData().Heading.x > 0 ? 1 : -1), y);
                _atom.SetTile(x + (_physicsComponent.GetPhysicsData().Heading.x > 0 ? 1 : -1), y - 1);
                _atom.SetTile(x + (_physicsComponent.GetPhysicsData().Heading.x > 0 ? 1 : -1), y + 1);
                _atom.SetTile(x + (_physicsComponent.GetPhysicsData().Heading.x > 0 ? 2 : -2), y);
                _atom.SetTile(x + (_physicsComponent.GetPhysicsData().Heading.x > 0 ? 2 : -2), y - 1);
                _atom.SetTile(x + (_physicsComponent.GetPhysicsData().Heading.x > 0 ? 2 : -2), y + 1);
            }
                //startPosition = (Vector2)_entity.transform.position + (Vector2)_physicsComponent.GetPhysicsData().Heading * 0.16f;
            else
            {
                _atom.SetTile(x, y + (_physicsComponent.GetPhysicsData().Heading.y > 0 ? 1 : -1) * 2);
                _atom.SetTile(x - 1, y + (_physicsComponent.GetPhysicsData().Heading.y > 0 ? 1 : -1) * 2);
                _atom.SetTile(x + 1, y + (_physicsComponent.GetPhysicsData().Heading.y > 0 ? 1 : -1) * 2);
                _atom.SetTile(x, y + (_physicsComponent.GetPhysicsData().Heading.y > 0 ? 1 : -1));
                _atom.SetTile(x - 1, y + (_physicsComponent.GetPhysicsData().Heading.y > 0 ? 1 : -1));
                _atom.SetTile(x + 1, y + (_physicsComponent.GetPhysicsData().Heading.y > 0 ? 1 : -1));
            }
                //startPosition = (Vector2)_entity.transform.position + (Vector2)_physicsComponent.GetPhysicsData().Heading * 0.32f;
            //leftPosition = startPosition + (Vector2)_physicsComponent.GetPhysicsData().Side * 0.16f;
            //rightPosition = startPosition + (Vector2)_physicsComponent.GetPhysicsData().Side * -0.16f;

            //if (_atom.GetTileAtPosition(startPosition, out x, out y))
            //{
            //    _atom.SetTile(x, y);
            //}
            //if (_atom.GetTileAtPosition(leftPosition, out x, out y))
            //{
            //    _atom.SetTile(x, y);
            //}
            //if (_atom.GetTileAtPosition(rightPosition, out x, out y))
            //{
            //    _atom.SetTile(x, y);
            //}
            _atom.Build();
            Finish();
            //DoGetSpeed();

            //if (!everyFrame)
            //{
            //    Finish();
            //}
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
        }
    }
}
