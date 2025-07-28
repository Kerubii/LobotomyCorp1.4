using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Buffs
{
	public class BrokenBliss : ModBuff
	{
		public override void SetStaticDefaults()
		{
            BuffID.Sets.IsATagBuff[Type] = true;
            Main.debuff[Type] = true;
		}
		
		public override void Update(NPC npc, ref int BuffIndex)
		{
			LobotomyGlobalNPC.LNPC(npc).GoldRushBrokenBlissBuff = true;
		}
    }
}