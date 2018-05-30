namespace UtilityManager.Decisions
{
    public class DecisionFactory
    {
        public static IDecision Create(DecisionEnum decision)
        {
            switch (decision)
            {
                case DecisionEnum.MoveToTarget:
                    return new MoveToTarget();
                case DecisionEnum.RunFsm:
                    return new RunFsm();
                case DecisionEnum.MoveToAlly:
                    return new RunFsm();
                case DecisionEnum.Idle:
                    return new RunFsm();
                case DecisionEnum.SetTargetRunFsm:
                    return new SetTargetRunFsm();
                case DecisionEnum.SetAllyRunFsm:
                    return new SetAllyRunFsm();
            }
            return null;
        }
    }
}
