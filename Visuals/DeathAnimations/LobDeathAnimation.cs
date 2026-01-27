using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Steamworks;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace LobotomyCorp.Visuals.DeathAnimations
{
    public abstract class LobDeathAnimation : ModTexturedType
    {
        public LobDeathAnimation Clone() => (LobDeathAnimation)MemberwiseClone();

        public int Type { get; internal set; }

        protected sealed override void Register()
        {
            ModTypeLookup<LobDeathAnimation>.Register(this);
            Type = LobDeathLoader.Add(this);
        }

        public sealed override void SetupContent()
        {
            Asset<Texture2D> tex = ModContent.Request<Texture2D>(Texture);
            LobDeathLoader.AddTex(tex);
            SetStaticDefaults();
        }

        public Asset<Texture2D> GetTexture => LobDeathLoader.DeathTexture[Type];

        private bool active;
        public Vector2 DeathPosition;
        public Player DeathPlayerState;

        public int lifeTime;
        public float Opacity;

        public bool GetActive { get => active; }

        public void Defaults(Vector2 deathPosition, Player deathPlayer)
        {
            DeathPosition = deathPosition;
            DeathPlayerState = deathPlayer;
            lifeTime = 3000;
            Opacity = 1f;
            active = true;
            SetDefaults();
        }

        public virtual void SetDefaults()
        {
            active = false;
        }

        public void UpTick()
        {
            Update();
            lifeTime--;
            if (lifeTime < 0)
                active = false;
        }

        /// <summary>
        /// Update parameters and DeathPlayerState parameters
        /// </summary>
        public virtual void Update()
        {
        }

        public void Draw()
        {
            if (PreDraw())
            {
                Main.spriteBatch.End();
                Main.PlayerRenderer.DrawPlayer(Main.Camera, DeathPlayerState, DeathPlayerState.position, 0f, DeathPlayerState.fullRotationOrigin);
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            }
            PostDraw();
        }

        /// <summary>
        /// Return true to let Player Renderer run
        /// </summary>
        /// <returns></returns>
        public virtual bool PreDraw()
        {
            return true;
        }

        public virtual void PostDraw()
        {

        }

        public virtual bool SpecialDeathCondition(PlayerDeathReason reason, Player player)
        {
            return false;
        }
    }
}