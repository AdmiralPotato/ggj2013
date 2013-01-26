using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebGame
{
    public class StarSystem
    {
        public List<Player> Players = new List<Player>();
        public List<Entity> Entites = new List<Entity>();

        public void Update(TimeSpan timeElapsed)
        {
            foreach (var entity in Entites)
            {
                entity.Update(timeElapsed);
            }
        }
    }
}