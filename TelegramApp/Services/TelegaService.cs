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
        private static int _apiID;
        private static string _apiHash;
        private static string _numberAccount;
        public static string _passTelegram;

        public TelegaService(int apiID, string apiHash, string numberAccount, string passTelegram)
        {
            _apiID = apiID;
            _apiHash = apiHash;
            _numberAccount = numberAccount;
            _passTelegram = passTelegram;
        }


        public TelegramClient NewClient()
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

            var hash = await client.SendCodeRequestAsync(_numberAccount);

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
                user = await client.MakeAuthAsync(_numberAccount, hash, code);
            }
            catch (CloudPasswordNeededException ex)
            {
                var passwordSetting = await client.GetPasswordSetting();
                var password = _passTelegram;

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
