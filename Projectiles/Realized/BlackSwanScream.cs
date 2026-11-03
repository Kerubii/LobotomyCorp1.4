using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles.Realized
{
	public class BlackSwanScream : ModProjectile
	{
		public override string Texture => "LobotomyCorp/Projectiles/Realized/BlackSwanR";

		public override void SetStaticDefaults()
        {
			Main.projFrames[Projectile.type] = 2;
        }

        public override void SetDefaults() {
			Projectile.width = 18;
			Projectile.height = 18;
			Projectile.aiStyle = -1;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
			Projectile.alpha = 255;
			Projectile.timeLeft = 120;

			//Projectile.hide = true;
			Projectile.ownerHitCheck = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.tileCollide = false;
			Projectile.friendly = true;
		}

		public override void AI()
		{
			if (Projectile.soundDelay == 0)
            {
				Projectile.soundDelay = -1;
				SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Sis_skill") with { Volume = 0.5f }, Projectile.Center);
			}
            if (Projectile.localAI[0] % 20 == 0)
			{
                LobotomyCorp.ScreenShakePosition(Projectile.Center, 300, 1200, 10, 8f, 0.6f);
            }
            Projectile.localAI[0]++;

            Player player = Main.player[Projectile.owner];

			Vector2 center = player.RotatedRelativePoint(player.MountedCenter, true);

			Projectile.Center = center;
			player.itemTime = 15;
			player.itemAnimation = 15;

			for (int i = 0; i < 3; i++)
			{
				Vector2 vel = new Vector2(Main.rand.Next(36), 0).RotateRandom(6.28f);
				int type = Main.rand.Next(2, 4);
				Dust d = Dust.NewDustPerfect(Projectile.Center, type, vel);
				d.noGravity = true;
				d.fadeIn = 1.2f;

				if (Main.rand.NextBool(3))
                {
					vel = new Vector2(Main.rand.Next(36), 0).RotateRandom(6.28f);
					Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Misc.Dusts.BlackFeather>(), vel);
				}
				vel = new Vector2(Main.rand.Next(36), 0).RotateRandom(6.28f);
				Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, vel, ModContent.ProjectileType<BlackSwanGooeyWaste>(), 1, 0, player.whoAmI);
			}

			Projectile.ai[0]++;

			if (Projectile.ai[0] % 20 >= 15 && Projectile.ai[0] % 20 < 20)
			{
				Projectile.ai[1] += 178;
			}
			else
            {
				Projectile.ai[1] = 0;
            }
		}

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
			if (Projectile.ai[0] % 20 >= 15 && Projectile.ai[0] % 20 < 20)
            {
				Vector2 targetCenter = targetHitbox.TopLeft() + targetHitbox.Size() / 2;
				Vector2 projCenter = projHitbox.TopLeft() + projHitbox.Size() / 2;

				Vector2 lineDirection = targetCenter - projCenter;
				lineDirection.Normalize();
				lineDirection *= Projectile.ai[1];

				return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projCenter, projCenter + lineDirection);
            }
            return base.Colliding(projHitbox, targetHitbox);
        }
    }
}
