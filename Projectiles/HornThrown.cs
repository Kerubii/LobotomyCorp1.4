using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Security.Policy;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	public class HornThrown : ModProjectile
	{
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 300;

            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            if (Projectile.penetrate == 1 && Projectile.ai[0] == 0)
            {
                Projectile.ai[0]++;
                Projectile.timeLeft = 60;
                Projectile.velocity.X = (Math.Sign(Projectile.velocity.X) * -1) * Main.rand.Next(1, 6);
                Projectile.velocity.Y = -Main.rand.Next(8, 14);
                float rand = (float)Main.rand.Next(90);
                for (int i = 0; i < 10; i++)
                {
                    Vector2 vel = new Vector2(12, 0).RotatedBy(MathHelper.ToRadians(rand + 360 * (i / 10f)));

                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.JungleSpore, vel);
                    d.noGravity = true;
                }
                Projectile.tileCollide = false;
            }
            else if (Projectile.penetrate > 1)
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
            if (Projectile.ai[0] > 0)
            {
                Projectile.alpha += 4;
                Projectile.velocity.Y += 0.5f;
                Projectile.rotation += MathHelper.ToRadians(2 * Projectile.velocity.X);
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.penetrate == 1)
                return false;

            return base.CanHitNPC(target);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate--;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = new Vector2(28, 28);
            Vector2 pos = Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
            int trail = 5;
            if (Projectile.penetrate == 1)
                trail = 1;

            for (int i = 0; i < trail; i++)
            {
                Vector2 offset = -Projectile.velocity * 0.5f * i;
                float opacity = 1f - (float)i / 5f;
                opacity *= 1f - (Projectile.alpha / 255f);

                Main.EntitySpriteDraw(tex, pos + offset, tex.Frame(), lightColor * opacity, Projectile.rotation + MathHelper.ToRadians(135), origin, Projectile.scale, 0, 0);
            }
            return false;
        }
    }
}
