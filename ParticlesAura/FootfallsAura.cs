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

namespace LobotomyCorp.ParticlesAura
{
    public class FootfallsAura : AuraBehavior
    {
        public bool SpawnCond => true;

        public int Layer => 3;

        public Texture2D GetTexture(Mod mod) { return mod.Assets.Request<Texture2D>("ParticlesAura/CrossHatched").Value; }

        public Rectangle GetSourceRect(Texture2D texture, int index, int time)
        {
            return texture.Frame(5, 1, index);
        }

        public Color GetColor(PlayerDrawSet drawInfo, AuraParticle particle)
        {
            Color color = Color.Lerp(Color.DarkGray, Color.Black, (particle.particleTime / 20f)) * 0.9f;
            if (particle.particleTime > 5)
                color *= 1f - (particle.particleTime - 5) / 15f;
            return color;
        }

        public void SpawnParam(Player player, int dir, float gravDir, float time, AuraParticle particle, int index)
        {
            particle.textureIndex = Main.rand.Next(5);
            particle.Position.Y -= 10;
            particle.Velocity = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));

            particle.Rotation = Main.rand.NextFloat(6.28f);
            particle.Scale = 1f;
        }

        public void Behavior(Player player, int dir, float gravDir, float time, AuraParticle particle)
        {
            particle.Position += particle.Velocity;
            particle.Scale -= 0.02f;

            if (particle.particleTime > 20f)
            {
                particle.Active = false;
            }
        }
    }
}