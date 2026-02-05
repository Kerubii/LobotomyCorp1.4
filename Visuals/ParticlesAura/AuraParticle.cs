using LobotomyCorp.PlayerDrawEffects;
using LobotomyCorp.Players;
using LobotomyCorp.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace LobotomyCorp.Visuals.ParticlesAura
{
    public class AuraParticle
    {
        public bool Active;
        private bool local;
        public bool IsLocal { get { return local; } }
        public Vector2 Position;
        public Vector2 Velocity;
        public float Rotation;
        public float Scale;
        public int textureIndex;
        public int particleTime;
        AuraBehavior behavior;

        private Asset<Texture2D> particleTex => ModContent.Request<Texture2D>((GetType().Namespace + "." + behavior.GetTexture).Replace(".", "/"));

        public AuraBehavior GetAuraBehavior { get { return behavior; } }
        /*        
        public Vector2 Position { get { return position; } }
        public float Rotation { get { return rotation; } }
        public float Scale { get { return scale; } }
        */

        public AuraParticle(Player player, int dir, float gravDir, float time, AuraBehavior newBehavior, int index, bool isLocal = false)
        {
            behavior = newBehavior;
            Active = true;
            Position = player.Center;
            Velocity = Vector2.Zero;
            Rotation = 0;
            Scale = 1f;
            textureIndex = 0;
            particleTime = 0;
            local = isLocal;
            behavior.SpawnParam(player, dir, gravDir, time, this, index);
        }

        /// <summary>
        /// Ignores SpawnParam caused by AuraBehavior
        /// </summary>
        /// <param name="position"></param>
        /// <param name="velocity"></param>
        /// <param name="dir"></param>
        /// <param name="newBehavior"></param>
        /// <param name="index"></param>
        /// <param name="isLocal"></param>
        public AuraParticle(Vector2 position, Vector2 velocity, float rotation, float scale, int texIndex, int dir, AuraBehavior newBehavior, bool isLocal = false)
        {
            behavior = newBehavior;
            Active = true;
            Position = position;
            Velocity = velocity;
            Rotation = rotation;
            Scale = scale;
            textureIndex = texIndex;
            particleTime = 0;
            local = isLocal;
        }

        public void Update(Player player, int dir, float gravDir, float time)
        {
            behavior.Behavior(player, dir, gravDir, time, this);
            particleTime++;
        }

        public DrawData Draw(ref PlayerDrawSet drawInfo, Mod mod)
        {
            Texture2D tex = particleTex.Value;
            Rectangle frame = behavior.GetSourceRect(tex, textureIndex, particleTime);

            DrawData data = new DrawData(
                tex,
                Position - Main.screenPosition,
                frame,
                behavior.GetColor(drawInfo, this),
                Rotation,
                frame.Size() / 2,
                Scale,
                0,
                0);

            return data;
        }
    }
}