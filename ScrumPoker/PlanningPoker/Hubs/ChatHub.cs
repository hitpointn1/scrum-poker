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


        public async Task JoinGroup(int RoomId)
        {
            using (var _context = new PokerPlanningContext())
            {
                var messageslist = _context.Messages
                    .Where(msg => msg.PokerRoomId == RoomId)
                    .Select(msg => new { msg.Context, msg.PlayerId, msg.CreateDate});

                if (!(messageslist is null))
                {
                    var playername = _context.Players
                    .Join(messageslist, pl => pl.Id, msg => msg.PlayerId,
                    (players, messages) => new { players.Name, messages.Context, messages.CreateDate })
                    .OrderBy(time => time.CreateDate);

                    foreach (var item in playername)
                        await this.Clients.Caller.SendAsync("ForwardToClients", item.Name, item.Context);
                }
            }
            await this.Groups.AddToGroupAsync(this.Context.ConnectionId, RoomId.ToString());
        }
    }
}
