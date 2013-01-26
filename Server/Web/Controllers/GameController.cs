using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using WebGame;
using LT;

namespace WebGame
{
    public class GameController : BaseController
    {
        Game game;
        Player player;

        bool IsPlaying { get { return player != null; } }

        void Initalize(int id)
        {
            game = GameServer.GetGame(id);

            if (game == null)
            {
                Response.RedirectPermanent("/", true);
                Response.End();
                return;
            }

            if (LoggedIn)
            {
                player = game.GetPlayer(Account.Id);
                ViewBag.Player = player;
                if (player != null)
                    player.SessionId = Request.Cookies["ASP.Net_SessionId"].Value;
            }
        }

        void LoadMessages()
        {
            if (game == null)
                return;

            using (var db = new DBConnection())
            {
                ViewBag.Messages = new List<Message>();

                using (var messages = db.OpenQuery(String.Format("select m.id as id, to_id, from_id, fromAccount.name fromName, text, time from message as m join account as fromAccount on m.from_id = fromAccount.id where to_id = {0} order by id desc limit 150", -game.Id)))
                {
                    while (messages.Read())
                    {
                        var newMessage = new Message() { Id = (int)messages["id"], SourceId = (int)messages["from_id"], SourceName = (string)messages["fromName"], Text = messages["text"].ToString(), Sent = Utility.FromUnixTimestamp((int)messages["time"]) };
                        ViewBag.Messages.Add(newMessage);
                    }
                }
            }
        }

        public ActionResult Index(int id = -1)
        {
            if (id == -1)
                return HttpNotFound();

            if (!Request.Url.PathAndQuery.EndsWith("/"))
            {
                if (Request.IsLocal)
                    throw new Exception("Invalid game link detected.  All links to games must end with a forward slash.");
                return RedirectPermanent("Game-" + id + "/");
            }

            Initalize(id);
            LoadMessages();

            if (!game.Started || game.Ended)
                return View("Lobby", game);

            return View(game);
        }

        public ActionResult Local(int id = -1)
        {
            if (id == -1)
                return HttpNotFound();

            Initalize(id);
            LoadMessages();

            if (!game.Started || game.Ended)
                return View("Lobby", game);

            return View(game);
        }

        public ActionResult Create()
        {
            if (!LoggedIn)
                return Redirect("/");

            return View(game);
        }

