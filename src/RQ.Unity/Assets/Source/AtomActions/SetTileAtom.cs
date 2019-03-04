using System;
using RQ.Entity.AtomAction;
using RQ.Entity.Components;
using RQ2.UI;
using UnityEngine;

namespace RQ.AI.Action
{
    public class SetTileAtom : AtomActionBase
    {
        public int[] tileToChange;
        public int changeTo;
        //[SerializeField]
        //[UniqueIdentifier]
        //public string _uniqueId;
        private TilemapSetup _tilemapSetup;
        private tk2dTileMap tileMap;

        public override void Start(IComponentRepository entity)
        {
            base.Start(entity);
            if (_tilemapSetup == null)
            {
                _tilemapSetup = GameObject.FindObjectOfType<TilemapSetup>();
            }
            tileMap = _tilemapSetup.TileMap;

            //}
            
            //_physicsComponent = entity.Components.GetComponent<PhysicsComponent>();
            //_physicsComponent.Stop();
        }

        public bool GetTileAtPosition(Vector2 pos, out int x, out int y)
        {
            if (tileMap.GetTileAtPosition(pos, out x, out y))
            {
                //x = tileX;
                //y = tileY;
                return true;
            }
            else
            {
                //x = 0;
                //y = 0;
                return false;
            }
        }

        public void SetTile(int x, int y)
        {
            //var pos = entity.transform.position;
            //int tileX, tileY;
            //if (tileMap.GetTileAtPosition(pos, out tileX, out tileY))
            //{
            for (int i = 0; i < tileMap.Layers.Length; i++)
            {
                var currentTile = tileMap.GetTile(x, y, i);

                var hasTileToChange = Array.IndexOf(tileToChange, currentTile) > -1;
                if (!hasTileToChange)
                    continue;
                tileMap.SetTile(x, y, i, changeTo);
            }
        }

        public void Build()
        {
            tileMap.Build();
        }

        public override void End()
        {
            base.End();
        }

        public override AtomActionResults OnUpdate()
        {
            return AtomActionResults.Success;
        }
    }
}
