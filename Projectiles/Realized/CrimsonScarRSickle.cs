using LobotomyCorp.ModSystems;
using LobotomyCorp.Players;
using LobotomyCorp.Util;
using LobotomyCorp.Visuals.LobEffects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static tModPorter.ProgressUpdate;

namespace LobotomyCorp.Projectiles.Realized
{
	public class CrimsonScarRSickle : ModProjectile
	{
		public static Asset<Texture2D> Blur;

        public override void Load()
        {
			Blur = ModContent.Request<Texture2D>(Texture + "Blur");
        }

        public override void SetStaticDefaults()
        {
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 30;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
		{
			Projectile.width = 24;
			Projectile.height = 24;
			Projectile.aiStyle = -1;
			Projectile.penetrate = -1;

			Projectile.scale = 1f;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.tileCollide = true;
			Projectile.friendly = true;
			Projectile.extraUpdates = 3;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 20;
		}

        public override void AI()
        {
			// Delays the projectile
            if (Projectile.ai[0] > 0)
			{
				Projectile.ai[0]--;
				if (Projectile.ai[0] == 0)
				{
					if (Main.myPlayer == Projectile.owner)
					{
						Vector2 vel = Main.MouseWorld - Main.player[Projectile.owner].MountedCenter;
						vel.Normalize();
						float speed = 8;
						// ai2 randomizes Projectile properties
						if (Projectile.ai[2] != 0)
						{
							speed = Main.rand.Next(4, 8);
							Projectile.scale = Main.rand.NextFloat(0.6000f, 1f);
						}
						Projectile.velocity = (vel * speed).RotatedBy(Projectile.ai[2]);
						Projectile.netUpdate = true;
					}
					Projectile.rotation = Main.rand.NextFloat(6.28f);
                    SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Language/RedHood_Throw") with { Volume = 0.2f, PitchVariance = 0.1f }, Main.player[Projectile.owner].Center);
                }
				Projectile.Center = Main.player[Projectile.owner].MountedCenter;
                return;
			}
			//  ai1 gives the projectile a timer
			if (Projectile.ai[1] > 0)
			{
				Projectile.ai[1]--;
				if (Projectile.ai[1] < 20 * 4)
				{
					Projectile.alpha += 12 / 4;
				}
				if (Projectile.ai[1] == 0)
					Projectile.Kill();
			}
			else
			{
				if (Projectile.ai[0] == 0)
					Projectile.ai[1]--;
				if (Projectile.ai[1] < -20 * 4)
				{
					float speed = 12f;
					float accel = 0.5f;
					Vector2 delta = Main.player[Projectile.owner].Center - Projectile.Center;
					float dist = delta.Length();
					if (dist > 3000f)
					{
						Projectile.Kill();
					}
					float max = speed / dist;
					delta.X *= max;
					delta.Y *= max;
                    if (Projectile.velocity.X < delta.X)
                    {
                        Projectile.velocity.X += accel;
                        if (Projectile.velocity.X < 0f && delta.X > 0f)
                        {
                            Projectile.velocity.X += accel;
                        }
                    }
                    else if (Projectile.velocity.X > delta.X)
                    {
                        Projectile.velocity.X -= accel;
                        if (Projectile.velocity.X > 0f && delta.X < 0f)
                        {
                            Projectile.velocity.X -= accel;
                        }
                    }
                    if (Projectile.velocity.Y < delta.Y)
                    {
                        Projectile.velocity.Y += accel;
                        if (Projectile.velocity.Y < 0f && delta.Y > 0f)
                        {
                            Projectile.velocity.Y += accel;
                        }
                    }
                    else if (Projectile.velocity.Y > delta.Y)
                    {
						Projectile.velocity.Y -= accel;
                        if (Projectile.velocity.Y > 0f && delta.Y < 0f)
                        {
                            Projectile.velocity.Y -= accel;
                        }
                    }
                    if (Main.myPlayer == Projectile.owner)
                    {
                        if (Main.LocalPlayer.getRect().Intersects(Projectile.getRect()))
                        {
                            Projectile.Kill();
                        }
                    }
                }
			}

			// Dust Spawning
            int size = 70;
            int d = Dust.NewDust(Projectile.Center - new Vector2(size / 2, size / 2), size, size, DustID.Blood, -Projectile.velocity.X, -Projectile.velocity.Y);
            Main.dust[d].noGravity = true;

            Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
            Projectile.rotation += MathHelper.ToRadians(7.5f) * Projectile.spriteDirection;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			Player player = Main.player[Projectile.owner];
            LobotomyWawPlayer wawPlayer = player.GetModPlayer<LobotomyWawPlayer>();
			if (wawPlayer.CrimsonScarLowHealthActive && Main.rand.NextBool(3))
			{
				Vector2 velocity = new Vector2(8 * player.direction, 8).RotatedByRandom(MathHelper.ToRadians(10));
				Vector2 position = target.position + new Vector2(Main.rand.Next(target.width), Main.rand.Next(target.height)) - velocity * 15 * 4;
				int amount = 12;
				for (int i = 0; i < amount; i++)
				{
					float rot = 6.28f * (i / amount);
					Vector2 dustVel = new Vector2(3, 0).RotatedBy(rot);
					Dust d = Dust.NewDustPerfect(position, DustID.Blood, dustVel);
                    d.noGravity = true;
                }
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), position, velocity, ModContent.ProjectileType<CrimsonScarRSickle2>(), Projectile.damage / 4, 0, Projectile.owner, target.whoAmI, 30 * 4);
			}
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.GetGlobalNPC<LobotomyGlobalNPC>().CrimsonScarPrey)
            {
                modifiers.SourceDamage += LobotomyWawPlayer.CrimsonScarPreyBoost;
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            return Projectile.ai[0] <= 0;
        }

