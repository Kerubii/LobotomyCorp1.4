using LobotomyCorp.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Buffs
{
	public class Predator : ModBuff
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Alerted");
            // Description.SetDefault("What careless fool stepped on my baby?");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<LobotomyTethPlayer>().RedEyesPredator = true;
        }

        public override bool RightClick(int buffIndex)
        {
            return false;
        }
    }
}