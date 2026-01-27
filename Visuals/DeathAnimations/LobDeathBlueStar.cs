using LobotomyCorp.ModSystems;
using LobotomyCorp.Projectiles.Realized;
using LobotomyCorp.Visuals.PrimEffects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using System;
using Terraria;
using Terraria.ModLoader;

namespace LobotomyCorp.Visuals.DeathAnimations
{
    public class LobDeathBlueStar : LobDeathAnimation
    {
        public override string Texture => "LobotomyCorp/Visuals/DeathAnimations/BlueStarEyes";

        Color skinColor;
        Vector2 starPosition;

        public override void SetDefaults() 
        {
            DeathPosition -= new Vector2(DeathPlayerState.width / 2, DeathPlayerState.height / 2);
            skinColor = DeathPlayerState.skinColor;
            lifeTime = 600;
            starPosition = DeathPosition;
            foreach (Projectile p in Main.ActiveProjectiles)
            {
                if (p.type == ModContent.ProjectileType<SoundOfAStarBlueStar>())
                {
                    starPosition = p.Center - new Vector2(DeathPlayerState.width / 2, DeathPlayerState.height / 2);
                    break;
                }
            }
            int direction = (int)(starPosition.X - DeathPosition.X);
            if (direction != 0)
            {
                DeathPlayerState.direction = Math.Sign(direction);
            }
        }

        public override void Update()
        {
            float scale = 1f - MathHelper.Clamp((lifeTime - 540) / 60f, 0.5f, 1f);
            DeathPlayerState.skinColor = Color.Lerp(skinColor, Color.Gray, scale);
            DeathPlayerState.headRotation = MathHelper.ToRadians(Main.rand.Next(10)) * DeathPlayerState.direction;

            scale = 1f - MathHelper.Clamp((lifeTime - 15) / 300f, 0f, 1f);
            DeathPlayerState.position = Vector2.Lerp(DeathPosition, starPosition, scale);
            if (scale > 0f)
            {
                DeathPlayerState.bodyFrame.Y = DeathPlayerState.bodyFrame.Height * 1;
                DeathPlayerState.legFrame.Y = DeathPlayerState.legFrame.Height * 5;
                DeathPlayerState.legFrameCounter = 0;
                scale = MathHelper.Clamp((lifeTime - 285) / 30f, 0f, 1f);
                DeathPlayerState.headRotation = MathHelper.ToRadians(-15 * scale) * DeathPlayerState.direction;
                scale = MathHelper.Clamp((lifeTime) / 15f, 0f, 1f);
                Opacity = scale;
                DeathPlayerState.immuneAlpha = (int)(255 - 255 * Opacity);
            }
            if (lifeTime == 10)
            {
                Vector2 position = starPosition + new Vector2(DeathPlayerState.width / 2, DeathPlayerState.height / 2);
                Vector2 vel = Vector2.Zero;
                BlueStarShine shine = new BlueStarShine(position, 60, 1.8f, vel);
                LobCustomDraw.Instance().AddVEffects(shine);
            }
        }

        public override void PostDraw()
        {
            float scale = 1f - MathHelper.Clamp((lifeTime - 540) / 60f, 0f, 1f);
            Texture2D tex = GetTexture.Value;
            int dir = DeathPlayerState.direction;
            Vector2 pos = DeathPlayerState.position + new Vector2(9 + 3 * dir, 12);
            Rectangle frame = tex.Frame(2, 1, 0);
            Vector2 origin = frame.Size() / 2;
            Main.spriteBatch.Draw(tex, pos - Main.screenPosition, frame, Color.White, 0, origin, scale, 0, 0);

            scale = 1f - MathHelper.Clamp((lifeTime - 440) / 60f, 0f, 1f);
            frame = tex.Frame(2, 1, 1);
            Main.spriteBatch.Draw(tex, pos - Main.screenPosition, frame, Color.White, 0, origin, scale, 0, 0);
        }
    }
}