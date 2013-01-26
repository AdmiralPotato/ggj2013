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
        /// Meters Per Second Per Ton
        /// Applied in the direction of the Orientation
        /// Can be positive or negative
        /// </summary>
        public virtual double Force
        {
            get
            {
                return 0;
            }
        }

        public virtual void Update(TimeSpan elapsed)
        {
            ApplyAcceleration(elapsed);
            ApplyVelocity(elapsed);
        }

        public void ApplyVelocity(TimeSpan elapsed)
        {
            Position += Velocity.Multiply(elapsed.TotalSeconds);
        }

        public void ApplyAcceleration(TimeSpan elapsed)
        {            
            // Force = Mass * Acceleration;
            // Acceleration = Force / Mass
            var accelerationMagnitude = this.Force / this.Mass;
            var flatAcceleration = new Vector3((float)accelerationMagnitude, 0, 0);
            var acceleration = Vector3.Transform(flatAcceleration, Matrix.CreateRotationZ((float)this.Orientation));

            this.Velocity += acceleration;
            if (this.Velocity.Magnitude() < 0.1) // small enough not to care.
            {
                this.Velocity = Vector3.Zero;
            }
        }

    }
}