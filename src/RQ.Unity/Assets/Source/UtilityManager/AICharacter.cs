using RQ.Entity.Components;
using RQ.Physics.Components;
using UnityEngine;

namespace UtilityManager
{
    public class AICharacter : IAICharacter
    {
        public IComponentRepository Repo { get; private set; }
        public IntelligenceDef IntelligenceDef { get; private set; }

        public AICharacter(IComponentRepository repo, IntelligenceDef intelligenceDef)
        {
            Repo = repo;
            IntelligenceDef = intelligenceDef;
        }
        //private IComponentRepository _repo;

        //public void SetRepo(IComponentRepository repo)
        //{
        //    _repo = repo;
        //}

        //public IComponentRepository GetRepo()
        //{

        //}

        public float GetHealthCurrent()
        {
            var stats = Repo.Components.GetComponent<EntityStatsComponent>();
            return stats.GetEntityStats().CurrentHP;
        }

        public float GetHealthMax()
        {
            var stats = Repo.Components.GetComponent<EntityStatsComponent>();
            return stats.GetEntityStats().MaxHP;
        }

        public Vector2 GetPos()
        {
            return Repo.transform.position;
        }

        public bool IsAlive()
        {
            if (Repo == null)
                return false;
            if (Repo.transform == null)
                return false;
            else
                return true;
        }
    }
}