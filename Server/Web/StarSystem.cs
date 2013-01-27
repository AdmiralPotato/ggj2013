using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Xna.Framework;
using ProtoBuf;

namespace WebGame
{
    [ProtoContract]
    public class StarSystem
    {
        [ProtoMember(1)]
        public List<Entity> Entites { get; set; }

        Queue<Entity> addedEntities = new Queue<Entity>();
        Queue<Entity> removedEntities = new Queue<Entity>();

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

            foreach (var entity in Entites)
            {
                if (entity.Sounds.Count > 0)
                    entity.Sounds.Clear();
            }

            if (Utility.Random.Next(480) <= 2)
            {
                AddEntity(new Enemy() { Position = new Vector3(Utility.Random.Next(1000) - 500, Utility.Random.Next(1000) - 500, 0) });
            }

            foreach (var added in addedEntities)
            {
                Entites.Add(added);

                var ship = added as Ship;
                if (ship != null)
                    Ships.Add(ship);
            }
            addedEntities.Clear();

            foreach (var removed in removedEntities)
            {
                Entites.Remove(removed);
                var ship = removed as Ship;
                if (ship != null)
                {
                    Ships.Remove(ship);
                }
            }
            removedEntities.Clear();
        }

        public void AddEntity(Entity entity)
        {
            entity.Game = Game;
            entity.StarSystem = this;

            if (entity.Id == 0)
                entity.Id = ++Game.NextEntityId;

            addedEntities.Enqueue(entity);
        }

        public void RemoveEntity(Entity entity)
        {
            removedEntities.Enqueue(entity);
        }

        internal Entity GetEntity(int targetId)
        {
            return (from e in Entites where e.Id == targetId select e).FirstOrDefault();
        }
    }
}