using LobotomyCorp.Misc;
using LobotomyCorp.Players;
using LobotomyCorp.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static LobotomyCorp.LobotomyCorp;

namespace LobotomyCorp.Projectiles.Realized
{
	public class DaCapoMusicSlash : ModProjectile
	{
        public override string Texture => "LobotomyCorp/Projectiles/Slash2A";

        public override void SetDefaults() {
			Projectile.height = 80;
			Projectile.width = 80;
			Projectile.aiStyle = -1;
			Projectile.penetrate = -1;

			Projectile.tileCollide = false;
			Projectile.timeLeft = 300;
			Projectile.friendly = true;
		}

        public override void AI()
        {
			Player owner = Main.player[Projectile.owner];
            owner.heldProj = Projectile.whoAmI;
            Projectile.ai[2]++;
            if (Projectile.ai[2] > (int)(Projectile.ai[1] * 1.5f))
            {
                Projectile.Kill();
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
			Projectile.Center = owner.Center;
            if (Projectile.localAI[0] == 0)
            {
                if ((int)Projectile.ai[0] == -1)
                {
                    SoundEngine.PlaySound(LobotomyCorp.WeaponSound("silent2_1"), Projectile.Center);
                }
                else if ((int)Projectile.ai[0] == 1)
                {
                    SoundEngine.PlaySound(LobotomyCorp.WeaponSound("silent2_2") , Projectile.Center);
                }
                else
                {
                    SoundEngine.PlaySound(LobotomyCorp.WeaponSound("silent2_3"), Projectile.Center);
                }

                Projectile.localAI[0] = Main.rand.Next(2000);
                Projectile.localAI[0]++;
            }
            Projectile.localAI[0] += 5;
            Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
		}

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            float progress = Projectile.ai[2] / (int)(Projectile.ai[1] * 1.5f);
            if (Projectile.ai[0] == 0)
            {
                int width = (int)(300 + 380 * progress);
                hitbox.X += hitbox.Width / 2 - width / 2;
                hitbox.Width = width;
                hitbox.Y -= 20;
                hitbox.Height += 40;
                return;
            }
            Vector2 target = Projectile.Center + new Vector2(100 + 75 * progress, 0).RotatedBy(Rotation(progress, Math.Sign(Projectile.spriteDirection * (int)Projectile.ai[0])));

            hitbox.X = (int)(target.X - hitbox.Width / 2);
            hitbox.Y = (int)(target.Y - hitbox.Height / 2);
        }

        /*public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.ai[0] == 0)
                return base.Colliding(projHitbox, targetHitbox);

            for (int i = -2; i < 3; i++)
            {

            }
        }*/

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.ai[2] > (int)Projectile.ai[1] || (Projectile.ai[0] != 0 && Main.player[Projectile.owner].attackCD != 0))
                return false;

            return base.CanHitNPC(target);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            player.attackCD = (int)(Projectile.ai[1] / 5);

            if (Items.Ruina.Art.DaCapoR.GetPlayerCombo(player) == 1)
            {
                Items.Ruina.Art.DaCapoR.SetPlayerCombo(player, 2, player.itemAnimation + 8);
            }

            if (Items.Ruina.Art.DaCapoR.GetPlayerCombo(player) == 3)
            {
                Items.Ruina.Art.DaCapoR.SetPlayerCombo(player, 4, 60);
            }

