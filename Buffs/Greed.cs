using LobotomyCorp.Players;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace LobotomyCorp.Buffs
{
	public class Greed : ModBuff
	{
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<LobotomyAlephPlayer>().GoldRushGreed = true;
        }
    }
}