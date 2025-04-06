using LobotomyCorp.Players;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Buffs
{
	public class Pleasure : ModBuff
	{
        public override void SetStaticDefaults()
        {
        // DisplayName.SetDefault("Pleasure");
			// Description.SetDefault("Pleasant feelings fills your head");
            Main.debuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
		
		public override void Update(NPC npc, ref int BuffIndex)
		{
			npc.loveStruck = true;
			LobotomyGlobalNPC.LNPC(npc).PleasureDebuff = true;
		}

        public override void Update(Player player, ref int buffIndex)
        {
			player.loveStruck = true;
            player.GetModPlayer<LobotomyWawPlayer>().PleasureDebuff = true;
        }
    }
}