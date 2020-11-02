using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using TelegramApp.Models;

namespace TelegramApp.Services
{
    public class ConfigService
    {
        private readonly string FileConfigPath = @"App.Config.json";
        private ConfigModel AllSetting { get; set; }
        public ConfigService()
        {
        }

        public ConfigService(string fileConfigPath)
        {
            FileConfigPath = fileConfigPath;
        }

        public bool Initialization()
        {
            if (File.Exists(FileConfigPath))
            {
                try
                {
                    AllSetting = JsonConvert.DeserializeObject<ConfigModel>(File.ReadAllText(FileConfigPath));

                    if (AllSetting != null)
                    {
                        return true;
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    throw;
                }
            }

            return false;
        }

        public ConfigModel GetAllSetting()
        {
            return AllSetting;
        }
    }
}
