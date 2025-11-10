using LobotomyCorp.Players;
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

		private float shaderProgress;
		private Vector2[] shaderPosition;
		private Vector2[] shaderRotation;

		// It appears that for this AI, only the ai0 field is used!
		public override void AI() {
			// Since we access the owner player instance so much, it's useful to create a helper local variable for this
			// Sadly, Projectile/ModProjectile does not have its own
			Player projOwner = Main.player[Projectile.owner];
			// Here we set some of the Projectile's owner properties, such as held item and itemtime, along with Projectile direction and position based on the player
			Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);
			Projectile.direction = projOwner.direction;
			LobotomyWawPlayer modPlayer = Main.player[Projectile.owner].GetModPlayer<LobotomyWawPlayer>();
			bool ExtenderActive = (modPlayer.BlackSwanNettleClothing >= 4 || modPlayer.BlackSwanBrokenDream);
			if (!ExtenderActive)
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

					//If player has Broken Dream or 4 Nettles, Swoosh
					if (Main.myPlayer == Projectile.owner && ExtenderActive)
					{
						//Projectile.NewProjectile(Projectile.GetSource_FromThis(), ownerMountedCenter, new Vector2(20f, 0).RotatedBy(Projectile.rotation), ModContent.ProjectileType<SpearExtender>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 4, 3);
						Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<BlackSwanRExtended>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
					}
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

				if (Projectile.ai[1] == 30)
				{
					Projectile.frame = 0;
					for (int i = 0; i < 10; i++)
                    {
						Vector2 vel = new Vector2(1, 0).RotatedBy(i/10f * 6.28f);
						int dustType = ModContent.DustType<Misc.Dusts.BlackFeather>();
						Dust.NewDustPerfect(Projectile.Center, dustType, vel).noGravity = true;
					}
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
					if (proj.active && (!proj.friendly || proj.hostile) && reflectHitbox.Intersects(proj.getRect()))
					{
						proj.velocity = Projectile.velocity * 3f;
						proj.friendly = true;
						proj.hostile = false;
						proj.owner = Projectile.owner;
						if (proj.damage > Projectile.damage * 2)
							proj.damage = Projectile.damage * 2;
						else if (proj.damage < Projectile.damage / 2)
							proj.damage = Projectile.damage / 2;
						proj.GetGlobalProjectile<LobotomyGlobalProjectile>().BlackSwanReflected = true;
						proj.netUpdate = true;
						for (int i = 0; i < 16; i++)
                        {
							int dustType = Main.rand.Next(2, 4);
							Vector2 dustVel = new Vector2(2f * (float)Math.Cos(6.28f * (i / 16f)), 4f * (float)Math.Sin(6.28f * (i / 16f)));
							dustVel = dustVel.RotatedBy(Projectile.velocity.ToRotation());

							Dust.NewDustPerfect(Projectile.Center, dustType, dustVel).noGravity = true;
							if (Main.rand.NextBool(5))
                            {
								Dust.NewDustPerfect(proj.Center, dustType, Projectile.velocity / 4f).noGravity = true;
							}

							if (Main.rand.NextBool(3))
                            {
								dustType = ModContent.DustType<Misc.Dusts.BlackFeather>();
								float angle = Main.rand.NextFloat(6.28f);
								dustVel = new Vector2(4f * (float)Math.Cos(angle), 6f * (float)Math.Sin(angle));

								Dust.NewDustPerfect(Projectile.Center, dustType, dustVel).noGravity = true;
							}
						}

						SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Sis_Reflect") with { Volume = 0.25f }, Projectile.Center);
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

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
			if (Main.player[Projectile.owner].GetModPlayer<LobotomyWawPlayer>().BlackSwanNettleClothing >= 4 || Main.player[Projectile.owner].GetModPlayer<LobotomyWawPlayer>().BlackSwanBrokenDream)
				modifiers.FinalDamage *= 2;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			if (Main.player[Projectile.owner].GetModPlayer<LobotomyWawPlayer>().BlackSwanNettleClothing >= 5 || Main.player[Projectile.owner].GetModPlayer<LobotomyWawPlayer>().BlackSwanBrokenDream)
			{
				target.AddBuff(BuffID.Ichor, 300);
			}
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

	public class BlackSwanAlternate : ModProjectile
    {
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 2;
		}

		public override string Texture => "LobotomyCorp/Projectiles/Realized/BlackSwanR";

		public override void SetDefaults()
		{
			Projectile.width = 46;
			Projectile.height = 46;
			Projectile.aiStyle = -1;
			Projectile.penetrate = -1;
			Projectile.scale = 1.2f;
			Projectile.alpha = 0;

			//Projectile.hide = true;
			Projectile.ownerHitCheck = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.tileCollide = false;
			Projectile.friendly = true;
		}

        public override void AI()
        {
			Player projOwner = Main.player[Projectile.owner]; 
			Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);
			Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
			projOwner.heldProj = Projectile.whoAmI;
			projOwner.itemTime = projOwner.itemAnimation;

			float rotation = Projectile.velocity.ToRotation();

			float prog = 1f - projOwner.itemAnimation / (float)projOwner.itemAnimationMax;
			//rotation += Rotation(prog);

			Projectile.rotation = rotation + AltRotation(projOwner.itemAnimationMax - projOwner.itemAnimation) * projOwner.direction;

			Items.LobCorpLight.LobItemFrame(projOwner, rotation + AltRotation(projOwner.itemAnimationMax - projOwner.itemAnimation), projOwner.direction);

			Projectile.Center = ownerMountedCenter + new Vector2((60 + Projectile.ai[0]), 0).RotatedBy(Projectile.rotation);

			if (projOwner.itemAnimation >= (int)(projOwner.itemAnimationMax * 0.5f) ||
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
						if (proj.damage > Projectile.damage * 2)
							proj.damage = Projectile.damage * 2;
						else if (proj.damage < Projectile.damage / 2)
							proj.damage = Projectile.damage / 2;
						proj.GetGlobalProjectile<LobotomyGlobalProjectile>().BlackSwanReflected = true;

						for (int i = 0; i < 16; i++)
						{
							int dustType = Main.rand.Next(2, 4);
							Vector2 dustVel = new Vector2(2f * (float)Math.Cos(6.28f * (i / 16f)), 4f * (float)Math.Sin(6.28f * (i / 16f)));
							dustVel = dustVel.RotatedBy(Projectile.velocity.ToRotation());

							Dust.NewDustPerfect(Projectile.Center, dustType, dustVel).noGravity = true;
							if (Main.rand.NextBool(5))
							{
								Dust.NewDustPerfect(proj.Center, dustType, Projectile.velocity / 4f).noGravity = true;
							}

							if (Main.rand.NextBool(3))
							{
								dustType = ModContent.DustType<Misc.Dusts.BlackFeather>();
								float angle = Main.rand.NextFloat(6.28f);
								dustVel = new Vector2(4f * (float)Math.Cos(angle), 6f * (float)Math.Sin(angle));

								Dust.NewDustPerfect(Projectile.Center, dustType, dustVel).noGravity = true;
							}
						}

						SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Sis_Reflect") with { Volume = 0.25f }, Projectile.Center);
						if (Projectile.ai[1] == 0)
						{
							Projectile.ai[1] = 120;
							Projectile.frame = 1;
						}
					}
				}
			}

			if (projOwner.dead || projOwner.itemAnimation == 1)
				Projectile.Kill();
		}

		private float Rotation(float progress)
        {
			float rot;
			if (progress < 0.4f)
            {
				rot = -135 + 160 * (float)Math.Sin(progress / 0.4f * 1.57f);
            }
			else
            {
				rot = 25;
            }
			rot = MathHelper.ToRadians(rot);
			return rot;
        }

		private float AltRotation(int i)
        {
			float rot = -135 + 160 * (float)(1f - Math.Pow(0.8f,i));
			rot = MathHelper.ToRadians(rot);
			return rot;
		}

        public override bool PreDraw(ref Color lightColor)
        {
			Player projOwner = Main.player[Projectile.owner];
			Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
			//Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);
			Vector2 position = Items.LobCorpLight.LobItemLocation(projOwner, tex.Frame(), MathHelper.ToDegrees(Projectile.rotation), projOwner.direction, 2) - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
			Vector2 origin = new Vector2((Projectile.spriteDirection == 1 ? 2 : 62), 64);// + originOffset;
			float rotation = Projectile.rotation + MathHelper.ToRadians(Projectile.spriteDirection == 1 ? 45 : 135);
			SpriteEffects spriteEffect = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			Main.EntitySpriteDraw(tex, position, tex.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame), lightColor * ((float)(255 - Projectile.alpha) / 255f), rotation, origin, Projectile.scale, spriteEffect, 0);

			return false;
        }
    }
}
