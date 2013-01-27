using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebGame
{
    class MissionGoToLocation : Mission
    {
        const float COMPLETED_MISSION_DISTANCE = 50;
        float targetLocationX;
        float targetLocationY;
        Ship missionOwnerShip;

        public MissionGoToLocation(Ship initMissionOwnerShip, int initTargetX, int initTargetY, string initMissionText)
            : base(MissionType.MISSION_TYPE_GO_TO_LOCATION, initMissionText)
        {
            missionOwnerShip = initMissionOwnerShip;
            targetLocationX = initTargetX;
            targetLocationY = initTargetY;
        }

        public override bool IsComplete()
        {
            if (Math.Abs(missionOwnerShip.Position.X - targetLocationX) < COMPLETED_MISSION_DISTANCE && Math.Abs(missionOwnerShip.Position.Y - targetLocationY) < COMPLETED_MISSION_DISTANCE )
            {
                return true;
            }
            return false;
        }
    }
}
