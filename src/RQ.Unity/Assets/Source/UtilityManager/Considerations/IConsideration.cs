using System.Collections.Generic;

namespace UtilityManager
{
    public interface IConsideration
    {
        Consideration.InputEnum Input { get; set; }
        bool IsActive { get; }
        List<Parameter> Parameters { get; set; }

        float AllyHealthScore(IDecisionContext context);
        float ComputeResponseCurve(float score);
        float DistanceToEntity(IAICharacter entityOne, IAICharacter entityTwo);
        float MyHealthScore(IDecisionContext context);
        float Score(IDecisionContext context, List<Parameter> parameters);
        float TargetHealthScore(IDecisionContext context);
    }
}