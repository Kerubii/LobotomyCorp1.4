using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using LobotomyCorp.Utils;
using Terraria.GameContent;
using System.IO;
using LobotomyCorp.Players;

namespace LobotomyCorp.Projectiles.Realized
{
	public class MagicBulletR : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Magic Bullet"); // The English name of the Projectile
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 100; // The length of old position to be recorded
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2; // The recording Mode
		}

		public override void SetDefaults()
		{
			Projectile.width = 8; // The width of Projectile hitbox
			Projectile.height = 8; // The height of Projectile hitbox
			Projectile.aiStyle = -1; // The ai style of the Projectile, please reference the source code of Terraria
			Projectile.friendly = true; // Can the Projectile deal damage to enemies?
			Projectile.hostile = false; // Can the Projectile deal damage to the player?
			Projectile.DamageType = DamageClass.Ranged; // Is the Projectile shoot by a ranged weapon?
			Projectile.penetrate = -1; // How many monsters the Projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
			Projectile.timeLeft = 1000; // The live time for the Projectile (60 = 1 second, so 600 is 10 seconds)
			Projectile.light = 0.5f; // How much light emit around the Projectile
			Projectile.ignoreWater = true; // Does the Projectile's speed be influenced by water?
			Projectile.tileCollide = false; // Can the Projectile collide with tiles?
			Projectile.extraUpdates = 5; // Set to above 0 if you want the Projectile to update multiple time in a frame
			Projectile.netImportant = true;

			AIType = ProjectileID.Bullet; // Act exactly like default Bullet
			PlayerTarget = false;
		}

		public bool PlayerTarget;

        public override void AI()
        {
			if (!PlayerTarget)
			{
				LobotomyWawPlayer modPlayer = Main.player[Projectile.owner].GetModPlayer<LobotomyWawPlayer>();
				if (modPlayer.MagicBulletNthShot == 6)
				{
					Projectile.usesLocalNPCImmunity = true;
					Projectile.localNPCHitCooldown = 15;
				}
			}

			Projectile.localAI[0]++;
			if (Projectile.localAI[0] > 1)
			{
				Projectile.localAI[0] = 0;
				int i = Dust.NewDust(Projectile.position - Projectile.getRect().Size(), Projectile.width * 3, Projectile.height * 3, ModContent.DustType<Misc.Dusts.ElecDust>(), 0.05f, 0.05f);
				Main.dust[i].velocity *= 0.01f;
			}
			Projectile.rotation = Projectile.velocity.ToRotation();

			//Redirect motherfucker
			Projectile.ai[1]++;
			if (Projectile.timeLeft > 150 && Projectile.ai[1] > 15 * Projectile.extraUpdates)
			{
				if (PlayerTarget)
				{
					Player p = Main.player[(int)Projectile.ai[0] - 1];
					if (p.active && !p.dead)
					{
						float angle = Terraria.Utils.AngleLerp(Projectile.velocity.ToRotation(), (p.Center - Projectile.Center).ToRotation(), MathHelper.ToRadians(7));

						Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0).RotatedBy(angle);
					}
					else Projectile.ai[0] = -1;
				}
				if (Projectile.ai[0] > 0)
                {
					NPC n = Main.npc[(int)Projectile.ai[0] - 1];
					if (n.active && n.life > 0)
					{
						float angle = Terraria.Utils.AngleLerp(Projectile.velocity.ToRotation(), (n.Center - Projectile.Center).ToRotation(), MathHelper.ToRadians(7));

						Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0).RotatedBy(angle);
					}
					else Projectile.ai[0] = -1;
                }
            }

			if (Projectile.ai[0] < 0)
				Projectile.alpha = Projectile.alpha + 2;
			if (Projectile.alpha >= 255)
				Projectile.alpha = 255;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
			writer.Write(PlayerTarget);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
			PlayerTarget = reader.ReadBoolean();
        }

        public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

			// Redraw the Projectile with the color not influenced by light
			Rectangle frame = texture.Frame();
			Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
			Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
			float Opacity = 1f - ((float)Projectile.alpha / 255f);
			Color color = Color.White * Opacity;
			Main.EntitySpriteDraw(texture, drawPos, frame, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

			CustomShaderData shader = LobotomyCorp.LobcorpShaders["MagicBulletTrail"].UseOpacity(0.5f);
			SlashTrail trail = new SlashTrail(40, 0);
			trail.color = Color.White * Opacity;

			trail.DrawTrail(Projectile, shader);
			return false;
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            LobotomyWawPlayer modPlayer = Main.player[Projectile.owner].GetModPlayer<LobotomyWawPlayer>();

            switch (modPlayer.MagicBulletNthShot)
            {
				case 3:
					target.AddBuff(BuffID.OnFire, 600);
					break;
				case 4:
					target.AddBuff(BuffID.Ichor, 600);
					break;
				case 5:
					target.AddBuff(BuffID.OnFire, 1200);
					break;
            }

			Projectile.ai[0] = -1;
			Projectile.timeLeft = 150;
        }

        public override bool? CanHitNPC(NPC target)
        {
			if (target.friendly && target.townNPC && Projectile.ai[0] < 0)
				return false;
            return base.CanHitNPC(target);
        }

        public override bool CanHitPlayer(Player target)
        {
			if (target.whoAmI != Projectile.ai[0] && Projectile.ai[0] < 0)
				return false;
			return base.CanHitPlayer(target);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
			Projectile.ai[0] = -1;
			Projectile.timeLeft = 150;
		}
    }
}
