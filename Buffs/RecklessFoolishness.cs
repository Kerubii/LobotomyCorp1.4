using LobotomyCorp.Items.Aleph;
using LobotomyCorp.PlayerDrawEffects;
using LobotomyCorp.Players;
using LobotomyCorp.Visuals.ParticlesAura;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace LobotomyCorp.Buffs
{
	public class RecklessFoolishness : ModBuff
	{
        public static LocalizedText Description { get; private set; }
        public static LocalizedText Description2 { get; private set; }

        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Reckless Foolishness");
			// Description.SetDefault("");
			BuffID.Sets.TimeLeftDoesNotDecrease[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = true;

            Description = this.GetLocalization("Description");
            Description2 = this.GetLocalization("Description2");
        }

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            int gift = Main.LocalPlayer.GetModPlayer<LobotomyHePlayer>().LifeForADareDevilGift;
            if (gift < 600)
            {
                tip = Description2.Value;
            }
            else
                tip = Description.Value;
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