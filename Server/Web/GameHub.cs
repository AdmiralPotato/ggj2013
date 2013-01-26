using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace WebGame
{
    public class GameHub : Hub
    {
        static bool groupsWorking;

        static Dictionary<string, Player> PlayersByConnectionId = new Dictionary<string,Player>();

        static string GetShipGroupName(int gameId, int shipId)
        {
            return "Ship-" + gameId.ToString() + "-" + shipId.ToString();
        }

        public override Task OnConnected()
        {
            var sessionCookie = Context.RequestCookies["ASP.Net_SessionId"];
            if (sessionCookie != null)
                Groups.Add(Context.ConnectionId, sessionCookie.Value);

            Uri referrer;
            if (Uri.TryCreate(Context.Headers["Referer"], UriKind.RelativeOrAbsolute, out referrer))
            {
                if (referrer.Segments.Length > 1 && referrer.Segments[1].StartsWith("Game-"))
                {
                    int gameId;
                    if (Int32.TryParse(referrer.Segments[1].TrimEnd('/').Substring("Game-".Length), out gameId))
                    {
                        var defaultShip = GameServer.GetGame(gameId).DefaultShip;
                        if (defaultShip != null)
                        {
                            //Groups.Add(Context.ConnectionId, GetShipGroupName(gameId, defaultShip.Id));
                            //groupsWorking = true;

                            SetShip(gameId, defaultShip.Id, true);
                        }
                    }
                }
            }

            return base.OnConnected();
        }

        public override Task OnDisconnected()
        {
            if (PlayersByConnectionId.ContainsKey(Context.ConnectionId))
            {
                var player = PlayersByConnectionId[Context.ConnectionId];
                player.Game.DisconnectPlayer(player);
            }
            return base.OnDisconnected();
        }

        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }

        //public Task SetGroup(int gameId)
        //{
        //    this.
        //    Caller.GameId = gameId;
        //    Caller.GroupName = "Game-" + gameId;
        //    return Groups.Add(Context.ConnectionId, Caller.GroupName);
        //}

        public Task SetShip(int gameId, int shipId, bool useGroups)
        {
            var game = GameServer.GetGame(gameId);
            if (game != null)
            {
                var player = game.ConnectPlayer(Context.ConnectionId, Context.RequestCookies["ASP.Net_SessionId"].Value);
                if (player != null)
                    PlayersByConnectionId[Context.ConnectionId] = player;

                if (useGroups)
                {
                    //Caller.ShipId = shipId;
                    //Caller.GroupName = GetShipGroupName(gameId, shipId);
                    groupsWorking = true;
                    return Groups.Add(Context.ConnectionId, GetShipGroupName(gameId, shipId));
                }
            }
            return null;
        }

        public Task Disconnecting(int gameId, int shipId)
        {
            var game = GameServer.GetGame(gameId);
            if (game != null)
            {
                game.DisconnectPlayer(Context.ConnectionId, Context.RequestCookies["ASP.Net_SessionId"].Value);

                //GameServer.GetGame(gameId);
                if (groupsWorking)
                    return Groups.Remove(Context.ConnectionId, GetShipGroupName(gameId, shipId));
            }
            return null;
        }

        public void TestThrow()
        {
            throw new NotImplementedException();
        }

        internal static void Say(int gameId, string message)
        {
            Say(GameServer.GetGame(gameId), message);
        }

        internal static void Say(Game game, string message)
        {
            if (game != null && game.DefaultShip != null)
            {
                var groupName = GetShipGroupName(game.Id, game.DefaultShip.Id);

                var context = GlobalHost.ConnectionManager.GetHubContext<GameHub>();
                context.Clients.Group(groupName).addMessage(0, "System", message);
            }
        }

        public static void SendUpdate(int gameId, int shipId, UpdateToClient update)
        {
            var groupName = GetShipGroupName(gameId, shipId);

            var context = GlobalHost.ConnectionManager.GetHubContext<GameHub>();

            //if (groupsWorking)
            //{
            //    var group = context.Clients.Group(groupName);
            //    group.handleUpdate(update);
            //}
            //else
                context.Clients.All.handleUpdate(update);
        }

        public static void Refresh(string group)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<GameHub>();
            context.Clients.Group(group).reload();
        }

        public static void SendMessage(string sessionKey, int sourceId, string sourceName, string text)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<GameHub>();
            context.Clients.Group(sessionKey).recieveMessage(sourceId, sourceName, text);
        }

        public static void SendNotification(string sessionKey, string title, string text, string targetUri = "")
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<GameHub>();
            context.Clients.Group(sessionKey).sendNotification(title, text, targetUri);
        }
    }
}