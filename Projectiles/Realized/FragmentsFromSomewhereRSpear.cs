﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace LobotomyCorp.Projectiles.Realized
{
	class FragmentsFromSomewhereRSpear : LobcorpSpear
	{
		protected override float HoldoutRangeMax => 224; 
		
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            LobotomyGlobalNPC.ApplyTentacles(target, 0.2f, 4f);
        }

        public override void ProjectileSpawn(int duration)
        {
            if (Projectile.timeLeft == 2 * duration / 3 && Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 4, ModContent.ProjectileType<FragmentsFromSomewhereProjectile>(), (int)(Projectile.damage * 0.75f), Projectile.knockBack, Projectile.owner);
            }
        }
    }

    /*
	public class FragmentsFromSomewhereRSpear : ModProjectile
	{
		public override void SetDefaults() {
			Projectile.width = 18;
			Projectile.height = 18;
			Projectile.aiStyle = 19;
			Projectile.penetrate = -1;
			Projectile.scale = 1.3f;
			Projectile.alpha = 0;

			Projectile.hide = true;
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
			// As long as the player isn't frozen, the spear can move
			if (!projOwner.frozen) {
				if (movementFactor == 0f) // When initially thrown out, the ai0 will be 0f
				{
					movementFactor = 6f; // Make sure the spear moves forward when initially thrown out
					Projectile.netUpdate = true; // Make sure to netUpdate this spear
				}
				if (projOwner.itemAnimation > projOwner.itemAnimationMax * 0.75f) // Somewhere along the item animation, make sure the spear moves back
				{
					movementFactor -= 0.1f;
				}
				else if (projOwner.itemAnimation > projOwner.itemAnimationMax / 2)
                {
					movementFactor += 7.8f;
				}					
				else // Otherwise, increase the movement factor
				{
					movementFactor -= 0.24f;
				}
			}
			// Change the spear position based off of the velocity and the movementFactor
			Projectile.position += Projectile.velocity * movementFactor;

			if (projOwner.itemAnimation > projOwner.itemAnimationMax / 2)
			{
				for (int i = 0; i < 5; i++)
				{
					int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.AncientLight, Projectile.velocity.X * 1.4f, Projectile.velocity.Y * 1.4f);
					Main.dust[d].noGravity = true;
				}
			}
			// When we reach the end of the animation, we can kill the spear Projectile
			if (projOwner.itemAnimation == 0) {
				Projectile.Kill();
			}
			// Apply proper rotation, with an offset of 135 degrees due to the sprite's rotation, notice the usage of MathHelper, use this class!
			// MathHelper.ToRadians(xx degrees here)
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(135f);
			// Offset by 90 degrees here
			if (Projectile.spriteDirection == -1) {
				Projectile.rotation -= MathHelper.ToRadians(90);
			}
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			LobotomyGlobalNPC.ApplyTentacles(target, 0.1f);
        }
    }*/
}
