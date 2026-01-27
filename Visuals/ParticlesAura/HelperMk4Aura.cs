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

namespace LobotomyCorp.Visuals.ParticlesAura
{
    public class HelperMk4Aura : AuraBehavior
    {
        public bool SpawnCond => Main.timeForVisualEffects % 5 == 2;

        public int Layer => 0;

        public string GetTexture => "Lightning";

        public Rectangle GetSourceRect(Texture2D texture, int index, int time)
        {
            int frameY = (int)(time / 4);
            return texture.Frame(5, 5, index, frameY);
        }

        public Color GetColor(PlayerDrawSet drawInfo, AuraParticle particle)
        {
            Color color = Color.Yellow;
            return color;
        }

        public void SpawnParam(Player player, int dir, float gravDir, float time, AuraParticle particle, int index)
        {
            particle.textureIndex = Main.rand.Next(5);
            particle.Position = player.position + new Vector2(Main.rand.Next(player.width), Main.rand.Next(player.height));
            particle.Velocity = new Vector2(Main.rand.Next(-2, 3), Main.rand.Next(-2, 3));

            particle.Rotation = Main.rand.NextFloat(6.28f);
            particle.Scale = Main.rand.NextFloat(0.2f, 0.7f);
        }

        public void Behavior(Player player, int dir, float gravDir, float time, AuraParticle particle)
        {
            particle.Position += particle.Velocity;
            if (particle.particleTime > 20f)
            {
                particle.Active = false;
            }
        }
    }
}