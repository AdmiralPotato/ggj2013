using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProtoBuf;
using System.Collections.ObjectModel;

namespace WebGame
{
    [ProtoInclude(100, typeof(Enemy))]
    [ProtoContract]
    public class Ship : Entity
    {
        public Ship()
            : this(1000)
        {
        }

        public Ship(Vector3 position)
            : this(1000, position: position)
        {

        }

        public Ship(double mass, Vector3? position = null)
            : base(mass, position: position)
        {
            //if (Type.Equals("Ship") )  // aka not enemy
            //{
            Players = new List<Player>();
            //}
            Shields = new List<Shield>();
            for (int i = 0; i < ShieldCount; i++)
            {
                Shields.Add(new Shield());
            }
        }

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

        private List<Tube> _projectileTubes = new List<Tube>();
        private ReadOnlyCollection<Tube> ProjectileTubes
        {
            get
            {
                return new ReadOnlyCollection<Tube>(_projectileTubes);
            }
        }

        // shield set stored lower
        //[ProtoMember(9)]
        //public int FrontShield { get; set; }
        //[ProtoMember(10)]
        //public int RearShield { get; set; }
        //[ProtoMember(11)]
        //public int LeftShield { get; set; }
        //[ProtoMember(12)]
        //public int RightShield { get; set; }
        [ProtoMember(13)]
        public bool ShieldsEngaged { get; set; }

        [ProtoMember(14)]
        public int DefaultShipNumber { get; set; }

        /// <summary>
        /// The string is the name of the system they are repairing
        /// </summary>
        public readonly List<string> RepairCrewTargets = new List<string>();

        private static TimeSpan timeToLoadProjectile = TimeSpan.FromSeconds(5);
        public TimeSpan EffectiveTimeToLoadProjectile { get { return this.Effective(timeToLoadProjectile, "Projectile Tube"); } }

        /// <summary>
        /// Angle Per Second
        /// </summary>
        [ProtoMember(15)]
        public double TurnRate = Math.PI / 4;

        /// <summary>
        /// AnglePerSecond
        /// </summary>
        public double EffectiveTurnRate { get { return this.Effective(TurnRate, "Thrusters"); } }

        [ProtoMember(16)]
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

        [ProtoMember(17)]
        public List<BeamType> BeamWeapons = new List<BeamType>();

        [ProtoMember(18)]
        public Dictionary<ProjectileType, int> Projectiles = new Dictionary<ProjectileType, int>();

        [ProtoMember(19)]
        public int RepairCrews { get; set; }

        [ProtoMember(20)]
        public int MaxShields { get; set; }

        private int _tubes = 0;
        [ProtoMember(21)]
        public int Tubes
        {
            get { return _tubes; }
            set
            {
                _tubes = value;
                // make sure there's enough tubes
                while (this.ProjectileTubes.Count < this.Tubes)
                {
                    this._projectileTubes.Add(new Tube());
                }
                // make sure there aren't too many 
                while (this.ProjectileTubes.Count > this.Tubes)
                {
                    this._projectileTubes.RemoveAt(this._projectileTubes.Count - 1); // remove the last one
                }
            }
        }

        [ProtoMember(22)]
        public int PhaserBanks { get; set; }

        [ProtoMember(23)]
        public string Name { get; set; }

        [ProtoMember(24)]
        public List<Shield> Shields { get; set; }

        private const int ShieldCount = 4;

        private Shield DetermineShieldFrom(double orientation)
        {
            return Shields[(int)((orientation + Math.PI / 4).NormalizeOrientation() / (Math.PI / 2))];
        }

        public Dictionary<int, DateTime> LastBeamBanksUsed = new Dictionary<int, DateTime>();

        public override string Type { get { return "Ship"; } }
        public MissionStatus missionState = null;

        internal void SetupMissions()
        {
            // this.StarSystem must be set at this point
            missionState = new MissionStatus(this);
        }

        protected override double InitialEnergy { get { return 1000; } }

