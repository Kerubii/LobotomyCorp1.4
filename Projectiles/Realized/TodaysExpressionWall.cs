using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using LobotomyCorp.Util;
using Terraria.GameContent;
using System.Collections.Generic;
using Terraria.Audio;

namespace LobotomyCorp.Projectiles.Realized
{
	public class TodaysExpressionWall : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("What the fuck do you want from me?"); // The English name of the Projectile
			Main.projFrames[Projectile.type] = 2;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.width = 128;
			Projectile.height = 128; 
			Projectile.aiStyle = -1;
			Projectile.scale = 1f;
			Projectile.alpha = 255;

			Projectile.DamageType = DamageClass.Magic;
			Projectile.friendly = true;
			Projectile.tileCollide = false;

			Projectile.penetrate = -1;
			Projectile.timeLeft = 30;

			ReflectedProjectiles = new List<int>();
		}

		private List<int> ReflectedProjectiles;

        public override void AI()
        {
			if (ReflectedProjectiles == null)
				ReflectedProjectiles = new List<int>();

			Projectile.rotation = Projectile.velocity.ToRotation();
			int ReflectionChance = 100;

			if (Projectile.ai[0] < 2)
			{
				Player owner = Main.player[Projectile.owner];
				Vector2 ownerMountedCenter = owner.RotatedRelativePoint(owner.MountedCenter, true);

				float length = Projectile.ai[1];
				Projectile.Center = ownerMountedCenter + new Vector2(length, 0).RotatedBy(Projectile.rotation);
				Projectile.ai[1] += Projectile.velocity.Length();
			}			

			if (Projectile.ai[0] == 2)
				ReflectionChance = 50;

			if (Projectile.ai[0] == 3)
				ReflectionChance = 10;

			if (Projectile.ai[0] == 4)
				Projectile.frame = 1;

			if (Projectile.ai[0] < 4)
			{
				Projectile.velocity *= 0.95f;
				foreach (Projectile proj in Main.projectile)
				{
					if (proj.active && !proj.friendly && !ReflectedProjectiles.Contains(proj.whoAmI) && Projectile.getRect().Intersects(proj.getRect()))
					{
						if (Projectile.ai[0] == 0)
						{
							proj.Kill();
							continue;
						}
						if (Main.rand.Next(100) < ReflectionChance)
						{
							float Speed = proj.velocity.Length();
							Vector2 delta = proj.Center - Projectile.Center;
							delta.Normalize();
							delta *= Speed;

							proj.velocity = delta;
						}
						ReflectedProjectiles.Add(proj.whoAmI);
					}
				}
			}

			if (Main.rand.NextBool(5))
            {
				int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood);
				Main.dust[d].noGravity = true;
            }

			if (Projectile.timeLeft < 10)
            {
				Projectile.alpha += 26;
				Projectile.scale -= 0.05f;
				return;
            }
			Projectile.alpha -= 25;
			if (Projectile.alpha < 0)
				Projectile.alpha = 0;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
			float Distance = (Projectile.Center - Main.player[Projectile.owner].Center).Length();
			float damageBonus = 0f;
			if (Distance > 80)
				damageBonus = (Distance - 80) / 500;
			if (damageBonus > 0.5f)
				damageBonus = 0.5f;
			modifiers.FinalDamage += damageBonus;
        }

        public override bool PreDraw(ref Color lightColor)
        {
			Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;

			Rectangle frame = tex.Frame(1, 2, 0, Projectile.frame);
			Vector2 origin = frame.Size() / 2;
			Color color = lightColor * (1f - Projectile.alpha / 255f);
			Vector2 scale = new Vector2(1f, Projectile.scale);

			int amount = 6;
			if (Projectile.ai[0] > 1)
            {
				for (int i = 0; i < amount; i++)
				{
					Vector2 oldPos = Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
					Color glowColor = color * (1f - (1f + i) / (amount + 1f));
					glowColor.A = (byte)(glowColor.A * 0.8f);

					Main.EntitySpriteDraw(tex, oldPos, frame, glowColor, Projectile.oldRot[i], origin, scale, SpriteEffects.None, 0);
				}
            }

			Vector2 pos = Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
			Main.EntitySpriteDraw(tex, pos, frame, color, Projectile.rotation, origin, scale, SpriteEffects.None, 0);

			return false;
        }
    }
}
