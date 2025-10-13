using System;
using LobotomyCorp.Items;
using LobotomyCorp.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	public class EngulfingDreamCall : ModProjectile
	{
		public override void SetStaticDefaults() {
            // DisplayName.SetDefault("WAKE UP");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 192;
            Projectile.height = 192;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 62;
            Projectile.extraUpdates = 2;

            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
        }    
            
        public override void AI() {
            Projectile.ai[0]++;
            
            Player player = Main.player[Projectile.owner];
            if (Projectile.ai[1] > 0)
            {
                Projectile.ai[0]++;
                return;
            }
            Vector2 mountedCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            Projectile.Center = mountedCenter + new Vector2(16, 0).RotatedBy(Projectile.velocity.ToRotation());
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CanHitRadius(Projectile.ai[0] * 3, 8, 8, targetHitbox);
        }

        private bool CanHitRadius(float radius, float min, float max, Rectangle hitbox)
        {
            Vector2 delta = hitbox.Center() - Projectile.Center;
            delta.Normalize();
            delta = delta.RotatedBy(3.14f) * (hitbox.Width > hitbox.Height ? hitbox.Width : hitbox.Height);
            delta = hitbox.Center() + delta - Projectile.Center;
            return delta.Length() > radius - min && delta.Length() < radius + max;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDust(target.Center, 1, 1, 16, 4 * (float)Math.Cos((target.Center - Projectile.Center).ToRotation()), 4 * (float)Math.Sin((target.Center - Projectile.Center).ToRotation()));
            }
        }

        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            if (LobItemBase.RedMistMaskUpgrade(Main.player[Projectile.owner], RiskLevel.Teth) && Projectile.ai[1] == 1 && Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner, 0, 2);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 position = Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
            Vector2 origin = texture.Size() / 2;
            Color color = Color.White * 0.2f;
            color.A = (byte)(color.A * 0.8f);
            int limit = 31;
            if (Projectile.ai[1] > 0)
                limit *= 2;
            if (Projectile.ai[0] > limit)
            {
                color *= (Projectile.ai[0] - limit) / (float)limit;
            }
            float scale = (Projectile.ai[0]/23f);
            Main.EntitySpriteDraw(texture, position, (Rectangle)texture.Frame(), color, Main.rand.NextFloat(6.28f), origin, scale, 0, 0);
            return false;
        }
    }
}