        public static Ship Create(ShipType type, Vector3? position = null)
        {
            Ship result;
            switch (type)
            {
                case ShipType.Spearhead:
                    result = new Ship(2000, position) { MaxShields = 500, Tubes = 2, PhaserBanks = 1, RepairCrews = 3 };
                    result.BeamWeapons.AddRange((BeamType[])Enum.GetValues(typeof(BeamType)));
                    result.BeamWeapons.Remove(BeamType.ShadowTether);
                    result.Projectiles[ProjectileType.Torpedo] = 10;
                    result.Projectiles[ProjectileType.Nuke] = 1;
                    result.Projectiles[ProjectileType.Hardshell] = 1;
                    result.Projectiles[ProjectileType.Knockshot] = 1;
                    result.Projectiles[ProjectileType.Skattershot] = 1;
                    break;
                case ShipType.Skirmisher:
                    result = new Ship(2000, position) { MaxShields = 500, Tubes = 2, PhaserBanks = 2, RepairCrews = 3 };
                    result.BeamWeapons.AddRange((BeamType[])Enum.GetValues(typeof(BeamType)));
                    result.BeamWeapons.Remove(BeamType.ShadowTether);
                    result.Projectiles[ProjectileType.Torpedo] = 10;
                    break;
                case ShipType.Beserker:
                    result = new Ship(5000, position) { MaxShields = 0, Tubes = 0, PhaserBanks = 1, MaximumForce = 12500, RepairCrews = 3 };
                    result.BeamWeapons.Add(BeamType.HullPiercing);
                    result.BeamWeapons.Add(BeamType.SelfDestruct);
                    result.BeamWeapons.Add(BeamType.PlasmaVent);
                    break;
                case ShipType.Gunboat:
                    result = new Ship(1500, position) { MaxShields = 600, Tubes = 4, PhaserBanks = 1, RepairCrews = 3 };
                    result.BeamWeapons.Add(BeamType.SuppresionPulse);
                    result.BeamWeapons.Add(BeamType.ShadowTether);
                    result.Projectiles[ProjectileType.Torpedo] = 10;
                    result.Projectiles[ProjectileType.Nuke] = 1;
                    result.Projectiles[ProjectileType.Hardshell] = 1;
                    break;
                case ShipType.Capital:
                    result = new Ship(3000, position) { MaxShields = 1000, Tubes = 5, RepairCrews = 3 };
                    result.BeamWeapons.Add(BeamType.StandardPhaser);
                    result.BeamWeapons.Add(BeamType.ShieldDampener);
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

        public bool LoadProjectile(int tubeNumber, ProjectileType type)
        {
            if (this.ProjectileTubes[tubeNumber].ProjectileStatus == ProjectileStatus.Unloaded)
            {
                this.ProjectileTubes[tubeNumber].ProjectileLoadTime = TimeSpan.Zero;
                this.ProjectileTubes[tubeNumber].ProjectileStatus = ProjectileStatus.Loading;
                PlaySound("MissileLoad");
                return true;
            }
            return false;
        }

        public Projectile LaunchProjectile(int tubeNumber, Entity target)
        {
            if (this.ProjectileTubes[tubeNumber].ProjectileStatus == ProjectileStatus.Loaded && this.StarSystem == target.StarSystem)
            {
                var projectile = new Projectile(this.Velocity);
                projectile.Target = target;
                this.StarSystem.AddEntity(projectile);
                PlaySound("MissileLaunch");
                return projectile;
            }
            return null;
        }

        public override void Update(TimeSpan elapsed)
        {
            UpdateShields(elapsed);
            UpdateProjectileLoading(elapsed);
            UpdateTargetVelocity(elapsed);
            UpdateOrientation(elapsed);
            UpdateRepair(elapsed);

            UpdateMission();

            base.Update(elapsed);

        }

        /// <summary>
        /// hp / second
        /// </summary>
        private const double repairRate = 1;
        private const double energyPerRepair = 0.01;

        private void UpdateRepair(TimeSpan elapsed)
        {
            for (int i = 0; i < this.RepairCrewTargets.Count; i++)
            {
                var target = this.RepairCrewTargets[i];
                if (target != null)
                {
                    var repairAmount = repairRate * elapsed.TotalSeconds;
                    var repairEnergy = energyPerRepair * repairAmount;
                    if (!this.SpendRawEnergy(repairEnergy))
                    {
                        break;
                    }
                    this.parts[target] += repairAmount;
                    if (this.parts[target] >= partsHp)
                    {
                        this.parts[target] = partsHp;
                        this.RepairCrewTargets[i] = null;
                    }
                }
            }
        }

        private void UpdateTargetVelocity(TimeSpan elapsed)
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
                    if (Math.Abs(velocityOrientationAngle) > Math.PI / 2) // if it would be less turning for us to turn into the velocity, do that instead
                    {
                        this._desiredOrientation = (velocityDirection + Math.PI).NormalizeOrientation(); // we need to directly access the underscore members so that we don't call the set method, which will unset the target speed 
                    }

                    // apply a slowing force
                    this._impulsePercentage = -100 * Math.Cos(velocityOrientationAngle); // we need to directly access the underscore members so that we don't call the set method, which will unset the target speed 
                    // but make sure it won't push us past the desired velocity
                    var decelerationAmount = this.Force / this.Mass * elapsed.TotalSeconds;
                    var targetDeceleration = speed - this.TargetSpeedMetersPerSecond.Value;
                    if (targetDeceleration < Math.Abs(decelerationAmount))
                    {
                        this._impulsePercentage = targetDeceleration / (this.EffectiveMaximumForce / this.Mass) / elapsed.TotalSeconds * 100 * Math.Sign(decelerationAmount); // we need to directly access the underscore members so that we don't call the set method, which will unset the target speed 
                    }
                }
            }
        }

