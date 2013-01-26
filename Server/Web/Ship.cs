using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProtoBuf;

namespace WebGame
{
    [ProtoContract]
    public class Ship : Entity
    {
        public List<Player> Players;

        [ProtoMember(1)]
        public double ImpulsePercentage
        {
            get
            {
                return _impulsePercentage;
            }
            set
            {
                _impulsePercentage = value;
                TargetSpeedMetersPerSecond = null; // setting impulse means "forget about what I said about 'all stop' or whatever other speed I said to try to get to"
            }
        }
        private double _impulsePercentage;
        [ProtoMember(2)]
        public int? TargetSpeedMetersPerSecond { get; set; }
        [ProtoMember(3)]
        public double DesiredOrientation
        {
            get
            {
                return _desiredOrientation;
            }
            set
            {
                _desiredOrientation = value;
                TargetSpeedMetersPerSecond = null; // setting orientation means "forget about what I said about 'all stop' or whatever other speed I said to try to get to"
            }
        }
        private double _desiredOrientation;

        private const double turnRateAnglePerSecond = Math.PI / 4; // this will likely get changed to something from engineering
        private const double maximumAvailableForceMetersPerSecondPerTon = 10000; // force = mass * acceleration

        public override string Type { get { return "Ship"; } }

        public Ship() : this(1000)
        {
        }
        public Ship(double massTons)
        {
            this.MassTons = massTons;
            Players = new List<Player>();
        }

        public override void Update(TimeSpan elapsed)
        {
            if (this.TargetSpeedMetersPerSecond.HasValue)
            {
                var speed = this.VelocityMetersPerSecond.Magnitude();
                if (this.TargetSpeedMetersPerSecond > speed)
                {
                    // speed up
                    this.ImpulsePercentage = 100;
                }
                else if (this.TargetSpeedMetersPerSecond.Value == speed)
                {
                    // stop target speed, reset values
                    this.TargetSpeedMetersPerSecond = null;
                    this.ImpulsePercentage = 0;
                    this.DesiredOrientation = this.Orientation;
                }
                else
                {
                    // align with velocity
                    var velocityDirection = Math.Atan2(this.VelocityMetersPerSecond.Y, this.VelocityMetersPerSecond.X).NormalizeOrientation();
                    var velocityOrientationAngle = this.Orientation - velocityDirection;
                    this.DesiredOrientation = velocityDirection;
                    if (Math.Abs(velocityOrientationAngle) > Math.PI / 2)
                    {
                        this.DesiredOrientation = (velocityDirection + Math.PI).NormalizeOrientation();
                    }
                    // apply a slowing force
                    this.ImpulsePercentage = -100 * Math.Cos(velocityOrientationAngle);
                    // but make sure it won't push us past the desired velocity
                    var decelerationAmount = this.ImpulsePercentage / 100 * MaxAccelerationMagnitude();
                    var targetDeceleration = speed - this.TargetSpeedMetersPerSecond.Value;
                    if (targetDeceleration < Math.Abs(decelerationAmount))
                    {
                        this.ImpulsePercentage = targetDeceleration / MaxAccelerationMagnitude() * 100 * Math.Sign(decelerationAmount);
                    }
                }
            }
            TurnShipToDesiredOrientation(elapsed);
            AccelerateShip(elapsed);
            ApplyVelocity(elapsed); // or alternately base.Update
        }

        private void AccelerateShip(TimeSpan elapsed)
        {
            var accelerationMagnitude = this.ImpulsePercentage / 100 * MaxAccelerationMagnitude();
            var flatAcceleration = new Vector3((float)accelerationMagnitude, 0, 0);
            var acceleration = Vector3.Transform(flatAcceleration, Matrix.CreateRotationZ((float)this.Orientation));

            this.VelocityMetersPerSecond += acceleration;
            if (this.VelocityMetersPerSecond.Magnitude() < 0.1) // small enough not to care.
            {
                this.VelocityMetersPerSecond = Vector3.Zero;
            }
        }

        private double MaxAccelerationMagnitude()
        {
            if (this.MassTons == 0)
            {
                throw new InvalidOperationException("Can't calculate force on an object without mass.");
            }
            return maximumAvailableForceMetersPerSecondPerTon / this.MassTons;
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
                this.Orientation = (this.Orientation + currentAllowedDiffAngle * Math.Sign(desiredDiffAngle)).NormalizeOrientation();
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
                    update.Entities.Add(new EntityUpdate() { Id = entity.Id, Type = entity.Type, Rotation = (float)entity.Orientation, Position = entity.Position });
                }
                GameHub.SendUpdate(Game.Id, Id, update);
                System.Diagnostics.Debug.WriteLine("Update Sent.");
            }
        }
    }
}