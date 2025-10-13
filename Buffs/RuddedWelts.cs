using LobotomyCorp.Items.Ruina.Language;
using LobotomyCorp.Players;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace LobotomyCorp.Buffs
{
	public class RuddedWelts : ModBuff
	{
		public override void SetStaticDefaults()
		{
		}
        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            int boost = (int)(statBoost(Main.LocalPlayer) * 100);
            if (Main.LocalPlayer.HeldItem.type != ModContent.ItemType<CrimsonScarR>())
                boost /= 2;
            tip += $"\n{Language.GetTextValue("Mods.LobotomyCorp.Buffs.RuddedWelts.Description2", boost)}";
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<LobotomyWawPlayer>().CrimsonScarRuddedWelts = true;
            float boost = statBoost(player);
            if (player.HeldItem.type != ModContent.ItemType<CrimsonScarR>())
                boost /= 2;
            player.GetDamage(DamageClass.Generic) += boost;
            player.endurance += boost;
        }

        private float statBoost(Player player)
        {
            float playerhealth = 1f - player.statLife / (float)player.statLifeMax2;
            float stat = (float)Math.Min(0.5f, playerhealth * 0.666f);
            return stat;
        }
    }
}