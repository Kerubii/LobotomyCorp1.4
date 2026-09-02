using LobotomyCorp.Visuals.ParticlesAura;
using LobotomyCorp.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Buffs
{
	public class FaintAromaUnwithering : ModBuff
	{
		public override void SetStaticDefaults()
		{
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<LobotomyWawPlayer>().FaintAromaUnwitheringFlower = true;
        }

        public override bool RightClick(int buffIndex)
        {
            return false;
        }
    }
}