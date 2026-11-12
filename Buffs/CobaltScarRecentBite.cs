using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Buffs
{
	//Applies after a sucessful hit on a boss but NOT a regular enemy, for 2-3 seconds. Get +1 on WolfSated every time you get this debuff.
	//Endurance -= 0.15f and reduced movement speed(maybe?) so you can't just bite EoC for free buffs?
	public class CobaltScarRecentBite : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Bite Cooldown");
			// Description.SetDefault("You just recently bit something big. Yum.");
			Main.debuff[Type] = true;
			BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.endurance -= 0.15f; //Adjust this as needed.
		}
		

    }
}