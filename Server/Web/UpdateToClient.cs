using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebGame
{
    public class UpdateToClient
    {
        public int ShipId;
        public int Energy;
        public int FrontShield;
        public int RearShield;
        public int LeftShield;
        public int RightShield;
        public bool ShieldsEngaged;
        public bool Alert;
        public MainView View;
        public List<EntityUpdate> Entities = new List<EntityUpdate>();
        public String missionUpdate;
    }
}