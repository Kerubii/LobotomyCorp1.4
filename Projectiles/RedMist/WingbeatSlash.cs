using LobotomyCorp.ModSystems;
using LobotomyCorp.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles.RedMist
{
	public class WingbeatSlash : ModProjectile
	{
        public override string Texture => "LobotomyCorp/Projectiles/WingbeatFairy";

        public override void SetStaticDefaults() {
            // DisplayName.SetDefault("Wingbeat");
        }

		public override void SetDefaults() {
			Projectile.width = 140;
			Projectile.height = 140;
			Projectile.aiStyle = -1;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
			Projectile.alpha = 0;
            Projectile.timeLeft = 15;

			//Projectile.hide = true;
			Projectile.ownerHitCheck = true;
			Projectile.DamageType = DamageClass.Melee;
			//Projectile.tileCollide = false;
			Projectile.friendly = true;

            //Projectile.extraUpdates = 5;

		}

        public override void AI()
        {
            Main.player[Projectile.owner].heldProj = Projectile.whoAmI;
            Projectile.ai[0]++;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Main.player[Projectile.owner].attackCD = Main.player[Projectile.owner].itemAnimationMax / 6;
            target.immune[Projectile.owner] = Main.player[Projectile.owner].itemAnimation;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Main.player[Projectile.owner].attackCD > 0)
                return false;
            return base.CanHitNPC(target);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float prog = (Projectile.ai[0]) / 15f;

            Player player = Main.player[Projectile.owner];
            float opacity = 1f;
            if (prog > 0.5f)
                opacity *= 1f - (prog - 0.5f) / 0.5f;
            CustomShaderData shader = LobotomyCorp.LobcorpShaders["SwingTrail"].UseOpacity(opacity);
            shader.UseImage1(Mod, "Misc/FX_Tex_Trail1");
            shader.UseImage2(Mod, "Misc/FX_Tex_Trail1");
            shader.UseImage3(Mod, "Misc/Worley");

            int dir = player.direction;

            SlashTrail trail = new SlashTrail(24, 1.57f);
            trail.color = new Color(158, 255, 249);

            float rot = -MathHelper.ToRadians(135f) * dir;
            float rotationOffset = 6.8f * (float)Math.Sin(prog * 1.57f) * dir;
            trail.DrawCircle(Projectile.Center, Projectile.velocity.ToRotation() + rot + rotationOffset, dir, 64, 64, shader);
            return false;
        }
    }
}
