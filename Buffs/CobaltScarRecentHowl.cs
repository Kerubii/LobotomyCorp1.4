using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Buffs
{
	public class CobaltScarRecentHowl : ModBuff
	{
		//This debuff applies after using the ability for 8 seconds. 
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Howl Cooldown");
			// Description.SetDefault("Everyone heard you, now it's time to feast.");
			Main.debuff[Type] = true;
			BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
		}
		

    }
	//Done.
	//maybe add a damage buff?
}