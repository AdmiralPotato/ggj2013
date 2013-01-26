using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebGame
{
    public class StarSystem
    {
        public Game Game;
        public List<Entity> Entites = new List<Entity>();
        public List<Ship> Ships = new List<Ship>();

        public StarSystem()
        {
            Entites = new List<Entity>();
            Ships = new List<Ship>();
        }

        public void Update(TimeSpan timeElapsed)
        {
            foreach (var entity in Entites)
            {
                entity.Update(timeElapsed);
            }

            foreach (var ship in Ships)
            {
                ship.SendUpdate();
            }
        }

        public void AddEntity(Entity entity)
        {
            entity.Game = Game;
            entity.StarSystem = this;

            if (entity.Id == 0)
                entity.Id = Game.NextEntityId++;
            Entites.Add(entity);

            var ship = entity as Ship;
            if (ship != null)
                Ships.Add(ship);
        }
    }
}