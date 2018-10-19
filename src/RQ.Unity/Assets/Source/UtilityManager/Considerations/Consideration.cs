using RQ.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UtilityManager
{
    [Serializable]
    public class Consideration : IConsideration
    {
        public enum InputEnum
        {
            MyHealth = 0,
            TargetHealth = 1,
            AllyHealth = 2,
            DistanceToTarget = 3,
            DistanceToAlly = 4,
            AllyThreat = 5,
            DecisionEvaluatorCooldown = 6
        }

        // From Presentation 1
        public enum InputEnum2
        {
            MyHealth,
            MyAmmo,
            MySpeed,
            MyCurrentWeaponRange,
            _3SecondRange,
            DistanceFromMe,
            HealthOf,
            StrengthOf,
            AllyCount,
            EnemyCount,
            AllyEnemyRatio
        }

        // From Presentation 2
        public enum InputEnum3
        {
            ConsiderationTargetIsPlayer,
            Constant,
            DecisionContextCooldown,
            DecisionContextRuntime,
            DecisionCooldown,
            DecisionEvaluatorCooldown,
            DecisionRuntime,
            EnvironmentDaylight,
            EnvironmentHabitatCustomScore,
            EnvironmentHabitatScore,
            EnvironmentTimeOfDay,
            IAmWieldingBundle,
            IHaveBuff,
            InfluenceAllyBreadcrumbs,
            InfluenceAllyConcentration,
            InfluenceAllyProximity,
            InfluenceAllyProximityAtOffset,
            InfluenceAllyProximityAtTarget,
            InfluenceAllyThreat,
            InfluenceAllyThreatAtTarget,
            InfluenceBattlefrontPhysical,
            InfluenceBattlefrontThreat,
            InfluenceEcologyRating,
            InfluenceEcologyRatingAtMyLocation,
            InfluenceEnemyBreadcrumbs,
            InfluenceEnemyConcentration,
            InfluenceEnemyProximity,
            InfluenceEnemyProximityAtOffset,
            InfluenceEnemyProximityAtTarget,
            InfluenceEnemyThreat,
            InfluenceEnemyThreatAtTarget,
            InfluenceStrageticConflict,
            InfluenceStrategicConflictAtMyLocation,
            MyBuffStatus,
            MyCombatFutility,
            MyHealth,
            TargetHealth,
            MyDistanceToTarget,
            ValidLineOfSight
        }

        [SerializeField]
        private Curve _curve;
        [SerializeField]
        public bool _isActive = true;
        public bool IsActive { get { return _isActive; } }
        [SerializeField]
        private InputEnum _input;
        public InputEnum Input { get { return _input; } set { _input = value; } }
        [SerializeField]
        private List<Parameter> _parameters;
        public List<Parameter> Parameters { get { return _parameters; } set { _parameters = value; } }

        public float Score(IDecisionContext context, List<Parameter> parameters)
        {
            float inputValue = 0f;
            switch(_input)
            {
                case InputEnum.MyHealth:
                    return MyHealthScore(context);
                case InputEnum.TargetHealth:
                    return TargetHealthScore(context);
                case InputEnum.AllyHealth:
                    return AllyHealthScore(context);
                case InputEnum.DistanceToTarget:
                    return DistanceToEntity(context.Self, context.EnemyEntity);
                case InputEnum.DistanceToAlly:
                    return DistanceToEntity(context.Self, context.AllyEntity);
                case InputEnum.AllyThreat:
                    return AllyThreat(context);
                case InputEnum.DecisionEvaluatorCooldown:
                    return DecisionEvaluatorCooldown(context);
            }

            //var score = _curve.Score();
            return inputValue;
        }

        public float MyHealthScore(IDecisionContext context)
        {
            var entity = context.Self;
            if (entity == null)
                throw new Exception("(AI) Could not locate entity");
            return entity.GetHealthCurrent() / entity.GetHealthMax();
        }

        public float TargetHealthScore(IDecisionContext context)
        {
            var entity = context.EnemyEntity;
            if (entity == null)
                throw new Exception("(AI) Could not locate target");
            return entity.GetHealthCurrent() / entity.GetHealthMax();
        }

        public float AllyHealthScore(IDecisionContext context)
        {
            var entity = context.AllyEntity;
            if (entity == null)
                throw new Exception("(AI) Could not locate ally");
            return entity.GetHealthCurrent() / entity.GetHealthMax();
        }

        public float DistanceToEntity(IAICharacter entityOne, IAICharacter entityTwo)
        {
            if (!entityOne.IsAlive() || !entityTwo.IsAlive())
                return 1000f;
            var one = entityOne.GetPos();
            var two = entityTwo.GetPos();
            var distance = (two - one).magnitude;
            float rangeMin;
            float rangeMax;
            float.TryParse(Parameters[0].Value, out rangeMin);
            float.TryParse(Parameters[1].Value, out rangeMax);

            return Mathf.Clamp01((distance - rangeMin) / (rangeMax - rangeMin));
        }

        public float AllyThreat(IDecisionContext context)
        {
            if (!context.AllyEntity.IsAlive())
                return 1000f;
            // Return closest distance between ally and closest enemy
            var allyPos = context.AllyEntity.GetPos();
            float closestDistSq = 9999f;
            foreach (var enemy in context.EnemyEntities)
            {
                if (enemy.Repo == null)
                    continue;
                var dist = (enemy.GetPos() - allyPos).sqrMagnitude;
                if (dist < closestDistSq)
                {
                    closestDistSq = dist;
                }
            }

            // Compare regular distance instead of distance squared
            var distance = Mathf.Sqrt(closestDistSq);
            float rangeMin;
            float rangeMax;
            float.TryParse(Parameters[0].Value, out rangeMin);
            float.TryParse(Parameters[1].Value, out rangeMax);

            return Mathf.Clamp01((distance - rangeMin) / (rangeMax - rangeMin));
        }

        public float DecisionEvaluatorCooldown(IDecisionContext context)
        {
            string dseName;
            float designatedTime;
            dseName = Parameters[0].Value;
            float.TryParse(Parameters[1].Value, out designatedTime);
            float timeSinceLastUse = Time.time - context.Self.IntelligenceDef.GetHistory(dseName);
            // Cooldowns are pretty boolean
            if (timeSinceLastUse > designatedTime)
            {
                Debug.LogWarning("DSE Cooldown returned 1");
                return 1f;
            }
            else
                return 0f;
        }

        public float ComputeResponseCurve(float score)
        {
            return _curve.Score(score);
        }
    }
}
