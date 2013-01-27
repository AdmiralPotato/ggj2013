using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WebGame
{
    class MissionDefeatEnemy : Mission
    {
        int enemyId;
        StarSystem starSystem;

        public MissionDefeatEnemy(StarSystem initStarSys, string initMissionText)
            : base(MissionType.MISSION_TYPE_DEFEAT_ENEMY, initMissionText)
        {
            starSystem = initStarSys;
        }
        
        internal override bool IsComplete()
        {
            return (starSystem.GetEntity(enemyId) == null);
        }

        internal override void MissionSetup()
        {
            Enemy missionEnemy = new Enemy() { Position = new Vector3((Utility.randomBool() ? 1 : -1) * Utility.Random.Next(100) + 200, (Utility.randomBool() ? 1 : -1) * Utility.Random.Next(100) + 200, 0) };
            starSystem.AddEntity(missionEnemy);
            enemyId = missionEnemy.Id;
        }
    }
}
