using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	public class MagicBulletBullet : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Magic Bullet"); // The English name of the Projectile
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 60; // The length of old position to be recorded
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording Mode
		}

		public override void SetDefaults()
		{
			Projectile.width = 8; // The width of Projectile hitbox
			Projectile.height = 8; // The height of Projectile hitbox
			Projectile.aiStyle = 1; // The ai style of the Projectile, please reference the source code of Terraria
			Projectile.friendly = true; // Can the Projectile deal damage to enemies?
			//Projectile.hostile = true; // Can the Projectile deal damage to the player?
			Projectile.DamageType = DamageClass.Ranged; // Is the Projectile shoot by a ranged weapon?
			Projectile.penetrate = -1; // How many monsters the Projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
			Projectile.timeLeft = 1000; // The live time for the Projectile (60 = 1 second, so 600 is 10 seconds)
			Projectile.light = 0.5f; // How much light emit around the Projectile
			Projectile.ignoreWater = true; // Does the Projectile's speed be influenced by water?
			Projectile.tileCollide = false; // Can the Projectile collide with tiles?
			Projectile.extraUpdates = 3; // Set to above 0 if you want the Projectile to update multiple time in a frame

			AIType = ProjectileID.Bullet; // Act exactly like default Bullet
		}

        public override void AI()
        {
			//Normalize Bullet's velocity
			if (Projectile.ai[0] == 0)
			{
				Projectile.ai[0]++;
				Projectile.velocity.Normalize();
				Projectile.velocity *= 12f;
			}

			Projectile.localAI[0]++;
			if (Projectile.localAI[0] > 4)
			{
				Projectile.localAI[0] = 0;
				int i = Dust.NewDust(Projectile.position - Projectile.getRect().Size(), Projectile.width * 3, Projectile.height * 3, ModContent.DustType<Misc.Dusts.ElecDust>(), 0.05f, 0.05f);
				Main.dust[i].velocity *= 0.01f;
			}
            Player owner = Main.player[Projectile.owner];
            if (owner.team > 0)
			{
                foreach (Player player in Main.ActivePlayers)
                {
					if (player.whoAmI != Projectile.owner && player.team == owner.team && player.getRect().Intersects(Projectile.getRect()))
					{
						PlayerDeathReason reason = PlayerDeathReason.ByProjectile(player.whoAmI, Projectile.whoAmI);
                        player.Hurt(reason, Projectile.damage, Projectile.direction, out Player.HurtInfo hurt, quiet: false);
                        Projectile.ai[2] -= 8;
						if (Projectile.ai[2] <= -8)
						{
							Projectile.ai[2] = -8;
						}
					}
                }
            }

			foreach (NPC n in Main.ActiveNPCs)
			{
				if (n.townNPC && n.immune[Projectile.owner] <= 0 && n.getRect().Intersects(Projectile.getRect()))
				{
					owner.ApplyDamageToNPC(n, Projectile.damage, Projectile.knockBack, Projectile.direction, false, DamageClass.Ranged, true);
					n.immune[Projectile.owner] = 15;
                    Projectile.ai[2] -= 8;
                    if (Projectile.ai[2] <= -8)
                    {
                        Projectile.ai[2] = -8;
                    }
                }
			}
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			Projectile.ai[2]++;
            base.OnHitNPC(target, hit, damageDone);
        }

		/*
        public override bool CanHitPlayer(Player target)
        {
			if (target.whoAmI == Projectile.owner)
				return false;
			return true;
        }*/

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float markiplier = 1f - (Projectile.ai[2] - 1) / 10f * 0.75f;
            if (markiplier < 0.25f)
                markiplier = 0.25f;
            modifiers.FinalDamage *= markiplier;
            markiplier = 1f + 0.05f * AllyAmount();
			if (markiplier > 1.5f)
				markiplier = 1.5f;
            modifiers.FinalDamage *= markiplier;

            base.ModifyHitNPC(target, ref modifiers);
        }

		private int AllyAmount()
		{
			int amount = 0;
            Player owner = Main.player[Projectile.owner];
			if (owner.team == 0)
				return amount;

            foreach (Player player in Main.ActivePlayers)
            {
                if (!player.dead && player.whoAmI != Projectile.owner && player.team == owner.team)
                {
					amount++;
                }
            }
			return amount;
        }

        public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = LobotomyCorp.MagicBulletBullet.Value;

			// Redraw the Projectile with the color not influenced by light
			Rectangle frame = texture.Frame();
			frame.Height /= 2;
			Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				if (k > 0)
					frame.Y = frame.Height;
				Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(Color.White) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.EntitySpriteDraw(texture, drawPos, frame, color, Projectile.rotation - 1.57f, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
			}

			return false;
		}
	}
}
