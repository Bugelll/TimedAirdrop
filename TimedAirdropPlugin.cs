using Newtonsoft.Json;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;

namespace Emqo.TimedAirdrop
{
    public class TimedAirdropPlugin : RocketPlugin<TimedAirdropConfiguration>
    {
        private string CooldownsFilePath => Path.Combine(Directory, "cooldowns.json");

        protected override void Load()
        {
            LoadCooldowns();
            Logger.Log($"{Name} {Assembly.GetName().Version.ToString(3)} has been loaded!");
        }

        protected override void Unload()
        {
            SaveCooldowns();
            CommandAirdrop.Cooldowns.Clear();
            Logger.Log($"{Name} has been unloaded!");
        }

        private void LoadCooldowns()
        {
            if (!File.Exists(CooldownsFilePath)) return;
            try
            {
                var json = File.ReadAllText(CooldownsFilePath);
                var data = JsonConvert.DeserializeObject<Dictionary<ulong, DateTime>>(json);
                if (data != null)
                {
                    foreach (var kvp in data)
                        CommandAirdrop.Cooldowns[new CSteamID(kvp.Key)] = kvp.Value;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to load cooldowns: {ex.Message}");
            }
        }

        private void SaveCooldowns()
        {
            try
            {
                var data = new Dictionary<ulong, DateTime>();
                foreach (var kvp in CommandAirdrop.Cooldowns)
                    data[kvp.Key.m_SteamID] = kvp.Value;
                File.WriteAllText(CooldownsFilePath, JsonConvert.SerializeObject(data, Formatting.Indented));
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to save cooldowns: {ex.Message}");
            }
        }
    }
}