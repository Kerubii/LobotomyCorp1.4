using LobotomyCorp.Players;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Buffs
{
	public class Matchstick : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Matchstick");
			// Description.SetDefault("Reduce to ashes...");
            Main.debuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
		
		public override void Update(NPC npc, ref int BuffIndex)
        { 
            LobotomyGlobalNPC.LNPC(npc).MatchstickBurn = true;
            npc.oiled = true;
		}

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<LobotomyTethPlayer>().MatchstickBurn = true;
        }
    }
}