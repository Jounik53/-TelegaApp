using System;
using System.Linq;
using System.Threading.Tasks;
using TelegramApp.Services;
using TeleSharp.TL;
using TeleSharp.TL.Contacts;
using TeleSharp.TL.Messages;
using TLSharp.Core;
using TLSharp.Core.Exceptions;
using System.Configuration;
using System.Diagnostics;

namespace TelegramApp
{
    class Program
    {
        private static string ApiHash { get; set; }
        private static int ApiId { get; set; }
        private static string NumberToAuthenticate { get; set; }
        private static string PasswordToAuthenticate { get; set; }
        private static int TargetUserId { get; set; }
        private static string Message { get; set; }

        public static TelegaService _service;
        

        static async Task Main(string[] args)
        {
            Console.WriteLine("Start app");

            //Getting configuration
            GetConfig();

            //Initializing and getting a client
            var client = await Initialization();

            //Getting target user
            var targetUser = await GetTargetUser(client);

            var result = await _service.SendMessage(client, targetUser, Message);

            if (result)
            {
                Console.WriteLine("Sent");
            }
            
        }

        #region Initialization and Get client
        private static async Task<TelegramClient> Initialization()
        {
            _service = new TelegaService(ApiId, ApiHash, NumberToAuthenticate, PasswordToAuthenticate);

            Console.WriteLine("Attempting to create a client...");
            var client = _service.NewClient();

            try
            {
                var user = await _service.Connect(client);

                Console.WriteLine("User received!");

                return client;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }
        }
        #endregion

        #region GetConfig
        private static void GetConfig()
        {
            string appConfigMsgWarning = "{0} not configured in app.config! Some tests may fail.";

            var apiId = ConfigurationManager.AppSettings[nameof(ApiId)];
            if (string.IsNullOrEmpty(apiId))
                Debug.WriteLine(appConfigMsgWarning, nameof(ApiId));
            else
                ApiId = int.Parse(apiId);

            ApiHash = ConfigurationManager.AppSettings[nameof(ApiHash)];
            if (string.IsNullOrEmpty(ApiHash))
                Debug.WriteLine(appConfigMsgWarning, nameof(ApiHash));

            NumberToAuthenticate = ConfigurationManager.AppSettings[nameof(NumberToAuthenticate)];
            if (string.IsNullOrEmpty(NumberToAuthenticate))
                Debug.WriteLine(appConfigMsgWarning, nameof(NumberToAuthenticate));

            PasswordToAuthenticate = ConfigurationManager.AppSettings[nameof(PasswordToAuthenticate)];
            if (string.IsNullOrEmpty(PasswordToAuthenticate))
                Debug.WriteLine(appConfigMsgWarning, nameof(PasswordToAuthenticate));

            var targetUserID = ConfigurationManager.AppSettings[nameof(TargetUserId)];
            if (string.IsNullOrEmpty(apiId))
                Debug.WriteLine(appConfigMsgWarning, nameof(TargetUserId));
            else
                TargetUserId = int.Parse(targetUserID);

            Message = ConfigurationManager.AppSettings[nameof(Message)];
            if (string.IsNullOrEmpty(Message))
                Debug.WriteLine(appConfigMsgWarning, nameof(Message));

        }
        #endregion

        private static async Task<TLUser> GetTargetUser(TelegramClient client)
        {
            var dialogs = await _service.GetDialogs(client);

            var targetUser = dialogs.Users.OfType<TLUser>().FirstOrDefault(x => x.Id == TargetUserId);

            if (targetUser != null)
            {
                return targetUser;
            }
            else
            {
                Debug.WriteLine("Error getting target user");
                return null;
            }
        }

    }
}
