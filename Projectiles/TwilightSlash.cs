using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	public class TwilightSlash : ModProjectile
	{
		public override void SetDefaults() {
			Projectile.height = 200;
			Projectile.width = 200;
			Projectile.aiStyle = -1;
			Projectile.penetrate = 6;

			Projectile.usesIDStaticNPCImmunity = true;
			Projectile.idStaticNPCHitCooldown = 15;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 24;
			Projectile.friendly = true;
		}

        public override void AI()
        {
			Projectile.velocity *= 0.95f;

			for (int i = 0; i < 3; i++)
			{
				bool flag = Main.rand.NextBool(3);
				int type = flag ? 64 : DustID.Wraith;
				int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, type);
				Main.dust[d].noGravity = true;
				Main.dust[d].fadeIn = 1.2f;
				Main.dust[d].scale = 2;
				Main.dust[d].velocity *= 0;
				if (!flag)
					Main.dust[d].color = Color.Black;
			}

			if (Projectile.penetrate == 1 && Projectile.timeLeft > 5)
				Projectile.timeLeft = 5;


			if (Projectile.timeLeft < 5)
            {
				Projectile.alpha -= 50;
				return;
            }

			if (Projectile.alpha < 255)
				Projectile.alpha += 60;
			if (Projectile.alpha > 255)
				Projectile.alpha = 255;

		}

        public override bool? CanHitNPC(NPC target)
        {
			if (Projectile.penetrate == 1)// && target.immune[Projectile.owner] > 0)
				return false;
            return base.CanHitNPC(target);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			float angle = Main.rand.NextFloat(6.28f);
			Vector2 velocity = new Vector2(16f, 0f).RotatedBy(angle);
			Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center - velocity * 15, velocity, ModContent.ProjectileType<Projectiles.TwilightStrikes>(), hit.Damage / (int)(4 * 1.5f), 0, Projectile.owner, target.whoAmI, 1);
		}

		public override bool PreDraw(ref Color lightColor)
        {
			//Player projOwner = Main.player[Projectile.owner];
			//Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);

			Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
			Vector2 pos = Projectile.Center - Main.screenPosition + Projectile.gfxOffY * Vector2.UnitY;
			Rectangle frame = tex.Frame();
			Vector2 origin = frame.Size() / 2;
			float alpha = (Projectile.alpha / 255f);
			Color color = Color.Lerp(Color.Black, Color.White, alpha);
			//Color color = Color.Black * alpha;
			//color.A = (byte)(color.A * 0.7f);
			//color *= 0.9f;
			Main.EntitySpriteDraw(tex, pos, frame, color * alpha, Projectile.velocity.ToRotation() + Projectile.rotation, origin, Projectile.scale, 0, 0);

			return false;
        }
    }
}