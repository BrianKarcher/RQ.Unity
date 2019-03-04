using System.Linq;
using RQ.Common;
using RQ.Common.Container;
using RQ.Entity.Components;

namespace UtilityManager
{
    public class DSExTarget
    {
        private IDSE _dse;
        private IEntity _target;
        private string _targetUniqueId;
        private IDecisionContext _context;
        private IEntityContainer _entityContainer;

        public DSExTarget(IEntityContainer entityContainer)
        {
            _entityContainer = entityContainer;
        }

        public void CreateContext(IntelligenceDef intelligenceDef, IDSE dse, IComponentRepository repo, string[] allyTags, string[] enemyTags, IEntity target)
        {
            _target = target;
            if (target != null)
                _targetUniqueId = target.UniqueId;
            _dse = dse;
            var dc = new DecisionContext();
            dc.Self = new AICharacter(repo, intelligenceDef);
            if (allyTags != null && allyTags.Length != 0)
            {
                var ally = _entityContainer.GetEntitiesFromTag(allyTags[0]).FirstOrDefault();
                dc.AllyEntity = new AICharacter((IComponentRepository)ally, null);
            }
            if (enemyTags != null && enemyTags.Any())
            {
                var allEnemies = _entityContainer.GetEntitiesFromTags(enemyTags);
                dc.EnemyEntities = allEnemies.Select(i => new AICharacter((IComponentRepository)i, null)).Cast<IAICharacter>().ToList();
            }
            if (target != null)
            {
                dc.EnemyEntity = new AICharacter((IComponentRepository)target, null);                
            }
            _context = dc;
        }

        public IDecisionContext GetContext()
        {
            return _context;
        }

        public IDSE GetDSE()
        {
            return _dse;
        }

        public IEntity GetTarget()
        {
            return _target;
        }
    }
}
