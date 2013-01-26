using Microsoft.Xna.Framework;
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

        private const double turnRateAnglePerSecond = Math.PI / 4; // this will likely get changed to something from engineering
        private const double maximumAvailableForceMetersPerSecondPerTon = 1; // force = mass * acceleration

        public override string Type { get { return "Ship"; } }

        public Ship()
        {
            Players = new List<Player>();
        }

        public override void Update(TimeSpan elapsed)
        {
            if (this.TargetSpeedMetersPerSecond.HasValue)
            {
                AlignShipToVelocity(elapsed);
                throw new NotImplementedException();
                // apply impulse proportional to -1 * cosine of the angle between desired and current
                // consider when velocity is almost zero? Calculate the inverse of the mass to determine impulse
            }
            TurnShipToDesiredOrientation(elapsed);
            AccelerateShip(elapsed);
            ApplyVelocity(elapsed); // or alternately base.Update
        }

        private void AccelerateShip(TimeSpan elapsed)
        {
            var accelerationMagnitude = (float)(maximumAvailableForceMetersPerSecondPerTon / this.MassTons);
            var flatAcceleration = new Vector3(accelerationMagnitude, 0, 0);
            var acceleration = Vector3.Transform(flatAcceleration, Matrix.CreateRotationZ((float)this.Orientation));

            this.VelocityMetersPerSecond += acceleration;
        }

        private void AlignShipToVelocity(TimeSpan elapsed)
        {
            // Expand TurnShip todo this kind of stuff
            throw new NotImplementedException(); // copy 
        }

        private void TurnShipToDesiredOrientation(TimeSpan elapsed)
        {
            var desiredDiffAngle = TurnAngleNeededForDesire();
            var currentAllowedDiffAngle = elapsed.TotalSeconds * turnRateAnglePerSecond;
            var absoluteDesiredDiffAngle = Math.Abs(desiredDiffAngle);
            if (currentAllowedDiffAngle >= absoluteDesiredDiffAngle)
            {
                this.Orientation = this.DesiredOrientation.NormalizeOrientation();
            }
            else
            {
                this.Orientation += (currentAllowedDiffAngle * Math.Sign(desiredDiffAngle)).NormalizeOrientation();
            }
        }

        private double TurnAngleNeededForDesire()
        {
            // difference
            var remaining = this.DesiredOrientation - this.Orientation;
            // normalize to -pi < remaining < pi
            remaining = remaining.NormalizeTurn();
            return remaining;
        }

        //// input methods
        //public void SetAllStop()
        //{
        //}
        //public void SetImpluse(int impulsePercentage) // -100 to 100;
        //{
        //}
        //public void SetDesiredOrientationAngle(int desiredOrientationAngle) 0 to 2pi
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
                foreach (var entity in StarSystem.Entites)
                {
                    update.Entities.Add(new EntityUpdate() { Id = entity.Id, Type = entity.Type, Rotation = (float)entity.Orientation/*, Position = entity.Position*/ });
                }
                GameHub.SendUpdate(Game.Id, Id, update);
                System.Diagnostics.Debug.WriteLine("Update Sent.");
            }
        }
    }
}