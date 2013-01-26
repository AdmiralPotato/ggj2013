using Microsoft.Xna.Framework;
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
        public override double Radius
        {
            get { return 1; }
        }

        private const int destructionPower = partsHp;

        public Entity Target { get; set; }

        public override void Update(TimeSpan elapsed)
        {
            if (this.StarSystem != Target.StarSystem)
            {
                throw new ArgumentException("The target is not in the current star system");
            }
            var displacementToTarget = DisplacementToTarget();
            var displacementToTargetQuadrance = displacementToTarget.Quadrance();
            this.Orientation = Math.Atan2(displacementToTarget.Y, displacementToTarget.X).NormalizeOrientation();

            base.Update(elapsed);
        }

        private Vector3 DisplacementToTarget()
        {
            return DisplacementToTarget(this.Position);
        }

        private Vector3 DisplacementToTarget(Vector3 referencePosition)
        {
            return Target.Position - referencePosition;
        }

        protected override void CheckForCollisions(Vector3 oldPosition)
        {
            var collisionQuadrance = Target.Radius * Target.Radius + this.Radius * this.Radius;
            var currentDisplacementQuadrance = DisplacementToTarget().Quadrance();
            if (currentDisplacementQuadrance < collisionQuadrance) // we may want to have an alternate check for having gone through the target || DisplacementToTarget(oldPosition).Quadrance() < currentDisplacementQuadrance) // this only works if missiles are the fastest thing
            {
                Detonate();
            }
        }

        private void Detonate()
        {
            Target.Damage(this.EffectiveDamage);
            this.Destroy();
        }

        public Projectile(double mass)
            : base(mass)
        {
        }
        public override string Type { get { return "Projectile"; } }


        public int EffectiveDamage
        {
            get
            {
                return (int)this.Effective(destructionPower, "Warhead");
            }
        }

        protected override IEnumerable<string> PartList
        {
            get { yield return "Warhead"; }
        }
    }
}