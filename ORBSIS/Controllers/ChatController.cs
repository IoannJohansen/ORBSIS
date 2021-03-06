using Microsoft.AspNetCore.Mvc;
using ORBSIS.Model.ViewModel;
using Serilog;
using System;

namespace ORBSIS.Controllers
{
    public class ChatController : Controller
    {
        private static string LastMessage { get; set; } = null;

        private static string LastMessageAuthor { get; set; }
        
        private static DateTime LastMessageTime { get; set; }
        
        private static int CountBro { get; set; }

        private static int CountSis { get; set; }

        [HttpGet]
        public IActionResult Index()
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
        public IActionResult ProcessMessage(string button)
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
            return RedirectToAction("Index");
        }
    }
}
