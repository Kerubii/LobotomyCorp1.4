using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	public class AmritaMiniLight : ModProjectile
	{
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.timeLeft = 60;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
                Projectile.localAI[0]++;
                for (int i = 0; i < 16; i++)
                {
                    Vector2 circle = new Vector2(8, 0).RotatedBy((float)MathHelper.ToRadians(360 / 16 * i));
                    Vector2 pos = Projectile.Center + circle;
                    circle.Normalize();
                    Dust.NewDustPerfect(pos, DustID.GemDiamond, circle).noGravity = true;
                }
            }

            Projectile.velocity *= 0.95f;
            if (Projectile.timeLeft < 15)
            {
                Projectile.alpha += 255 / 15;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.White;
            lightColor.A = (byte)(lightColor.A * 0.6f);

            return base.PreDraw(ref lightColor);
        }
    }
}
