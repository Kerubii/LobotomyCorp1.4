using LobotomyCorp.Players;
using LobotomyCorp.Projectiles.Realized;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace LobotomyCorp.Buffs
{
	public class SilentMusic : ModBuff
	{
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            int phase = Main.LocalPlayer.GetModPlayer<LobotomyAlephPlayer>().DaCapoSilentMusicPhase;
            if (phase > 4)
                tip += "\n" + Language.GetTextValue("Mods.LobotomyCorp.Buffs.SilentMusic.Tooltip1Alt");
            else
                tip += "\n" + Language.GetTextValue("Mods.LobotomyCorp.Buffs.SilentMusic.Tooltip1");
            if (phase % 5 > 0)
                tip += "\n" + Language.GetTextValue("Mods.LobotomyCorp.Buffs.SilentMusic.Tooltip2");
            if (phase % 5 > 1)
                tip += "\n" + Language.GetTextValue("Mods.LobotomyCorp.Buffs.SilentMusic.Tooltip3");
            if (phase % 5 > 2)
            {
                if (phase > 4)
                    tip += "\n" + Language.GetTextValue("Mods.LobotomyCorp.Buffs.SilentMusic.Tooltip4Alt");
                else
                    tip += "\n" + Language.GetTextValue("Mods.LobotomyCorp.Buffs.SilentMusic.Tooltip4");
            }
        }

        public override bool RightClick(int buffIndex)
        {
            Player player = Main.LocalPlayer;
            if (player.townNPCs > 2)
            {
                player.GetModPlayer<LobotomyAlephPlayer>().DaCapoSilentMusic = false;
                player.GetModPlayer<LobotomyAlephPlayer>().DaCapoSilentMusicPhase = 0;
                foreach (Projectile p in Main.ActiveProjectiles)
                {
                    if (p.type == ModContent.ProjectileType<DaCapoPerformance>() && p.owner == player.whoAmI)
                        p.Kill();
                }
                return base.RightClick(buffIndex);
            }
            return false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            LobotomyAlephPlayer modPlayer = player.GetModPlayer<LobotomyAlephPlayer>();

            modPlayer.DaCapoSilentMusic = true;
            int phase = modPlayer.DaCapoSilentMusicPhase;
            if (phase > 4)
                player.statDefense -= 10;
            if (phase % 5 > 0)
            {
                player.moveSpeed += .1f;
            }
            if (phase % 5 > 1)
            {
                player.GetAttackSpeed(DamageClass.Melee) += .12f;
                player.GetDamage(DamageClass.Generic) += .12f;
            }
            if (phase % 5 > 2)
            {
                player.endurance += .15f;
            }
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<LobotomyGlobalNPC>().DaCapoSilentMusic = true;
            int phase = npc.GetGlobalNPC<LobotomyGlobalNPC>().DaCapoSilentMusicPhase;
        }
    }
}