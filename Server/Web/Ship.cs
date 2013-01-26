using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebGame
{
    public class Ship : Entity
    {
        public List<Player> Players { get; private set; }

        public int ImpulsePercentage { get; set; }

        public int? TargetSpeedMetersPerSecond { get; set; }

        public double DesiredOrientation { get; set; }

        private const double turnRateAnglePerSecond = Math.PI / 4;

        public Ship()
        {
            Players = new List<Player>();
        }

        public override void Update(TimeSpan elapsed)
        {
            if (this.TargetSpeedMetersPerSecond.HasValue)
            {
                throw new NotImplementedException();
                // desire the ship to turn to be along the velocity vector
                // apply impulse proportional to -1 * cosine of the angle between desired and current
                // consider when velocity is almost zero? Calculate the inverse of the mass to determine impulse
            }
            TurnShip(elapsed);
            // apply new velocity
        }

        private void TurnShip(TimeSpan elapsed)
        {
            var desiredDiffAngle = TurnAngleNeededForDesire();
            var currentAllowedDiffAngle = elapsed.TotalSeconds * turnRateAnglePerSecond;
            var absoluteDesiredDiffAngle = Math.Abs(desiredDiffAngle);
            if (currentAllowedDiffAngle >= absoluteDesiredDiffAngle)
            {
                this.Orientation = this.DesiredOrientation;
            }
            else
            {
                this.Orientation += currentAllowedDiffAngle * Math.Sign(desiredDiffAngle);
            }
        }

        private double TurnAngleNeededForDesire()
        {
            // difference
            var remaining = this.DesiredOrientation - this.Orientation;
            // normalize to -pi < remaining < pi
            while (remaining > Math.PI)
            {
                remaining -= 2 * Math.PI;
            }
            while (remaining < -Math.PI)
            {
                remaining += 2 * Math.PI;
            }
            return remaining;
        }

        //// input methods
        //public void SetAllStop()
        //{
        //}
        //public void SetImpluse(int impulsePercentage) // -100 to 100;
        //{
        //}
        //public void SetDesiredOrientationAngle(int desiredOrientationAngle)
        //{
        //}

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