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
	public class Spiderling : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 4;
		}

		public override void SetDefaults()
		{
			Projectile.width = 8;
			Projectile.height = 8; 
			Projectile.aiStyle = -1;

			Projectile.alpha = 255;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.friendly = true;
			Projectile.tileCollide = false;

			Projectile.penetrate = -1;
			Projectile.timeLeft = 300;

			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;
		}

		public override void AI()
		{
			//Find target, if no valid target, fade away
			NPC n = null;
			if (Projectile.ai[0] >= 0)
			{
				n = Main.npc[(int)Projectile.ai[0]];
			}
			else if (Projectile.timeLeft > 20)
				Projectile.timeLeft = 20;

			//Has valid target
			if (n != null)
            {
				if (!n.active || n.life <= 0)
					Projectile.ai[0] = -1;

				Vector2 delta = n.Center -  Projectile.Center;
				float dist = delta.Length();
				delta.Normalize();

				//If not on attack mode, follow target
				if (Projectile.ai[1] == 0)
				{
					int range = (int)((n.width > n.height ? n.width : n.height) * 2f);
					//Check if enemy hitbox is within striking distance, then go to attack mode
					if (dist < range)
					{
						Projectile.ai[1]++;
					}
					float accel = 1.2f;
					float maxSpeed = 8f;
					Projectile.velocity += delta * accel;
					if (Projectile.velocity.Length() > maxSpeed)
					{
						Projectile.velocity.Normalize();
						Projectile.velocity *= maxSpeed;
					}
					Projectile.rotation = Projectile.velocity.ToRotation() - 1.57f;
				}
				//Attack mode, charge up and the strike
				else if (Projectile.ai[1] < 10)
                {
					Projectile.ai[1]++;
					Projectile.velocity *= 0.5f;
					Projectile.rotation = delta.ToRotation() - 1.57f;
                }
				//Leap towards target
				else if (Projectile.ai[1] == 10)
				{
					//localAI[0] Handles the hit effect fade out
					Projectile.localAI[0] = 10;
					Projectile.ai[1]++;
					Projectile.velocity = delta * 16;
					SoundEngine.PlaySound(LobotomyCorp.ItemLobSound("Literature/Spidermom_Babyatk", -1, 1, 0.05f, 0.25f), Projectile.Center);
				}
				//if Projectile did not hit target, return back to follow mode
				else
                {
					Projectile.ai[1]++;
					if (Projectile.ai[1] > 40)
						Projectile.ai[1] = 0;
                }
            }

			//Fade out
			if (Projectile.timeLeft < 20)
			{
				Projectile.velocity *= 0.95f;
				Projectile.alpha += 13;
			}
			else if (Projectile.alpha > 0)
            {
				Projectile.alpha -= 25;
				if (Projectile.alpha < 0)
					Projectile.alpha = 0;
            }

			if (Projectile.localAI[0] > 0)
				Projectile.localAI[0]--;
			
			Projectile.localAI[1] += Projectile.velocity.Length() / 32;
			if (Projectile.ai[1] == 0)
			{
				Projectile.frameCounter++;
				if (Projectile.frameCounter > 5)
				{
					Projectile.frameCounter = 0;
					Projectile.frame++;
					if (Projectile.frame > 3)
						Projectile.frame = 1;
				}
				Projectile.rotation += 0.16f * (float)Math.Sin(Projectile.localAI[1]);
			}
			else
				Projectile.frame = 1;
		}

        public override bool? CanHitNPC(NPC target)
        {
			if (target.whoAmI != (int)Projectile.ai[0] || Projectile.ai[1] < 10)
				return false;
            return Projectile.localNPCImmunity[target.whoAmI] == 0;
        }

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Projectile.ai[0] = -1;
		}

        public override bool PreDraw(ref Color lightColor)
        {
			Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
			Rectangle frame = tex.Frame( 1, Main.projFrames[Projectile.type], 0, Projectile.frame);
			Vector2 origin = frame.Size() / 2;
			Vector2 position = Projectile.Center - Main.screenPosition + Projectile.gfxOffY * Vector2.UnitY;
			lightColor *= 1f - (Projectile.alpha / 255f);

			Main.EntitySpriteDraw(tex, position, frame, lightColor, Projectile.rotation, origin, Projectile.scale, 0, 0);
			return false;
        }
    }
}
