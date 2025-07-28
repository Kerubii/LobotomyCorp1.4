using LobotomyCorp.Players;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace LobotomyCorp.Buffs
{
	public class RoadOfGold : ModBuff
	{
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<LobotomyAlephPlayer>().GoldRushRoadCooldown = true;
        }
    }
}