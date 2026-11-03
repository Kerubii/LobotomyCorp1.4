using LobotomyCorp.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles.RedMist
{
    class GrinderMk4Saw : ModProjectile
    {
        public override string Texture => "LobotomyCorp/Projectiles/GrinderMk4Head";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.penetrate = 21;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 300;
            directionX = 1;
            directionY = 1;
            ai0 = 0;
            oldVelocity = Projectile.velocity;
        }

        public Vector2 oldVelocity;

        public override void AI()
        {
            int maxSpeed = 24;

            if (Projectile.penetrate == 1 && Projectile.timeLeft > 20)
                Projectile.timeLeft = 20;

            if (Projectile.timeLeft <= 20)
            {
                Projectile.penetrate = 1;
                Projectile.ai[0] = 0;
                if (Projectile.ai[1] < 3)
                {
                    Projectile.velocity.X = (Math.Sign(Projectile.velocity.X) * -1) * Main.rand.Next(1, 6);
                    Projectile.velocity.Y = -Main.rand.Next(8, 14);
                    float rand = (float)Main.rand.Next(90);
                    for (int i = 0; i < 3; i++)
                    {
                        Gore gore = Main.gore[Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(), Main.rand.Next(61, 64), 1f)];
                        gore.velocity *= 0.6f;
                    }
                    Projectile.tileCollide = false;
                    Projectile.ai[1] = 3;

                    SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
                    if (Main.myPlayer == Projectile.owner)
                    {
                        for (int i = 0; i < 12; i++)
                        {
                            Vector2 vector3 = Projectile.velocity * 48 * 3f;
                            vector3.Normalize();
                            vector3 = vector3.RotatedByRandom(6.28f);
                            vector3 *= Main.rand.Next(35, 81) * 0.1f;
                            int num15 = (int)(Projectile.damage * 0.9f);
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, vector3.X, vector3.Y, ModContent.ProjectileType<GrinderMk4Spark>(), num15, Projectile.knockBack * 0.2f, Projectile.owner);
                        }
                    }
                }
                else
                {
                    Projectile.alpha += 255 / 20;
                    Projectile.velocity.Y += 0.5f;
                    Projectile.rotation += MathHelper.ToRadians(2 * Projectile.velocity.X);
                }
                return;
            }

            if (Projectile.ai[1] > 0)
            {
                bool collideX = oldVelocity.X != Projectile.velocity.X;
                bool collideY = oldVelocity.Y != Projectile.velocity.Y;

                float smooth = Projectile.ai[0] / 30;
                if (smooth > 1f)
                    smooth = 1f;
                if (Projectile.ai[1] == 1)
                {
                    Projectile.rotation += (directionX * directionY) * MathHelper.ToRadians(16 + 8 * smooth);
                    Projectile.direction = (directionX * directionY);
                    if (collideY)
                    {
                        ai0 = 2;
                    }

                    if (!collideY && ai0 == 2)
                    {
                        directionX = -directionX;
                        Projectile.ai[1] = 2;
                        ai0 = 1;
                    }
                    if (collideX)
                    {
                        directionY = -directionY;
                        Projectile.ai[1] = 2;
                    }
                }
                else if (Projectile.ai[1] == 2)
                {
                    Projectile.rotation -= (directionX * directionY) * MathHelper.ToRadians(16 + 8 * smooth);
                    Projectile.direction = -(directionX * directionY);
                    if (collideX)
                    {
                        ai0 = 2;
                    }

                    if (!collideX && ai0 == 2)
                    {
                        directionY = -directionY;
                        Projectile.ai[1] = 1;
                        ai0 = 1;
                    }
                    if (collideY)
                    {
                        directionX = -directionX;
                        Projectile.ai[1] = 1;
                    }
                }

                if (Projectile.ai[0] == 30)
                {
                    int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Snow, -Projectile.velocity.X, -Projectile.velocity.Y);
                    Main.dust[d].noGravity = true;
                }
                if (Projectile.ai[0] > 30)
                {
                    Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Silver)].noGravity = true;
                }

                Projectile.ai[0]++;
                Projectile.velocity.X = maxSpeed * directionX;
                Projectile.velocity.Y = maxSpeed * directionY;
            }
            else
            {
                Projectile.direction = Math.Sign(Projectile.velocity.X);
                Projectile.rotation += MathHelper.ToRadians(16) * Projectile.direction;

                directionX = Math.Sign(Projectile.velocity.X);
                directionY = Math.Sign(Projectile.velocity.Y);

                Projectile.velocity.X *= 0.98f;
                Projectile.velocity.Y += 0.4f;
            }

            Projectile.spriteDirection = Projectile.direction;
            oldVelocity = Projectile.velocity;
        }

        public override bool ShouldUpdatePosition()
        {
            return Projectile.ai[0] == 0 || Projectile.ai[0] > 30;
        }

        private int directionX;
        private int directionY;
        private float ai0;

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.ai[1] == 0)
            {
                SoundEngine.PlaySound(SoundID.NPCHit4, Projectile.Center);
                Projectile.ai[1]++;
                Projectile.ai[0] = 1;

                Projectile.Center += Projectile.velocity;
                Collision.HitTiles(Projectile.Center, oldVelocity * 0.5f, Projectile.width, Projectile.height);
            }
            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.penetrate == 1)
                return false;

            return base.CanHitNPC(target);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Projectile.owner] = 5;
            int num208 = Realized.GrinderMk2Cleaner2.SpawnTrailDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Misc.Dusts.ElectricTrail>(), Projectile.velocity, 0.5f, 5);

            if (Main.myPlayer == Projectile.owner)
            {
                Vector2 vector3 = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                vector3 += Projectile.Center.DirectionTo(target.Center) * 48 * 3f;
                vector3.Normalize();
                vector3 *= Main.rand.Next(35, 81) * 0.1f;
                int num15 = (int)(damageDone * 0.5f);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, vector3.X, vector3.Y, ModContent.ProjectileType<GrinderMk4Spark>(), num15, Projectile.knockBack * 0.2f, Projectile.owner);
            }

            if (Projectile.ai[0] < 30)
            {
                Vector2 vel = Projectile.Center.DirectionTo(target.Center) * 4f;
                int d = Dust.NewDust(target.Center - Projectile.Size / 2, Projectile.width, Projectile.height, DustID.Blood);
                Main.dust[d].velocity = vel;
                Main.dust[d].noGravity = true;
            }

            if (Projectile.ai[1] > 0 && Projectile.ai[0] > 33)
            {
                Projectile.ai[0] = 1;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), TextureAssets.Projectile[Projectile.type].Frame(), lightColor, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Size() / 2, Projectile.scale, Projectile.spriteDirection > 0 ? 0 : (SpriteEffects)1 , 0);

            for (int i = 4; i >= 0; i--)
            {
                Vector2 oldPos = Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
                float opacity = 1f - (float)i / 5f;
                opacity *= Projectile.Opacity;
                if (i > 0)
                    opacity /= 2f;

                Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, oldPos, TextureAssets.Projectile[Projectile.type].Frame(), lightColor * opacity, Projectile.oldRot[i], TextureAssets.Projectile[Projectile.type].Size() / 2, Projectile.scale, Projectile.spriteDirection > 0 ? 0 : (SpriteEffects)1, 0);
            }
            return false;
        }
    }
}
