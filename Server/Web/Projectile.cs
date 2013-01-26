using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebGame
{
    public class Projectile : Entity
    {
        public Projectile(double mass)
            : base(mass)
        {
        }
        public override string Type { get { return "Projectile"; } }
    }
}