using Terraria;
using Terraria.ModLoader;

namespace LobotomyCorp.Buffs
{
	public class CobaltScarScars : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Scars");
			// Description.SetDefault("The scars of the past");
			Main.debuff[Type] = true;
		}
		
		public override void Update(NPC npc, ref int BuffIndex)
		{ 
			LobotomyGlobalNPC.LNPC(npc).CobaltScarScars = true;
			int Scars = LobotomyGlobalNPC.LNPC(npc).CobaltScarScarsAmount;

			if (npc.buffTime[BuffIndex] <= 4)
			{
				npc.buffTime[BuffIndex] = 240;
				//I did make it so stacks are refreshed on hit and all of them are gone when they are no longer being applied... Or did i?

			}

		}

        public override void Update(Player player, ref int buffIndex)
        {
			if (player.itemAnimation == player.itemAnimationMax)//Not sure what this does, honestly.
            {

            }
        }
    }
}

//Editing the Following: This file, LobotomyGlobalNPC, CobaltScarR.cs,CobaltScarRSlash.cs CobaltScarWolf/Consuming/FailedConsume , (and so on)