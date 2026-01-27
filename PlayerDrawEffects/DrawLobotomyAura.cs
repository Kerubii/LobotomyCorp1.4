using LobotomyCorp.Visuals.ParticlesAura;
using LobotomyCorp.Utils;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace LobotomyCorp.PlayerDrawEffects
{
    public class LobotomyPlayerParticle : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.MountBack);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            return !Main.gameMenu;
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            LobotomyModPlayer modPlayer = LobotomyModPlayer.ModPlayer(drawInfo.drawPlayer);
            AuraParticle[] particle = modPlayer.PlayerParticles;
            List<AuraParticle>[] layer = new List<AuraParticle>[3];
            layer[0] = new List<AuraParticle>();
            layer[1] = new List<AuraParticle>();
            layer[2] = new List<AuraParticle>();
            if (particle != null)
            {
                for (int i = 0; i < particle.Length; i++)
                {
                    if (particle[i] != null && particle[i].Active && particle[i].GetAuraBehavior.Layer > 0)
                    {
                        layer[particle[i].GetAuraBehavior.Layer - 1].Add(particle[i]);
                    }
                }
                for (int i = 2; i >= 0; i--)
                {
                    if (layer[i].Count > 0)
                    {
                        foreach (AuraParticle part in layer[i])
                        {
                            drawInfo.DrawDataCache.Add(part.Draw(ref drawInfo, Mod));
                        }
                    }
                }
            }
        }

        public static void GenerateAuraParticle(LobotomyModPlayer modPlayer, AuraBehavior auraUsed)
        {
            Player player = modPlayer.Player;
            for (int i = 0; i < modPlayer.PlayerParticles.Length; i++)
            {
                AuraParticle particle = modPlayer.PlayerParticles[i];
                if (particle == null || !particle.Active)
                {
                    modPlayer.PlayerParticles[i] = new AuraParticle(modPlayer.Player, player.direction, player.gravDir, (float)Main.timeForVisualEffects, auraUsed, i);
                    break;
                }
            }
        }

        public static void GeneratePlayerParticle(LobotomyModPlayer modPlayer, AuraBehavior particleBehavior)
        {
            Player player = modPlayer.Player;
            for (int i = 0; i < modPlayer.PlayerParticles.Length; i++)
            {
                AuraParticle particle = modPlayer.PlayerParticles[i];
                if (particle == null || !particle.Active)
                {
                    modPlayer.PlayerParticles[i] = new AuraParticle(modPlayer.Player, player.direction, player.gravDir, (float)Main.timeForVisualEffects, particleBehavior, i);
                    break;
                }
            }
        }

        public static void GenerateDirectParticle(LobotomyModPlayer modPlayer, AuraBehavior particleBehavior, Vector2 position, Vector2 velocity, float rot, float scale, int texFrameX)
        {
            Player player = modPlayer.Player;
            for (int i = 0; i < modPlayer.PlayerParticles.Length; i++)
            {
                AuraParticle particle = modPlayer.PlayerParticles[i];
                if (particle == null || !particle.Active)
                {
                    modPlayer.PlayerParticles[i] = new AuraParticle(position, velocity, rot, scale, texFrameX, player.direction, particleBehavior);
                    break;
                }
            }
        }
    }

    public class LobotomyPlayerParticleFront : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.LastVanillaLayer);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            return !Main.gameMenu;
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            LobotomyModPlayer modPlayer = LobotomyModPlayer.ModPlayer(drawInfo.drawPlayer);
            AuraParticle[] particle = modPlayer.PlayerParticles;
            if (particle != null)
            {
                for (int i = 0; i < particle.Length; i++)
                {
                    if (particle[i] != null && particle[i].Active && particle[i].GetAuraBehavior.Layer == 0)
                    {
                        drawInfo.DrawDataCache.Add(particle[i].Draw(ref drawInfo, Mod));
                    }
                }
            }
        }
    }
}
