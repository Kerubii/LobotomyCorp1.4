using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	public class AlriuneDeathAnimation : ModProjectile
	{
        public override void SetDefaults()
        {
            Projectile.width = 108;
            Projectile.height = 1080;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 300;
            
            Projectile.tileCollide = false;
            Projectile.friendly = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }    
            
        public override void AI() {
            Projectile.ai[1]++;

            if (Projectile.ai[0] > -1)
            {
                NPC n = Main.npc[(int)Projectile.ai[0]];
                if (!n.active || n.life <= 0)
                {
                    Projectile.ai[0] = -1;
                }
                Projectile.width = n.width + 30;
                Projectile.height = n.height + 30;
                Projectile.Center = n.Center;
            }

            if (Main.rand.NextBool(5))
            {
                Vector2 vel = new Vector2(-4, 4) * (1f + Main.rand.NextFloat(0.2f));

                Dust dust = Main.dust[Dust.NewDust(Projectile.position + new Vector2(16, 0), Projectile.width - 16, Projectile.height - 16, DustID.VenomStaff)];
                dust.noGravity = true;
                dust.velocity = vel;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Mod.Assets.Request<Texture2D>("Misc/AlriuneDeathAnimationCurtain").Value;
            Vector2 position = Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
            Vector2 origin = new Vector2(texture.Width / 2, 0); 
            Color color = Color.White;
            Vector2 scale = new Vector2(1f, 1f);
            for (int i = -3; i < 0; i++)
            {
                scale = new Vector2(1f, 0);
                scale.X *= (((float)Projectile.width / 4f) / (float)texture.Width);
                if (Projectile.ai[1] > 60 && Projectile.ai[1] < 240)
                {
                    if (Projectile.ai[1] > 180 && Projectile.ai[1] < 240)
                        AnimationHelper(ref scale.Y, Projectile.ai[1], 180, 240, true);
                    else if (Projectile.ai[1] < 120)
                        AnimationHelper(ref scale.Y, Projectile.ai[1], 60, 120);
                    else
                        scale.Y = 1f;
                }
                Vector2 Offset = new Vector2(((float)Projectile.width / 7 * i) - ((float)Projectile.width / 14) * Math.Sign(i), (-(float)Projectile.height / 6) * 2);
                scale.Y *= (((float)Projectile.height - (float) Projectile.height / 6)/ 74);
                float startPosition = (Projectile.position.X + ((float)Projectile.width / 14) * (4 + i)) - (Projectile.Center.X + Offset.X);
                float time = 1f;
                if (Projectile.ai[1] < 120)
                {
                    time = 1f;
                }
                else if (Projectile.ai[1] < 180)
                {
                    AnimationHelper( ref time, Projectile.ai[1], 120, 180, true);
                }
                else
                {
                    time = 0f;
                }
                Offset.X += startPosition * time;

                position = Projectile.Center + Offset - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
                Main.EntitySpriteDraw(texture, position, (Rectangle)texture.Frame(), color, 0, origin, scale, 0, 0);

                Offset.X *= -1;
                position = Projectile.Center + Offset - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
                Main.EntitySpriteDraw(texture, position, (Rectangle)texture.Frame(), color, 0, origin, scale, 0, 0);
            }

            texture = TextureAssets.Projectile[Projectile.type].Value;
            origin = texture.Size() / 2;
            scale = new Vector2(1f, 1f);

            for (int i = -2; i < 0; i++)
            {
                scale = new Vector2(1f, 1f);
                if (Projectile.ai[1] < 60)
                {
                    AnimationHelper(ref scale.Y, Projectile.ai[1], 0 - 6 * i, 60);
                }
                else if (Projectile.ai[1] > 240)
                {
                    AnimationHelper(ref scale.Y, Projectile.ai[1], 240 + 6 * (i + 2), 300, true);
                }
                else
                {
                    scale.Y = 1f;
                }
                scale.Y *= (((float)Projectile.height / 6f) / 18f);
                scale.X *= (((float)Projectile.width / 5f) / 34f);

                position = Projectile.Center + new Vector2(Projectile.width/5 * i, (-Projectile.height/6) * 2) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
                Main.EntitySpriteDraw(texture, position, (Rectangle)texture.Frame(), color, 0, origin, scale, 0, 0);

                position = Projectile.Center + new Vector2(Projectile.width / 5 * -i, (-Projectile.height / 6) * 2) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
                Main.EntitySpriteDraw(texture, position, (Rectangle)texture.Frame(), color, 0, origin, scale, 0, 0);
            }
            scale = new Vector2(1f, 1f);
            if (Projectile.ai[1] < 60)
            {
                AnimationHelper(ref scale.Y, Projectile.ai[1], 0, 60);
            }
            else if (Projectile.ai[1] > 240)
            {
                AnimationHelper(ref scale.Y, Projectile.ai[1], 252, 300, true);
            }
            else
            {
                scale.Y = 1f;
            }
            scale.Y *= (((float)Projectile.height / 6f) / 18f);
            scale.X *= (((float)Projectile.width / 5f) / 34f);
            position = Projectile.Center + new Vector2( 0, (-Projectile.height / 6) * 2) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
            Main.EntitySpriteDraw(texture, position, (Rectangle)texture.Frame(), color, 0, origin, scale, 0, 0);

            return false;
        }

        private void AnimationHelper(ref float scale, float timer,float min, float max, bool reverse = false)
        {
            if (!reverse)
            {
                if (timer < min)
                    scale = 0f;
                else if (timer > max)
                    scale = 1f;
                else
                    scale = (timer - min) / (max - min);
            }
            else
            {
                if (timer < min)
                    scale = 1f;
                else if (timer > max)
                    scale = 0f;
                else
                    scale = 1f - (timer - min) / (max - min);
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            return target.whoAmI == (int)Projectile.ai[0] && Projectile.ai[1] == 180;
        }
    }
}
