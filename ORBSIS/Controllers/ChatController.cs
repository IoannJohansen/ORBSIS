﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ORBSIS.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ORBSIS.Controllers
{
    public class ChatController : Controller
    {
        private static string LastMessage { get; set; }
        private static string LastMessageAuthor { get; set; }
        private static DateTime LastMessageTime { get; set; }
        private static int CountBro { get; set; }
        private static int CountSis { get; set; }
        //SignInManager<IdentityUser> _signInManager;

        public ChatController(/*SignInManager<IdentityUser> signInManager*/)
        {
            //this._signInManager = signInManager;
        }

        public IActionResult Index()
        {
            var messagesData = new ChatViewModel();
            messagesData.CountBro = CountBro;
            messagesData.CountSis = CountSis;
            messagesData.LastMessage = LastMessage;
            messagesData.LastMessageAuthor = LastMessageAuthor;
            messagesData.LastMessageTime = LastMessageTime.ToLongTimeString();
            messagesData.UserSigned = /*_signInManager.IsSignedIn(User)*/true;
            return View(messagesData);
        }

        [HttpPost]
        //[Authorize]
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
            return RedirectToAction("Index");
        }

        [Route("User/LoginFacebook")]
        public IActionResult LoginFacebook()
        {
            return View();
        }

    }
}