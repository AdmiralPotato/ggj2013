using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WebGame
{
    public class EntityUpdate
    {
        public int Id;
        public string Type;
        public Vector3 Position;
        public float Rotation;
        public int Health;
        public int MaxHealth;
    }
}