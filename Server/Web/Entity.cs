using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Xna.Framework;

namespace WebGame
{
    public abstract class Entity
    {
        public int Id { get; set; }

        public Game Game;
        public StarSystem StarSystem;
        public abstract string Type
        {
            get;
        }

        public Vector3 Position { get; protected set; }
        public double Orientation { get; protected set; }
        public Vector3 VelocityMetersPerSecond { get; protected set; }
        public double MassTons { get; set; }

        public virtual void Update(TimeSpan elapsed)
        {
            ApplyVelocity(elapsed);
        }

        public void ApplyVelocity(TimeSpan elapsed)
        {
            Position += VelocityMetersPerSecond.Multiply(elapsed.TotalSeconds);
        }
    }
}