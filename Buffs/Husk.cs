using LobotomyCorp.Players;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace LobotomyCorp.Buffs
{
	public class Husk : ModBuff
	{
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            //BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        /*
        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            LobotomyModPlayer modPlayer = LobotomyModPlayer.ModPlayer(Main.LocalPlayer);

            tip += "\n" + Language.GetTextValue("Mods.LobotomyCorp.Buffs.Husk.Description2", modPlayer.MimicryShellHealth, modPlayer.MimicryShellDamage, modPlayer.MimicryShellDefense);
        }
        */
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<LobotomyAlephPlayer>().MimicryHusk = true;
            //player.statDefense -= 8;
            //LobotomyModPlayer.ModPlayer(player).MimicryHuskDeficit = player.buffTime[buffIndex];
            //if (player.HeldItem.type == ModContent.ItemType<Items.Ruina.Language.MimicryR>())
                //player.buffTime[buffIndex] = 60 * 5;
        }
    }
}