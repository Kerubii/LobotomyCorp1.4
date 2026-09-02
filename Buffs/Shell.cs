using LobotomyCorp.Players;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace LobotomyCorp.Buffs
{
	public class Shell : ModBuff
	{
        public override void SetStaticDefaults()
        {
            //Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        /* override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            LobotomyModPlayer modPlayer = LobotomyModPlayer.ModPlayer(Main.LocalPlayer);

            tip += "\n" + Language.GetTextValue("Mods.LobotomyCorp.Buffs.Shell.Description2", modPlayer.MimicryBonusHealth, (int)(modPlayer.MimicryBonusDamage * 100), modPlayer.MimicryBonusDefense);
        }*/

        public override void Update(Player player, ref int buffIndex)
        {
            LobotomyAlephPlayer modPlayer = player.GetModPlayer<LobotomyAlephPlayer>();
            modPlayer.MimicryShell = true;

            player.statLifeMax2 += modPlayer.MimicryBonusHealth;
            player.statDefense += 10;
            //player.GetDamage(DamageClass.Generic) += modPlayer.MimicryBonusDamage;

            if (player.HeldItem.type == ModContent.ItemType<Items.Ruina.Language.MimicryR>())
                player.buffTime[buffIndex] += 1;

            /*
            if (player.buffTime[buffIndex] <= 0)
                player.AddBuff(ModContent.BuffType<Husk>(), 2);
            */
        }
    }
}