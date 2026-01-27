using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Threading.Channels;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;

namespace LobotomyCorp.Projectiles.RedMist
{
	public class LaetitiaExplosion : ModProjectile
	{
        public override string Texture => "Terraria/Images/Extra_57";

        public override void SetStaticDefaults() {
            //DisplayName.SetDefault("Spear");
        }

        public override void SetDefaults()
        {
            Projectile.width = 200;
            Projectile.height = 200;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 2;

            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = false;
            Projectile.friendly = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            if (Projectile.localAI[1] == 0)
            {
                for (int i = 0; i < 30; i++)
                {
                    Dust d = Dust.NewDustDirect(Projectile.Center - new Vector2(-10, -10), 20, 20, DustID.Blood);
                    d.noGravity = true;
                    d.fadeIn = 1.5f;
                    d.velocity *= Main.rand.NextFloat(.6f, 2f);
                }                
            }
            Projectile.localAI[1]++;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.7f);
            LobotomyGlobalNPC ltarget = target.GetGlobalNPC<LobotomyGlobalNPC>();

            if (ltarget.LaetitiaGiftRM)
            {
                LobotomyGlobalProjectile.LaetitiaExplodeOutcome(target, Projectile.damage, Projectile.owner);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
