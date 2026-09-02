using LobotomyCorp.Players;
using Terraria;
using Terraria.ModLoader;

namespace LobotomyCorp.Buffs
{
	public class Satiated : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Satiated");
			// Description.SetDefault("I CRAVE THIS MELODY");
		}

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<LobotomyHePlayer>().HarmonyConnected = true;
			if (player.buffTime[buffIndex] == 1)
			{
                player.GetModPlayer<LobotomyHePlayer>().HarmonyLoseRhythm(1);
            }
        }
    }
}