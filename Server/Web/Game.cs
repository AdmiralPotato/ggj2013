﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ProtoBuf;
using System.IO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Threading;
using LT;

namespace WebGame
{
    [ProtoContract]
    public class Game
    {
        public static Action<Game, Player> OnEnd;

        [ProtoMember(1)]
        public int Id { get; set; }
        [ProtoMember(2)]
        public string GameName { get; set; }

        [Required]
        [Display(Name = "Max Number of Players")]
        [Range(2, 8, ErrorMessage = "Max players must be between 2 and 8")]
        [ProtoMember(5)]
        public int MaxPlayers { get; set; }

        [Required]
        [Display(Name = "Invite Only")]
        [ProtoMember(20)]
        public bool IsPrivate { get; set; }

        [ProtoMember(13)]
        public bool Started { get; set; }
        [ProtoMember(14)]
        public DateTime StartTime { get; set; }

        [ProtoMember(15)]
        public bool Ended { get; set; }
        [ProtoMember(16)]
        public DateTime EndTime { get; set; }

        [ProtoMember(18, AsReference = true)]
        public List<Player> Players { get; set; }

        [ProtoMember(22, IsRequired = false)]
        public List<Invite> Invites { get; set; }

        [ProtoMember(23, IsRequired = false)]
        public List<StarSystem> StarSystems = new List<StarSystem>();

        public Dictionary<int, Ship> DefaultShips = new Dictionary<int,Ship>();

        public int NextEntityId { get; set; }

        System.Timers.Timer timer;
        DateTime lastUpdate;

        public bool Running
        {
            get { return Started && !Ended; }
        }

        public int CurrentPlayers
        {
            get { return Players.Count; }
        }

        public int Status
        {
            get
            {
                if (!Started)
                    return 0;
                return Ended ? 2 : 1;
            }
        }

        public static Random Random = new Random();
        public bool IsRunning;

        public Game()
        {
            MaxPlayers = 2;

            Players = new List<Player>();
            Invites = new List<Invite>();
        }

        public void Run()
        {
            IsRunning = true;
            System.Diagnostics.Debug.WriteLine("Game " + Id + " thread started");
            timer = new System.Timers.Timer(250);
            timer.Elapsed += timer_Elapsed;
            timer.Start();
        }

        public void StopRunning()
        {
            if (timer != null)
            {
                timer.Stop();
                timer = null;
                IsRunning = false;
            }
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Game " + Id + " update");
            var now = DateTime.UtcNow;
            var elapsed = now - lastUpdate;

            Update(elapsed);

            lastUpdate = now;

            if (timer != null)
                timer.Enabled = true;
        }

        public void Update(TimeSpan elapsed)
        {
            foreach (var starSystem in StarSystems)
            {
                starSystem.Update(elapsed);
            }
        }

        public Player GetPlayer(int accountId)
        {
            foreach (var player in Players)
            {
                if (player.AccountId == accountId)
                    return player;
            }
            return null;
        }

        public Player Join(int accountId, string name, int rating)
        {
            var result = new Player() { AccountId = accountId, Name = name, Rating = rating };
            Join(result);
            return result;
        }

        public void Join(Player player)
        {
            player.Game = this;
            Players.Add(player);

            foreach (var invite in (from i in Invites where i.AccountId == player.AccountId select i))
            {
                Invites.Remove(invite);
                break;
            }

            SendForumMessage(player.Name + " joined the game.", player);

            if (Players.Count >= MaxPlayers)
                Start();
        }

        public void Unjoin(Player player)
        {
            Players.Remove(player);

            SendForumMessage(player.Name + " left the game.", player);
        }

        public void SendForumMessage(string text, Player source, string sourceName = "Computer")
        {
            using (var db = new DBConnection())
            {
                GameServer.SendMessage(db, -Id, source.AccountId, sourceName, text);
            }

            var message = new Message() { Sent = DateTime.UtcNow, Text = text, SourceId = source.AccountId, SourceName = sourceName };
            GameHub.Say(this, source.Ship, message.Print(false));
        }

