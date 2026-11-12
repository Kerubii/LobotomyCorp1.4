using LobotomyCorp.Players;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Buffs
{//If it naturally expires, which is 6 seconds of dealing with doubled incoming damage, increase WolfSated by 4 which triggers the CobaltScarWolf buff and resets it to 0.
//If the user takes Maxhealth/4 damage during the duration of the debuff, apply CobaltScarFailedConsume and remove this debuff, in that order.
	public class CobaltScarConsuming : ModBuff
	{
		
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Consuming");
			// Description.SetDefault("I was fated to be bad… It doesn’t matter… I don’t have to care…");
			Main.debuff[Type] = true;
			BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<LobotomyWawPlayer>().CobaltScarConsuming = true;

            if (player.GetModPlayer<LobotomyWawPlayer>().CobaltScarConsumingDamageTaken > (player.statLifeMax2 * 0.25))
            {
				player.GetModPlayer<LobotomyWawPlayer>().CobaltScarConsumingDamageTaken = 0; //Resets the instanced value here, since we fucked up.
                player.AddBuff(ModContent.BuffType<CobaltScarFailedConsume>(), 600);
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }

    }

}