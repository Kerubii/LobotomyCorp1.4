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
    public interface AuraBehavior
    {
        /// <summary>
        /// When does this aura spawns on the player every frame
        /// </summary>
        bool SpawnCond { get; }

        /// <summary>
        /// Null if none
        /// </summary>
        //CustomShaderData shaderData { get ; }

        /// <summary>
        /// Layer 0 is Front, Layer 1-3 goes from Front to Back, 1 has priority Textures while 3 uses Smoke-like effects
        /// </summary>
        int Layer { get; }

        /// <summary>
        /// Use { return mod.Assets.Request<Texture2D>("Folder/File").Value; }
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        Texture2D GetTexture(Mod mod);
        Rectangle GetSourceRect(Texture2D texture, int index, int time);
        Color GetColor(PlayerDrawSet drawInfo, AuraParticle particle);

        /// <summary>
        /// By default, Particles spawn on the player's center
        /// </summary>
        /// <param name="player"></param>
        /// <param name="dir"></param>
        /// <param name="gravDir"></param>
        /// <param name="time"></param>
        /// <param name="particle"></param>
        /// <param name="index"></param>
        void SpawnParam(Player player, int dir, float gravDir, float time, AuraParticle particle, int index);
        void Behavior(Player player, int dir, float gravDir, float time, AuraParticle particle);
    }
}