using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	public class Candy : ModProjectile
	{
		public override void SetStaticDefaults() {
            //DisplayName.SetDefault("Spear");
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 60;

            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
        }    
            
        public override void AI() {
            Projectile.frame = (int)Projectile.ai[0];
            if (Projectile.ai[1] == 0)
            {
                Projectile.scale += Main.rand.NextFloat(0.5f);
                Projectile.timeLeft = Main.rand.Next(60, 120);
                Projectile.rotation = Main.rand.NextFloat((float)Math.PI * 2);
            }
            Projectile.ai[1]++;
            if (Projectile.ai[1] > 15)
            {
                Projectile.velocity.Y -= 0.04f;
                Projectile.velocity.X *= 0.98f;
            }
            Projectile.rotation += MathHelper.ToRadians(8) * Projectile.velocity.X / 12f;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item54, Projectile.Center);
            Dust dust;
            float offset = Main.rand.NextFloat(1.57f);
            for (int i = 0; i < 3; i++)
            {
                Vector2 position = Projectile.Center + new Vector2(7, 0).RotatedBy(offset + MathHelper.ToRadians(120 * i));
                dust = Main.dust[Terraria.Dust.NewDust(position, 30, 30, 33, 0f, 0f, 0, new Color(255, 255, 255), 1f)];
                dust.noGravity = true;
            }
        }
    }
}