            LobotomyAlephPlayer modPlayer = player.GetModPlayer<LobotomyAlephPlayer>();
            if (modPlayer.DaCapoSilentMusic)
                modPlayer.DaCapoTotalDamage += damageDone;
        }

        private float Rotation(float progress, int direction)
        {
            float rot = Projectile.velocity.ToRotation() - MathHelper.ToRadians(125) * direction;
            if (progress < .66f)
            {
                progress /= .66f;
                rot += MathHelper.ToRadians(265) * (float)Math.Sin(1.57f * progress) * direction;
            }
            else
            {
                progress = (progress - .66f) / .33f;
                rot += MathHelper.ToRadians(265 + 5 * progress) * direction;
            }
            return rot;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player projOwner = Main.player[Projectile.owner];
            // Circle rounder damage
            if (Projectile.ai[0] == 0)
            {
                float opacity = 1f;
                float distance = 100;
                float progress = Projectile.ai[2] / (int)(Projectile.ai[1] * 1.5f);
                int direction = 1;

                float xbig = 60 + 60 * progress;
                float ybig = 20 + 20 * progress;

                distance += 380 * progress;

                float rot = Rotation(progress, direction);
                if (progress > .66f)
                {
                    opacity = 1f - (progress - .66f) / .33f;
                }

                SlashTrail trail = new SlashTrail(xbig, ybig, 1.57f);
                CustomShaderData shader = LobcorpShaders["SwingTrail"].UseOpacity(opacity);
                shader.UseImage1(MiscAssets.RainbowTexture);
                shader.UseImage2(MiscAssets.TerrariaExtra201);
                shader.UseImage3(MiscAssets.PlasmaNoise);
                shader.UseCustomShaderDate(0, 0, 0, Projectile.localAI[0]);
                trail.color = Color.White;

                trail.DrawEllipse(Projectile.Center, Projectile.rotation, rot, direction, distance, distance * 0.2f, 60, shader);

                float diff = 30;
                trail = new SlashTrail(xbig + diff * 2, ybig + diff, 1.57f);
                shader = LobcorpShaders["SwingTrail"].UseOpacity(opacity);
                shader.UseCustomShaderDate(0, 0, 0, Projectile.localAI[0]);
                trail.color = Color.White * opacity;

                shader.UseImage1(MiscAssets.SheetNote2RGBGlow);
                shader.UseImage2(MiscAssets.SheetNote2WAGlow);
                shader.UseImage3(MiscAssets.SheetNote2WAGlow);

                trail.DrawEllipse(Projectile.Center, Projectile.rotation, rot, direction, distance + diff, distance * 0.2f + diff/2, 60, shader);

                shader.UseImage1(MiscAssets.SheetNote2WA);
                shader.UseImage2(MiscAssets.SheetNote2WA);
                shader.UseImage3(MiscAssets.SheetNote2WA);

                trail.DrawEllipse(Projectile.Center, Projectile.rotation, rot, direction, distance + diff, distance * 0.2f + diff / 2, 60, shader);

                shader.UseCustomShaderDate(0);
                return false;
            }
            else
            {
                SlashTrail trail = new SlashTrail(40, 1.57f);

                float opacity = 1f;
                float length = 0.4f;// + 0.6f * pprog;
                float distance = 100;
                float progress = Projectile.ai[2] / (int)(Projectile.ai[1] * 1.5f);
                int direction = Math.Sign(Projectile.spriteDirection * (int)Projectile.ai[0]);

                //progress = (progress - 0.5f) / 0.4f;
                distance += 75 * progress;

                length += 1.2f * progress;
                if (length > 1f)
                    length = 1f;

                float rot = Rotation(progress, direction);
                if (progress > .66f)
                {
                    opacity = 1f - (progress - .66f) / .33f;
                }

                CustomShaderData shader = LobcorpShaders["SwingTrail"].UseOpacity(opacity);
                shader.UseImage1(MiscAssets.FlatColor);
                shader.UseImage2(MiscAssets.TerrariaExtra201);
                shader.UseImage3(MiscAssets.PlasmaNoise);
                //shader.UseCustomShaderDate(Projectile.localAI[0], Projectile.localAI[1]);
                trail.color = Color.Black;// * opacity;

                trail.DrawPartCircle(projOwner.MountedCenter, rot, MathHelper.ToRadians(180f) * length, direction, distance, 60, shader);

                trail = new SlashTrail(60, 1.57f);

                shader = LobcorpShaders["SwingTrail"].UseOpacity(0.92f * opacity);
                shader.UseImage1(MiscAssets.SheetNote2B);
                shader.UseImage2(MiscAssets.SheetNote2A);
                shader.UseImage3(MiscAssets.TerrariaExtra201);
                shader.UseCustomShaderDate(0, 0, 1200 * (0.1f + progress), Projectile.localAI[0]);
                trail.color = Color.White;

                trail.DrawPartCircle(projOwner.MountedCenter, rot, MathHelper.ToRadians(180f) * length, direction, distance, 60, shader);
                shader.UseCustomShaderDate(0); // Reset it fucks up the others :V
                return false;
            }
        }
    }
}