using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProtoBuf;

namespace WebGame
{
    [ProtoContract]
    public class StarSystem
    {
        [ProtoMember(1)]
        public List<Entity> Entites { get; set; }

        public Game Game;
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
                entity.Id = ++Game.NextEntityId;
            Entites.Add(entity);

            var ship = entity as Ship;
            if (ship != null)
                Ships.Add(ship);
        }

        public void RemoveEntity(Entity entity)
        {
            Entites.Remove(entity);
            var ship = entity as Ship;
            if (ship != null)
            {
                Ships.Remove(ship);
            }
        }
    }
}