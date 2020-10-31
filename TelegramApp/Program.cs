using System;
using System.Linq;
using System.Threading.Tasks;
using TeleSharp.TL;
using TeleSharp.TL.Messages;
using TLSharp.Core;
using TLSharp.Core.Exceptions;

namespace TelegramApp
{
    class Program
    {
        public static int _apiID = 1901606;
        public static string _apiHash = "db347a73b1919868fa2ff1bfc4fb76dd";
        public static string _number = "+79887687607";
        public static string _pass = "283647";
        public static int _targetUser = 1385739983;
        public static string _targetName = "OMGBOT";
        public static string _numberUser = "+79966307012";


        static async Task Main(string[] args)
        {
            Console.WriteLine("Запуск приложения!");

            Console.WriteLine("Попытка создания клиента");
            var client = NewClient();

            var user = await Connect(client);


            if (user != null && client.IsUserAuthorized())
            {
                Console.WriteLine("Пользователь получен!");
            }

            var dialogs = await GetDialogs(client);

            //var targetUser = result.Users.OfType<TLUser>().FirstOrDefault(x => x.Id == _targetUser);
            var targetUser = dialogs.Users.OfType<TLUser>().FirstOrDefault(x => x.Phone == _numberUser);
            if (targetUser != null)
            {
                var result = await SendMessage(client, targetUser);

            }
            
        }

        private static TelegramClient NewClient()
        {
            try
            {
                return new TelegramClient(_apiID, _apiHash);
            }
            catch (MissingApiConfigurationException ex)
            {
                throw new Exception($"Please add your API settings to the `app.config` file. (More info: {MissingApiConfigurationException.InfoUrl})",
                    ex);
            }
        }

        private static async Task<TLUser> Connect(TelegramClient client)
        {
            await client.ConnectAsync();

            var hash = await client.SendCodeRequestAsync(_number);

            Console.WriteLine("Ввести код из приложения: ");
            var code = Console.ReadLine();

            if (String.IsNullOrWhiteSpace(code))
            {
                Console.WriteLine("CodeToAuthenticate is empty in the app.config file, fill it with the code you just got now by SMS/Telegram");
            }

            Console.WriteLine("Попытка получения пользователя");
            TLUser user = null;
            try
            {
                user = await client.MakeAuthAsync(_number, hash, code);
            }
            catch (CloudPasswordNeededException ex)
            {
                var passwordSetting = await client.GetPasswordSetting();
                var password = _pass;

                user = await client.MakeAuthWithPasswordAsync(passwordSetting, password);
            }
            catch (InvalidPhoneCodeException ex)
            {
                Console.WriteLine("CodeToAuthenticate is wrong in the app.config file, fill it with the code you just got now by SMS/Telegram",
                    ex);
            }

            return user;
        }

        private static async Task<TLDialogs> GetDialogs(TelegramClient client)
        {
            try
            {
                return (TLDialogs)await client.GetUserDialogsAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static async Task<bool> SendMessage(TelegramClient client, TLUser targetUser)
        {
            try
            {
                await client.SendMessageAsync(
                    new TLInputPeerChannel() { ChannelId = targetUser.Id, AccessHash = targetUser.AccessHash.Value },
                    "Test msg");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            return true;
        }
    }
}
