using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Xml.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
    public class ParadiseLostBase : ModProjectile
	{
        public static Asset<Texture2D> Scythe;
        public static Asset<Texture2D> Spear;
        public static Asset<Texture2D> Staff;
        public static Asset<Texture2D> Air;

        public override void Load()
        {
            Scythe = Mod.Assets.Request<Texture2D>("Projectiles/ParadiseScythe");
            Spear = Mod.Assets.Request<Texture2D>("Projectiles/ParadiseSpear");
            Staff = Mod.Assets.Request<Texture2D>("Projectiles/ParadiseStaff");
            Air = Mod.Assets.Request<Texture2D>("Projectiles/ParadiseLostBaseAir");
        }

        public override void SetStaticDefaults() {
            // DisplayName.SetDefault("Judgement");
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = 0;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 60;
            Projectile.alpha = 255;

            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.friendly = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            if (Projectile.localAI[0]++ == 0)
            {
                //Projectile.localAI[0] = Main.rand.Next(3) + 1;
                Projectile.spriteDirection = Main.rand.Next(2) == 0 ? -1 : 1;
                Projectile.scale = 0f;
                Projectile.alpha = 0;
                Projectile.localAI[1] = Main.rand.NextFloat(3.1432179865f);

                Vector2 speed = Vector2.Normalize(Projectile.velocity) * 8;
                for (int i = 0; i < 10; i++)
                {
                    Dust dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 5, speed.X, speed.Y, 0, default(Color), 1.5f)];
                    dust.fadeIn = 1.25f;
                    dust.noGravity = true;
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90);

            if (Projectile.timeLeft < 8)
            {
                Projectile.scale -= 0.125f;
                return;
            }

            if (Projectile.scale < 1f)
                Projectile.scale += 0.125f;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            bool hit = false;
            for (int i = -1; i < 2; i++)
            {
                Vector2 origin = Projectile.Center + new Vector2( 6 * i, Projectile.height / 2 - 2);
                float scale = 1f;
                if (i < 0)
                    scale = 0.8f;
                if (i > 0)
                    scale = 0.9f;
                float rotOffset = MathHelper.ToRadians(15 * i);
                Vector2 endpoint = origin + new Vector2(128 * scale, 0).RotatedBy(Projectile.velocity.ToRotation() + rotOffset);
                if (Collision.CheckAABBvLineCollision2(targetHitbox.TopLeft(), targetHitbox.Size(), origin, endpoint))
                {
                    hit = true;
                    break;
                }
            }
            return hit;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            Vector2 speed = Vector2.Normalize(Projectile.velocity) * 4;
            for (int i = 0; i < 10; i++)
            {
                Dust dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 5, speed.X, speed.Y, 0, default(Color), 1)];
                dust.fadeIn = 1.25f;
                dust.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex;
            switch (Math.Abs(Projectile.ai[0] % 3))
            {
                case 1:
                    tex = Scythe.Value;
                    break;
                case 2:
                    tex = Spear.Value;
                    break;
                default:
                    tex = Staff.Value;
                    break;
            }            
            Vector2 Position = Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.height / 2);//.RotatedBy(Projectile.rotation);
            Vector2 origin = tex.Size();
            origin.X /= 2;
            SpriteEffects effect = Projectile.spriteDirection > 0 ? SpriteEffects.FlipHorizontally : 0;
            for (int i = -1; i < 2; i++)
            {
                float rotOffset = MathHelper.ToRadians(15 * i);
                Vector2 posOffset = new Vector2(6 * i, -2);//.RotatedBy(Projectile.rotation + rotOffset);
                float scale = 1f;
                if (i < 0)
                    scale = 0.8f;
                if (i > 0)
                    scale = 0.9f;
                Main.EntitySpriteDraw(tex, Position + posOffset, tex.Frame(), lightColor, Projectile.rotation + rotOffset, origin, Projectile.scale * scale, effect, 0);
            }
            tex = Scythe.Value;
            float scalep = 0.3f;
            float extra = 0;
            if (Projectile.ai[0] < 0)
                extra = MathHelper.ToRadians(30);
            else if (Projectile.ai[0] > 0)
                extra = -MathHelper.ToRadians(30);
            float RotOffset = MathHelper.ToRadians(30) * (float)Math.Sin(Projectile.localAI[1]) + extra;
            Vector2 PosOffset = new Vector2(8 * (float)Math.Sin(Projectile.localAI[1]), 0);
            Main.EntitySpriteDraw(tex, Position + PosOffset, tex.Frame(), lightColor, RotOffset, origin, Projectile.scale * scalep, effect, 0);

            tex = Spear.Value;
            RotOffset = MathHelper.ToRadians(30) * (float)Math.Sin(Projectile.localAI[1] + 2.0944f) + extra;
            PosOffset.X = (8 * (float)Math.Sin(Projectile.localAI[1] + 2.0944f));
            Main.EntitySpriteDraw(tex, Position + PosOffset, tex.Frame(), lightColor, RotOffset, origin, Projectile.scale * scalep, effect, 0);

            tex = Staff.Value;
            RotOffset = MathHelper.ToRadians(30) * (float)Math.Sin(Projectile.localAI[1] + 2.0944f * 2) + extra;
            PosOffset.X = (8 * (float)Math.Sin(Projectile.localAI[1] + 2.0944f * 2));
            Main.EntitySpriteDraw(tex, Position + PosOffset, tex.Frame(), lightColor, RotOffset, origin, Projectile.scale * scalep, effect, 0);
            
            tex = TextureAssets.Projectile[Projectile.type].Value;
            if (Projectile.ai[1] != 0)
            {
                tex = Air.Value;
                origin = tex.Frame().Size() / 2;
            }
            Main.EntitySpriteDraw(tex, Position, tex.Frame(), lightColor, 0, origin, Projectile.scale, effect, 0);
            return false;
        }
    }
}
