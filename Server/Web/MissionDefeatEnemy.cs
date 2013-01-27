using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebGame
{
    class MissionDefeatEnemy : Mission
    {
        int enemyId;
        StarSystem starSystem;

        public MissionDefeatEnemy(StarSystem initStarSys, int initEnemyId, string initMissionText)
            : base(MissionType.MISSION_TYPE_DEFEAT_ENEMY, initMissionText)
        {
            starSystem = initStarSys;
            enemyId = initEnemyId;
        }
        
        public override bool IsComplete()
        {
            return (starSystem.GetEntity(enemyId) == null);
        }
    }
}
