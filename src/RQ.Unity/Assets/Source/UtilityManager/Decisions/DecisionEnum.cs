using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilityManager.Decisions
{
    public enum DecisionEnum
    {
        Bark = 0,
        MeleeAttack = 1,
        MoveToWaypoint = 2,
        MoveToTarget = 3,
        MoveToAlly = 4,
        Idle = 5,
        RunFsm = 6,
        SetTargetRunFsm = 7,
        SetAllyRunFsm = 8
    }
}
