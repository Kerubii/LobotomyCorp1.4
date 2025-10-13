using System;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles.RedMist
{
	public class SolitudeSmoke: ModProjectile
	{
		public override void SetStaticDefaults() {
            //DisplayName.SetDefault("Spear");
            Main.projFrames[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 180;
            Projectile.alpha = 110;

            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.friendly = true;

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 60;
        }    
            
        public override void AI() {
            if (Projectile.localAI[0] == 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Smoke, new Vector2(0.5f, 0).RotatedBy(6.28f * i / 8f));
                    d.noGravity = false;
                }

                Projectile.localAI[0] = 1 + Main.rand.Next(1000);
                Projectile.frame += Main.rand.Next(2);
                Projectile.rotation = Main.rand.NextFloat(3.14f);
            }
            if (Projectile.timeLeft < 30)
            {
                Projectile.alpha += 4;
            }
            Projectile.rotation += 0.008f * Math.Sign(Projectile.velocity.X);

            if (Projectile.ai[0] == 0)
            {
                /*float nearest = 160;
                foreach (NPC n in Main.npc)
                {
                    if (n.active && !n.dontTakeDamage && n.chaseable && n.life > 0)
                    {
                        float dist = n.Center.Distance(Projectile.Center);
                        if (dist < nearest)
                        {
                            nearest = dist;
                            Projectile.ai[0] = n.whoAmI + 1;
                            Projectile.velocity.Normalize();
                            Projectile.velocity *= Main.rand.Next((n.width + n.height) / 2);
                            Projectile.timeLeft = 300;
                        }
                    }
                }*/
            }
            else
            {
                NPC target = Main.npc[(int)Projectile.ai[0] - 1];
                if (!target.active || target.life <= 0)
                {
                    Projectile.ai[0] -= 1;
                    Projectile.ai[1] = 0;
                    float speed = Projectile.velocity.Length();
                    if (speed > 10 && speed < 6)
                    {
                        Projectile.velocity.Normalize();
                        Projectile.velocity *= 8;
                    }
                    return;
                }
                Projectile.ai[1]++;
                if (Projectile.ai[1] < 30)
                {
                    Projectile.Center = Vector2.Lerp(Projectile.Center, target.Center + Projectile.velocity, Projectile.ai[1] / 30f);
                }
                else
                {
                    Projectile.Center = target.Center + Projectile.velocity;
                }
            }

            if (Main.rand.NextBool(15))
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke);
                Main.dust[d].noGravity = true;
                Main.dust[d].fadeIn = 1.2f;
            }
        }

        public override bool ShouldUpdatePosition()
        {
            return Projectile.ai[0] == 0;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[0] = target.whoAmI + 1;
                Projectile.velocity.Normalize();
                Projectile.velocity *= Main.rand.Next((target.width + target.height) / 2);
                Projectile.timeLeft = 300;
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.ai[0] > 0 && target.whoAmI != (int)(Projectile.ai[0] - 1))
            {
                return false;
            }
            return base.CanHitNPC(target);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle frame = tex.Frame(1, Main.projFrames[Projectile.type]);
            Vector2 Position = Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);//.RotatedBy(Projectile.rotation);
            Vector2 origin = frame.Size() / 2;
            SpriteEffects effect = Projectile.spriteDirection > 0 ? SpriteEffects.FlipHorizontally : 0;
            Color color = lightColor * (1f - Projectile.alpha / 255f);
            if (Projectile.ai[0] == 0)
            {
                Main.EntitySpriteDraw(tex, Position, frame, color, Projectile.rotation, origin, Projectile.scale + 0.2f * (float)Math.Sin(MathHelper.ToRadians((float)Main.timeForVisualEffects + 421 + Projectile.localAI[0])), 0, 0);
                return false;
            }            
            for (int i = 0; i < 5; i++)
            {
                Vector2 offset = new Vector2(i * 6f, 0).RotatedBy(8.23419f * i + Projectile.localAI[0]);
                frame = tex.Frame(1, Main.projFrames[Projectile.type], 0, i % 2);
                float rot = Projectile.rotation + 2.5903421f * i + Projectile.localAI[0];
                Main.EntitySpriteDraw(tex, Position + offset, frame, color, rot, origin, Projectile.scale + 0.2f * (float)Math.Sin(MathHelper.ToRadians((float)Main.timeForVisualEffects + 421 * i + Projectile.localAI[0])), 0, 0);
            }
            return false;
        }
    }
}
