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
using LobotomyCorp.Players;

namespace LobotomyCorp.Projectiles.Realized
{
	public class RedEyesSlash : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Red Eyes");
		}

		public override void SetDefaults()
		{
			Projectile.width = 140;
			Projectile.height = 140; 
			Projectile.aiStyle = -1;
			Projectile.scale = 1f;

			Projectile.DamageType = DamageClass.Melee;
			Projectile.friendly = true;
			Projectile.tileCollide = false;

			Projectile.ownerHitCheck = true;
			Projectile.hide = true;
			Projectile.penetrate = -1;
		}

        public override void AI()
        {
			Player owner = Main.player[Projectile.owner];

			Projectile.Center = owner.MountedCenter;
			Projectile.spriteDirection = owner.direction;
			Projectile.rotation = Projectile.velocity.ToRotation();

			float progress = owner.itemAnimation / (float)owner.itemAnimationMax;

			if (owner.itemAnimation == 1)
				Projectile.Kill();
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
			overPlayers.Add(index);

            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        }

		public override void ModifyDamageHitbox(ref Rectangle hitbox)
		{
			if (Projectile.ai[0] == 1)
			{
				Player owner = Main.player[Projectile.owner];
				float prog = owner.itemAnimation / (float)owner.itemAnimationMax;
				if (prog > 0.5f)
				{
					prog = (prog - 0.5f) / 0.5f;
					prog = (float)Math.Sin(3.14f * prog);
					hitbox.X += (int)(Projectile.velocity.X * 220 * prog);
					hitbox.Y += (int)(Projectile.velocity.Y * 220 * prog);
				}
				base.ModifyDamageHitbox(ref hitbox);
			}
        }

        public override bool? CanHitNPC(NPC target)
        {
			if (Main.player[Projectile.owner].attackCD != 0 && Main.player[Projectile.owner].itemAnimation < Main.player[Projectile.owner].itemAnimationMax/2)
				return false;
            return base.CanHitNPC(target);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
			if (Projectile.ai[0] == 1)
			{
				Player owner = Main.player[Projectile.owner];
				float prog = owner.itemAnimation / (float)owner.itemAnimationMax;
				if (prog > 0.5f)
				{
					prog = 1f - (prog - 0.5f) / 0.5f;
					prog = (float)Math.Sin(3.14f * prog);
					modifiers.FinalDamage += 0.4f * prog;
				}
			}
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			target.immune[Projectile.owner] = Main.player[Projectile.owner].itemAnimation;
			Main.player[Projectile.owner].attackCD = (int)(Main.player[Projectile.owner].itemAnimationMax * 0.2f);
			bool predator = Main.player[Projectile.owner].GetModPlayer<LobotomyTethPlayer>().RedEyesPredator;

            LobotomyGlobalNPC modNPC = target.GetGlobalNPC<LobotomyGlobalNPC>();
			if (Main.player[Projectile.owner].altFunctionUse == 2)
			{
				target.AddBuff(ModContent.BuffType<Buffs.Cocoon>(), 600);
				modNPC.RedEyesCocoon = true;
				modNPC.RedEyesCocoonPlayer = Main.player[Projectile.owner].whoAmI;
			}
			if (target.realLife >= 0)
				Main.npc[target.realLife].GetGlobalNPC<LobotomyGlobalNPC>().RedEyesApplyMeal(60, predatorBoosted: predator);
			else
				modNPC.RedEyesApplyMeal(60, predatorBoosted: predator);

			int dustAmount = Main.rand.Next(4, 8);
			for (int i = 0; i < dustAmount; i++)
            {
				Vector2 speed = Projectile.velocity;// target.Center - Main.player[Projectile.owner].Center;
				speed.Normalize();
				speed *= Main.rand.Next(3, 7);
				Dust d = Dust.NewDustPerfect(target.Center, DustID.Wraith, speed.RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)), 0, default, Main.rand.NextFloat(1f, 2f));
				d.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
		{
			Player owner = Main.player[Projectile.owner];

			float prog = owner.itemAnimation / (float)owner.itemAnimationMax;
			prog = Math.Clamp(prog, 0f, 1f);
			float opacity = 1f;
			if (prog < 0.5f)
				opacity = prog / 0.5f;
			CustomShaderData shader = LobotomyCorp.LobcorpShaders["RedEyesTrail"].UseOpacity(opacity);
			if (Projectile.ai[0] == 0)
			{
				SlashTrail trail = new SlashTrail(60, 1.57f);
				trail.color = Color.White * opacity;
				float rotation = -45;
				float length = 30 + 180 * prog;
				if (prog > 0.5f)
				{
					prog = (prog - 0.5f) / 0.5f;
					rotation += 100f * (float)Math.Sin(1.57f + 1.57f * prog);
				}
				else
				{
					prog = 1f - prog / 0.5f;
					rotation += 100f + 10f * prog;
				}
				float rot = Projectile.rotation + MathHelper.ToRadians(rotation) * Projectile.spriteDirection;
				trail.DrawPartCircle(owner.MountedCenter, rot, MathHelper.ToRadians(length), Projectile.spriteDirection, 110, 32, shader);
			}
			else
            {
				SlashTrail trail = new SlashTrail(220, 60, 1.57f);
				trail.color = Color.White * opacity;
				float rotation = -90;
				float length = 30 + 150 * (float)Math.Sin(3.14f * prog);
				if (prog > 0.5f)
				{
					prog = (prog - 0.5f) / 0.5f;
					rotation += 225f * (float)Math.Sin(1.57f + 1.57f * prog);
				}
				else
				{
					prog = 1f - prog / 0.5f;
					rotation += 225f + 10f * prog;
				}
				float rot = MathHelper.ToRadians(rotation) * Projectile.spriteDirection;
				trail.DrawPartEllipse(owner.MountedCenter + Projectile.velocity * 24f, Projectile.rotation, rot, MathHelper.ToRadians(length), Projectile.spriteDirection, 275, 80, 32, shader);
			}
			return false;
		}
    }
}
