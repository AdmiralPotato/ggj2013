using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProtoBuf;

namespace WebGame
{
    [ProtoInclude(100, typeof(Enemy))]
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
                _desiredOrientation = value.NormalizeOrientation();
                TargetSpeedMetersPerSecond = null; // setting orientation means "forget about what I said about 'all stop' or whatever other speed I said to try to get to"
            }
        }
        private double _desiredOrientation;

        [ProtoMember(4)]
        public bool Alert { get; set; }

        [ProtoMember(5)]
        public MainView MainView { get; set; }

        /// <summary>
        /// How long ago the projectile was loaded
        /// </summary>
        [ProtoMember(6)]
        public TimeSpan ProjectileLoadTime { get; private set; }

        [ProtoMember(7)]
        public ProjectileStatus ProjectileStatus { get; set; }

        [ProtoMember(9)]
        public int FrontShield { get; set; }
        [ProtoMember(10)]
        public int RearShield { get; set; }
        [ProtoMember(11)]
        public int LeftShield { get; set; }
        [ProtoMember(12)]
        public int RightShield { get; set; }
        [ProtoMember(13)]
        public bool ShieldsEngaged { get; set; }

        [ProtoMember(14)]
        public int DefaultShipNumber { get; set; }

        /// <summary>
        /// The string is the name of the system they are repairing
        /// </summary>
        public List<string> RepairCrews { get; set; }

        private static TimeSpan timeToLoadProjectile = TimeSpan.FromSeconds(5);
        public TimeSpan EffectiveTimeToLoadProjectile { get { return this.Effective(timeToLoadProjectile, "Projectile Tube"); }}

        /// <summary>
        /// Angle Per Second
        /// </summary>
        [ProtoMember(15)]
        public double TurnRate = Math.PI / 4;

        /// <summary>
        /// AnglePerSecond
        /// </summary>
        public double EffectiveTurnRate { get { return this.Effective(TurnRate, "Thrusters"); } }

        [ProtoMember(15)]
        public double MaximumForce = 10000;
        protected int EffectiveMaximumEnergy
        {
            get
            {
                return (int)this.Effective(MaximumForce, "Energy Storage");
            }
        }

        /// <summary>
        /// Meters Per Second Per Ton
        /// </summary>
        public double EffectiveMaximumForce { get { return this.Effective(MaximumForce, "Engines"); } }

        [ProtoMember(16)]
        public List<BeamType> BeamWeapons = new List<BeamType>();

        [ProtoMember(17)]
        public Dictionary<ProjectileType, int> Projectiles = new Dictionary<ProjectileType, int>();

        public override string Type { get { return "Ship"; } }
        public MissionStatus missionState = null;

        public Ship()
            : this(1000)
        {
        }

        public Ship(double mass)
            : base(mass)
        {
            //if (Type.Equals("Ship") )  // aka not enemy
            //{
                Players = new List<Player>();
            //}
        }

        internal void SetupMissions()
        {
            // this.StarSystem must be set at this point
            missionState = new MissionStatus(this);
        }

        protected override double InitialEnergy { get { return 1000; } }

        public static Ship Create(ShipType type)
        {
            Ship result;
            switch (type)
            {
                case ShipType.Spearhead:
                    result = new Ship(2000) { MaxShields = 500, TorpedoTubes = 2, PhaserBanks = 1 };
                    result.BeamWeapons.AddRange((BeamType[])Enum.GetValues(typeof(BeamType)));
                    result.BeamWeapons.Remove(BeamType.ShadowTether);
                    result.Projectiles[ProjectileType.Torpedo] = 10;
                    result.Projectiles[ProjectileType.Nuke] = 1;
                    result.Projectiles[ProjectileType.Hardshell] = 1;
                    result.Projectiles[ProjectileType.Knockshot] = 1;
                    result.Projectiles[ProjectileType.Skattershot] = 1;
                    break;
                case ShipType.Skirmisher:
                    result = new Ship(2000) { MaxShields = 500, TorpedoTubes = 2, PhaserBanks = 2 };
                    result.BeamWeapons.AddRange((BeamType[])Enum.GetValues(typeof(BeamType)));
                    result.BeamWeapons.Remove(BeamType.ShadowTether);
                    result.Projectiles[ProjectileType.Torpedo] = 10;
                    break;
                case ShipType.Beserker:
                    result = new Ship(5000) { MaxShields = 0, TorpedoTubes = 0, PhaserBanks = 1 , MaximumForce = 12500 };
                    result.BeamWeapons.Add(BeamType.HullPiercing);
                    result.BeamWeapons.Add(BeamType.SelfDestruct);
                    result.BeamWeapons.Add(BeamType.PlasmaVent);
                    break;
                case ShipType.Gunboat:
                    result = new Ship(1500) { MaxShields = 600, TorpedoTubes = 4, PhaserBanks = 1 };
                    result.BeamWeapons.Add(BeamType.SuppresionPulse);
                    result.BeamWeapons.Add(BeamType.ShadowTether);
                    result.Projectiles[ProjectileType.Torpedo] = 10;
                    result.Projectiles[ProjectileType.Nuke] = 1;
                    result.Projectiles[ProjectileType.Hardshell] = 1;
                    break;
                case ShipType.Capital:
                    result = new Ship(3000) { MaxShields = 1000, TorpedoTubes = 5 };
                    result.BeamWeapons.Add(BeamType.StandardPhaser);
                    result.BeamWeapons.Add(BeamType.ShieldDamper);
                    result.BeamWeapons.Add(BeamType.ShadowTether);
                    result.Projectiles[ProjectileType.Torpedo] = 10;
                    result.Projectiles[ProjectileType.Nuke] = 1;
                    result.Projectiles[ProjectileType.Hardshell] = 1;
                    break;
                default:
                    throw new NotImplementedException("Unknown ship type " + type.ToString());
            }
            return result;
        }

        public bool LoadProjectile()
        {
            if (this.ProjectileStatus == ProjectileStatus.Unloaded)
            {
                this.ProjectileLoadTime = TimeSpan.Zero;
                this.ProjectileStatus = ProjectileStatus.Loading;
                return true;
                PlaySound("MissileLoad");
            }
            return false;
        }

        public Projectile LaunchProjectile(Entity target)
        {
            if (this.ProjectileStatus == ProjectileStatus.Loaded && this.StarSystem == target.StarSystem)
            {
                var projectile = new Projectile();
                projectile.Target = target;
                this.StarSystem.AddEntity(projectile);
                PlaySound("MissileLaunch");
                return projectile;
            }
            return null;
        }

        public override void Update(TimeSpan elapsed)
        {
            if (ShieldsEngaged)
            {
                FrontShield++;
                if (FrontShield > 100)
                    FrontShield = 100;
            }

            if (this.ProjectileStatus == ProjectileStatus.Loading)
            {
                this.ProjectileLoadTime += elapsed;
                if (this.ProjectileLoadTime >= EffectiveTimeToLoadProjectile)
                {
                    this.ProjectileStatus = ProjectileStatus.Loaded;
                }
            }
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

            if( missionState != null )
                UpdateMission();
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

        public override double ApplyForce(TimeSpan elapsedTime)
        {
            var intendedForce = this.ImpulsePercentage / 100 * MaximumForce;
            this.LoseEnergyFrom(intendedForce, elapsedTime); // the idea here is that if their engines aren't working at full capacity, they'll still lose energy as if they were. They're punching it, and losing all that energy, but only the Effective force is output.
            return Force;
        }

        private double Force
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

        private void UpdateMission()
        {
            missionState.checkSuccess();
        }

        public void AddPlayer(Player player)
        {
            if (!Players.Contains(player))
            {
                Players.Add(player);
                player.Ship = this;
            }
        }

        public void RemovePlayer(Player player)
        {
            if (Players.Contains(player))
            {
                Players.Remove(player);
                if (player.Ship == this)
                    player.Ship = null;
            }
        }

        internal void SendUpdate()
        {
            if (Players.Count > 0)
            {
                var update = new UpdateToClient() { ShipId = Id, Energy = (int)this.Energy, FrontShield = this.FrontShield, RearShield = this.RearShield, LeftShield = this.LeftShield, RightShield = this.RightShield, ShieldsEngaged = this.ShieldsEngaged };
                foreach (var entity in StarSystem.Entites)
                {
                    if (entity.Sounds.Count > 0)
                        update.Sounds.AddRange(entity.Sounds);
                    update.Entities.Add(new EntityUpdate() { Id = entity.Id, Type = entity.Type, Rotation = (float)entity.Orientation, Position = entity.Position });
                }
                update.missionUpdate = missionState.getMissionStatusUpdate();
                GameHub.SendUpdate(Game.Id, Id, update);
                System.Diagnostics.Debug.WriteLine("Update Sent. Mission status:" + update.missionUpdate);
            }
        }

        protected override IEnumerable<string> PartList
        {
            get
            {
                yield return "Thrusters";
                yield return "Engines";
                yield return "Projectile Tube";
                yield return "Shield Regenerators";
                yield return "Energy Collectors";
                yield return "Energy Storage";
            }
        }

        internal void SetPower(string part, float amount)
        {
        }

        internal void SetCoolant(string part, int amount)
        {
        }

        public void SetRepairTarget(string part)
        {
        }

        public int MaxShields { get; set; }

        public int TorpedoTubes { get; set; }

        public int PhaserBanks { get; set; }
    }
}