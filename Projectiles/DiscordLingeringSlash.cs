using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static LobotomyCorp.LobotomyCorp;

namespace LobotomyCorp.Projectiles
{
    public class DiscordLingeringSlash : ModProjectile
    {
        public override void SetStaticDefaults() {
            //DisplayName.SetDefault("Spear");
            Main.projFrames[Projectile.type] = 9;
        }

        public override void SetDefaults() {
            Projectile.width = 74;
            Projectile.height = 112;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 18;

            //Projectile.hide = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 1)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.White;
            if (Projectile.ai[0] == 0)
            {
                lightColor.R = 0;
                lightColor.G = 0;
                lightColor.B = 0;
            }

            return base.PreDraw(ref lightColor);
        }
    }
}
