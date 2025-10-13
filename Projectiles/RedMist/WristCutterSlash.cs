using LobotomyCorp.ModSystems;
using LobotomyCorp.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles.RedMist
{
	public class WristCutterSlash : ModProjectile
	{
        public override string Texture => "LobotomyCorp/Projectiles/SpearTrail";

        public override void SetStaticDefaults() {
            // DisplayName.SetDefault("Wingbeat");
        }

		public override void SetDefaults() {
			Projectile.width = 32;
			Projectile.height = 32;
			Projectile.aiStyle = -1;
			Projectile.penetrate = -1;
            Projectile.timeLeft = 20;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.friendly = true;
		}

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.rotation = Main.rand.NextFloat(6.28f);
                Projectile.localAI[0]++;
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (target.whoAmI == (int)Projectile.ai[0])
                return base.CanHitNPC(target);
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 pos = Projectile.Center + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
            Texture2D tex = SpearExtender.SpearTrail;
            Rectangle? sourceRectangle2 = null;
            Color color = Projectile.GetAlpha(Color.DarkRed);
            Vector2 origin = new Vector2(0, tex.Height/2);
            SpriteEffects spriteEffects = 0;
            Vector2 scale = new Vector2(0.2f, 0.2f) * Projectile.scale;
            pos -= new Vector2(tex.Width / 2 * scale.X, 0).RotatedBy(Projectile.rotation);
            if (Projectile.timeLeft > 10)
            {
                scale.X *= 1f - (Projectile.timeLeft - 10f) / 10f;
            }
            else
            {
                scale.Y *= Projectile.timeLeft / 10f;
            }

            Main.EntitySpriteDraw(tex, pos, sourceRectangle2, color, Projectile.rotation, origin, scale, spriteEffects, 0);

            return false;
        }
    }
}
