using LobotomyCorp.Items;
using LobotomyCorp.Util;
using LobotomyCorp.Visuals.LobEffects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static LobotomyCorp.Misc.MiscAssets;

namespace LobotomyCorp.Projectiles.RedMist
{
    public class LanternNom : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 120;
            Projectile.height = 120;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;

            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.friendly = true;

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 20;

            //DrawHeldProjInFrontOfHeldItemAndArms = true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.ai[1] >= 0 && Projectile.ai[2] > 0)
            {
                NPC target = Main.npc[(int)Projectile.ai[1]];
                if (target.life <= 0)
                    Projectile.Kill();
                Projectile.Center = target.Center;

                Projectile.ai[2]--;
                Projectile.ai[0] = 0;
                Projectile.localAI[0] = MathHelper.ToRadians(90);
                Projectile.scale = 0;
                Projectile.localAI[1] = -20;
                Projectile.timeLeft = 30;
                return;
            }

            if (Projectile.soundDelay == 0)
            {
                SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Entity/MeatLantern/Bunny_Start") with { Volume = 0.2f }, Projectile.Center);
                Projectile.soundDelay = 600;
            }
            
            Projectile.ai[0]++;

            if (Projectile.ai[0] <= 10)
            {
                Projectile.localAI[0] = MathHelper.ToRadians(90) * Easing.EaseOutCubic(1f - Projectile.ai[0] / 10);
                Projectile.scale = Math.Min(1f, Projectile.ai[0] / 5f);
                Projectile.localAI[1] = -20 * (1f - Projectile.ai[0] / 10f);
            }
            else if (Projectile.ai[0] > 20)
            {
                Projectile.localAI[1] = -90 * Easing.EaseOutCubic((Projectile.ai[0] - 20f) / 10f);
                Projectile.alpha += 255 / 10;
            }

            if (Projectile.ai[0] == 10)
            {
                for (int i = 0; i < 15; i++)
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood);
                }
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            return Projectile.ai[0] > 5 && Projectile.ai[0] < 20;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[2] > 0)
                return false;

            drawJaw(1, lightColor);
            drawJaw(-1, lightColor);

            return false;
        }

        private void drawJaw(int dir, Color color)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 offset = new Vector2(-90 + Projectile.localAI[1], -10.5f * dir).RotatedBy(Projectile.rotation);
            Vector2 pos = Projectile.Center - Main.screenLastPosition + Vector2.UnitY * Projectile.gfxOffY + offset;
            Vector2 origin = new Vector2(dir > 0 ? 10 : tex.Width - 10, tex.Height - 10);
            float rot = Projectile.rotation + (dir > 0 ? MathHelper.ToRadians(35) : MathHelper.ToRadians(155)) - Projectile.localAI[0] * dir;

            Main.EntitySpriteDraw(tex, pos, null, color * Projectile.Opacity, rot, origin, Projectile.scale, dir > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
        }
    }
}
