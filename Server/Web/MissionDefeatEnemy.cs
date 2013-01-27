using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebGame
{
    class MissionDefeatEnemy : Mission
    {
        int enemyId;

        public MissionDefeatEnemy(int initEnemyId, string initMissionText)
            : base(MissionType.MISSION_TYPE_DEFEAT_ENEMY, initMissionText)
        {
            enemyId = initEnemyId;
        }
        
        public override bool IsComplete()
        {
            //if (Enemies[enemyId].isDefeated())
            //    return true;
            return false;
        }
    }
}
