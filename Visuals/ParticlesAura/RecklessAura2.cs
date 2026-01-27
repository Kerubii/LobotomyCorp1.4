using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.Localization;
using LobotomyCorp.PlayerDrawEffects;
using LobotomyCorp.Players;

namespace LobotomyCorp.Visuals.ParticlesAura
{
    public class RecklessAura2 : AuraBehavior
    {
        public bool SpawnCond => Main.timeForVisualEffects % 3 == 0;

        public int Layer => 2;

        public string GetTexture => "FlameParticlesL";

        public Rectangle GetSourceRect(Texture2D texture, int index, int time)
        {
            return texture.Frame(4, 1, index);
        }

        public Color GetColor(PlayerDrawSet drawInfo, AuraParticle particle) { return Color.Red * 0.9f * (1f - particle.particleTime / 13f); }

        public void SpawnParam(Player player, int dir, float gravDir, float time, AuraParticle particle, int index)
        {
            particle.textureIndex = Main.rand.Next(4);
            particle.Position.Y -= 12;
            particle.Position.X += (8 * (float)Math.Sin(0.436332f * time));
            particle.Velocity.Y -= 2f;
            particle.Rotation = Main.rand.NextFloat(6.28f);
            particle.Scale = 1f;
        }

        public void Behavior(Player player, int dir, float gravDir, float time, AuraParticle particle)
        {
            particle.Position.Y += particle.Velocity.Y;
            particle.Scale -= 0.04f;

            if (particle.particleTime > 10)
            {
                particle.Active = false;
            }
        }
    }
}