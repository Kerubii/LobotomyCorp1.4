using LobotomyCorp.PlayerDrawEffects;
using LobotomyCorp.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace LobotomyCorp.ParticlesAura
{
    public class LoveAndHateAura : AuraBehavior
    {
        public bool SpawnCond => Main.timeForVisualEffects % 30 == 6;

        public int Layer => 0;

        public Texture2D GetTexture(Mod mod) { return mod.Assets.Request<Texture2D>("ParticlesAura/Heart").Value; }

        public Rectangle GetSourceRect(Texture2D texture, int index, int time)
        {
            return texture.Frame();
        }

        public Color GetColor(PlayerDrawSet drawInfo, AuraParticle particle)
        {
            Color color = Color.Pink;
            float opacity = 1f;
            if (particle.particleTime > 40f)
            {
                opacity = 1f - (particle.particleTime - 40f) / 20f;
            }
            return color * opacity;
        }

        public void SpawnParam(Player player, int dir, float gravDir, float time, AuraParticle particle, int index)
        {
            particle.textureIndex = Main.rand.Next(5);
            particle.Velocity = new Vector2(Main.rand.Next(player.width + 40), Main.rand.Next(player.height / 2));

            particle.Scale = 0;
        }

        public void Behavior(Player player, int dir, float gravDir, float time, AuraParticle particle)
        {
            particle.Position = new Vector2(player.position.X - 20, player.Center.Y) + particle.Velocity;
            particle.Velocity.Y -= 0.4f;
            if (particle.particleTime < 10)
                particle.Scale += Main.rand.NextFloat(0.05f, 0.075f);

            if (particle.particleTime > 60)
            {
                particle.Active = false;
            }
        }
    }
}