using BehaviorDesigner.Runtime;
using RQ.Animation;
using RQ.Common.Components;
using RQ.Entity.Data;
using RQ.FSM.V2;
using RQ2.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RQ.Physics.Components
{
    [AddComponentMenu("RQ/Components/Hiding")]
    public class HidingComponent : ComponentPersistence<HidingComponent>
    {
        //public bool IsHiding;
        public List<int> HideTileIds;
        //private EntityStatsData _entityStatsData;
        public tk2dTileMap _tileMap;
        private AnimationComponent _animComponent;
        private EntityStatsComponent _entityStatsComponent;

        //private Behavior _behaviorTree;

        //private WaypointData _waypointData;

        //public override void Awake()
        //{
        //    //_behaviorTree = GetComponent<Behavior>();
        //}

        public override void Start()
        {
            base.Start();
            if (!Application.isPlaying)
                return;
            _entityStatsComponent = GetComponentRepository().Components.GetComponent<EntityStatsComponent>();
            _animComponent = GetComponentRepository().Components.GetComponent<AnimationComponent>();
            var tileMapSetup = GameObject.FindObjectOfType<TilemapSetup>();
            if (tileMapSetup == null)
                return;
            _tileMap = tileMapSetup.TileMap;
        }

        public override void OnEnable()
        {
            base.OnEnable();
            if (!Application.isPlaying)
                return;
            StartCoroutine(MyFixedUpdate());
        }

        public override void OnDisable()
        {
            base.OnDisable();
            if (!Application.isPlaying)
                return;
            StopCoroutine(MyFixedUpdate());
        }

        public IEnumerator MyFixedUpdate()
        {
            while (true)
            {
                if (_entityStatsComponent == null)
                    yield return null;
                var _entityStatsData = _entityStatsComponent.GetEntityStats();
                if (_entityStatsData == null)
                    yield return null;
                //base.FixedUpdate();
                if (_tileMap == null || _tileMap.data == null)
                    yield break;

                int numLayers = _tileMap.data.NumLayers;
                var pos = GetComponentRepository().transform.position;
                bool hideTileFound = false;
                for (int layerId = 0; layerId < numLayers; ++layerId)
                {
                    //var layer = _tileMap.Layers[layerId];
                    //var layerData = _tileMap.data.tileMapLayers[layerId];
                    var tileId = _tileMap.GetTileIdAtPosition(pos, layerId);
                    if (HideTileIds.Contains(tileId))
                    {
                        hideTileFound = true;
                        break;
                    }
                }
                if (_entityStatsData.IsHiding != hideTileFound)
                    Debug.LogWarning("Changing hiding status");
                _entityStatsData.IsHiding = hideTileFound;
                //Color color;
                //color = hideTileFound ? new Color(0f, 1f, 0f) : Color.white;
                //_animComponent.GetSpriteRenderer().SetColor(color);
                yield return new WaitForSeconds(.04f); // 25 FPS
            }
        }

        //public void EnableBT()
        //{
        //    gameObject.SetActive(true);
        //    _behaviorTree.EnableBehavior();

        //    //BehaviorManager.instance.EnableBehavior(_behaviorTree);
        //    //_behaviorTree.Initialize()
        //}

        //public void DisableBT()
        //{
        //    gameObject.SetActive(false);
        //    _behaviorTree.DisableBehavior();
        //}
        //    base.Awake();
        //    if (!Application.isPlaying)
        //        return;

        //    _waypointData = new WaypointData();
        //    _waypointData.Waypoints = new Dictionary<string, Serialization.Vector3Serializer>();
        //    var waypoints = GetComponentsInChildren<Transform>();
        //    foreach (var waypoint in waypoints)
        //    {
        //        _waypointData.Waypoints.Add(waypoint.name, waypoint.position);
        //    }
        //}

        //public override void Serialize(Serialization.EntitySerializedData entitySerializedData)
        //{
        //    base.Serialize(entitySerializedData);
        //    base.SerializeComponent(entitySerializedData, _waypointData);
        //}

        //public override void Deserialize(Serialization.EntitySerializedData entitySerializedData)
        //{
        //    base.Deserialize(entitySerializedData);
        //    _waypointData = base.DeserializeComponent<WaypointData>(entitySerializedData);

        //    CreateWaypoints();
        //}

        //private void CreateWaypoints()
        //{
        //    var currentWaypoints = GetComponentsInChildren<Transform>();
        //    foreach (var waypoint in _waypointData.Waypoints)
        //    {
        //        if (currentWaypoints.Any(i => i.name == waypoint.Key))
        //            continue;
        //        var go = new GameObject(waypoint.Key);
        //        go.transform.position = waypoint.Value;
        //        go.transform.parent = transform;
        //    }
        //}
    }
}
