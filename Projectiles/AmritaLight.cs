using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	public class AmritaLight : ModProjectile
	{
        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 46;
            Projectile.timeLeft = 120;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
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
            if (Projectile.timeLeft < 30)
            {
                Projectile.alpha += 255 / 60;
                DrawOriginOffsetX = Main.rand.Next(-1, 2);
                DrawOriginOffsetY = Main.rand.Next(-1, 2);
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
            {
                float rot = Projectile.rotation + MathHelper.ToRadians(45) * i;
                Vector2 startPos = Projectile.Center + new Vector2(14, 0).RotatedBy(rot);
                Vector2 vel = new Vector2(Main.rand.Next(4, 8), 0).RotatedBy(rot);
                if (Main.myPlayer == Projectile.owner)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), startPos, vel, ModContent.ProjectileType<AmritaMiniLight>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                rot -= MathHelper.ToRadians(22.5f);
                startPos = Projectile.Center + new Vector2(14, 0).RotatedBy(rot);
                vel = new Vector2(Main.rand.Next(2, 4), 0).RotatedBy(rot);
                Dust d = Dust.NewDustPerfect(startPos, DustID.PurificationPowder, vel);
                d.noGravity = true;
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
