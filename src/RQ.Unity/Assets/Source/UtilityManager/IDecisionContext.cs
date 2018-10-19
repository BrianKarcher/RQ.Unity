using System.Collections.Generic;

namespace UtilityManager
{
    public interface IDecisionContext
    {
        IAICharacter AllyEntity { get; set; }
        IAICharacter Self { get; set; }
        IAICharacter EnemyEntity { get; set; }
        IList<IAICharacter> EnemyEntities { get; set; }
    }
}