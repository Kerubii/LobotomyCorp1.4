using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using LobotomyCorp.Util;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.DataStructures;

namespace LobotomyCorp.Projectiles.Realized
{
	public class FaintAromaSlash : ModProjectile
	{
		public override string Texture => "LobotomyCorp/Projectiles/FaintAromaS";

        public override void SetDefaults()
        {
            Projectile.width = 120;
            Projectile.height = 120;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.alpha = 0;
            Projectile.timeLeft = 20;

            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.friendly = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Projectile.spriteDirection = -1;// Math.Sign(Projectile.velocity.X);
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];

            float Opacity = 1f;
            if (Projectile.timeLeft < 5)
            {
                Opacity = Projectile.timeLeft / 5f;
            }

            CustomShaderData shader = LobotomyCorp.LobcorpShaders["SwingTrail"].UseOpacity(Opacity);
            shader.UseImage1(Mod, "Misc/FlatColor");
            shader.UseImage2(Mod, "Misc/FX_Tex_Trail1");
            shader.UseImage3(Mod, "Misc/Worley");

            SlashTrail trail = new SlashTrail(90, 30, 1.57f);
            Color color1 = new Color(255, 225, 255);
            Color color2 = new Color(249, 159, 253);
            trail.color = Color.Lerp(color2, color1, Opacity) * Opacity;

            float distance = 350 - (200 * Projectile.timeLeft / 20f);
            Vector2 position = Projectile.Center - new Vector2(distance - Projectile.width / 2, 0).RotatedBy(Projectile.rotation);
            //trail.DrawPartCircle(Projectile.Center, Projectile.rotation + MathHelper.ToRadians(140), MathHelper.ToRadians(280), Projectile.direction, 140 - 15, 39, shader);
            trail.DrawPartEllipse( position, Projectile.rotation, MathHelper.ToRadians(140) * Projectile.spriteDirection, MathHelper.ToRadians(280), Projectile.spriteDirection, distance, 60, 32, shader);

            return false;
        }
    }
}
