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
        // Key = DSE Name, value = last time run
        private Dictionary<string, float> _dseHistory;

        public void Init(IComponentRepository repo, IEntityContainer entityContainer, IDecisionMaker dm)
        {
            _entityContainer = entityContainer;
            _dseHistory = new Dictionary<string, float>();
            _dm = dm;
            CreateDSExTargets(repo);
            // TODO Make the Decision Context values update automatically based on 
            // entities created or destroyed
            // TODO Create a DecisionConext/DSE pairing so we can compare a DSE
            // against multiple targets or allies as we do normal iteration in the DecisionMaker
            //_decisionContext = CreateContext(repo);
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

        public void RunDecision(DSExTarget dsexTarget)
        {
            // Currently running? Do not disturb unless finished
            if (_currentDSExTarget != null && !_currentDSExTarget.GetDSE().IsFinished() && _currentDSExTarget == dsexTarget)
                return;
            if (_currentDSExTarget != null)
            {
                _currentDSExTarget.GetDSE().Stop(_currentDSExTarget.GetContext());
                // Log the last use of a DSE at the end of it.
                AddOrUpdateDseHistory(_currentDSExTarget.GetDSE().Name, Time.time);
            }            
            _currentDSExTarget = dsexTarget;
            _currentDSExTarget.GetDSE().RunDecision(_currentDSExTarget.GetContext());
            
        }

        private void AddOrUpdateDseHistory(string name, float value)
        {
            if (_dseHistory.ContainsKey(name))
                _dseHistory[name] = value;
            else
                _dseHistory.Add(name, value);
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
            var dseList = _dm.DSEList;
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
