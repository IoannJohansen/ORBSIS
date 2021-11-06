using Microsoft.AspNetCore.SignalR;
using System.Collections;
using System.Threading.Tasks;

namespace ORBSIS.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessageToClients(string message, string author)
        {
            await Clients.Others.SendAsync("Notify", message, author);
        }

        public async Task ProcessUserChoice(string message)
        {
            await SendMessageToClients(message, Context.User.Identity.Name);
        }
    }
}
