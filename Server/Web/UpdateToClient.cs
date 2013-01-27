using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebGame
{
    public class UpdateToClient
    {
        public int Energy;
        public int FrontShield;
        public int RearShield;
        public List<EntityUpdate> Entities = new List<EntityUpdate>();
        public String missionUpdate;
    }
}