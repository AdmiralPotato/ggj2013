using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace WebGame
{
    public class MissionStatus
    {
        const string END_MISSION_TEXT = "[Congratulations on completing episode 0! Feel free to wander and explore on your own. See you next episode!]";

        int END_MISSION_STEP;
        int missionStep;
        Mission[] missions;
        bool missionChanged;

        public MissionStatus(Ship initShip)
        {
            missionStep = 0;
            missions = new Mission[7];
            int m = 0;
            missions[m++] = new MissionDefeatEnemy(initShip.StarSystem, "HQ: "+initShip.Name+", an enemy ship near you refuses to answer hails and... it is charging its weapons! Prepare for attack!");
            missions[m++] = new MissionGoToLocation(initShip, -422, 173, "HQ: Good work! Hold on... we are picking up a distress signal from an escape pod at 35, 73. We need you to investigate.");
            missions[m++] = new MissionDefeatEnemy(initShip.StarSystem, "Escape Pod: Thanks for coming, but be careful! Two enemy ships cloaked when you arrived. There's one now!");
            missions[m++] = new MissionDefeatEnemy(initShip.StarSystem, "Escape Pod: You got it! Nice work, "+initShip.Name+"! My name's Rogers by the way... Oh! Here comes the other one! Watch out!");
            missions[m++] = new MissionGoToLocation(initShip, 312, -936, "Rogers: That was close! My ship had a cardiac system anomaly and began a jump to the Outer Sector; I couldn't control it at all! Please, help me get it back. It should be located at 312, -936.");
            missions[m++] = new MissionGoToLocation(initShip, -55, -1111, "Rogers: There's my ship! But what is that huge mass...? And this sound; is that a heartbeat? Let's get a little closer. Approach -55, -1111.");
            missions[m++] = new MissionGoToLocation(initShip, 0, 400, "HQ: What are you doing, "+initShip.Name+"? Get out of there, now, as fast as you can! That’s an order! You cannot fight that thing! Rendezvous at Central Station located at 0, 400!");
            END_MISSION_STEP = m;
            missionChanged = true;  // first time is new
            missions[missionStep].MissionSetup();  // set up first mission
        }

        internal void updateMissionStatus()
        {
            //missions[n].Update(time_elapsed);?
            if (missionStep < END_MISSION_STEP)
            {
                if (missions[missionStep].IsComplete())
                {
                    // go to next mission
                    missionStep++;
                    missionChanged = true;

                    // setup next mission
                    missions[missionStep].MissionSetup();
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
                {
                    return missions[missionStep].Text;
                }
                else
                {
                    return END_MISSION_TEXT;
                }
            }
        }
    }
}
