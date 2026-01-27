//css_ref ../../tModLoader.dll
using LobotomyCorp.NPCs.RedMist;
using LobotomyCorp.Projectiles.Realized;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace LobotomyCorp.Misc.Dusts
{
    public class RedMistSCene : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossLow;

        private static int redmistIndex = 0;

        public override int Music => GetRedMistMusic();

        private int GetRedMistMusic()
        {
            int index = NPC.FindFirstNPC(ModContent.NPCType<RedMist>());
            if (index != -1)
            {
                int Phase = (int)Main.npc[index].ai[0];
                if (Phase > 0)
                {
                    if (Phase < 3)
                        return MusicLoader.GetMusicSlot(Mod, "Sounds/Music/TilaridsDistortedNight");
                    else
                        return MusicLoader.GetMusicSlot(Mod, "Sounds/Music/TilaridsInsigniaDecay");
                }
            }            
            return MusicLoader.GetMusicSlot(Mod, "Sounds/Music/PMSecondWarning");
        }

        public override bool IsSceneEffectActive(Player player)
        {
            if (NPC.AnyNPCs(ModContent.NPCType<RedMist>()))
                return true;
            return false;
        }

        private float filterProgress;

        public override void SpecialVisuals(Player player, bool isActive)
        {
            //player.ManageSpecialBiomeVisuals("LobotomyCorp:RedMistOverlay", isActive);
            if (isActive)
            {
                redmistIndex = NPC.FindFirstNPC(ModContent.NPCType<RedMist>());
                NPC n = Main.npc[redmistIndex];
                RedMist rm = n.ModNPC as RedMist;
                if (!Filters.Scene["LobotomyCorp:RedMistOverlay"].IsActive())
                {
                    Filters.Scene.Activate("LobotomyCorp:RedMistOverlay");
                }
                else
                {
                    float distance = n.Distance(Main.LocalPlayer.Center);
                    float minDist = 120;
                    float maxDist = 250;
                    rm.bubbleShieldRange((int)n.ai[0], ref minDist, ref maxDist);

                    distance = (distance - minDist) / (maxDist - minDist);
                    distance = Math.Clamp(distance, 0.1f, 1f);

                    Filters.Scene["LobotomyCorp:RedMistOverlay"].GetShader().UseIntensity(distance);

                    
                    filterProgress += 1f / 180f;
                    if (filterProgress > 1f)
                        filterProgress -= 1f;

                    Filters.Scene["LobotomyCorp:RedMistOverlay"].GetShader().UseProgress(filterProgress);
                }
            }
            else
            {
                Filters.Scene["LobotomyCorp:RedMistOverlay"].Deactivate();
            }        
        }
    }
}