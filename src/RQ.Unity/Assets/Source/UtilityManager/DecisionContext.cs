using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilityManager
{
    public class DecisionContext : IDecisionContext
    {
        public IAICharacter Self { get; set; }
        public IAICharacter EnemyEntity { get; set; }
        public IAICharacter AllyEntity { get; set; }
        public IEnumerable<IAICharacter> EnemyEntities { get; set; }
    }
}
