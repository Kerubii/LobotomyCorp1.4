using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Buffs
{
	public class Bliss : ModBuff
	{
		public override void SetStaticDefaults()
		{
			BuffID.Sets.IsATagBuff[Type] = true;
		}
		
		public override void Update(NPC npc, ref int BuffIndex)
		{
			LobotomyGlobalNPC.LNPC(npc).GoldRushBlissBuff = true;
			npc.buffTime[BuffIndex] = 50;
			if (npc.HasBuff(ModContent.BuffType<BrokenBliss>()))
			{
				npc.DelBuff(BuffIndex);
				BuffIndex--;
			}
		}
    }
}