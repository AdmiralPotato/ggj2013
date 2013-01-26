using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Xna.Framework;
using ProtoBuf;

namespace WebGame
{
    [ProtoInclude(10, typeof(Ship))]
    [ProtoInclude(11, typeof(Base))]
    [ProtoContract]
    public abstract class Entity
    {
        [ProtoMember(1)]
        public int Id { get; set; }

        public Game Game;
        public StarSystem StarSystem;
        public abstract string Type { get; }

//        [ProtoMember(2)]
        public Vector3 Position;// { get; protected set; }
        [ProtoMember(3)]
        public double Orientation { get; protected set; }
        //[ProtoMember(4)]
        public Vector3 VelocityMetersPerSecond;// { get; protected set; }
        [ProtoMember(5)]
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