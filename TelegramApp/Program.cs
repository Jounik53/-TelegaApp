using System;
using System.Linq;
using System.Threading.Tasks;
using TelegramApp.Services;
using TeleSharp.TL;
using TLSharp.Core;
using System.Diagnostics;
using System.Configuration;
using System.Collections.Specialized;
using System.Timers;

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

        private static TelegramClient client { get; set; }
        private static TLUser targetUser { get; set; }

        public static TelegaService _telegaService;
        public static ConfigService _configService;

        private static Timer aTimer;
        private static int interval = 10800000;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Start app");

            _configService = new ConfigService();
            _configService.Initialization();

            //Getting configuration
            GetConfig();

            //Initializing and getting a client
            client = await Initialization();

            //Getting target user
            targetUser = await GetTargetUser(client);

            Console.WriteLine("Start send by timer: ");
            SetTimer();

        }

        #region Initialization and Get client
        private static async Task<TelegramClient> Initialization()
        {
            _telegaService = new TelegaService(ApiId, ApiHash, NumberToAuthenticate, PasswordToAuthenticate);

            Console.WriteLine("Attempting to create a client...");
            var client = _telegaService.NewClient();

            try
            {
                var user = await _telegaService.Connect(client);

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
            var config = _configService.GetAllSetting();

            ApiId = config.ApiId;

            ApiHash = config.ApiHash;
            if (string.IsNullOrEmpty(ApiHash))
                Debug.WriteLine(appConfigMsgWarning, nameof(ApiHash));

            NumberToAuthenticate = config.NumberToAuthenticate;
            if (string.IsNullOrEmpty(NumberToAuthenticate))
                Debug.WriteLine(appConfigMsgWarning, nameof(NumberToAuthenticate));

            PasswordToAuthenticate = config.PasswordToAuthenticate;
            if (string.IsNullOrEmpty(PasswordToAuthenticate))
                Debug.WriteLine(appConfigMsgWarning, nameof(PasswordToAuthenticate));

            TargetUserId = config.TargetUserID;

            Message = config.Message;
            if (string.IsNullOrEmpty(Message))
                Debug.WriteLine(appConfigMsgWarning, nameof(Message));

        }
        #endregion

        private static async Task<TLUser> GetTargetUser(TelegramClient client)
        {
            var dialogs = await _telegaService.GetDialogs(client);

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

        private static async Task SendMessageByInterval()
        {
            var result = await _telegaService.SendMessage(client, targetUser, Message);

            if (result)
            {
                Console.WriteLine("Sent");
            }
        }

        private static void SetTimer()
        {
            // Create a timer with a two second interval.
            aTimer = new System.Timers.Timer(interval);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            SendMessageByInterval().GetAwaiter().GetResult();
        }
    }
}
