using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProtoBuf;
using Microsoft.Xna.Framework;

namespace WebGame
{
    [ProtoContract]
    public class Enemy : Ship
    {
        public Enemy()
        {
        }

        public Enemy(Vector3 position) : base(position)
        {

        }
        public override string Type
        {
            get
            {
                return "Enemy";
            }
        }

        private static TimeSpan reFireTime = TimeSpan.FromSeconds(500);

        private TimeSpan timeSinceLastFire = TimeSpan.Zero;

        public override void Update(TimeSpan elapsed)
        {
            if (timeSinceLastFire > reFireTime)
            {
                foreach (var ship in this.StarSystem.Ships)
                {
                    if (!(ship is Enemy))
                    {
                        this.LaunchProjectile(0, ship);
                        this.LoadProjectile(0, ProjectileType.Torpedo);
                    }
                }
                timeSinceLastFire = TimeSpan.Zero;
            }
            else
            {
                timeSinceLastFire += elapsed;
            }
            base.Update(elapsed);
        }
    }
}