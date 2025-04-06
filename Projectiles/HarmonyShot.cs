using LobotomyCorp.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	public class HarmonyShot : ModProjectile
	{
		public override void SetDefaults() {
			Projectile.width = 24;
			Projectile.height = 24;
			Projectile.aiStyle = -1;
			Projectile.penetrate = 3;
			Projectile.scale = 1f;

            Projectile.alpha = 255;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.tileCollide = false;
			Projectile.friendly = true;

			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;
		}

		public override void AI() {
			for (int i = 0; i < 3; i++)
            {
				int n = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Misc.Dusts.NoteDust>());
				Main.dust[n].scale = 0.5f;
				Main.dust[n].noGravity = true;
			}
			if (Main.rand.Next(3) == 0)
			{
				int i = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Misc.Dusts.NoteDust>());
				Main.dust[i].scale = 1f;
				Main.dust[i].noGravity = true;
			}
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			for (int i = 0; i < 8; i++)
			{
				Main.dust[Dust.NewDust(target.position, target.width, target.height, ModContent.DustType<Misc.Dusts.NoteDust>())].velocity.Y -= 1f;
			}

            float bounceDistance = 1000;
			bool hasTarget = false;
            foreach (NPC n in Main.npc)
			{
				float nDist = n.Center.Distance(Projectile.Center);
				if (n.active && n.CanBeChasedBy(this) && Projectile.localNPCImmunity[n.whoAmI] >= 0 && nDist < bounceDistance)
				{
					bounceDistance = nDist;
					float speed = Projectile.velocity.Length();
					Projectile.velocity = n.Center - Projectile.Center;
					Projectile.velocity.Normalize();
					Projectile.velocity *= speed;
					hasTarget = true;
					//TargetPos = n.Center;
				}
			}
			if (!hasTarget)
				Projectile.Kill();
		}
    }

	public class HarmonyShotR : ModProjectile
	{
		public override string Texture => "LobotomyCorp/Projectiles/HarmonyShot";

		public override void SetDefaults()
		{
			Projectile.width = 24;
			Projectile.height = 24;
			Projectile.aiStyle = -1;
			Projectile.penetrate = 1;
			Projectile.scale = 1f;

			Projectile.alpha = 255;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.tileCollide = false;
			Projectile.friendly = true;
		}

		public override void AI()
		{
			if (Projectile.ai[0] != 0)
            {
				float rotation = Projectile.velocity.ToRotation();
				float wave = 160 * (float)Math.Sin(Projectile.ai[1] * 0.1f * Projectile.ai[0]);
				Vector2 wavePosition = new Vector2(0, wave).RotatedBy(rotation);
				Projectile.position -= wavePosition;

				Projectile.ai[1]++;
				wave = 160 * (float)Math.Sin(Projectile.ai[1] * 0.1f * Projectile.ai[0]);
				wavePosition = new Vector2(0, wave).RotatedBy(rotation);
				Projectile.position += wavePosition;
			}

			for (int i = 0; i < 3; i++)
			{
				int n = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Misc.Dusts.NoteDust>());
				Main.dust[n].scale = 0.5f;
				Main.dust[n].noGravity = true;
			}
			if (Main.rand.Next(3) == 0)
			{
				int i = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Misc.Dusts.NoteDust>());
				Main.dust[i].scale = 1f;
				Main.dust[i].noGravity = true;
			}
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			foreach (Player p in Main.ActivePlayers)
			{
				if ((p.whoAmI == Projectile.owner || p.team == Main.player[Projectile.owner].team) && !p.dead)
				{
                    LobotomyHePlayer modPlayer = p.GetModPlayer<LobotomyHePlayer>();
                    modPlayer.HarmonyTime += 30;
					if (modPlayer.HarmonyTime > 600)
                        modPlayer.HarmonyTime = 600;
					p.AddBuff(ModContent.BuffType<Buffs.MusicalAddiction>(), modPlayer.HarmonyTime, true);
				}
			}
			foreach (NPC n in Main.npc)
			{
				if (n.active && !n.friendly && !n.dontTakeDamage && n.Center.Distance(target.Center) < 500)
				{
					n.AddBuff(ModContent.BuffType<Buffs.CrookedNotes>(), 300);
				}
			}

			target.AddBuff(ModContent.BuffType<Buffs.CrookedNotes>(), 600);

			for (int i = 0; i < 3; i++)
			{
				Vector2 spe = new Vector2(16f, 0).RotatedByRandom(6.28f);
                int number = Item.NewItem(Projectile.GetSource_DropAsItem(), target.getRect(), ModContent.ItemType<Items.Ruina.Technology.HarmonyNote>(), 1, true, 0);
                Main.item[number].velocity = spe;
                if (Main.netMode == 1)
                {
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, number, 1f);
                }
            }

            for (int i = 0; i < 8; i++)
			{
				Main.dust[Dust.NewDust(target.position, target.width, target.height, ModContent.DustType<Misc.Dusts.NoteDust>())].velocity.Y -= 1f;
			}
		}
	}
}
