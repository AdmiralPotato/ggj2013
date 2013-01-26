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
        //public Entity Target;

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

        [ProtoMember(4)]
        public bool Alert { get; set; }

        [ProtoMember(5)]
        public MainView MainView { get; set; }

        /// <summary>
        /// Angle Per Second
        /// </summary>
        private const double turnRate = Math.PI / 4;
        /// <summary>
        /// AnglePerSecond
        /// </summary>
        public double EffectiveTurnRate
        {
            get
            {
                return this.Effective(turnRate, "Thrusters");
            }
        }
        /// <summary>
        /// Meters Per Second Per Ton
        /// </summary>
        private const double maximumForce = 10000;
        public double EffectiveMaximumForce
        {
            get
            {
                return this.Effective(maximumForce, "Engines");
            }
        }

        public override string Type { get { return "Ship"; } }



        public Ship()
            : this(1000)
        {
        }

        public Ship(double mass)
            : base(mass)
        {
            Players = new List<Player>();
        }

        public Projectile LaunchProjectile(Entity target)
        {
            if (this.StarSystem != target.StarSystem)
            {
                throw new ArgumentException("The target is not in the current star system");
            }
            var projectile = new Projectile();
            projectile.Target = target;
            this.StarSystem.AddEntity(projectile);
            return projectile;
        }

        public override void Update(TimeSpan elapsed)
        {
            if (this.TargetSpeedMetersPerSecond.HasValue)
            {
                var speed = this.Velocity.Magnitude();
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
                    var velocityDirection = Math.Atan2(this.Velocity.Y, this.Velocity.X).NormalizeOrientation();
                    var velocityOrientationAngle = this.Orientation - velocityDirection;
                    this._desiredOrientation = velocityDirection; // we need to directly access the underscore members so that we don't call the set method, which will unset the target speed 
                    if (Math.Abs(velocityOrientationAngle) > Math.PI / 2)
                    {
                        this._desiredOrientation = (velocityDirection + Math.PI).NormalizeOrientation(); // we need to directly access the underscore members so that we don't call the set method, which will unset the target speed 
                    }
                    // apply a slowing force
                    this._impulsePercentage = -100 * Math.Cos(velocityOrientationAngle); // we need to directly access the underscore members so that we don't call the set method, which will unset the target speed 
                    // but make sure it won't push us past the desired velocity
                    var decelerationAmount = this.Force / this.Mass;
                    var targetDeceleration = speed - this.TargetSpeedMetersPerSecond.Value;
                    if (targetDeceleration < Math.Abs(decelerationAmount))
                    {
                        this._impulsePercentage = targetDeceleration / (this.EffectiveMaximumForce / this.Mass) * 100 * Math.Sign(decelerationAmount); // we need to directly access the underscore members so that we don't call the set method, which will unset the target speed 
                    }
                }
            }
            TurnShipToDesiredOrientation(elapsed);

            base.Update(elapsed);
        }

        private void TurnShipToDesiredOrientation(TimeSpan elapsed)
        {
            var desiredDiffAngle = TurnAngleNeededForDesire();
            var currentAllowedDiffAngle = elapsed.TotalSeconds * this.EffectiveTurnRate;
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

        public override double Force
        {
            get
            {
                return this.ImpulsePercentage / 100 * this.EffectiveMaximumForce;
            }
        }
        public override double Radius
        {
            get
            {
                return 10;
            }
        }

        public void AddPlayer(Player player)
        {
            if (!Players.Contains(player))
                Players.Add(player);
        }

        public void RemovePlayer(Player player)
        {
            if (Players.Contains(player))
                Players.Remove(player);
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

        internal void ToggleShields()
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<string> PartList
        {
            get
            {
                yield return "Thrusters";
                yield return "Engines";
                yield return "Weapons";
            }
        }

        internal void SetPower(string part, float amount)
        {
        }

        internal void ToggleAlert()
        {
            Alert = !Alert;
        }

        internal void SetMainScreenView(MainView view)
        {
            MainView = view;
        }
    }
}