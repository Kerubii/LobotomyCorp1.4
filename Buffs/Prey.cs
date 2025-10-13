using LobotomyCorp.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Buffs
{
	public class Prey : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.debuff[Type] = true;
		}

        public override void Update(Player player, ref int buffIndex)
        {
			player.GetModPlayer<LobotomyWawPlayer>().CrimsonScarPrey = true;
        }

        public override void Update(NPC npc, ref int BuffIndex)
		{
			npc.GetGlobalNPC<LobotomyGlobalNPC>().CrimsonScarPrey = true;
		}
    }
}