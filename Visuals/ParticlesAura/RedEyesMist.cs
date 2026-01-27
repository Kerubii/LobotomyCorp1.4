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
    public class RedEyesMist : AuraBehavior
    {
        public bool SpawnCond => Main.timeForVisualEffects % 2 == 0;

        public int Layer => 3;

        public string GetTexture => "CrossHatched";

        public Rectangle GetSourceRect(Texture2D texture, int index, int time)
        {
            return texture.Frame(4, 1, index);
        }

        public Color GetColor(PlayerDrawSet drawInfo, AuraParticle particle) { return new Color(25, 20, 28, 200); }

        public void SpawnParam(Player player, int dir, float gravDir, float time, AuraParticle particle, int index)
        {
            particle.textureIndex = 3;
            particle.Velocity = new Vector2(40, 0).RotatedBy(Main.rand.NextFloat(6.28f));
            particle.Rotation = Main.rand.NextFloat(6.28f);
            particle.Scale = 1f;
        }

        public void Behavior(Player player, int dir, float gravDir, float time, AuraParticle particle)
        {
            particle.Position = particle.Velocity + player.MountedCenter;
            particle.Position.Y -= 12;
            particle.Velocity *= 0.8f;
            if (particle.particleTime > 20)
            {
                particle.Active = false;
            }
        }
    }
}