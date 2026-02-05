using LobotomyCorp.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	public class GreenStemTrap : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 36;
			Projectile.height = 36;
			Projectile.aiStyle = -1;
			Projectile.penetrate = 2;
			Projectile.scale = 1f;
			Projectile.timeLeft = 140;

			Projectile.DamageType = DamageClass.Magic;
			Projectile.tileCollide = false;
			Projectile.friendly = true;

			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;
		}

		public override void AI()
        {
            if (Projectile.ai[0] < 2)
            {
				if (Projectile.localAI[0] == 0)
				{
					Projectile.localAI[0]++;
					Projectile.scale = 0.1f;
				}

                Projectile.scale += 0.1f;
				if (Projectile.scale > 1f)
				{
					Projectile.scale = 1f;
				}
            }

			// If Projectile is near death, just despawn
            if (Projectile.timeLeft < 10)
            {
				Projectile.ai[0] = 2;
				Projectile.scale -= 0.1f;

				return;
            }

            // Spawning projectile
            if (Projectile.ai[0] == 0)
			{
				// Apply random rotation and sprite direction
				if (Projectile.ai[1] == 0)
				{
					Projectile.rotation += Main.rand.NextFloat(6.28f);
					Projectile.spriteDirection = Main.rand.NextBool(2) ? 1 : -1;
				}

				//Make it spin
				Projectile.rotation += MathHelper.ToRadians(5f);

				// Projectile is ready after a delay
				if (Projectile.ai[1]++ > 20)
				{
					for (int i = 0; i < 16; i++)
					{
						Vector2 vel = new Vector2(Main.rand.NextFloat(1.3f, 2f), 0).RotatedByRandom(6.28f);
						int dust = Main.rand.NextBool(2) ? DustID.GrassBlades : DustID.Grass;
						Dust.NewDustPerfect(Projectile.Center, dust, vel).noGravity = true;
					}

					Projectile.ai[0]++;
				}
			}

			// Projectile now checks for nearby enemies to shoot
			else if (Projectile.ai[0] == 1)
			{
				// Find nearest npc in 160, factor in their width and height (smaller side so it doesn't miss)
				float Nearest = 160;
				int target = -1;
				Vector2 targetPos = Vector2.Zero;
				foreach (NPC n in Main.npc)
				{
					if (n.active && !n.dontTakeDamage && n.CanBeChasedBy(this))
					{
						float distance = Projectile.Center.Distance(n.Center) - (n.width < n.height ? n.width : n.height);
						if (distance < Nearest)
						{
							Nearest = distance;
							target = n.whoAmI;
							targetPos = n.Center;
						}
					}
				}

				// Having target valid, spawn a vine towards enemy
				if (target >= 0)
				{
					// Used for vine direction
					Vector2 delta = targetPos - Projectile.Center;
					delta.Normalize();
					if (Main.myPlayer == Projectile.owner)
					{
						Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, delta, ModContent.ProjectileType<GreenStemPoke>(), (int)(Projectile.damage * 0.6f), Projectile.knockBack, Projectile.owner);
					}

					// After spawning the vine, set time to 10 so it dies
					Projectile.ai[0]++;
					Projectile.timeLeft = 10;
				}
			}
		}

        public override bool? CanHitNPC(NPC target)
        {
			return false;
        }

        public override bool CanHitPvp(Player target)
        {
			return false;
        }
    }
}