        public override bool CanHitPvp(Player target)
        {
            return Projectile.ai[0] <= 0;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
			if (Projectile.ai[1] < 0 && Projectile.ai[1] > -30 * 4)
			{
				Projectile.velocity = -oldVelocity/2;
				if (Projectile.ai[1] > -20 * 3)
					Projectile.ai[1] = -20 * 3;
				return false;
			}
            return base.OnTileCollide(oldVelocity);
        }

        public override bool PreDraw(ref Color lightColor)
        {
			if (Projectile.ai[0] > 0)
				return false;

            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 pos = Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
            Vector2 origin = new Vector2(tex.Width / 2 - 10 * Projectile.spriteDirection, 15);
            Rectangle frame = tex.Frame();
            Texture2D tex2 = Blur.Value;
			Rectangle frame2 = tex2.Frame();
			Vector2 origin2 = frame2.Size() / 2;
			lightColor *= Projectile.Opacity;
			SpriteEffects sp = (Projectile.spriteDirection > 0 ? 0 : SpriteEffects.FlipHorizontally);
			for (int i = 1; i < 30; i++)
			{
				if (i > 2f && i % 8 != 0)
					continue;
				Vector2 oldPos = Projectile.oldPos[i];
				Color trailColor = Lighting.GetColor((int)oldPos.X/16, (int)oldPos.Y/16) * 0.8f * (1f - i/30f) * Projectile.Opacity;
				oldPos += Projectile.Size/2 - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
				float rot = Projectile.oldRot[i];
				if (i <= 2f)
					rot += MathHelper.ToRadians(120) * i;
                Main.EntitySpriteDraw(tex2, oldPos, frame2, trailColor * 0.8f, rot, origin2, Projectile.scale, sp, 0);
                Main.EntitySpriteDraw(tex, oldPos, frame, trailColor, rot, origin, Projectile.scale, sp, 0);
            }
            //Main.EntitySpriteDraw(tex2, pos, frame2, lightColor * 0.3f, Projectile.rotation + MathHelper.ToRadians(120), origin2, Projectile.scale, 0, 0);
            //Main.EntitySpriteDraw(tex2, pos, frame2, lightColor * 0.6f, Projectile.rotation + MathHelper.ToRadians(240), origin2, Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(tex2, pos, frame2, lightColor * 0.8f, Projectile.rotation, origin2, Projectile.scale, sp, 0);
            //Main.EntitySpriteDraw(tex, pos, frame, lightColor * 0.3f, Projectile.rotation + MathHelper.ToRadians(120), origin, Projectile.scale, 0, 0);
            //Main.EntitySpriteDraw(tex, pos, frame, lightColor * 0.6f, Projectile.rotation + MathHelper.ToRadians(240), origin, Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(tex, pos, frame, lightColor, Projectile.rotation, origin, Projectile.scale, sp, 0);
            return false;
        }
    }
}
