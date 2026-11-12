using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Buffs
{//While having this debuff set WolfSated to 0. 
//Could make the player endurance be set player.endurance -= 0.15f
	public class CobaltScarFailedConsume : ModBuff
	{

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Role of The Wolf:Disabled");
			// Description.SetDefault("Ouch, I deserved that.");
			Main.debuff[Type] = true;
			BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
		}
		public override void Update(Player player, ref int buffIndex)
		{
			player.endurance -= 0.15f; //Adjust this as needed.
		}
    }

}