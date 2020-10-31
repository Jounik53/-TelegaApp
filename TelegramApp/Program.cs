using System;
using System.Linq;
using System.Threading.Tasks;
using TelegramApp.Services;
using TeleSharp.TL;
using TeleSharp.TL.Contacts;
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
        public static string _passTelegramm = "283647";
        public static int _targetUser = 1385739983;
        public static string _targetName = "OMGBOT";
        public static string _message = "💎Забрать бонус";
        

        static async Task Main(string[] args)
        {
            var service = new TelegaService(_apiID, _apiHash);
            TLUser targetUser;
            Console.WriteLine("Запуск приложения!");

            Console.WriteLine("Попытка создания клиента");
            var client = service.NewClient();

            var user = await service.Connect(client);


            if (user != null && client.IsUserAuthorized())
            {
                Console.WriteLine("Пользователь получен!");
            }


            var dialogs = await service.GetDialogs(client);

            targetUser = dialogs.Users.OfType<TLUser>().FirstOrDefault(x => x.Id == _targetUser);

            if (targetUser != null)
            {
                var result = await service.SendMessage(client, targetUser, _message);
            }
        }
    }
}
