using Microsoft.AspNetCore.SignalR;
using System.Collections;
using System.Threading.Tasks;

namespace ORBSIS.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessageToClient(string message, string author)
        {
            await Clients.Others.SendAsync("Send", message, author);
        }

        public async Task ProcessUserChoice(string message)
        {
            await SendMessageToClient(message, Context.User.Identity.Name);
        }
    }
}
