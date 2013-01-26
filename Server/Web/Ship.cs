using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebGame
{
    public class Ship : Entity
    {
        public List<Player> Players { get; set; }

        public int ImpulsePower { get; set; }

        public int TargetSpeedMetersPerSecond { get; set; }

        public float DesiredHeadingRadians { get; set; }

        private const float turnRatePerSecondRadians = (float)(Math.PI / 4);

        public Ship()
        {
            Players = new List<Player>();
        }

        public override void Update(TimeSpan elapsed)
        {            
            // first turn ship

            // apply new velocity
            //Velocity += 
        }


        // input methods
        public void SetAllStop()
        {

        }

        public void SetImpluse(int impulsePower) // -100 to 100;
        {

        }

        public void SetDesiredHeading(int desiredHeadingRadians)
        {

        }

        public void AddPlayer(Player player)
        {
            if (!Players.Contains(player))
                Players.Add(player);
        }

        internal void SendUpdate()
        {
            if (Players.Count > 0)
            {
                var update = new UpdateToClient();
                GameHub.SendUpdate(Game.Id, Id, update);
            }
        }
    }
}