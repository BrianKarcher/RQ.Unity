using System.Collections.Generic;

namespace UtilityManager
{
    public class DecisionContext : IDecisionContext
    {
        public IAICharacter Self { get; set; }
        public IAICharacter EnemyEntity { get; set; }
        public IAICharacter AllyEntity { get; set; }
        private IList<IAICharacter> _enemyEntities;
        public IList<IAICharacter> EnemyEntities { get {
                return _enemyEntities; }
            set {
                _enemyEntities = value;
            } }
    }
}