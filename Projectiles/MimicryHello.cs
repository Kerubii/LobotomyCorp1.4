using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	public class MimicryHello : ModProjectile
	{
		public override void SetStaticDefaults() {
            // DisplayName.SetDefault("Hello?");
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 30;

            Projectile.tileCollide = true;
            Projectile.hostile = true;
        }

        public override void AI()
        {
            if (Projectile.localAI[1]++ == 2)
            {
                Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, -Projectile.velocity.X/2, -Projectile.velocity.Y/2    )].noGravity = true;
                Projectile.localAI[1] = 0;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                Vector2 splosh = new Vector2(Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f));
                Dust dust2 = Dust.NewDustPerfect(Projectile.Center, 133, splosh, 0, default(Color), 0.1f);
                dust2.fadeIn = Main.rand.NextFloat(0.5f, 1.2f);
            }
            for (int i = 0; i < 10; i++)
            {
                Vector2 splosh = new Vector2(Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f));
                Dust dust2 = Dust.NewDustPerfect(Projectile.Center, 5, splosh, 0, default(Color), 1f);
                dust2.fadeIn = Main.rand.NextFloat(1f, 1.6f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Main.EntitySpriteDraw(
                tex,
                Projectile.Center - Main.screenPosition,
                null,
                lightColor,
                Projectile.rotation,
                tex.Size() / 2 + new Vector2(tex.Width/2 - Projectile.width/2, 0),
                Projectile.scale,
                0,
                0);
            return false;
        }
    }
}
