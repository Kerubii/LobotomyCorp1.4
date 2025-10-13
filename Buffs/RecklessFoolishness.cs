using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.Localization;
using LobotomyCorp.PlayerDrawEffects;
using LobotomyCorp.Players;
using LobotomyCorp.ParticlesAura;

namespace LobotomyCorp.Buffs
{
	public class RecklessFoolishness : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Reckless Foolishness");
			// Description.SetDefault("");
			BuffID.Sets.TimeLeftDoesNotDecrease[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = true;
        }

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            int gift = Main.LocalPlayer.GetModPlayer<LobotomyHePlayer>().LifeForADareDevilGift;
            if (gift < 600)
            {
                tip = Language.GetTextValue("Mods.LobotomyCorp.Buffs.RecklessFoolishness.Description2");
            }
            else
                tip = Language.GetTextValue("Mods.LobotomyCorp.Buffs.RecklessFoolishness.Description");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetAttackSpeed(DamageClass.Melee) += 0.15f;
			player.moveSpeed += 0.15f;

            player.GetModPlayer<LobotomyHePlayer>().LifeForADareDevilGiftActive = true;
            int gift = player.GetModPlayer<LobotomyHePlayer>().LifeForADareDevilGift;
            if (gift < 600)
            {
                player.endurance -= 0.15f;
                LobotomyModPlayer.ModPlayer(player).CurrentAura.Add(new RecklessAura2());
            }
            else if (gift < 1200)
            {
                player.endurance -= 0.1f;
                LobotomyModPlayer.ModPlayer(player).CurrentAura.Add(new RecklessAura1());
            }
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}