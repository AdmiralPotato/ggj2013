using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;
using System.ComponentModel;

namespace WebGame
{
    [ProtoContract]
    public class Player
	{
        [ProtoMember(1)]
        public int AccountId { get; set; }
        [ProtoMember(3)]
        public string Name;

        [ProtoMember(4)]
        public bool Done;

        [ProtoMember(9)]
        public int Place { get; set; }
        [ProtoMember(10)]
        public double Score { get; set; }
        [ProtoMember(11)]
        public double ScoreExpected { get; set; }
        [ProtoMember(12)]
        public int Rating { get; set; }
        [ProtoMember(13)]
        public int RatingChange { get; set; }

        public Station Station;
        public string SessionId;
        public Game Game;
        public Ship Ship;

        public bool IsEliminated
        {
            get { return Place > 0; }
        }

        public string GetPlace()
        {
            switch (Place)
            {
                case 1:
                    return "1st Place";
                case 2:
                    return "2nd Place";
                case 3:
                    return "3rd Place";
                default:
                    return Place + "th Place";
            }
        }

		public override string ToString()
		{
			return Name;
		}
    }
}
