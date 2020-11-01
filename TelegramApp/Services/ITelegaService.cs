using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeleSharp.TL;
using TeleSharp.TL.Contacts;
using TeleSharp.TL.Messages;
using TLSharp.Core;

namespace TelegramApp.Services
{
    interface ITelegaService
    {
        TelegramClient NewClient();
        TelegramClient NewClient(int apiId, string apiHash);
        Task<TLUser> Connect(TelegramClient client);
        Task<TLDialogs> GetDialogs(TelegramClient client);
        Task<TLContacts> GetContactAsync(TelegramClient client);
        Task<bool> SendMessage(TelegramClient client, TLUser targetUser, string message);
    }
}
