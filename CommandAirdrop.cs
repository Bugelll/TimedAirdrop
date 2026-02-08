using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Emqo.TimedAirdrop
{
    public class CommandAirdrop : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "callair";
        public string Help => "召唤空投到地图随机位置";
        public string Syntax => "";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string>();

        public static Dictionary<CSteamID, DateTime> Cooldowns = new Dictionary<CSteamID, DateTime>();

        private static readonly FieldInfo AirdropNodesField = typeof(LevelManager).GetField("airdropNodes", BindingFlags.Static | BindingFlags.NonPublic);

        public void Execute(IRocketPlayer caller, string[] command)
        {
            var player = (UnturnedPlayer)caller;
            var steamId = player.CSteamID;

            bool isSvip = player.HasPermission("timedairdrop.svip");
            bool isVip = player.HasPermission("timedairdrop.vip");

            if (!isSvip && !isVip)
            {
                UnturnedChat.Say(player, "你没有权限使用此命令！", UnityEngine.Color.red);
                return;
            }

            int cooldownHours = isSvip ? 4 : 12;

            if (Cooldowns.TryGetValue(steamId, out DateTime lastUse))
            {
                var remaining = lastUse.AddHours(cooldownHours) - DateTime.Now;
                if (remaining.TotalSeconds > 0)
                {
                    UnturnedChat.Say(player, $"冷却中！剩余时间: {remaining.Hours}小时{remaining.Minutes}分钟", UnityEngine.Color.yellow);
                    return;
                }
            }

            var nodes = AirdropNodesField.GetValue(null) as List<AirdropDevkitNode>;
            if (nodes == null || nodes.Count == 0)
            {
                UnturnedChat.Say(player, "地图没有空投点！", UnityEngine.Color.red);
                return;
            }

            var node = nodes[UnityEngine.Random.Range(0, nodes.Count)];
            var point = node.transform.position;

            LevelManager.airdrop(point, node.id, 128f);
            Cooldowns[steamId] = DateTime.Now;
            UnturnedChat.Say(player, $"空投将投放在 ({point.x:F0}, {point.y:F0}, {point.z:F0}) 坐标", UnityEngine.Color.green);
        }
    }
}
