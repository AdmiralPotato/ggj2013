using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebGame
{
    public class StarSystem
    {
        public Game Game { get; set; }
        public List<Player> Players = new List<Player>();
        public List<Entity> Entites = new List<Entity>();
        public List<Ship> Ships = new List<Ship>();

        public StarSystem()
        {
            Players = new List<Player>();
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

            if (entity.Id == 0)
                entity.Id = Game.NextEntityId++;
            Entites.Add(entity);

            var ship = entity as Ship;
            if (ship != null)
                Ships.Add(ship);
        }
    }
}