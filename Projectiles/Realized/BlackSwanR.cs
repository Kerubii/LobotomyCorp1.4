﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles.Realized
{
	public class BlackSwanR : ModProjectile
	{
        public override void SetStaticDefaults()
        {
			Main.projFrames[Projectile.type] = 2;
        }

        public override void SetDefaults() {
			Projectile.width = 18;
			Projectile.height = 18;
			Projectile.aiStyle = -1;
			Projectile.penetrate = -1;
			Projectile.scale = 1.3f;
			Projectile.alpha = 0;

			//Projectile.hide = true;
			Projectile.ownerHitCheck = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.tileCollide = false;
			Projectile.friendly = true;
		}

		// In here the AI uses this example, to make the code more organized and readable
		// Also showcased in ExampleJavelinProjectile.cs
		public float movementFactor // Change this value to alter how fast the spear moves
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		// It appears that for this AI, only the ai0 field is used!
		public override void AI() {
			// Since we access the owner player instance so much, it's useful to create a helper local variable for this
			// Sadly, Projectile/ModProjectile does not have its own
			Player projOwner = Main.player[Projectile.owner];
			// Here we set some of the Projectile's owner properties, such as held item and itemtime, along with Projectile direction and position based on the player
			Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);
			Projectile.direction = projOwner.direction;
			projOwner.heldProj = Projectile.whoAmI;
			projOwner.itemTime = projOwner.itemAnimation;
			Projectile.position.X = ownerMountedCenter.X - (float)(Projectile.width / 2);
			Projectile.position.Y = ownerMountedCenter.Y - (float)(Projectile.height / 2);
			int limit = projOwner.itemAnimationMax;
			// As long as the player isn't frozen, the spear can move
			if (!projOwner.frozen) {
				if (movementFactor == 0f) // When initially thrown out, the ai0 will be 0f
				{
					SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
					movementFactor = 6f; // Make sure the spear moves forward when initially thrown out
					Projectile.netUpdate = true; // Make sure to netUpdate this spear
				}
				if (projOwner.itemAnimation > limit / 2)
				{
					if (projOwner.itemAnimation > limit * 0.75f)
						movementFactor += 2.6f;
					else
						movementFactor -= 0.22f;
				}
			}
			// Change the spear position based off of the velocity and the movementFactor
			Projectile.position += Projectile.velocity * movementFactor;
			// When we reach the end of the animation, we can kill the spear Projectile
			if (Projectile.ai[1] > 1 && projOwner.channel)
			{
				if (Main.myPlayer == Projectile.owner)
                {
					Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0).RotatedBy((Main.MouseWorld - projOwner.Center).ToRotation());
				}
				if (projOwner.itemAnimation == 3)
				{
					projOwner.itemAnimation = 4;
					Projectile.ai[1]--;
				}
			}
			else if (projOwner.itemAnimation == 3) {
				Projectile.Kill();
			}
			// Apply proper rotation, with an offset of 135 degrees due to the sprite's rotation, notice the usage of MathHelper, use this class!
			// MathHelper.ToRadians(xx degrees here)
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(135f);
			// Offset by 90 degrees here
			/*
			Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
			if (Projectile.spriteDirection == -1) {
				Projectile.rotation -= MathHelper.ToRadians(90);
			}
			*/

			if ((projOwner.itemAnimation < projOwner.itemAnimationMax * 0.9f && projOwner.itemAnimation > projOwner.itemAnimationMax * 0.6f) ||
				Projectile.ai[1] > 0)
            {
				Vector2 reflectSize = new Vector2(64, 64);
				Vector2 reflectOffset = Projectile.velocity;
				reflectOffset.Normalize();
				reflectOffset *= Projectile.width * 0.4f;
				Rectangle reflectHitbox = new Rectangle((int)Projectile.Center.X, (int)Projectile.Center.Y, (int)reflectSize.X, (int)reflectSize.Y);

				reflectHitbox.X -= (int)(reflectSize.X / 2 + reflectOffset.X);
				reflectHitbox.Y -= (int)(reflectSize.Y / 2 + reflectOffset.Y);

				foreach (Projectile proj in Main.projectile)
                {
					if (proj.active && !proj.friendly && reflectHitbox.Intersects(proj.getRect()))
					{
						proj.velocity = Projectile.velocity * 3f;
						proj.friendly = true;
						proj.owner = Projectile.owner;
						proj.damage = Projectile.damage;
						proj.GetGlobalProjectile<LobotomyGlobalProjectile>().BlackSwanReflected = true;
						if (Projectile.ai[1] == 0)
						{
							Projectile.ai[1] = 120;
							Projectile.frame = 1;
						}
                    }
				}
			}

			if (projOwner.dead)
				Projectile.Kill();
		}

        public override bool? CanHitNPC(NPC target)
        {
			if (Main.player[Projectile.owner].itemAnimation <= Main.player[Projectile.owner].itemAnimationMax / 2)
				return false;
            return base.CanHitNPC(target);
        }

        public override bool PreDraw(ref Color lightColor)
        {
			Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
			Vector2 pos = Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
			Rectangle frame = tex.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);

			Vector2 origin = new Vector2(
				frame.Width - (4 + Projectile.width / 2),
				4 + Projectile.height / 2
				);

			Main.EntitySpriteDraw(tex, pos, frame, lightColor, Projectile.rotation - 1.57f, origin, Projectile.scale, 0f, 0);
            return false;
        }
    }
}