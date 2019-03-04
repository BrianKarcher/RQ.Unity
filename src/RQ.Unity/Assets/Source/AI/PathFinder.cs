using Pathfinding;
using RQ.FSM.Components;
using RQ.Model.ObjectPool;
using RQ.Model.Physics;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.AI
{
    [AddComponentMenu("RQ/AI/Path Finder")]
    public class PathFinder : MonoBehaviour, IPathFinder
    {
        [SerializeField]
        private Seeker _seeker;
        public Seeker Seeker { get { return _seeker; } set { _seeker = value; } }

        //private List<Vector3> _path = new List<Vector3>();

        public event Action<PathfindingData> OnPathComplete;

        public void Awake()
        {
            if (Seeker == null)
                Seeker = GetComponent<Seeker>();
            //_seeker.
            Seeker.pathCallback += ProcessPath;
        }

        public void OnDestroy()
        {
            if (Seeker != null)
                Seeker.pathCallback -= ProcessPath;
        }

        public void StartPathProcessing(Vector3 pos, Vector3 target, int graphMask)
        {
            _seeker.StartPath(pos, target);
            //_seeker.StartPath(pos, target, null, graphMask);
        }

        private void ProcessPath(Path _p)
        {
            ABPath p = _p as ABPath;
            if (p == null) throw new System.Exception("This function only handles ABPaths, do not use special path types");

            //canSearchAgain = true;

            //Claim the new path
            //p.Claim(this);

            // Path couldn't be calculated of some reason.
            // More info in p.errorLog (debug string)
            if (p.error)
            {
                Debug.LogError(p.errorLog);
                //p.Release(this);
                return;
            }

            //Release the previous path
            //if (path != null) path.Release(this);
            var path = ObjectPool.Instance.PullFromPool<List<Vector3>>(ObjectPoolType.Vector3List);
            path.Clear();
            for (int i = 0; i < p.vectorPath.Count; i++)
            {
                path.Add(p.vectorPath[i]);
            }
            PathfindingData pathfindingData = new PathfindingData(path, p.originalStartPoint, p.originalEndPoint);
            
            //pathfindingData.path = new List<Vector3>(p.vectorPath.Count);
            //pathfindingData.originalStartPoint = p.originalStartPoint;
            //pathfindingData.originalEndPoint = p.originalEndPoint;
            //var newPath = 
            //Data.Path.Clear();
            //path.

            OnPathComplete(pathfindingData);
        }
    }
}