        private void UpdateProjectileLoading(TimeSpan elapsed)
        {
            foreach (var tube in this.ProjectileTubes)
            {
                if (tube.ProjectileStatus == ProjectileStatus.Loading)
                {
                    tube.ProjectileLoadTime += elapsed;
                    if (tube.ProjectileLoadTime >= EffectiveTimeToLoadProjectile)
                    {
                        tube.ProjectileStatus = ProjectileStatus.Loaded;
                    }
                }
            }
        }

        private void UpdateShields(TimeSpan elapsed)
        {
            if (ShieldsEngaged)
            {
                foreach (var shield in this.Shields)
                {
                    shield.Update(elapsed, this);
                }
            }
        }

        private void UpdateOrientation(TimeSpan elapsed)
        {
            var desiredDiffAngle = TurnAngleNeededForDesiredOrientation();
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

        private double TurnAngleNeededForDesiredOrientation()
        {
            // difference
            var remaining = this.DesiredOrientation - this.Orientation;
            // normalize to -pi < remaining < pi
            remaining = remaining.NormalizeTurn();
            return remaining;
        }

        public override double? ApplyForce(TimeSpan elapsedTime)
        {
            var intendedForce = this.ImpulsePercentage / 100 * MaximumForce;
            if (this.LoseEnergyFrom(intendedForce, elapsedTime)) // the idea here is that if their engines aren't working at full capacity, they'll still lose energy as if they were. They're punching it, and losing all that energy, but only the Effective force is output.
            {
                return Force;
            }
            else
            {
                return null;
            }
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
            if (missionState != null)
            {
                missionState.updateMissionStatus();
            }
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
                var update = new UpdateToClient() { GameId = Game.Id, ShipId = Id, Energy = (int)this.Energy, FrontShield = (int)Shields[0].Strength, RearShield = (int)Shields[2].Strength, LeftShield = (int)Shields[3].Strength, RightShield = (int)Shields[1].Strength, ShieldsEngaged = this.ShieldsEngaged };
                foreach (var entity in StarSystem.Entites)
                {
                    if (entity.Sounds.Count > 0)
                        update.Sounds.AddRange(entity.Sounds);
                    update.Entities.Add(new EntityUpdate() { Id = entity.Id, Type = entity.Type, Rotation = (float)entity.Orientation, Position = entity.Position });
                }
                if (missionState != null)
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
                yield return "Projectile Tube"; // expand to number of tubes
                yield return "Shield Regenerators";
                yield return "Energy Collectors"; // not implemented
                yield return "Energy Storage";
                yield return "Beam Weapon Bank"; // not implemented
            }
        }

        internal void SetPower(string part, float amount)
        {
        }

        internal void SetCoolant(string part, int amount)
        {
        }

        public void SetRepairTarget(string part, int repairCrewIndex = -1)
        {
            if (repairCrewIndex > this.RepairCrews)
            {
                // Fit repair crew to correct size
                while (this.RepairCrewTargets.Count < this.RepairCrews)
                {
                    this.RepairCrewTargets.Add(null);
                }
                while (this.RepairCrewTargets.Count > this.RepairCrews)
                {
                    // I can't imagine how this would happen though.
                    this.RepairCrewTargets.RemoveAt(this.RepairCrews);
                }

                if (repairCrewIndex == -1) // Let Main Computer decide;
                {
                    var bestHp = -1;
                    repairCrewIndex = 0; // choose something other than -1 if for some reason all of them are at negative Hp;
                    for (int i = 0; i < this.RepairCrewTargets.Count; i++)
                    {
                        var targetPart = this.RepairCrewTargets[i];
                        if (targetPart == null)
                        {
                            // found a lazy repair crew? Use them immediately
                            repairCrewIndex = i;
                            break;
                        }
                        var repairingHp = this.parts[targetPart];
                        if (repairingHp > bestHp)
                        {
                            repairingHp = bestHp;
                            repairCrewIndex = i;
                        }
                    }
                }

                this.RepairCrewTargets[repairCrewIndex] = part;
            }
        }

