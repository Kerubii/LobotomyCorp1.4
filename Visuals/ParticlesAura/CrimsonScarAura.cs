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
    public class CrimsonScarAura : AuraBehavior
    {
        public bool SpawnCond => true;

        public int Layer => 3;

        public string GetTexture => "FlameParticles";

        public Rectangle GetSourceRect(Texture2D texture, int index, int time)
        {
            return texture.Frame(5, 1, index);
        }

        public Color GetColor(PlayerDrawSet drawInfo, AuraParticle particle)
        {
            Color color = Color.Lerp(Color.Red, Color.Red * 0.4f, (particle.particleTime / 60f)) * 0.2f;
            return color;
        }

        public void SpawnParam(Player player, int dir, float gravDir, float time, AuraParticle particle, int index)
        {
            particle.textureIndex = Main.rand.Next(5);
            particle.Velocity = new Vector2(Main.rand.Next(-12, 12), player.height / 2 - 6);

            particle.Rotation = Main.rand.NextFloat(6.28f);
            particle.Scale = Main.rand.NextFloat(0.9f, 1.2f);
        }

        public void Behavior(Player player, int dir, float gravDir, float time, AuraParticle particle)
        {
            particle.Position = player.Center + player.velocity + particle.Velocity;
            particle.Velocity.Y -= 0.8f;
            if (particle.particleTime > 60f)
            {
                particle.Active = false;
            }
        }
    }
}