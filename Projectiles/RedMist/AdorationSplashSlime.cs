using LobotomyCorp.NPCs.RedMist;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using LobotomyCorp.Util;
using Terraria.GameContent;
using LobotomyCorp.Visuals.LobEffects;

namespace LobotomyCorp.Projectiles.RedMist
{
	public class AdorationSplashSlime : ModProjectile
	{
		public override void SetDefaults() {
            Projectile.height = 16;
            Projectile.width = 16;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;

            Projectile.localNPCHitCooldown = 60;
            Projectile.usesLocalNPCImmunity = true;

            Projectile.timeLeft = 60 * 5;
		}

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (Projectile.ai[1]++ == 0)
            {
                Projectile.rotation = Main.rand.NextFloat(6.28f);   
            }
            if (Projectile.timeLeft < 15)
            {
                Projectile.scale -= 1f / 60f;
            }

            if (Projectile.ai[0] >= 0)
            {
                NPC n = Main.npc[(int)Projectile.ai[0]];
                if (!n.active || n.life <= 0)
                {
                    Projectile.ai[0] = -1;
                    return;
                }

                Projectile.Center = n.Center + Projectile.velocity;
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.ai[0] < 0 && Main.myPlayer == Projectile.owner)
            {
                int enemyyyyyy = Projectile.FindTargetWithLineOfSight(1000);
                if (enemyyyyyy == -1)
                    return;
                NPC n = Main.npc[enemyyyyyy];

                Vector2 vel = Projectile.Center.DirectionTo(n.Center) * 7f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel, ModContent.ProjectileType<MeltyLoveSmall>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai2: -1);
            }
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;

            Vector2 pos = Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
            float wobble = (float)Math.Sin(Projectile.ai[1] * MathHelper.ToRadians(3));
            Vector2 scale = new Vector2(1f + 0.05f * wobble, 1f + 0.05f * -wobble);
            Main.EntitySpriteDraw(tex, pos, null, lightColor, Projectile.rotation, tex.Size() / 2, Projectile.scale * scale, 0, 0);
            return false;
        }
    }
}
