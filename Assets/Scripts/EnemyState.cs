using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class EnemyState  {
    public enum EEnemyMoveMode { SimpleMoveMode, StopMoveMode }

    public static EnemyMoveMode GetEnemyMoveMode(EEnemyMoveMode eEnemyMoveMode)
    {
        switch (eEnemyMoveMode)
        {
            default:
            case EEnemyMoveMode.SimpleMoveMode:
                return new SimpleMoveMode();
            case EEnemyMoveMode.StopMoveMode:
                return new StopMoveMode();
        }
    }
}
