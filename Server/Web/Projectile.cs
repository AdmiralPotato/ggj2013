using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebGame
{
    public class Projectile : Entity
    {
        public Projectile()
            : this(1)
        {
        }

        public override double Force
        {
            get
            {
                return 100;
            }
        }

        public Entity Target { get; set; }

        public override void Update(TimeSpan elapsed)
        {
            if (this.StarSystem != Target.StarSystem)
            {
                throw new ArgumentException("The target is not in the current star system");
            }
            var displacementToTarget = Target.Position - this.Position;
            this.Orientation = Math.Atan2(displacementToTarget.X, displacementToTarget.X).NormalizeOrientation();

            base.Update(elapsed);
        }

        public Projectile(double mass)
            : base(mass)
        {
        }
        public override string Type { get { return "Projectile"; } }
    }
}