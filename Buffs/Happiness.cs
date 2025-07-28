using LobotomyCorp.Players;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace LobotomyCorp.Buffs
{
	public class Happiness : ModBuff
	{
        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            int hap = Main.LocalPlayer.GetModPlayer<LobotomyAlephPlayer>().GoldRushHappiness;
            if (hap > 0)
            {
                int dmg = (int)(0.05f * hap * 100);
                tip += $"\n{Language.GetTextValue("Mods.LobotomyCorp.Buffs.Happiness.Tooltip1", dmg, hap)}";
            }
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Generic) += 0.05f * player.GetModPlayer<LobotomyAlephPlayer>().GoldRushHappiness;
            player.GetModPlayer<LobotomyAlephPlayer>().GoldRushHappinessBuff = true;
        }
    }
}