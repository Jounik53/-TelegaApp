using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeleSharp.TL;
using TeleSharp.TL.Contacts;
using TeleSharp.TL.Messages;
using TLSharp.Core;
using TLSharp.Core.Exceptions;

namespace TelegramApp.Services
{
    class TelegaService: ITelegaService
    {
        private static int ApiId;
        private static string ApiHash;
        private static string NumberToAuthenticate;
        private static string PasswordToAuthenticate;

        public TelegaService(int apiID, string apiHash, string numberToAuthenticate, string passwordToAuthenticate)
        {
            ApiId = apiID;
            ApiHash = apiHash;
            NumberToAuthenticate = numberToAuthenticate;
            PasswordToAuthenticate = passwordToAuthenticate;
        }


        public TelegramClient NewClient()
        {
            try
            {
                return new TelegramClient(ApiId, ApiHash);
            }
            catch (MissingApiConfigurationException ex)
            {
                throw new Exception($"Please add your API settings to the `app.config` file. (More info: {MissingApiConfigurationException.InfoUrl})",
                    ex);
            }
        }

        public TelegramClient NewClient(int apiId, string apiHash)
        {
            try
            {
                return new TelegramClient(apiId, apiHash);
            }
            catch (MissingApiConfigurationException ex)
            {
                throw new Exception($"Please add your API settings to the `app.config` file. (More info: {MissingApiConfigurationException.InfoUrl})",
                    ex);
            }
        }

        public async Task<TLUser> Connect(TelegramClient client)
        {
            await client.ConnectAsync();

            var hash = await client.SendCodeRequestAsync(NumberToAuthenticate);

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
                user = await client.MakeAuthAsync(NumberToAuthenticate, hash, code);
            }
            catch (CloudPasswordNeededException ex)
            {
                var passwordSetting = await client.GetPasswordSetting();
                var password = PasswordToAuthenticate;

                user = await client.MakeAuthWithPasswordAsync(passwordSetting, password);
            }
            catch (InvalidPhoneCodeException ex)
            {
                Console.WriteLine("CodeToAuthenticate is wrong in the app.config file, fill it with the code you just got now by SMS/Telegram",
                    ex);
            }

            return user;
        }

        public async Task<TLDialogs> GetDialogs(TelegramClient client)
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

        public async Task<TLContacts> GetContactAsync(TelegramClient client)
        {
            try
            {
                return await client.GetContactsAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<bool> SendMessage(TelegramClient client, TLUser targetUser, string message)
        {
            try
            {
                await client.SendMessageAsync(
                    new TLInputPeerUser() { UserId = targetUser.Id, AccessHash = targetUser.AccessHash.Value },
                    message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            return true;
        }

        #region Private utils methods
        private static string GetNormalizeNumber(string number)
        {
            return number.StartsWith("+") ?
                number.Substring(1, number.Length - 1) :
                number;
        }
        #endregion
    }
}
