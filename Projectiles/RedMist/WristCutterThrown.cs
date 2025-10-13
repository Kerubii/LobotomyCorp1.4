using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles.RedMist
{
	public class WristCutterThrown : ModProjectile
	{
		public override void SetDefaults() {
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.aiStyle = -1;
            Projectile.penetrate = -1;

            Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.timeLeft = 60;
		}

        public override void AI()
        {
            if (Projectile.ai[0] > 0)
			{
				NPC n = Main.npc[(int)Projectile.ai[1]];
				if (!n.active || n.life < 0 || n.dontTakeDamage)
				{
                    Projectile.ai[0] = -1;
					Projectile.timeLeft = 0;
					Projectile.Kill();
					return;
                }

				Projectile.Center = n.Center + Projectile.velocity;

				if (Main.rand.NextBool(4))
				{
					Dust d = Dust.NewDustPerfect(Projectile.Center + new Vector2(10, 0).RotatedBy(Projectile.rotation), DustID.Blood);
				}

				Projectile.ai[2]++;

				if (Projectile.ai[2] > 120)
				{
					// Spawn Slashes
					if (Projectile.owner == Main.myPlayer)
					{
                        Vector2 pos = Projectile.Center + new Vector2(10, 0).RotatedBy(Projectile.rotation);
						Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos, Vector2.Zero, ModContent.ProjectileType<WristCutterSlash>(), Projectile.damage / 4, 0, Projectile.owner, Projectile.ai[1]);
                    }
					Projectile.ai[2] = 0;
                }
				Projectile.timeLeft = 60;
            }
			else
			{
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
			if (Projectile.ai[0] != 0)
				return false;
            return base.CanHitNPC(target);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			if (Projectile.ai[0] == 0)
			{
                Projectile.ai[0]++;
                Projectile.ai[1] = target.whoAmI;
                Projectile.timeLeft = 60;
				Projectile.velocity = Projectile.Center - target.Center;

                target.AddBuff(ModContent.BuffType<Buffs.Scars>(), 60);
            }
		}

        public override bool ShouldUpdatePosition()
        {
            return Projectile.ai[0] <= 0;
        }

        public override bool PreDraw(ref Color lightColor)
        {
			Vector2 pos = Projectile.Center + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
			Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
			Color color = Projectile.GetAlpha(lightColor);
			Vector2 origin = tex.Size() / 2;
			SpriteEffects spriteEffects = 0;
			if (Projectile.spriteDirection == -1)
				spriteEffects = (SpriteEffects)1;
			float rot = Projectile.rotation + ((float)Math.PI / 4f * Projectile.spriteDirection);
			Main.EntitySpriteDraw(tex, pos, null, color, rot, origin, Projectile.scale, spriteEffects, 0);
			
			return false;
        }
    }
}