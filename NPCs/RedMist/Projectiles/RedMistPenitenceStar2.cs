using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Chat;
using LobotomyCorp;
using Terraria.GameContent;
using LobotomyCorp.Visuals.LobEffects;
using static LobotomyCorp.Misc.MiscAssets;

namespace LobotomyCorp.NPCs.RedMist
{
    class RedMistPenitenceStar2 : ModProjectile
    {
        public override string Texture => "LobotomyCorp/NPCs/RedMist/PenitenceStar";

        private const int ACTIVATIONTIME = 70;
        private const int TIME = 90;
        private const int RAYLENGTH = 2000;

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.tileCollide = false;

            Projectile.hostile = true;
            Projectile.timeLeft = TIME;
        }

        public override void AI()
        {
            if (Projectile.ai[0]++ == 0)
            {
                Projectile.rotation = Main.rand.NextFloat(6.28f);
                Projectile.netUpdate = true;
            }

            if (Projectile.ai[0] == ACTIVATIONTIME)
            {
                WeaponSmearLine smearline = new WeaponSmearLine();
                smearline.Setup(Projectile.Center - new Vector2(RAYLENGTH / 2, 0).RotatedBy(Projectile.rotation), Vector2.Zero, Projectile.rotation, 20, 1);
                smearline.SetupLine(16, RAYLENGTH, widthOffset: -14);
                smearline.SetShaderImage(FlatColor, GeneralTrail, WindTrail);
                smearline.Color = Color.Wheat;
                smearline.AddEffect();
            }
            
            
            if (Main.rand.NextBool(5))
            {
                Dust d = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 20)];
                d.noGravity = true;
            }

            Projectile.localAI[0]++;
            if (Projectile.ai[0] >= ACTIVATIONTIME - 30)
            {
                Projectile.velocity *= 0;
                return;
            }

            Projectile.rotation += MathHelper.ToRadians(4) * (Projectile.ai[0] / 40f);
            Projectile.velocity *= 0.95f;
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] > ACTIVATIONTIME)
                return false;

            drawLine(false);
            drawLine(true);

            //Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D tex = TextureAssets.Projectile[927].Value;
            Vector2 pos = Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
            Vector2 origin = tex.Size() / 2;
            Rectangle frame = tex.Frame();
            float scale = 0.8f;
            float opacity = 1f;
            if (Projectile.timeLeft < 15)
            {
                scale += 1f - Projectile.timeLeft / 15f;
                opacity *= Projectile.timeLeft / 15f;
            }

            float starscale = (1f + 0.1f * (float)Math.Sin(3.14f * Projectile.localAI[0]/20)) * scale;
            //Main.EntitySpriteDraw(tex, pos, frame, Color.White * opacity, 0, origin, starscale, 0, 0);
            drawStar(tex, pos, Color.Yellow * opacity, 0, origin, starscale);

            starscale = 1f * scale;
            //Main.EntitySpriteDraw(tex, pos, frame, Color.White * 0.4f * opacity, Projectile.rotation, origin, starscale, 0, 0);
            drawStar(tex, pos, Color.White * 0.4f, Projectile.rotation, origin, starscale);
            //Main.EntitySpriteDraw(tex, pos, frame, Color.White * 0.4f * opacity, -Projectile.rotation, origin, starscale, 0, 0);
            drawStar(tex, pos, Color.White * 0.4f, -Projectile.rotation, origin, starscale);
            return false;
        }

        private void drawStar(Texture2D tex, Vector2 pos, Color color, float rotation, Vector2 origin, float scale)
        {
            Main.EntitySpriteDraw(tex, pos, tex.Frame(), color, rotation, origin, scale * 0.6f, 0, 0);
            Main.EntitySpriteDraw(tex, pos, tex.Frame(), color, rotation + 1.57f, origin, scale * 0.6f, 0, 0);
        }

        private void drawLine(bool flip)
        {
            float time = Projectile.ai[0] / ACTIVATIONTIME;
            float maxLength = 16 * 8 * time;
            Vector2 offset = new Vector2(maxLength, 0).RotatedBy(Projectile.rotation + (flip ? 0 : 3.14f));
            float thick = 2 + 2 * time;
            Vector2 offset2 = new Vector2(0, thick).RotatedBy(Projectile.rotation) * (flip ? 1 : -1);

            Utils.DrawLine(Main.spriteBatch, Projectile.Center - offset2, Projectile.Center + offset - offset2, Color.Wheat * time, Color.Transparent, thick);
        }
    }
}
