using LobotomyCorp.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Buffs
{
	public class Scars : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Scars");
			// Description.SetDefault("A wound that will never heal");
            Main.debuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
		
		public override void Update(NPC npc, ref int BuffIndex)
		{
            npc.buffTime[BuffIndex] = 10;
			LobotomyGlobalNPC.LNPC(npc).WristCutterScars = true;
		}

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<LobotomyTethPlayer>().WristCutterScars = true;
            player.buffTime[buffIndex] = 10;
        }
    }
}