        [HttpPost]
        public ActionResult Create(Game model)
        {
            if (ModelState.IsValid)
            {
                GameServer.SaveNewGame(model);

                model.Join(Account.Id, Account.Name, Account.Rating);
                GameServer.PlayerJoined(model, Account.Id);

                return Redirect("/Game-" + model.Id + "/");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        public ActionResult Join(int id)
        {
            Initalize(id);

            LoadMessages();

            if (!LoggedIn)
            {
                ViewBag.ErrorMessage = "You must be logged in to join the game.";
                return View("Index", game);
            }

            if (game.GetPlayer(Account.Id) != null)
            {
                ViewBag.ErrorMessage = "You are already part of the game.";
                return View("Index", game);
            }

            if (game.Started)
            {
                ViewBag.ErrorMessage = "Game already started.";
                return View("Index", game);
            }

            if (game.IsPrivate)
            {
                var invite = (from i in game.Invites where i.AccountId == Account.Id select i).FirstOrDefault();
                if (invite == null)
                {
                    ViewBag.ErrorMessage = "You must be invited to a private game.";
                    return View("Index", game);
                }
            }

            game.Join(Account.Id, Account.Name, Account.Rating);
            GameServer.PlayerJoined(game, Account.Id);

            Initalize(id);

            LoadMessages();

            return View("Index", game);
        }

        public ActionResult Invite(int id, string inviteEmail)
        {
            Initalize(id);

            if (!String.IsNullOrEmpty(inviteEmail))
            {
                var invites = inviteEmail.Split(',', '\n');
                foreach (var invite in invites)
                {
                    var trimmedInvite = invite.Trim();
                    if (!String.IsNullOrEmpty(trimmedInvite))
                    {
                        using (var db = new DBConnection())
                        {
                            var account = FindAccount(trimmedInvite);
                            bool accountCreated = false;
                            if (account == null)
                            {
                                int accountId;
                                AddErrorMessage(CreateAccount(trimmedInvite, game.Id, out accountId));
                                accountCreated = true;
                                account = Account.Load(db.EvaluateRow("select * from account where id = {0}", accountId));
                            }

                            if (account != null)
                            {
                                // Check for existing invite
                                if ((from i in game.Invites where i.AccountId == account.Id select i).Count() > 0)
                                {
                                    AddErrorMessage(account.Name + " has already been invited to this game.");
                                    continue;
                                }

                                if ((from p in game.Players where p.AccountId == account.Id select p).Count() > 0)
                                {
                                    AddErrorMessage(account.Name + " is already playing this game.");
                                    continue;
                                }

                                game.Invites.Add(new Invite() { AccountId = account.Id, Name = account.Name });
                                game.SendForumMessage(String.Format("{0} invited {1} to this game.", Account.Name, account.Name));
                                GameServer.PlayerInvited(game, account);

                                if (!accountCreated)
                                {
                                    GameServer.SendMessage(db, account.Id, Account.Id, Account.Name, String.Format(
@"You've been challenged to a game of {3} by {0}.

Visit http://{1}/Game-{2}/ to view the details and join the game.
", Account.Name, Request.Url.Host, game.Id, HtmlUtils.SiteName));

                                }
                            }
                        }
                    }
                }
            }

            LoadMessages();

            return View("Index", game);
        }

        void AddErrorMessage(string errorMessage)
        {
            if (ViewBag.ErrorMessage != null)
            {
                ViewBag.ErrorMessage += "<br />" + errorMessage;
            }
            else
                ViewBag.ErrorMessage = errorMessage;
        }

        public ActionResult Error(string message)
        {
            ViewBag.ErrorMessage = message;
            return View("Index", game);
        }

        public ActionResult Quit(int id)
        {
            Initalize(id);

            if (!LoggedIn || !IsPlaying)
                return Redirect("/Game-" + game.Id + "/");

            if (game.Started)
                game.EliminatePlayer(player);
            else
            {
                game.Unjoin(player);
                GameServer.PlayerUnjoined(game, Account.Id);

                if (game.Players.Count <= 0)
                    return Redirect("/");
            }

            Initalize(id);

            LoadMessages();

            return View("Index", game);
        }

        //public ActionResult Kick(int id, int playerNumber)
        //{
        //    Initalize(id);

        //    if (!LoggedIn)
        //        return Redirect("/Game-" + game.Id + "/");

        //    if (!game.Started && IsHost)
        //    {
        //        var kickPlayer = game.GetPlayerByNumber(playerNumber);
        //        if (kickPlayer != null)
        //        {
        //            game.Unjoin(kickPlayer);
        //            GameServer.PlayerUnjoined(game, kickPlayer.AccountId);

        //            if (game.Players.Count <= 0)
        //                return Redirect("/");
        //        }
        //    }

        //    Initalize(id);

        //    LoadMessages();

        //    return View("Index", game);
        //}

        public ActionResult Start(int id)
        {
            Initalize(id);

            if (!LoggedIn)
                return Redirect("/Game-" + game.Id + "/");

            if (!game.IsRunning || player.Station == Station.GameMaster)

            game.MaxPlayers = game.CurrentPlayers;
            game.Start();

            Initalize(id);

            LoadMessages();

            return View("Index", game);
        }

        [ValidateInput(false)]
        public string Send(int id, string message)
        {
            if (String.IsNullOrWhiteSpace(message))
                return null;

            Initalize(id);

            message = HttpUtility.HtmlEncode(message);
            game.SendForumMessage(message, Account.Id, Account.Name);

            return null;
        }

        public string SelectStation(int id, Station station)
        {
            Initalize(id);

            if (game.DefaultShip != null)
            {
                foreach (var shipPlayer in game.DefaultShip.Players)
                {
                    if (shipPlayer.Station == station)
                        return "Station already taken.";
                }

                player.Station = station;
            }

            return null;
        }

        public string SetImpulse(int id, int amount)
        {
            Initalize(id);

            if (game.DefaultShip != null && player.Station == Station.Helm)
            {
                game.DefaultShip.ImpulsePercentage = amount;
            }

            return null;
        }


        public ActionResult SetDesiredHeading(int id, float amount)
        {
            Initalize(id);

            if (game.DefaultShip != null && player.Station == Station.Helm)
            {
                game.DefaultShip.DesiredOrientation = amount;
            }

            return null;
        }

        public ActionResult SetDesiredSpeed(int id, int amount)
        {
            Initalize(id);

            if (game.DefaultShip != null && player.Station == Station.Helm)
                game.DefaultShip.TargetSpeedMetersPerSecond = amount;

            return null;
        }

        #region Weapons
        public ActionResult ToggleShields(int id)
        {
            Initalize(id);

            if (game.DefaultShip != null && player.Station == Station.Weapons)
                game.DefaultShip.ToggleShields();

            return null;
        }

        public ActionResult LaunchProjectile(int id, int targetId)
        {
            Initalize(id);

            if (game.DefaultShip != null && player.Station == Station.Weapons)
                game.DefaultShip.ToggleShields();

            return null;
        }
        #endregion

        #region GM
        public ActionResult BuildBase(int id)
        {
            Initalize(id);

            if (game.DefaultShip != null && player.Station == Station.GameMaster)
                game.DefaultShip.StarSystem.AddEntity(new Starbase() { Position = game.DefaultShip.Position });

            return null;
        }

        public ActionResult BuildShip(int id)
        {
            Initalize(id);

            if (game.DefaultShip != null && player.Station == Station.GameMaster)
                game.DefaultShip.StarSystem.AddEntity(new Ship() { Position = game.DefaultShip.Position });

            return null;
        }
        #endregion
    }
}