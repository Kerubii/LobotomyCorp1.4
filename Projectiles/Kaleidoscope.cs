using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	public class Kaleidoscope : ModProjectile
	{
        public Asset<Texture2D> AltTex;

        public override void Load()
        {
            AltTex = ModContent.Request<Texture2D>(Texture + "2");
        }

		public override void SetStaticDefaults() {
            // DisplayName.SetDefault("Kaleidoscope of Butterflies");
            Main.projFrames[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 300;

            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] <= 0)
            {
                Projectile.localAI[0] = 1 + Main.rand.Next(2);
            }

            if (Projectile.ai[1] < 30)
            {
                Projectile.ai[1]++;
            }
            else
            {
                if (Projectile.ai[0] >= 0)
                {
                    NPC target = Main.npc[(int)Projectile.ai[0]];
                    if (target.active && target.life > 0)
                    {
                        Vector2 delt = target.Center - Projectile.Center;
                        delt.Normalize();
                        delt *= 6f;

                        float speed = 0.3f;
                        if (Projectile.position.X < target.position.X)
                        {
                            Projectile.velocity.X += speed;
                            if (Projectile.velocity.X < 0)
                                Projectile.velocity.X += speed;
                            if (Projectile.velocity.X > delt.X)
                                Projectile.velocity.X = delt.X;
                        }
                        else if (Projectile.position.X > target.position.X)
                        {
                            Projectile.velocity.X -= speed;
                            if (Projectile.velocity.X > 0)
                                Projectile.velocity.X -= speed;
                            if (Projectile.velocity.X < delt.X)
                                Projectile.velocity.X = delt.X;
                        }

                        if (Projectile.position.Y < target.position.Y)
                        {
                            Projectile.velocity.Y += speed;
                            if (Projectile.velocity.Y < 0)
                                Projectile.velocity.Y += speed;
                            if (Projectile.velocity.Y > delt.Y)
                                Projectile.velocity.Y = delt.Y;
                        }
                        else if (Projectile.position.Y > target.position.Y)
                        {
                            Projectile.velocity.Y -= speed;
                            if (Projectile.velocity.Y > 0)
                                Projectile.velocity.Y -= speed;
                            if (Projectile.velocity.Y < delt.Y)
                                Projectile.velocity.Y = delt.Y;
                        }
                    }
                }
                else
                    Projectile.ai[0] = -1;
            }
            if (Projectile.ai[0] < -1 && Projectile.type != (int)Projectile.ai[0] * -1)
            {
                AIType = (int)Projectile.ai[0];
            }

            float limit = Projectile.velocity.X;
            if (limit > 6f)
                limit = 6f;
            Projectile.rotation = (limit / 6f) * (float)MathHelper.ToRadians(60);
            Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);

            if (Projectile.frameCounter <= 24)
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter >= 18)
                    Projectile.frame = 1;
                else
                    Projectile.frame = (int)Math.Floor(Projectile.frameCounter / 6f);
            }
            else
                Projectile.frameCounter = 0;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int dustType = 91;

            if (Projectile.localAI[0] == 1)
            {
                dustType = 109;
            }

            Vector2 pos = target.position;
            pos.X += Main.rand.Next(target.width);
            pos.Y += Main.rand.Next(target.height);
            int limit = 16;

            float speed = Main.rand.NextFloat(8f);
            for (int a = 0; a < limit; a++)
            {
                float angle = (float)a / limit * 6.34f;

                Vector2 velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                Dust d = Dust.NewDustPerfect(pos + velocity * speed, dustType, velocity * 2);
                d.noGravity = true;
                d.scale = 0.5f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            if (Projectile.localAI[0] == 1)
                tex = AltTex.Value;
            Vector2 position = Projectile.Center + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
            Rectangle frame = tex.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);

            Vector2 origin = frame.Size() / 2;

            Main.EntitySpriteDraw(tex, position, frame, lightColor, Projectile.rotation, origin, Projectile.scale, Projectile.spriteDirection > 0 ? 0 : SpriteEffects.FlipHorizontally, 0);

            return false;
        }
    }
}
