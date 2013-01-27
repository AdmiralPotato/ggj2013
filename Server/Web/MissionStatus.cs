using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.Xna.Framework;

namespace WebGame
{
    public class MissionStatus
    {
        const string END_MISSION_TEXT = "Congratulations on completing episode 0! Feel free to wander and explore on your own.";

        int END_MISSION_STEP;
        int missionStep;
        Mission[] missions;
        bool missionChanged;

        public MissionStatus(Ship initShip)
        {
            missionStep = 0;
            missions = new Mission[4];
            Enemy firstMissionEnemy = new Enemy(new Vector3((Utility.randomBool() ? 1 : -1) * Utility.Random.Next(100) + 200, (Utility.randomBool() ? 1 : -1) * Utility.Random.Next(100) + 200, 0));
            initShip.StarSystem.AddEntity(firstMissionEnemy);
            int enemyId = firstMissionEnemy.Id;
            int m = 0;
            missions[m++] = new MissionDefeatEnemy(initShip.StarSystem, enemyId, "Welcome! Your first mission is to defeat enemy "+enemyId+".");
            missions[m++] = new MissionGoToLocation(initShip, 35, 73, "Good work! Now investigate this distress call at 35, 73.");
            missions[m++] = new MissionGoToLocation(initShip, 212, 9, "Thank you for saving me! My ship came alive and went to 212, 9. Please find it!");
            missions[m++] = new MissionGoToLocation(initShip, 16, 414, "Warning! Huge energy readings in your area! You can't fight that thing! Return to base 16, 414 immediately!");
            END_MISSION_STEP = m;
            missionChanged = true;  // first time is new
        }

        internal void checkSuccess()
        {
            //missions[n].Update(time_elapsed);?
            if (missionStep < END_MISSION_STEP)
            {
                if (missions[missionStep].IsComplete())
                {
                    missionStep++;
                    missionChanged = true;
                }
            }
        }

        internal string getMissionStatusUpdate()
        {
            if (!missionChanged)
                return null;  // ""?
            else
            {
                missionChanged = false;
                if (missionStep < END_MISSION_STEP)
                    return missions[missionStep].Text;
                else
                    return END_MISSION_TEXT;
            }
        }
    }
}
