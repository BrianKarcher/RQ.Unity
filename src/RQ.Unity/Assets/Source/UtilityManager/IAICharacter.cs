using RQ.Entity.Components;
using UnityEngine;

namespace UtilityManager
{
    public interface IAICharacter
    {
        IntelligenceDef IntelligenceDef { get; }
        float GetHealthCurrent();
        float GetHealthMax();
        IComponentRepository Repo { get; }
        Vector2 GetPos();
        bool IsAlive();
        //void SetRepo(IComponentRepository repo);
        //IComponentRepository GetRepo();
    }
}