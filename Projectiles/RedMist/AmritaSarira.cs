using LobotomyCorp.NPCs.RedMist;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using LobotomyCorp.Util;

namespace LobotomyCorp.Projectiles.RedMist
{
	public class AmritaSarira : ModProjectile
	{
		public override void SetDefaults() {
            Projectile.height = 16;
            Projectile.width = 18;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;

            Projectile.localNPCHitCooldown = 5;
            Projectile.usesLocalNPCImmunity = true;
		}

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            int order = Order();
            int max = 1 + (int)Math.Floor(owner.statLife / (owner.statLifeMax / 4f));
            if (max > 4)
                max = 4;
            if (order > max)
            {
                Projectile.ai[0] = -1;
            }

            if (Projectile.ai[0] == 0)
            {
                Projectile.velocity *= 0.98f;

                Projectile.ai[1]++;

                if (owner.HeldItem.type != ModContent.ItemType<Items.Waw.Amrita>())
                {
                    Projectile.ai[0] = -1;
                }
                else
                {
                    if (owner.heldProj > -1)
                    {
                        Projectile staff = Main.projectile[owner.heldProj];
                        if (Projectile.getRect().Intersects(staff.getRect()))
                        {
                            Projectile.velocity = Vector2.Normalize(staff.velocity) * 16;
                            Projectile.ai[0]++;
                        }
                    }
                }
            }
            else if (Projectile.ai[0] > 0)
            {
                Projectile.ai[0]++;
                if (Projectile.ai[0] > 60)
                {
                    AIHelper.ChaseTargetDirectAccel(Projectile.Center, owner.MountedCenter, ref Projectile.velocity, 16f, 0.1f);
                    if (Projectile.getRect().Intersects(owner.getRect()))
                    {
                        Projectile.Kill();
                    }
                }
            }
        }

        private int Order()
        {
            int count = 0;
            for (int i = Projectile.whoAmI - 1; i >= 0; i--) 
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.type == Projectile.type && proj.owner == Projectile.owner)
                {
                    count++;
                }
            }
            return count;
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            if (Projectile.ai[1] == 0)
                return;

            NPC n = Main.npc[(int)Projectile.ai[1] - 1];
            if (!n.active || n.life <= 0)
            {
                return;
            }
            hitbox.X = (int)n.position.X;
            hitbox.Y = (int)n.position.Y;
        }
    }
}