        public void Start()
        {
            // make sure not already created
            //if (Started)
            //    throw new Exception("Game already started.");

            //if (Players.Count < 2)
            //    throw new Exception("Must have 2 players to start.");

            Started = true;

            // update game status
            StartTime = DateTime.UtcNow;

            // send player messages
            //SendForumMessage("Game #" + GameName + " Started");

            DefaultShips.Clear();
            StarSystems.Clear();

            var starSystem = new StarSystem();
            Add(starSystem);

            GameServer.SaveGame(this);

            //        using (var db = CreateDB())
            //        {
            //            db.Execute("delete from player where game_id = {0} and isInvite = 1", game.Id);
            //        }
            //        EmailAllPlayers(game, game.GameName + " Started", "The Global Combat game (" + game.GameName + ") has started.\n\nVisit http://globalcombat.com/Game-" + game.Id + " to play your turn.", true);
            //        GameHub.Refresh("Game-" + game.Id);
        }

        public void Add(StarSystem starSystem)
        {
            starSystem.Game = this;
            StarSystems.Add(starSystem);
        }

        public void EliminatePlayer(Player loser)
        {
            if (!loser.IsEliminated)
            {
                var place = (from p in Players where !p.IsEliminated select p).Count();

                loser.Place = place;
                loser.Done = true;
            }

            if (loser.Place <= 2)
                End();
        }

        //public bool ForceEndCheck()
        //{
        //    return (Running && LastTurnTime < DateTime.UtcNow.AddDays(-14) && StartTime < DateTime.UtcNow.AddDays(-14)) || (Running && Turn > 70);
        //}

        public void ForceEnd()        
        {
            //SendForumMessage("Since the turn has not run in over 14 days or there have been over 70 turns the server has forced an end to this game.");

            foreach (var player in (from p in Players where !p.IsEliminated select p))
            {
                EliminatePlayer(player);
            }
        }

        public double GenScoreExpected(int playerRating, int opponentRating)
        {
            return 1 / (System.Math.Pow(10, ((opponentRating - playerRating) / 1500f)) + 1);
        }

        public double GetScoreExpected(Player player)
        {
            double scoreExpected = 0;
            foreach (var otherPlayer in Players)
            {
                if (otherPlayer != player)
                    scoreExpected += GenScoreExpected(player.Rating, otherPlayer.Rating);
            }
            scoreExpected /= CurrentPlayers - 1;
            return scoreExpected;
        }

        public double GenScore(int place)
        {
            return (1f / (CurrentPlayers - 1f)) * (CurrentPlayers - place);
        }

        public void End()
        {
            // make sure not already ended
            if (!Ended)
            {
                // Find winner
                var winner = (from p in Players where p.Place <= 1 select p).FirstOrDefault();

                // update winner
                if (winner != null)
                    winner.Place = 1;

                // generate scores, award points
                foreach (var player in Players)
                {
                    player.ScoreExpected = Math.Round(GetScoreExpected(player) * 100) / 100;
                    player.Score = Math.Round(GenScore(player.Place) * 100) / 100;
                    player.RatingChange = (int)System.Math.Round((player.Score - player.ScoreExpected) * 150);
                }

                // update game status
                Ended = true;
                EndTime = DateTime.UtcNow;

                if (OnEnd != null)
                    OnEnd(this, winner);
            }
        }


        public byte[] Save()
        {
            using (var result = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(result, this);
                return result.ToArray();
            }
        }

        public static Game Load(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            {
                var result = ProtoBuf.Serializer.Deserialize<Game>(stream);

                if (String.IsNullOrEmpty(result.GameName))
                    result.GameName = "Game #" + result.Id;

                foreach (var starSystem in result.StarSystems)
                {
                    starSystem.Game = result;
                    foreach (var entity in starSystem.Entites)
                    {
                        entity.Game = result;
                        entity.StarSystem = starSystem;
                        var ship = entity as Ship;
                        if (ship != null)
                        {
                            if (ship.DefaultShipNumber != 0)
                                result.DefaultShips[ship.DefaultShipNumber] = ship;
                            starSystem.Ships.Add(ship);
                        }
                    }
                }

                foreach (var player in result.Players)
                    player.Game = result;

                return result;
            }
        }

