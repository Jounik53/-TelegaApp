using System;
using System.Collections.Generic;
using System.Text;

namespace TelegramApp.Models
{
    public class ConfigModel
    {
        public int ApiId { get; set; }
        public string ApiHash { get; set; }
        public string NumberToAuthenticate { get; set; }
        public string PasswordToAuthenticate { get; set; }
        public int TargetUserID { get; set; }
        public string Message { get; set; }
    }
}
