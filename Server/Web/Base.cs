using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProtoBuf;

namespace WebGame
{
    [ProtoContract]
    public class Base : Entity
    {
        public override string Type { get { return "Base"; } }

        public override void Update(TimeSpan elapsed)
        {
            ApplyVelocity(elapsed);
        }
    }
}