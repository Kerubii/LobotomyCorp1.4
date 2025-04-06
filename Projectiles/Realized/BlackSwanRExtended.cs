using LobotomyCorp.Players;
using LobotomyCorp.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles.Realized
{
	public class BlackSwanRExtended : ModProjectile
	{
		public override string Texture => "LobotomyCorp/Projectiles/Realized/BlackSwanR";

		public override void SetStaticDefaults()
        {
			Main.projFrames[Projectile.type] = 2;
			//ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
			//ProjectileID.Sets.TrailingMode[Projectile.type] = 4;
		}

        public override void SetDefaults() {
			Projectile.width = 18;
			Projectile.height = 18;
			Projectile.aiStyle = -1;
			Projectile.penetrate = -1;
			Projectile.scale = 1.3f;
			Projectile.alpha = 0;
			Projectile.timeLeft = 14;

			//Projectile.hide = true;
			Projectile.ownerHitCheck = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.tileCollide = false;
			Projectile.friendly = true;
			Projectile.hide = true;
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
			projOwner.heldProj = Projectile.whoAmI;
			Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);
			Projectile.direction = projOwner.direction;
			Projectile.position.X = ownerMountedCenter.X - (float)(Projectile.width / 2);
			Projectile.position.Y = ownerMountedCenter.Y - (float)(Projectile.height / 2);
			if (!projOwner.frozen) {
				if (movementFactor == 0f) // When initially thrown out, the ai0 will be 0f
				{
					SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
					movementFactor = 2f; // Make sure the spear moves forward when initially thrown out
					Projectile.netUpdate = true; // Make sure to netUpdate this spear
				}
				movementFactor += 6f;
			}
			// Change the spear position based off of the velocity and the movementFactor
			Projectile.position += Projectile.velocity * movementFactor;
			Projectile.rotation = Projectile.velocity.ToRotation();

			if (Projectile.timeLeft % 3 == 0)
            {
				int type = Main.rand.Next(2, 4);
				for (int i = 0; i < 8; i++)
				{
					Vector2 vel = new Vector2(3 * (float)Math.Cos(MathHelper.ToRadians(45 * i)), 6 * (float)Math.Sin(MathHelper.ToRadians(45 * i)));
					vel = vel.RotatedBy(Projectile.velocity.ToRotation()) - Projectile.velocity * 0.2f;

					Dust d = Dust.NewDustPerfect(Projectile.Center, type, vel);
					d.noGravity = true;

				}
			}

			if (Projectile.timeLeft > 5)
				Projectile.localAI[0] += Projectile.velocity.Length() * 6;
			else
				Projectile.localAI[0] -= Projectile.velocity.Length() * 6;

			for (int i = 0; i < 3; i++)
            {
				int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Main.rand.Next(2, 4));
				Main.dust[d].velocity = Projectile.velocity * -0.2f;
				Main.dust[d].noGravity = true;
				
				if (Main.rand.NextBool(6))
                {
					Vector2 vel = (-Projectile.velocity).RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f));
					vel.Normalize();
					vel *= Main.rand.NextFloat(1f, 8f);
					Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, ModContent.DustType<Misc.Dusts.BlackFeather>(), vel.X, vel.Y);
				}
			}

			if (projOwner.dead)
				Projectile.Kill();
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			if (Main.player[Projectile.owner].GetModPlayer<LobotomyWawPlayer>().BlackSwanNettleClothing >= 5)
			{
				target.AddBuff(BuffID.Ichor, 300);
			}
        }

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D pierceTrail = SpearExtender.SpearTrail;
			Vector2 pos = Projectile.Center - Main.screenPosition + Projectile.gfxOffY * Vector2.UnitY;
			Rectangle frame = pierceTrail.Frame();
			Vector2 origin = frame.Size() / 2;
			origin.X += frame.Width / 4;
			Vector2 scale = new Vector2((Projectile.localAI[0] / 192f), Projectile.scale * 0.25f);
			float opacity = Math.Clamp(Projectile.timeLeft / 3f, 0f, 1f);

			Color BaseColor = new Color(103, 232, 192) * opacity;
			Main.EntitySpriteDraw(pierceTrail, pos, frame, BaseColor, Projectile.rotation, origin, scale, 0, 0);

			BaseColor = Color.White * opacity;
			scale.X *= 0.66f;
			scale.Y *= 0.33f;
			Main.EntitySpriteDraw(pierceTrail, pos, frame, Color.White, Projectile.rotation, origin, scale, 0, 0);


			SlashTrail trail = new SlashTrail(0, 0);
			trail.color = new Color(0, 150, 0);

			Player projOwner = Main.player[Projectile.owner];
			float trailScale = Projectile.scale;
			trailScale *= Projectile.timeLeft / 14f;
			if (Projectile.timeLeft < 5)
			{
				//trail.color *= opacity;
			}
			CustomShaderData shader = LobotomyCorp.LobcorpShaders["SwingTrail"].UseOpacity(opacity);
			shader.UseImage1(Mod, "Misc/Noise4");
			shader.UseImage2(Mod, "Misc/Trail713");
			shader.UseImage3(Mod, "Misc/Trail52");

			int max = Projectile.timeLeft > 4 ? 10 - (Projectile.timeLeft - 4) : 10;
			max = (int)(max * 0.7f);
			if (max < 2)
				max = 2;
			Vector2[] trail1 = new Vector2[max];
			Vector2[] trail2 = new Vector2[max];
			float[] rot = new float[max];
			for (int i = 0; i < max; i++)
			{
				Vector2 center = Projectile.Center - Projectile.velocity * i * 6 - Main.screenPosition;
				rot[i] = Projectile.rotation;
				float distance = 30 + 60 * trailScale;
				trail1[i] = center + new Vector2(0, distance).RotatedBy(Projectile.rotation);
				trail2[i] = center + new Vector2(0, -distance).RotatedBy(Projectile.rotation);
			}

			trail.DrawManual(trail1, trail2, shader);
			return false;
			/*
			SlashTrail trail = new SlashTrail(24, MathHelper.ToRadians(0));//, 0.785f - MathHelper.ToRadians(Projectile.direction == 1 ? 0 : 90));
			trail.color = new Color(0, 100, 0);

			Player projOwner = Main.player[Projectile.owner];
			float prog = (float)Math.Sin(1.57f * (Projectile.timeLeft / 14f));
			if (prog < 0)
				prog = 0;
			CustomShaderData windTrail = LobotomyCorp.LobcorpShaders["WindTrail"].UseOpacity(prog);

			int max = Projectile.timeLeft > 4 ? 10 - (Projectile.timeLeft - 4) : 10;
			Vector2[] posTrail = new Vector2[max];
			float[] rot = new float[max];
			for (int i = 0; i < posTrail.Length; i++)
            {
				posTrail[i] = Projectile.Center - Projectile.velocity * i * 6;
				rot[i] = Projectile.rotation;
            }

			trail.DrawSpecific(posTrail, rot, Vector2.Zero, windTrail);
			
			return false;*/
        }
    }
}
