using System;
using System.Collections.Generic;
using System.Text;

namespace Try4
{
    class Content
    {
        public string Username { get; set; }
        public string Message { get; set; }
        public string Time { get; set; }
        //public string Key { get; set; }
        public Content()
        {
            this.Username = "system";
            this.Message = "Start of conversation.";
            Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        public Content(string u, string c)
        {
            this.Username = u;
            this.Message = c;
            Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
