using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebGame
{
    public class Projectile : Entity
    {
        public override double Radius { get { return 1; } }

        const int destructionPower = partsHp * 0;

        public Entity Target { get; set; }

        public override string Type { get { return "Projectile"; } }

        protected override double InitialEnergy { get { return 1; } }

        public Projectile()
            : this(1)
        {
            
        }

        public Projectile(double mass)
            : base(mass)
        {
        }

        public Projectile(Vector3 position, Vector3 velocity): base(1, position, velocity)
        {
        }

        public static Projectile Create(ProjectileType type)
        {
            var result = new Projectile();
            switch (type)
            {
                case ProjectileType.Torpedo:
                    break;
                case ProjectileType.Skattershot:
                    break;
                case ProjectileType.Hardshell:
                    break;
                case ProjectileType.Nuke:
                    break;
                case ProjectileType.Knockshot:
                    break;
            }
            return result;
        }

        public override double? ApplyForce(TimeSpan elapsedTime)
        {
            var force = 100;
            if (this.LoseEnergyFrom(force, elapsedTime))
            {
                return force;
            }
            else
            {
                return null;
            }
        }

        public override void Update(TimeSpan elapsed)
        {
            if (this.StarSystem != Target.StarSystem)
            {
                this.Destroy();
            }
            else
            {
                var displacementToTarget = DisplacementToTarget();
                var displacementToTargetQuadrance = displacementToTarget.Quadrance();
                this.Orientation = Math.Atan2(displacementToTarget.Y, displacementToTarget.X).NormalizeOrientation();

                base.Update(elapsed);
            }
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
            if (currentDisplacementQuadrance < collisionQuadrance || Utility.SphereIntersectsLineSegment(oldPosition, this.Position, Target.Position, Target.Radius)) // we may want to have an alternate check for having gone through the target || DisplacementToTarget(oldPosition).Quadrance() < currentDisplacementQuadrance) // this only works if missiles are the fastest thing
            {
                Detonate();
            }
        }

        private void Detonate()
        {
            Target.Damage(this.EffectiveDamage);
            PlaySound("MissileDetonate");
            this.Destroy();
        }

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