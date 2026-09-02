using LobotomyCorp.Misc;
using LobotomyCorp.Players;
using LobotomyCorp.Util;
using LobotomyCorp.Visuals.LobEffects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

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
            Projectile.timeLeft = 300;

            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.friendly = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.ai[1] > 0)
            {
                Projectile.Center = Main.player[Projectile.owner].MountedCenter;

                if (Projectile.ai[1] == 1)
                {
                    float length = Projectile.velocity.Length();

                    Projectile.velocity = Projectile.Center.DirectionTo(Main.MouseWorld) * length;
                    Projectile.netUpdate = true;
                }

                Projectile.ai[1]--;
                return;
            }

            Dust dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.VenomStaff)];
            dust.noGravity = true;

            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0]++;
                WeaponSmearEllipse smear = new WeaponSmearEllipse();
                float dist = 180;
                smear.Setup(Projectile, -Vector2.Normalize(Projectile.velocity) * (dist/2), Projectile.rotation, (int)Projectile.ai[0], Projectile.spriteDirection);
                smear.SetupPartEllipse(45, 20, dist, 60, MathHelper.ToRadians(120) * Projectile.spriteDirection, MathHelper.ToRadians(240));

                smear.SetShaderImage(
                    MiscAssets.FlatColor,
                    MiscAssets.TexTrail1,
                    MiscAssets.Worley
                    );
                smear.Color = new Color(249, 159, 253);

                smear.AddEffect();
            }

            if (Projectile.ai[0] <= 0)
                Projectile.Kill();
            Projectile.ai[0]--;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.ai[1] <= 0)
                return null;
            return false;
        }

        public override bool CanHitPvp(Player target)
        {
            return Projectile.ai[1] <= 0;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            LobotomyWawPlayer wawPlayer = Main.player[Projectile.owner].GetModPlayer<LobotomyWawPlayer>();
            int variableAmount = 5;
            if (wawPlayer.FaintAromaPetalNum() <= 1)
                variableAmount *= 3;
            wawPlayer.FaintAromaAddPetal(variableAmount);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            /*
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
            */
            return false;
        }
    }
}
