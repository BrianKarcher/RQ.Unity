using RQ.Common;
using RQ.Common.Container;
using RQ.Entity.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UtilityManager
{
    [Serializable]
    public class IntelligenceDef
    {
        [SerializeField]
        [Tag]
        private string[] _allyTags;
        public string[] AllyTags { get { return _allyTags; } set { _allyTags = value; } }
        [SerializeField]
        [Tag]
        private string[] _enemyTags;
        public string[] EnemyTags { get { return _enemyTags; } set { _enemyTags = value; } }
        //private IDecisionContext _decisionContext;
        private DSExTarget _currentDSExTarget;
        private LinkedList<DSExTarget> _dSExTargets;
        private IEntityContainer _entityContainer;
        private IDecisionMaker _dm;
        private IComponentRepository _repo;
        // Key = DSE Name, value = last time run
        private Dictionary<string, float> _dseHistory;

        public void Init(IComponentRepository repo, IEntityContainer entityContainer, IDecisionMaker dm)
        {
            _entityContainer = entityContainer;
            _dseHistory = new Dictionary<string, float>();
            _dm = dm;
            _repo = repo;
            CreateDSExTargets(repo);
            // TODO Make the Decision Context values update automatically based on 
            // entities created or destroyed
            // TODO Create a DecisionConext/DSE pairing so we can compare a DSE
            // against multiple targets or allies as we do normal iteration in the DecisionMaker
            //_decisionContext = CreateContext(repo);
        }

        public void RemoveTarget(string uniqueId)
        {
            //var dsexTarget = _dSExTargets.First;
            //while (dsexTarget != null)
            //{
            var nodesToRemove = new List<LinkedListNode<DSExTarget>>();
            for (var dsexTarget = _dSExTargets.First; dsexTarget != null; dsexTarget = dsexTarget.Next)
            {
                var value = dsexTarget.Value;
                var target = value.GetTarget();
                if (target != null && target.UniqueId == uniqueId)
                {
                    //Debug.LogError("(IntelligenceDef RemoveTarget) Removing unique Id " + uniqueId);
                    //_dSExTargets.Remove(dsexTarget);
                    nodesToRemove.Add(dsexTarget);
                    if (_currentDSExTarget == dsexTarget.Value)
                    {
                        // Current target got destroyed, need to make a new decision
                        ScoreAndRunNewDecision();
                    }
                        //_currentDSExTarget = null;
                    //dsexTarget = dsexTarget.Next;
                    //continue;
                }
                
                var context = value.GetContext();
                if (context != null)
                {
                    for (int i = context.EnemyEntities.Count - 1; i >= 0; i--)
                    {
                        if (context.EnemyEntities[i].Repo.UniqueId == uniqueId)
                        {
                            //Debug.LogError("(IntelligenceDef) Removing EnemyEntity " + context.EnemyEntities[i].Repo.UniqueId + " " + context.EnemyEntities[i].Repo.name);
                            context.EnemyEntities.RemoveAt(i);
                        }
                    }
                    //dsexTarget.Value = value;
                    //List<IAICharacter> entitiesToRemove;
                    //var entitiesToRemove2 = context.EnemyEntities.Where(i => i.Repo.UniqueId == uniqueId);
                    //foreach (var entity in entitiesToRemove2)

                    //foreach (var entityToRemove in entitiesToRemove)
                    //{
                    //    Debug.LogError("(IntelligenceDef) Removing EnemyEntity " + entityToRemove.Repo.UniqueId + " " + entityToRemove.Repo.name);
                    //    context.EnemyEntities.Remove(entityToRemove);
                    //}

                    //context.EnemyEntities = context.EnemyEntities.Where(i => i.Repo.UniqueId != uniqueId);
                }
                    
                //dsexTarget = dsexTarget.Next;               
            }

            foreach (var node in nodesToRemove)
            {
                _dSExTargets.Remove(node);
            }
            //var targets = _dSExTargets.Where(i => i.GetTarget() != null && i.GetTarget().UniqueId == uniqueId);
            //Debug.LogError("Removing " + targets.Count() + " entities of unique Id " + uniqueId);
            //foreach (var target in targets)
            //{
            //    _dSExTargets.Remove(target);
            //}
        }

        public void ScoreAndRunNewDecision()
        {
            var dse = ScoreAllDecisions();
            if (dse != null)
            {
                var newDecision = RunDecision(dse);
            }
        }

        public DSExTarget ScoreAllDecisions()
        {
            //var dm = _dm.GetDecisionMaker();
            //foreach (var dm in _decisionMakers)
            //{
            return _dm.ScoreAllDecisions(_dSExTargets, null);
            //}
        }

        public bool IsCurrentDSELocked()
        {
            if (_currentDSExTarget == null)
                return false;
            var dse = _currentDSExTarget.GetDSE();
            if (dse == null)
                return false;
            return dse.IsDSELocked();
        }

        public bool RunDecision(DSExTarget dsexTarget)
        {
            // Currently running? Do not disturb unless finished
            if (_currentDSExTarget != null && !_currentDSExTarget.GetDSE().IsFinished() && _currentDSExTarget == dsexTarget)
                return false;
            if (_currentDSExTarget != null)
            {
                Debug.LogError("Finishing DSE " + _currentDSExTarget.GetDSE().Name);
                _currentDSExTarget.GetDSE().Stop(_currentDSExTarget.GetContext());
                // Log the last use of a DSE at the end of it.                
                AddOrUpdateDseHistory(_currentDSExTarget.GetDSE().Name, Time.time);
            }            
            _currentDSExTarget = dsexTarget;
            Debug.LogError("Running DSE " + dsexTarget.GetDSE().Name);
            _currentDSExTarget.GetDSE().RunDecision(_currentDSExTarget.GetContext());
            // Log the last use of a DSE at the start of it.                
            AddOrUpdateDseHistory(_currentDSExTarget.GetDSE().Name, Time.time);
            return true;
        }

        private void AddOrUpdateDseHistory(string name, float value)
        {
            _dseHistory[name] = value;
        }

        public float GetHistory(string dseName)
        {
            float value;
            if (_dseHistory.TryGetValue(dseName, out value))
            {
                return value;
            }
            return 0f;
        }

        private void CreateDSExTargets(IComponentRepository repo)
        {
            _dSExTargets = new LinkedList<DSExTarget>();
            var allEnemies = _entityContainer.GetEntitiesFromTags(_enemyTags).Where(i => i.isActiveAndEnabled);
            var dseList = _dm.GetDSEList();
            foreach (var dse in dseList)
            {
                var dseDef = dse;
                if (!dseDef.RunForAllTargets)
                {
                    var dseTarget = new DSExTarget(_entityContainer);
                    dseTarget.CreateContext(this, dseDef, repo, _allyTags, _enemyTags, null);
                    _dSExTargets.AddLast(dseTarget);
                    continue;
                }
                foreach (var enemy in allEnemies)
                {
                    var dseTarget = new DSExTarget(_entityContainer);
                    dseTarget.CreateContext(this, dseDef, repo, _allyTags, _enemyTags, enemy);
                    _dSExTargets.AddLast(dseTarget);
                }
            }
        }

        public LinkedList<DSExTarget> GetDsexTargets()
        {
            return _dSExTargets;
        }
    }
}
