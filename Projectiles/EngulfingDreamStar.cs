using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	public class EngulfingDreamStar : ModProjectile
	{
        public override string Texture => "Terraria/Images/Projectile_927";

        public override void SetStaticDefaults() {
            //DisplayName.SetDefault("Spear");
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 0.8f;
            Projectile.timeLeft = 300;

            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
                Projectile.localAI[0] += Main.rand.Next(360);
            Projectile.localAI[0]++;
            Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.YellowStarDust)].noGravity = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = tex.Size() / 2;
            Vector2 position = Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
            Rectangle frame = tex.Frame();
            for (int i = 0; i < 8; i++)
            {
                float scale = 0.5f + 0.5f * (float)Math.Sin(MathHelper.ToRadians(i * 42f));
                float rotation = MathHelper.ToRadians(Projectile.localAI[0] + i * 21) * (i % 3 == 0 ? 1 : -1);
                Color color = new Color(255, 247, 176);
                Main.EntitySpriteDraw(tex, position, frame, color * 0.2f, rotation, origin, scale, 0);
            }
            return false;
        }
    }
}
