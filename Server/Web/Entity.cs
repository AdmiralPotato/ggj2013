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
        public Entity(double mass)
        {
            if (mass <= 0)
            {
                throw new ArgumentException("Mass must be positive.");
            }
            this.Mass = mass;
            this.Energy = this.InitialEnergy;
            SetupParts();
        }

        private void SetupParts()
        {
            Parts = new Dictionary<string, int>();
            foreach (var part in PartList)
            {
                Parts.Add(part, partsHp);
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
        public Vector3 Position;// { get; protected set; }
        [ProtoMember(3)]
        public double Orientation { get; protected set; }

        /// <summary>
        /// Meters Per Second
        /// </summary>
        //[ProtoMember(4)]
        public Vector3 Velocity;// { get; protected set; }
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

        [ProtoMember(8)]
        private Dictionary<string, int> Parts { get; set; }

        [ProtoMember(9)]
        public double Energy { get; private set; }

        public void LoseEnergyFrom(double intendedForce, TimeSpan elapsedTime)
        {
            Energy -= intendedForce * energyCostPerForcePerSecond * elapsedTime.TotalSeconds;
        }

        protected virtual double InitialEnergy
        {
            get
            {
                return 0;
            }
        }

        protected const int partsHp = 100;

        /// <summary>
        /// Meters Per Second Per Ton
        /// Applied in the direction of the Orientation
        /// Can be positive or negative
        /// </summary>
        public virtual double ApplyForce(TimeSpan elapsed)
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
            var accelerationMagnitude = this.ApplyForce(elapsed) / this.Mass;
            var flatAcceleration = new Vector3((float)accelerationMagnitude, 0, 0);
            var acceleration = Vector3.Transform(flatAcceleration, Matrix.CreateRotationZ((float)this.Orientation));

            // before applying force, we need to check energy:
            if (CheckEnergy())
            {
                this.Velocity += acceleration;
                if (this.Velocity.Magnitude() < 0.1) // small enough not to care.
                {
                    this.Velocity = Vector3.Zero;
                }
            }
        }

        private bool CheckEnergy()
        {
            if (Energy < 0)
            {
                Energy = 0;
                return false;
            }
            return true;
        }

        public void Damage(int damage)
        {
            while (damage > 0 && this.Parts.Values.Sum() > 0)
            {
                var systemsThatCanBeDamaged = this.Parts.Where((pair) => pair.Value > 0).ToArray();
                var systemIndexToDamage = Utility.Random.Next(systemsThatCanBeDamaged.Length);
                var systemToDamage = systemsThatCanBeDamaged[systemIndexToDamage];
                this.Parts[systemToDamage.Key] = systemToDamage.Value - 1;
                damage--;
            }

            if (this.Parts.Values.Sum() == 0)
            {
                this.Destroy();
            }
        }

        public double Effective(double maximum, string partName)
        {
            return maximum * this.Parts[partName] / partsHp;
        }

        public TimeSpan Effective(TimeSpan minimum, string partName)
        {
            return TimeSpan.FromTicks((long)(minimum.Ticks / (this.Parts[partName] / (double)partsHp)));
        }

        protected void Destroy()
        {
            this.IsDestroyed = true;
            this.StarSystem.RemoveEntity(this);
        }

        internal void ReceiveCommand(Ship ship, Command command)
        {
        }
    }
}