        public string DisplayGameStatus(int accountId, bool isAdmin)
        {
            var result = new StringBuilder();
            result.Append("<a href=\"Game-" + Id + "/\">" + GameName + "</a> ");

            switch (Status)
            {

                case 0:
                    for (int LCount = 0; LCount < MaxPlayers; LCount++)
                    {
                        if (LCount < CurrentPlayers)
                            result.Append("<img src=/images/p.gif>");
                        else
                            result.Append("<img src=/images/pe.gif>");
                    }

                    if (IsPrivate)
                        result.Append(" <img src=/images/key.gif>");
                    break;

                case 1:
                    foreach (var player in Players)
                    {
                        if (player.IsEliminated)
                            result.Append("<img src=/images/px.gif>");
                        else
                        {
                            if (player.AccountId == accountId)
                            {
                                if (player.Done)
                                    result.Append("<img src=/images/pcd.gif>");
                                else
                                    result.Append("<img src=/images/pc.gif>");
                            }
                            else
                            {
                                if (player.Done)
                                    result.Append("<img src=/images/pd.gif>");
                                else
                                    result.Append("<img src=/images/p.gif>");
                            }
                        }
                    }
                    break;
                default:
                    result.Append(" Finished");
                    break;
            }

            if (isAdmin)
            {
                if (Status == 1) // running
                    result.Append(" <a href='javascript:if (confirm(\"You sure?\")) { self.location=\"Game-" + Id + "?ResetGame=1\" }'>RESET</a> ");
                if (Status < 2) // running or unstarted
                    result.Append(" <a href='javascript:if (confirm(\"You sure?\")) { self.location=\"Game-" + Id + "?KillGame=1\" }'>KILL</a> ");
            }

            result.Append("<br />");

            return result.ToString();
        }

        internal Player ConnectPlayer(string signalrConnectionId, string sessionId)
        {
            var player = (from p in Players where p.SessionId == sessionId select p).FirstOrDefault();
            if (player != null)
            {
                GetDefaultShip(1).AddPlayer(player);

                if (!IsRunning)
                    Run();

                GameHub.Say(this, player.Ship, player.Name + " connected.");
            }

            return player;
        }

        internal void DisconnectPlayer(string signalrConnectionId, string sessionId)
        {
            var player = (from p in Players where p.SessionId == sessionId select p).FirstOrDefault();
            DisconnectPlayer(player);
        }

        internal void DisconnectPlayer(Player player)
        {
            if (player != null)
            {
                if (player.Ship != null)
                {
                    GameHub.Say(this, player.Ship, player.Name + " disconnected.");
                    if (player.Ship != null)
                        player.Ship.RemovePlayer(player);
                }

                if (GetActivePlayerCount() <= 0)
                    StopRunning();

                GameServer.SaveGame(this);
            }
        }

        public int GetActivePlayerCount()
        {
            var result = 0;
            foreach (var star in StarSystems)
            {
                foreach (var ship in star.Ships)
                {
                    result += ship.Players.Count;
                }
            }
            return result;
        }

        internal Ship GetDefaultShip(int defaultShipNumber, string name = "Heart of the Deep")
        {
            if (!DefaultShips.ContainsKey(defaultShipNumber))
            {
                var defaultShip = Ship.Create(ShipType.Spearhead);
                defaultShip.DefaultShipNumber = defaultShipNumber;
                defaultShip.DesiredOrientation = 1;
                defaultShip.Name = name;
                DefaultShips[defaultShipNumber] = defaultShip;
                StarSystems[0].AddEntity(defaultShip);
                defaultShip.SetupMissions();  // must come after being added to starSystem
            }

            return DefaultShips[defaultShipNumber];
        }
    }
}