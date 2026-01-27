using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using System;
using Terraria;
using Terraria.ModLoader;

namespace LobotomyCorp.Visuals.DeathAnimations
{
    public class LobDeathBHeaven : LobDeathAnimation
    {
        public override string Texture => "LobotomyCorp/Visuals/DeathAnimations/HeavenDeath1";

        float BranchScale;
        float EyePos;
        float Branch1;
        float Branch2;
        Vector2 EyeOffset;

        public override void SetDefaults() 
        {
            lifeTime = 600;
            BranchScale = 0f;
            Branch1 = 0;
            Branch2 = 0;
            EyePos = 0;
            EyeOffset = Vector2.Zero;
        }

        public override void Update()
        {
            float SproutTime = 1f - MathHelper.Clamp((lifeTime - 570f) / 30f, 0f, 1f);
            SproutTime *= SproutTime * SproutTime;
            BranchScale = SproutTime;
            EyePos = SproutTime * 50;
            float BranchTime = MathHelper.Clamp((lifeTime - 510) / 30f, 0f, 1f);
            BranchTime *= BranchTime * BranchTime;
            Branch1 = MathHelper.ToRadians(45) * BranchTime;
            Branch2 = -Branch1;
            float sway = MathHelper.ToRadians(8) * (float)Math.Sin((lifeTime % 180f / 180f) * 6.28f);
            Branch1 += sway;
            sway = MathHelper.ToRadians(8) * (float)Math.Sin((lifeTime % 200f / 200f) * 6.28f);
            Branch2 += sway;
            if (lifeTime < 60)
            {
                Opacity = lifeTime / 60f;
                DeathPlayerState.immuneAlpha = (int)(255 - 255 * Opacity);
            }
            DeathPlayerState.headPosition = Vector2.Lerp(Vector2.Zero, GetPosition(new Vector2(56, -52), Branch2), SproutTime);
            DeathPlayerState.bodyPosition = Vector2.Lerp(Vector2.Zero, GetPosition(new Vector2(-26, -114), Branch1),  SproutTime);
            DeathPlayerState.legPosition = Vector2.Lerp(Vector2.Zero, GetPosition(new Vector2(-65, -65), Branch1),  SproutTime);
            EyeOffset = new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f));
        }

        private Vector2 GetPosition(Vector2 vector, float rot)
        {
            Vector2 offset = GetEyePos() - DeathPlayerState.position;
            offset += vector.RotatedBy(rot);
            return offset;
        }

        private Vector2 GetEyePos()
        {
            return DeathPosition + new Vector2(0, DeathPlayerState.height / 2 - EyePos);
        }

        public override bool PreDraw()
        {
            Vector2 basePos = GetEyePos();
            Color color = Lighting.GetColor((int)DeathPosition.X / 16, (int)DeathPosition.Y / 16) * Opacity;

            Texture2D tex = GetTexture.Value;
            // Base Frame
            Rectangle frame = new Rectangle(0, 128, 32, (int)EyePos);
            Vector2 origin = new Vector2(17, 0);

            Main.spriteBatch.Draw(tex, basePos - Main.screenPosition, frame, color, 0, origin, 1f, 0, 0);

            // Branch1
            frame = new Rectangle(0, 0, 98, 124);
            origin = new Vector2(87, 121);
            float rot = Branch1;
            Main.spriteBatch.Draw(tex, basePos - Main.screenPosition, frame, color, rot, origin, BranchScale, 0, 0);

            // Branch2
            frame = new Rectangle(102, 42, 74, 82);
            origin = new Vector2(3, frame.Height - 3);
            rot = Branch2;
            Main.spriteBatch.Draw(tex, basePos - Main.screenPosition, frame, color, rot, origin, BranchScale, 0, 0);

            // Eye
            frame = new Rectangle(128, 128, 18, 18);
            origin = frame.Size()/2;
            Main.spriteBatch.Draw(tex, basePos - Main.screenPosition, frame, color, 0f, origin, 1f, 0, 0);

            frame = new Rectangle(96, 128, 18, 18);
            Main.spriteBatch.Draw(tex, basePos - Main.screenPosition + EyeOffset, frame, color, 0f, origin, 1f, 0, 0);


            frame = new Rectangle(64, 128, 18, 18);
            Main.spriteBatch.Draw(tex, basePos - Main.screenPosition, frame, color, 0f, origin, 1f, 0, 0);

            return base.PreDraw();
        }
    }
}