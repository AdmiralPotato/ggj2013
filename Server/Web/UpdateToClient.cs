using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebGame
{
    public class UpdateToClient
    {
        public ShipStatus Status;
        public List<EntityUpdate> Entities;
    }

    public class ShipStatus
    {
        public int Energy;
        public int FrontShield;
        public int RearShield;
    }
}