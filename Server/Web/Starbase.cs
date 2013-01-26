using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProtoBuf;

namespace WebGame
{
    [ProtoContract]
    public class Starbase : Entity
    {
        public Starbase(double mass)
            : base(mass)
        {
        }

        public override string Type { get { return "Starbase"; } }

        public override void Update(TimeSpan elapsed)
        {
            ApplyVelocity(elapsed);
        }
    }
}