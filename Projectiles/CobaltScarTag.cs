using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles.Realized
{
    public class CobaltScarTag : ModProjectile
    {
        public override string Texture => "LobotomyCorp/Projectiles/Slash2A";

        public override void SetDefaults()
        {
            Projectile.height = 12;
            Projectile.width = 12;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;

            Projectile.tileCollide = false;
            Projectile.timeLeft = 610;
            Projectile.friendly = true;

            Projectile.hide = true;
        }

        public override void AI()
        {
            if (Projectile.ai[0] < 0)
                return;

            NPC n = Main.npc[(int)Projectile.ai[0]];
            if (!n.active || n.life <= 0)
            {
                Projectile.ai[0] = -1;
                return;
            }
            Projectile.Center = n.Center;
            Projectile.ai[1]++;
            if (Projectile.ai[1] % 60 == 0)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<CobaltScarExtraSlash>(), Projectile.damage, 0, Projectile.owner, Projectile.ai[0]);
                }
            }
        }

        public override bool? CanDamage()
        {
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}