        public bool IsEntityCloserThan(Entity target, float distance)
        {
            return (Position - target.Position).LengthSquared() < distance * distance;
        }

        public double BeamCoolDownTime(int bank)
        {
            if (LastBeamBanksUsed.ContainsKey(bank))
            {
                return (DateTime.UtcNow - LastBeamBanksUsed[bank]).TotalSeconds;
            }
            else
                return Single.MaxValue;
        }

        private void BeamDamage(Entity target, double amount)
        {
            target.Damage((int)Effective(amount, "Beam Weapon Bank"));
        }

        internal void FireBeam(int bank, Entity target, BeamType type)
        {
            if (!BeamWeapons.Contains(type))
                return;

            switch (type)
            {
                case BeamType.StandardPhaser:
                    if (IsEntityCloserThan(target, 150) && Energy > 5 && BeamCoolDownTime(bank) > 2)
                    {
                        target.Damage(100);
                        LastBeamBanksUsed[bank] = DateTime.UtcNow;
                        SpendRawEnergy(5);
                        PlaySound("FireStandardPhaser");
                    }
                    break;
                case BeamType.HullPiercing:
                    if (IsEntityCloserThan(target, 50) && SpendRawEnergy(25) && BeamCoolDownTime(bank) > 4)
                    {
                        target.Damage(300);
                        LastBeamBanksUsed[bank] = DateTime.UtcNow;
                        PlaySound("FireHullPiercing");
                    }
                    break;
                case BeamType.SuppresionPulse:
                    if (IsEntityCloserThan(target, 150) && SpendRawEnergy(75) && BeamCoolDownTime(bank) > 10)
                    {
                        target.DamagePart(100, target.GetRandomWorkingPart());
                        LastBeamBanksUsed[bank] = DateTime.UtcNow;
                        PlaySound("FireSuppresionPulse");
                    }
                    break;
                case BeamType.PlasmaVent:
                    if (SpendRawEnergy(100) && BeamCoolDownTime(bank) > 60)
                    {
                        foreach (var entity in StarSystem.Entites.ToArray()) // ToArray here to avoid threading issues. Lame I know.
                        {
                            if (IsEntityCloserThan(entity, 50))
                                target.Damage(250);
                        }

                        LastBeamBanksUsed[bank] = DateTime.UtcNow;
                        PlaySound("FirePlasmaVent");
                    }
                    break;
                case BeamType.TractorBeam:
                    if (IsEntityCloserThan(target, 200) && SpendRawEnergy(75) && BeamCoolDownTime(bank) > 5)
                    {
                        //target..Velocity
                        var displacement = this.Position - target.Position;
                        var orientation = Math.Atan2(displacement.Y, displacement.X).NormalizeOrientation();

                        target.ApplyEnergyForce(75, orientation);

                        LastBeamBanksUsed[bank] = DateTime.UtcNow;
                        PlaySound("FireTractorBeam");
                    }
                    break;
            }
        }

        protected override void HandleDamage(ref int damage, double orientation = 0)
        {
            if (ShieldsEngaged)
            {
                var shield = this.DetermineShieldFrom(orientation);
                if (damage > shield.Strength)
                {
                    damage -= (int)shield.Strength;
                    shield.Strength = 0;
                }
                else
                {
                    shield.Strength -= damage;
                    damage = 0;
                }
            }
        }

        [ProtoContract]
        public class Shield
        {
            [ProtoMember(1)]
            public double Strength { get; set; }

            /// <summary>
            /// Shield per second
            /// </summary>
            public const double shieldRegenerationRate = 2;
            public const double energyPerShield = 0.1;

            public void Update(TimeSpan elapsedTime, Ship currentShip)
            {
                var shieldRegenrationAmount = elapsedTime.TotalSeconds * shieldRegenerationRate;
                var energyCost = energyPerShield * shieldRegenrationAmount;
                Strength += shieldRegenrationAmount;
                currentShip.SpendRawEnergy(energyCost);
            }
        }

    }
}