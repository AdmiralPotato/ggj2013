using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProtoBuf;
using Microsoft.Xna.Framework;

namespace WebGame
{
    [ProtoContract]
    public class Starbase : Entity
    {
        public Starbase()
            : this(100000)
        {
        }

        public Starbase(Vector3 position)
            : base(100000, position: position)
        {
        }

        public Starbase(double mass)
            : base(mass)
        {
        }

        public override string Type { get { return "Starbase"; } }
        public override double Radius
        {
            get { return 1000; }
        }

        public override void Update(TimeSpan elapsed)
        {
            ApplyVelocity(elapsed);
        }

        protected override IEnumerable<string> PartList
        {
            get
            {
                return Enumerable.Range(1, 100).Select((i) => "Starbase Part " + i);
            }
        }
    }
}