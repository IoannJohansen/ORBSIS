using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ORBSIS.Hubs;
using ORBSIS.Model.ViewModel;
using System;
using System.Threading.Tasks;

namespace ORBSIS.Controllers
{
    public class ChatController : Controller
    {
        private static string LastMessage { get; set; } = "...";

        private static string LastMessageAuthor { get; set; } = "Noname";
        
        private static DateTime LastMessageTime { get; set; }
        
        private static int CountBro { get; set; }

        private static int CountSis { get; set; }

        private IHubContext<ChatHub> _chatHub;

        public ChatController(IHubContext<ChatHub> chatHub)
        {
            _chatHub = chatHub;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(new ChatViewModel()
            {
                CountBro = CountBro,
                CountSis = CountSis,
                LastMessage = LastMessage,
                LastMessageAuthor = LastMessageAuthor,
                LastMessageTime = LastMessageTime.ToLongTimeString(),
                UserSigned = User.Identity.IsAuthenticated
            });
        }

        [HttpPost]
        public async Task<IActionResult> ProcessMessage(string button)
        {
            if (button == "bro")
            {
                LastMessage = $"Bro!";
                CountBro++;
            }
            else
            {
                LastMessage = $"Sis!";
                CountSis++;
            }
            LastMessageTime = DateTime.Now;
            LastMessageAuthor = HttpContext.User.Identity.Name;
            await SendMessageToClient(LastMessage, LastMessageAuthor);
            return RedirectToAction("Index");
        }
        
        public async Task SendMessageToClient(string message, string author)
        {
            await _chatHub.Clients.All.SendAsync("Send", message, author, LastMessageTime.ToLongTimeString());
        }
    }
}
