using Microsoft.AspNetCore.SignalR;
using PlanningPoker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PlanningPoker
{
    public class VotingHub : Hub
    {
        public void JoinGroup(string groupName)
        {
            this.Groups.AddToGroupAsync(this.Context.ConnectionId, groupName);
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("EntranceUser");
           // await Clients.Caller.SendAsync("UpdatePage"); //при подключении другого пользователя - обновление (для отображения списка онлайн)
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Clients.All.SendAsync("ExitUser");
            await base.OnDisconnectedAsync(exception);
        }

        public void OnlineUser(int UserId)
        {
            using (var _context = new PokerPlanningContext())
            {
                var Player = _context.Players.Where(p => p.Id == UserId).SingleOrDefault();
                Player.IsOnline = true;

                _context.Players.Update(Player);
                _context.SaveChanges();
            }
        }

        public void OfflineUser(int RoomId)
        {
            using (var _context = new PokerPlanningContext())
            {
                var Player = _context.Players.Where(p => p.PokerRoomId == RoomId).ToList();
                foreach (var player in Player)
                {
                    player.IsOnline = false;
                }
                _context.Players.UpdateRange(Player);
                _context.SaveChanges();
            }
        }
    }
}
