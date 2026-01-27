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
    public class DarkFlameAura : AuraBehavior
    {
        public bool SpawnCond => Main.timeForVisualEffects % 3 == 0;

        public int Layer => 3;

        public string GetTexture => "FlameParticlesL";

        public Rectangle GetSourceRect(Texture2D texture, int index, int time)
        {
            return texture.Frame(4, 1, index);
        }

        public Color GetColor(PlayerDrawSet drawInfo, AuraParticle particle)
        {
            Color color = Color.Lerp(Color.DarkBlue, Color.Black, (particle.particleTime / 20f)) * 0.9f;
            if (particle.particleTime > 5)
                color *= 1f - (particle.particleTime - 5) / 20f;
            return color;
        }

        public void SpawnParam(Player player, int dir, float gravDir, float time, AuraParticle particle, int index)
        {
            particle.textureIndex = Main.rand.Next(4);
            particle.Position.Y += player.height / 2 - 10;
            particle.Velocity = new Vector2(Main.rand.NextFloat(-2, 2), Main.rand.NextFloat(-2, -3));

            particle.Rotation = Main.rand.NextFloat(6.28f);
            particle.Scale = 1f;
        }

        public void Behavior(Player player, int dir, float gravDir, float time, AuraParticle particle)
        {
            particle.Position += particle.Velocity;
            particle.Scale -= 0.04f;
            particle.Velocity.Y *= 0.9f;
            particle.Velocity.X *= 0.95f;

            if (particle.particleTime > 20f)
            {
                particle.Active = false;
            }
        }
    }
}