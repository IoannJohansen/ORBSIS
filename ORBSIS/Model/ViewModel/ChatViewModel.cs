using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ORBSIS.Model.ViewModel
{
    public class ChatViewModel
    {
        public int CountBro { get; set; }
        public int CountSis { get; set; }
        public string LastMessage { get; set; }
        public string LastMessageAuthor { get; set; }
        public string LastMessageTime { get; set; }
        public bool UserSigned { get; set; }
    }
}
