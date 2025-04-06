using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	public class CrimsonScarScythe: ModProjectile
	{
		public override void SetStaticDefaults() {
            //DisplayName.SetDefault("Spear");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 180;
            Projectile.extraUpdates = 1;

            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.friendly = true;

            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;
        }    
            
        public override void AI() {
            Projectile.rotation += MathHelper.ToRadians(36);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
            {
                int d = Dust.NewDust(Projectile.position, 10, 10, DustID.Ash);
                Main.dust[d].noGravity = false;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = new Vector2(14, 16);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, tex.Frame(), lightColor * (1f - Projectile.alpha / 255f), Projectile.rotation, origin, Projectile.scale, 0, 0);
            for (int i = 0; i < 6; i++)
            {
                Vector2 position = Projectile.oldPos[i * 2] + Projectile.Size / 2 - Main.screenPosition;
                Color color = lightColor * (1f - Projectile.alpha / 255f) * ((6 - i) / 7f);
                Main.EntitySpriteDraw(tex, position, tex.Frame(), color, Projectile.oldRot[i * 2], origin, Projectile.scale, 0, 0);
            }
            return false;
        }
    }
}
