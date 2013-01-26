﻿using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;
using System.Collections;
using WebGame;

namespace WebGame
{
	public class Account
	{
        public int Id;
        public string Name;
        public string EmailAddress;
        public bool IsAdmin;
        public DateTime SignedUp;

        public int Wins;
        public int Games;
        public int Rating;

        public DateTime LastOn;
        public int LoginCount;
        public string SessionKey;
        public string LastIpAddress;
        public string ForwardEmails;

        public bool IsDisabled;

        public string Rank
        {
            get
            {
                if (Rating < 8400)
                    return "Private";
                if (Rating < 8750)
                    return "Private First Class";
                if (Rating < 9100)
                    return "Corporal";
                if (Rating < 9500)
                    return "Sergeant";
                if (Rating < 10000)
                    return "Major";
                return "General";
            }
        }

        public static Account Load(Hashtable row)
        {
            if (row == null)
                return null;

            var result = new Account();

            result.Id = (int)row["id"];
            result.Name = (string)row["name"];
            result.EmailAddress = (string)row["email"];
            result.IsAdmin = (string)row["admin"] == "True";
            result.SignedUp = Utility.FromUnixTimestamp((int)row["signed_up"]);

            result.Wins = (int)row["wins"];
            result.Games = (int)row["games"];
            result.Rating = (int)row["rating"];

            result.LastOn = Utility.FromUnixTimestamp((int)row["last_on"]);
            result.LoginCount = (int)row["num_logins"];
            result.LastIpAddress = (string)row["last_ip"];

            result.IsDisabled = (string)row["status"] == "Disabled";
            
            result.ForwardEmails = (string)row["forward_emails"];

            return result;
        }
	}
}
