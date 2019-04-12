using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System;
using PlanningPoker.Models;
using System.Linq;

namespace PlanningPoker.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(int RoomId, int UserId, string Context)
        {
            string playername;
            using (var _context = new PokerPlanningContext())
            {
                var messagetodb = new Message()
                {
                    Context = Context,
                    CreateDate = DateTime.Now,
                    PokerRoomId = RoomId,
                    PlayerId = UserId
                };
                await _context.AddAsync<Message>(messagetodb);
                await _context.SaveChangesAsync();
                playername = _context.Players.FirstOrDefault(pl => pl.Id == UserId).Name.ToString();
            }
            await this.Clients.Groups($"{RoomId}").SendAsync("ForwardToClients", playername, Context);
        }
        public async Task JoinGroup(int groupName)
        {
          await this.Groups.AddToGroupAsync(this.Context.ConnectionId, groupName.ToString());
        }
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
    }
}
