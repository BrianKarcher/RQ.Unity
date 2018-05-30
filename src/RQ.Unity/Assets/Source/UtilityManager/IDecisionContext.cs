using System.Collections.Generic;

namespace UtilityManager
{
    public interface IDecisionContext
    {
        IAICharacter AllyEntity { get; set; }
        IAICharacter Self { get; set; }
        IAICharacter EnemyEntity { get; set; }
        IEnumerable<IAICharacter> EnemyEntities { get; set; }
    }
}