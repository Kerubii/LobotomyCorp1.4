using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles.RedMist
{
    public class CobaltScarBloodSlash : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.height = 56;
            Projectile.width = 56;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;

            Projectile.tileCollide = false;
            Projectile.timeLeft = 15;
            Projectile.friendly = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = Projectile.timeLeft;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.velocity *= 0.985f;
            if (Projectile.timeLeft < 5)
            {
                Projectile.alpha += 255 / 5;
            }

            for (int i = 0; i < 3; i++)
            {
                Dust d = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood)];
                d.noGravity = true;
                d.velocity *= 1.2f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, null, lightColor * Projectile.Opacity, Projectile.rotation, tex.Size()/2, Projectile.scale, 0, 0);
            return false;
        }
    }
}