using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	public class LampProjectile : ModProjectile
	{
		public override void SetDefaults() {
			Projectile.height = 8;
			Projectile.width = 8;
			Projectile.aiStyle = -1;

			Projectile.tileCollide = false;
			Projectile.timeLeft = 300;
			Projectile.friendly = true;

			Projectile.alpha = 255;
		}

		private int Target => (int)Projectile.ai[0];

		public override void AI()
		{
			Projectile.ai[1]++;
			if (Projectile.ai[1] < 60)
			{
				Projectile.velocity *= 0.94f;
                int type = DustID.GemTopaz;
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, type);
                Main.dust[d].noGravity = true;
				Projectile.alpha -= 20;
				if (Projectile.alpha < 0)
					Projectile.alpha = 0;
            }
			else
			{
				if (Projectile.ai[1] == 60)
				{
					for (int i = 0; i < 4; i++)
					{
						float angle = 1.57f * i;
						Vector2 vel = new Vector2(4, 0).RotatedBy(angle);
						Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.GemTopaz, vel * 1.2f);
						d.noGravity = true;
						d = Dust.NewDustPerfect(Projectile.Center, DustID.GemTopaz, vel);
						d.noGravity = true;
						d = Dust.NewDustPerfect(Projectile.Center, DustID.GemTopaz, vel * 0.5f);
						d.noGravity = true;
					}

					Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 16f;
				}

				Vector2 targetPos = Projectile.Center + Projectile.velocity;

				if (Target >= 0)
				{
					NPC n = Main.npc[Target];
					if (n.life <= 0 || !n.active)
						Projectile.ai[0] = -1;
					else
						targetPos = n.Center;
				}
				else
				{
					float dist = 2000;
					foreach (NPC n in Main.npc)
					{
						float mag = Projectile.Center.Distance(n.Center);
						if (n.CanBeChasedBy(this) && mag < dist)
						{
							dist = mag;
							targetPos = n.Center;
						}
					}
				}
				/*
				float speed = 16f;
				Vector2 delta = targetPos - Projectile.Center;
				float magni = delta.Length();
				Vector2 vel = delta * (speed / magni);
				Projectile.velocity = (Projectile.velocity * 16f + vel) / 17f;*/
				float targetAngle = (targetPos - Projectile.Center).ToRotation();
				float currentAngle = Projectile.velocity.ToRotation();
				float deviation = MathHelper.ToRadians(1 + (Projectile.ai[1] - 60) / 5f);
				float newAngle = Terraria.Utils.AngleLerp(currentAngle, targetAngle, deviation);
				Projectile.velocity = new Vector2(16, 0).RotatedBy(newAngle);
                for (int i = 0; i < 2; i++)
                {
                    int type = DustID.GemTopaz;
                    int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, type);
                    Main.dust[d].noGravity = true;
                }
            }

			Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
		}

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 4; i++)
            {
                float angle = 1.57f * i;
                Vector2 vel = new Vector2(4, 0).RotatedBy(angle);
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.GemTopaz, vel * 1.2f);
                d.noGravity = true;
                d = Dust.NewDustPerfect(Projectile.Center, DustID.GemTopaz, vel);
                d.noGravity = true;
                d = Dust.NewDustPerfect(Projectile.Center, DustID.GemTopaz, vel * 0.5f);
                d.noGravity = true;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
			modifiers.ScalingArmorPenetration += 1f;
        }

        public override bool? CanHitNPC(NPC target)
        {
			if (Projectile.ai[1] < 60)
				return false;
			if (target.type == NPCID.TargetDummy && Target >= 0 && Target != target.whoAmI)
				return false;

            return base.CanHitNPC(target);
        }

        public override bool PreDraw(ref Color lightColor)
        {
			Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
			Rectangle frame = tex.Frame();
			lightColor = Color.White;
			lightColor.A = 100;
			lightColor *= 1f - (Projectile.alpha / 255f);
			Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, frame, lightColor, Projectile.rotation, frame.Size() / 2, Projectile.scale, 0, 0);

            return false;
        }
    }
}