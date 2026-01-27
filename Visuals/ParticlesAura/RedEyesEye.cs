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
    public class RedEyesEye : AuraBehavior
    {
        public bool SpawnCond => Main.timeForVisualEffects % 10 == 0;

        public int Layer => 0;

        public string GetTexture => "RedEyesEye";

        public Rectangle GetSourceRect(Texture2D texture, int index, int time)
        {
            return texture.Frame(4, 1, index);
        }

        public Color GetColor(PlayerDrawSet drawInfo, AuraParticle particle) { return Color.White; }

        public void SpawnParam(Player player, int dir, float gravDir, float time, AuraParticle particle, int index)
        {
            particle.textureIndex = 0;
            int box = 20;
            particle.Velocity = new Vector2(Main.rand.Next(box), Main.rand.Next(box));
            particle.Velocity.X = Main.rand.NextBool(2) ? -box : 0;
            particle.Velocity.Y = Main.rand.NextBool(2) ? -box : 0;
            particle.Rotation = 0;
            particle.Scale = Main.rand.NextFloat(0.2f, 0.5f);
        }

        public void Behavior(Player player, int dir, float gravDir, float time, AuraParticle particle)
        {
            particle.Position = particle.Velocity + player.MountedCenter;

            if (particle.particleTime == 4 || particle.particleTime == 8 || particle.particleTime == 12)
                particle.textureIndex++;
            else if (particle.particleTime == 22)
                particle.textureIndex = 1;
            else if (particle.particleTime == 26)
                particle.textureIndex--;

            else if (particle.particleTime > 30)
            {
                particle.Active = false;
            }
        }
    }
}