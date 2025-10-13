using System;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Audio;
using Terraria.Localization;
using LobotomyCorp.Players;

namespace LobotomyCorp.Buffs
{
	public class NettleClothing : ModBuff
	{
		public override void SetStaticDefaults()
        {
			// DisplayName.SetDefault("Clothing of Nettle");
			// Description.SetDefault("Nettle regenerating, 75% reduced damage and Attackers also take damage");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        /*
        Holding BlackSwan gains following effects
        1 - 15% increased attack speed
        2 - 20% increased movement speed
        3 - 20% increased reflection damage
        4 - 100% increased true melee damage
        5 - Attacks and reflected projectiles inflict Ichor and Gooey Waste
        6 - Reduces 100% damage taken and extended invulnerability
        */
        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            float NettleCount = Main.LocalPlayer.GetModPlayer<LobotomyWawPlayer>().BlackSwanNettleClothing;
            if (NettleCount < 1)
                tip += "\n" + Language.GetTextValue("Mods.LobotomyCorp.Buffs.NettleClothing.Description2");
            else
                tip += "\n" + Language.GetTextValue("Mods.LobotomyCorp.Buffs.NettleClothing.Tooltip1");
            if (NettleCount >= 2)
                tip += "\n" + Language.GetTextValue("Mods.LobotomyCorp.Buffs.NettleClothing.Tooltip2");
            if (NettleCount >= 3)
                tip += "\n" + Language.GetTextValue("Mods.LobotomyCorp.Buffs.NettleClothing.Tooltip3");
            if (NettleCount >= 4)
                tip += "\n" + Language.GetTextValue("Mods.LobotomyCorp.Buffs.NettleClothing.Tooltip4");
            if (NettleCount >= 5)
                tip += "\n" + Language.GetTextValue("Mods.LobotomyCorp.Buffs.NettleClothing.Tooltip5");
            if (NettleCount >= 6)
                tip += "\n" + Language.GetTextValue("Mods.LobotomyCorp.Buffs.NettleClothing.Tooltip6");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            LobotomyWawPlayer modPlayer = player.GetModPlayer<LobotomyWawPlayer>();
            if (modPlayer.BlackSwanNettleClothing < 6)
            {
                modPlayer.BlackSwanNettleAdd(0.00333f);
            }
            else
                modPlayer.BlackSwanNettleClothing = 6;

            if ((int)Math.Floor(modPlayer.BlackSwanNettleClothing) >= 2 && player.HeldItem.type == ModContent.ItemType<Items.Ruina.Literature.BlackSwanR>())
                player.moveSpeed += 0.2f;

            player.buffTime[buffIndex] = 10;
            if (modPlayer.BlackSwanBrokenDream)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }

        public override bool RightClick(int buffIndex)
        {
            return false;
        }
    }
}