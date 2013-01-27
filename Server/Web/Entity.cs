using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Xna.Framework;
using ProtoBuf;

namespace WebGame
{
    [ProtoInclude(10, typeof(Ship))]
    [ProtoInclude(11, typeof(Starbase))]
    [ProtoContract]
    public abstract class Entity
    {
        public Entity(double mass, Vector3? position = null, Vector3? velocity = null)
        {
            if (mass <= 0)
            {
                throw new ArgumentException("Mass must be positive.");
            }
            this.Mass = mass;
            this.Energy = this.InitialEnergy;
            if (position.HasValue)
            {
                this.Position = position.Value;
            }
            if (velocity.HasValue)
            {
                this.Velocity = velocity.Value;
            }

            SetupParts();
        }

        private void SetupParts()
        {
            parts = new Dictionary<string, double>();
            foreach (var part in PartList)
            {
                parts.Add(part, partsHp);
            }
        }

        protected const double energyCostPerForcePerSecond = 0.0005;

        protected abstract IEnumerable<string> PartList
        {
            get;
        }

        [ProtoMember(1)]
        public int Id { get; set; }

        public Game Game;
        public StarSystem StarSystem;
        public abstract string Type { get; }

//        [ProtoMember(2)]
        public Vector3 Position { get; protected set; }
        [ProtoMember(3)]
        public double Orientation { get; protected set; }

        /// <summary>
        /// Meters Per Second
        /// </summary>
        //[ProtoMember(4)]
        public Vector3 Velocity { get; protected set; }
        /// <summary>
        /// Tons
        /// </summary>
        [ProtoMember(5)]
        public double Mass { get; private set; }

        /// <summary>
        /// Meters
        /// </summary>
        public abstract double Radius { get; }

        [ProtoMember(7)]
        public bool IsDestroyed { get; private set; }

        protected Dictionary<string, double> parts;

        [ProtoMember(9)]
        public double Energy { get; private set; }

        public bool SpendRawEnergy(double amountToUse)
        {
            if (amountToUse <= this.Energy)
            {
                this.Energy -= amountToUse;
                return true;
            }
            return false;
        }

        public bool LoseEnergyFrom(double intendedForce, TimeSpan elapsedTime)
        {
            return SpendRawEnergy(intendedForce * energyCostPerForcePerSecond * elapsedTime.TotalSeconds);
        }

        protected virtual double InitialEnergy
        {
            get
            {
                return 0;
            }
        }

        protected const int partsHp = 100;
        public List<string> Sounds = new List<string>();

        /// <summary>
        /// Meters Per Second Per Ton
        /// Applied in the direction of the Orientation
        /// Can be positive or negative
        /// </summary>
        public virtual double? ApplyForce(TimeSpan elapsed)
        {
            return 0;
        }

        public virtual void Update(TimeSpan elapsed)
        {
            ApplyAcceleration(elapsed);
            ApplyVelocity(elapsed);
        }

        public void ApplyVelocity(TimeSpan elapsed)
        {
            var oldPosition = this.Position;
            this.Position += Velocity.Multiply(elapsed.TotalSeconds);
            this.CheckForCollisions(oldPosition);
        }

        protected virtual void CheckForCollisions(Vector3 oldPosition)
        {
        }

        public void ApplyAcceleration(TimeSpan elapsed)
        {            
            // Force = Mass * Acceleration;
            // Acceleration = Force / Mass
            var force = this.ApplyForce(elapsed);
            if (force.HasValue)
            {
                var accelerationMagnitude = force.Value / this.Mass;
                var flatAcceleration = new Vector3((float)accelerationMagnitude, 0, 0);
                var acceleration = Vector3.Transform(flatAcceleration, Matrix.CreateRotationZ((float)this.Orientation));
                this.Velocity += acceleration * (elapsed.Ticks / (float)TimeSpan.FromSeconds(1).Ticks);
                //if (this.Velocity.Magnitude() < 0.01) // small enough not to care.
                //{
                //    //this.Velocity = Vector3.Zero;
                //}
            }
        }

        public void ApplyEnergyForce(double energy, double orientation)
        {
            const double efficiency = 0.1;
            var force = energy / energyCostPerForcePerSecond * efficiency; // No time in this equation, because we will apply this force over one second
            var accelerationMagnitude = force / this.Mass;
            var flatAcceleration = new Vector3((float)accelerationMagnitude, 0, 0);
            var acceleration = Vector3.Transform(flatAcceleration, Matrix.CreateRotationZ((float)orientation));
            this.Velocity += acceleration; // no time in this equation because it is applied as if it were one second
        }

        public string GetRandomWorkingPart()
        {
            var systemsThatCanBeDamaged = this.parts.Where((pair) => pair.Value > 0).ToArray();
            var systemIndexToDamage = Utility.Random.Next(systemsThatCanBeDamaged.Length);
            var systemToDamage = systemsThatCanBeDamaged[systemIndexToDamage];
            return systemToDamage.Key;
        }

        public void DamagePart(int damage, string part)
        {
            parts[part] = Math.Max(parts[part] - damage, 0);

            if (this.parts.Values.Sum() == 0)
            {
                this.Destroy();
            }
        }

        public void Damage(int damage, double orientation = 0)
        {
            HandleDamage(ref damage, Orientation);

            while (damage > 0 && this.parts.Values.Sum() > 0)
            {
                var systemsThatCanBeDamaged = this.parts.Where((pair) => pair.Value > 0).ToArray();
                var systemIndexToDamage = Utility.Random.Next(systemsThatCanBeDamaged.Length);
                var systemToDamage = systemsThatCanBeDamaged[systemIndexToDamage];
                this.parts[systemToDamage.Key] = systemToDamage.Value - 1;
                damage--;
            }

            if (this.parts.Values.Sum() == 0)
            {
                this.Destroy();
            }
        }

        protected virtual void HandleDamage(ref int damage, double orientation = 0)
        {
        }

        public double Effective(double maximum, string partName)
        {
            return maximum * this.parts[partName] / partsHp;
        }

        public TimeSpan Effective(TimeSpan minimum, string partName)
        {
            return TimeSpan.FromTicks((long)(minimum.Ticks / (this.parts[partName] / (double)partsHp)));
        }

        protected void Destroy()
        {
            this.IsDestroyed = true;
            this.StarSystem.RemoveEntity(this);
        }

        internal void ReceiveCommand(Ship ship, Command command)
        {
        }

        public void PlaySound(string name)
        {
            Sounds.Add(name);
        }
    }
}