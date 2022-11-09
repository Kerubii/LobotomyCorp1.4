﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	public class GreenStemVine : ModProjectile
	{
		public override void SetStaticDefaults() {
            DisplayName.SetDefault("Green Stem");
			Main.projFrames[Projectile.type] = 3;
        }

		public override void SetDefaults() {
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.aiStyle = -1;
			Projectile.penetrate = 2;
			Projectile.scale = 1f;
            Projectile.timeLeft = 30;
            
			Projectile.DamageType = DamageClass.Magic;
			Projectile.tileCollide = false;
			Projectile.friendly = true;

			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;
		}

		public override void AI() {
			if (Projectile.frameCounter == 0)
			{
				Projectile.frame = Main.rand.Next(3);
				Projectile.frameCounter = 1;
			}

			if (Projectile.timeLeft > 25)
				Projectile.scale = (1f - (float)Math.Sin((Projectile.timeLeft - 25) / 5f * 1.57f)) * 1.05f;
			else if (Projectile.timeLeft > 20)			
				Projectile.scale = 1f + 0.05f * (float)Math.Sin((Projectile.timeLeft - 20) / 5f * 1.57f + 1.57f);
			else
				Projectile.scale = 1f;

			if (Projectile.timeLeft > 25)
            {
				if (Projectile.timeLeft == 30)
                {
					for (int i = 1; i <= 20; i++)
                    {
						Vector2 target = Projectile.Center + new Vector2(156 * i/20f, 0).RotatedBy(Projectile.velocity.ToRotation());
						Dust d = Dust.NewDustPerfect(target + new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1)), 87);
						d.noGravity = true;
						d.velocity = Projectile.velocity;
						d.velocity.Normalize();
						d.velocity *= 4f;
					}
                }

				if (Projectile.timeLeft > 27)
				{
					Vector2 target = Projectile.Center + new Vector2(156 * Projectile.scale, 0).RotatedBy(Projectile.velocity.ToRotation());
					for (int i = 0; i < 16; i++)
					{
						float y = (float)Math.Cos((i / 16f) * 6.28f) * 8 * (Projectile.timeLeft - 26) / 4f;
						float x = (float)Math.Sin((i / 16f) * 6.28f) * 3 * (Projectile.timeLeft - 26) / 4f;

						Vector2 Offset = new Vector2(x, y).RotatedBy(Projectile.velocity.ToRotation());
						target += Offset;

						Dust d = Dust.NewDustPerfect(target, 157);
						d.noGravity = true;
						d.velocity = Offset;
						d.velocity.Normalize();
						d.velocity *= 2f;
					}
				}

				for (int i = 0; i < 5; i++)
				{
					Vector2 vel = Projectile.velocity;
					vel.Normalize();
					vel *= 16f;
					int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Grass, vel.X, vel.Y);
					Main.dust[d].noGravity = true;
				}
			}

            Player projOwner = Main.player[Projectile.owner];
			Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);
			Vector2 position = new Vector2(Projectile.ai[0], 0).RotatedBy(Projectile.ai[1]);
			Projectile.direction = projOwner.direction;
			Projectile.position.X = ownerMountedCenter.X - (float)(Projectile.width / 2);
            Projectile.position.Y = ownerMountedCenter.Y - (float)(Projectile.height / 2) + projOwner.height / 2;
			Projectile.position += position;

			Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool PreDraw(ref Color lightColor)
        {
			Rectangle frame = Terraria.Utils.Frame(TextureAssets.Projectile[Projectile.type].Value, 1, Main.projFrames[Projectile.type], 0, Projectile.frame);
			Vector2 position = Projectile.Center + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
			Vector2 origin = frame.Size() / 2; 
			origin.X = 4;

			Vector2 scale = new Vector2(1f, 0.75f);
			if (Projectile.timeLeft > 25)
            {
				scale.X = (1f - (float)Math.Sin((Projectile.timeLeft - 25) / 5f * 1.57f)) * 1.05f;
            }
			else if (Projectile.timeLeft > 20)
            {
				scale.X = 1f + 0.05f * (float)Math.Sin((Projectile.timeLeft - 20) / 5f * 1.57f + 1.57f);
			}
			else if (Projectile.timeLeft <= 10f)
            {
				scale.Y *= Projectile.timeLeft / 10f;
            }
			Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, position, frame, lightColor, Projectile.rotation, origin, scale, 0, 0);

            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
			if (Projectile.penetrate <= 1)
				return false;
            return base.CanHitNPC(target);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
			Vector2 startPoint = Projectile.Center;

			float scale = 1f;
			if (Projectile.timeLeft > 25)
			{
				scale = (1f - (float)Math.Sin((Projectile.timeLeft - 25) / 5f * 1.57f)) * 1.05f;
			}
			else if (Projectile.timeLeft > 20)
			{
				scale = 1f + 0.05f * (float)Math.Sin((Projectile.timeLeft - 20) / 5f * 1.57f + 1.57f);
			}

			Vector2 endPoint = startPoint + new Vector2(156 * scale, 0).RotatedBy(Projectile.velocity.ToRotation());
			if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), startPoint, endPoint))
				return true;
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
    }
}
