using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Chat;
using LobotomyCorp;
using Terraria.GameContent;

namespace LobotomyCorp.NPCs.RedMist
{
    class PenitenceStar : ModProjectile
    {
        public const int TIME = 60;
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
            if (Main.rand.NextBool(5))
            {
                Dust d = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 20)];
                d.noGravity = true;
            }

            Projectile.rotation += MathHelper.ToRadians(4);
            Projectile.localAI[0]++;

            Projectile.velocity *= 0.95f;
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
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

        public override void OnKill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<PenitenceLight>(), Projectile.damage, 0);
            }
        }
    }

    class PenitenceLight : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;

            Projectile.hostile = true;
        }

        readonly int LENGTH = 2000;

        public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
                for (int i = -4; i < 8; i++)
                {
                    Vector2 pos = Projectile.Center + Vector2.UnitY * 300 * i;

                    for (int j = 0; j < 16; j++)
                    {
                        float angle = 6.28f * j / 16f;
                        Vector2 vel = new Vector2(8 * (float)Math.Cos(angle), 3 * (float)Math.Sin(angle));

                        Dust d = Dust.NewDustPerfect(pos, 20, vel);
                        d.noGravity = true;
                    }
                }
            }

            Projectile.ai[0]++;

            Projectile.frame += 5;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle frame = tex.Frame();
            Vector2 scale = new Vector2(1, 1.4f + 0.1f * (float)Math.Sin(6.28f * Projectile.ai[0] / 10f));
            Vector2 origin = new Vector2(0, tex.Height / 2);
            //origin.X -= Projectile.frame % 36;
            int length = tex.Width;

            //Color white = Color.White;
            //white.A = 100;

            if (Projectile.ai[0] < 15)
                scale.Y *= Projectile.ai[0] / 13f;
            float opacity = 0.8f;
            if (Projectile.timeLeft < 10)
                opacity *= Projectile.timeLeft / 10f;

            for (int i = 0; i < LENGTH; i += length)
            {
                Vector2 pos = Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
                pos.Y -= LENGTH / 2;
                pos.Y += i;

                for (int j = 4; j >= 0; j--)
                {
                    float laserOpacity = opacity * (1f - j / 4f);
                    Vector2 laserScale = scale;
                    laserScale.Y *= 1f + j / 4f;
                    if (j == 0)
                    {
                        if (Projectile.ai[0] < 30)
                        {
                            laserScale = scale;
                            laserScale.Y = 1.5f * (Projectile.ai[0] / 10f);
                            laserOpacity = 1f - Projectile.ai[0] / 30f;
                        }
                        else break;
                    }
                    Color white = Color.Lerp(Color.White, Color.Yellow, laserOpacity);
                    white.A = 100;
                    Main.EntitySpriteDraw(tex, pos, frame, white * laserOpacity, 1.57f, origin, laserScale, 0, 0);
                }
            }

            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 unit = Projectile.velocity;
            float point = 0f;
            // Run an AABB versus Line check to look for collisions, look up AABB collision first to see how it works
            // It will look for collisions on the given line using AABB
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - Vector2.UnitY * LENGTH / 2,
                Projectile.Center + Vector2.UnitY * LENGTH / 2, 16, ref point);
        }
    }
}
