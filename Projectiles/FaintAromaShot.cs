using System;
using LobotomyCorp.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	public class FaintAromaShot: ModProjectile
	{
		public override void SetStaticDefaults() {
            //DisplayName.SetDefault("Spear");
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 180;

            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
        }    
            
        public override void AI() {
            if (Main.rand.NextBool(30))
            {
                int i = Dust.NewDust(Projectile.position, 10, 10, DustID.VenomStaff);
                Main.dust[i].noGravity = true;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;

            if (Projectile.ai[0] != 0)
            {
                if (Projectile.ai[0] > 1)
                {
                    Projectile.timeLeft = 180;
                    Projectile.ai[0]--;
                    return;
                }

                float maxRange = 300;
                int result = -1;
                for (int i = 0; i < 200; i++)
                {
                    NPC nPC = Main.npc[i];
                    bool flag = nPC.CanBeChasedBy(this);

                    if (flag && nPC.whoAmI != (int)Projectile.ai[1] - 1)
                    {
                        float dist = Projectile.Distance(Main.npc[i].Center);
                        if (dist < maxRange && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, nPC.position, nPC.width, nPC.height))
                        {
                            maxRange = dist;
                            result = i;
                        }
                    }
                }

                if (result > 0)
                {
                    NPC n = Main.npc[result];
                    float speed = 8f;// Projectile.velocity.Length();

                    AIHelper.ChaseTargetLerp(Projectile.Center, n.Center, ref Projectile.velocity, speed, 0.4f);
                }
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (target.whoAmI == (int)Projectile.ai[1] - 1) { return false; }

            return base.CanHitNPC(target);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 4; i++)
            {
                Dust.NewDust(Projectile.position, 10, 10, DustID.VenomStaff);
            }
        }
    }
}
