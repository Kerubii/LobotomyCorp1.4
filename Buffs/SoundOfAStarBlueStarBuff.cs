using LobotomyCorp.Players;
using LobotomyCorp.Projectiles;
using LobotomyCorp.Projectiles.Realized;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace LobotomyCorp.Buffs
{
    public class SoundOfAStarBlueStarBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true; // This buff won't save when you exit the world
            Main.buffNoTimeDisplay[Type] = true; // The time remaining won't display on this buff
        }

        public override void Update(Player player, ref int buffIndex)
        {
            // If the minions exist reset the buff time, otherwise remove the buff from the player
            int total = player.ownedProjectileCounts[ModContent.ProjectileType<SoundOfAStarBlueStar>()];
            if (total > 0)
            {
                player.buffTime[buffIndex] = 18000;
            }
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            tip += $"\n{Language.GetTextValue("Mods.LobotomyCorp.Buffs.SoundOfAStarBlueStarBuff.Description2", Main.LocalPlayer.GetModPlayer<LobotomyAlephPlayer>().SoundOfAStarBlueStarPower)}";
        }
    }
}