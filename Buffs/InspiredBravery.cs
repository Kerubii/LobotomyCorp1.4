using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.DataStructures;
using LobotomyCorp.PlayerDrawEffects;
using LobotomyCorp.Players;
using LobotomyCorp.ParticlesAura;

namespace LobotomyCorp.Buffs
{
	public class InspiredBravery : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Inspired Bravery");
			// Description.SetDefault("10% increased movement speed, melee speed and counter damage");
			BuffID.Sets.TimeLeftDoesNotDecrease[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetAttackSpeed(DamageClass.Melee) += 0.1f;
			player.moveSpeed += 0.1f;
            player.GetModPlayer<LobotomyHePlayer>().LifeForADareDevilGiftActive = true;
            LobotomyModPlayer.ModPlayer(player).CurrentAura.Add(new InspiredAura());
            if (player.GetModPlayer<LobotomyHePlayer>().LifeForADareDevilGift < 1200)
            {
                player.AddBuff(ModContent.BuffType<RecklessFoolishness>(), 10);
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}