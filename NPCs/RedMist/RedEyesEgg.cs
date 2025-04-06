using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Chat;
using LobotomyCorp;
using Terraria.GameContent;
using Terraria.Audio;

namespace LobotomyCorp.NPCs.RedMist
{
    class RedEyesEgg : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 20;
            Projectile.tileCollide = true;

            Projectile.hostile = true;
            Projectile.timeLeft = 60;
        }

        public override void AI()
        {
            Projectile.rotation += MathHelper.ToRadians(10f);
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 pos = Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
            Vector2 origin = tex.Size() / 2;
            Rectangle frame = tex.Frame();
            Main.EntitySpriteDraw(tex, pos, frame, lightColor, Projectile.rotation, origin, Projectile.scale, 0, 0);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.timeLeft < 20)
            {
                Projectile.scale += 0.015f;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Vector2 target = Projectile.Center + Projectile.velocity;
                float nearest = -1;
                foreach(Player p in Main.ActivePlayers)
                {
                    if (!p.dead)
                    {
                        if (nearest == -1)
                        {
                            nearest = p.Distance(Projectile.Center);
                            target = p.Center;
                        }
                        else
                        {
                            float distance = p.Distance(Projectile.Center);
                            if (distance < nearest)
                            {
                                nearest = distance;
                                target = p.Center;
                            }
                        }
                    }
                }
                
                Vector2 vel = Vector2.Normalize(target - Projectile.Center) * Projectile.velocity.Length();
                if (vel.Length() > 16)
                {
                    vel.Normalize();
                    vel *= 16;
                }
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel, ModContent.ProjectileType<RedEyesSpider>(), Projectile.damage, 0);
            }

            if (Projectile.timeLeft % 5 == 0)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Web);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 0;
            }

            float anglevar = Main.rand.NextFloat(1.57f);
            for (int i = 0; i < 16; i++)
            {
                float angle = (float)Math.PI * 2f * (i / 16f) + anglevar;

                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Web, new Vector2(4, 0).RotatedBy(angle));
                d.noGravity = true;
            }

            SoundEngine.PlaySound(SoundID.NPCDeath11, Projectile.position);
        }
    }

    class RedEyesSpider : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;

            Projectile.hostile = true;
        }

        public override void AI()
        {
            if (Projectile.timeLeft % 3 == 0)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Wraith);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 0;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90); 
        }
    }
}
