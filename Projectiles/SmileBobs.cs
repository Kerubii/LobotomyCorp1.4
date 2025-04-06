using LobotomyCorp.NPCs.RedMist;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Reflection.Metadata;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
    /// <summary>
    /// Negative ai0 for arc type, Positive ai0 for Shotgun type. Number is how many bits it releases. 0 is none
    /// positive ai1 controls speed bobs release and applies gravity, negative ai1 makes it explode at a certain time, zero makes it a normal non velocity projectile
    /// ai2 target, if negative it inherits velocity
    /// </summary>
    internal class SmileBobs : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Smile");
            Main.projFrames[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 300;
            Projectile.scale = 1f;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.frame = Main.rand.Next(3);
                Projectile.localAI[0]++;
            }
            Projectile.rotation += 0.01f;// athHelper.ToRadians(3);

            if (Main.rand.NextBool(3))
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Wraith);
                Main.dust[d].noGravity = true;
            }

            if (Projectile.ai[1] < 0)
            {
                Projectile.ai[1]++;
                if (Projectile.ai[1] >= 0)
                    Projectile.Kill();
            }
            else if (Projectile.ai[1] > 0)
            {
                Projectile.velocity.Y += 0.2f;
            }

            if (Projectile.timeLeft < 10)
                Projectile.scale -= 0.025f;
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            if (Projectile.velocity.Y > 0)
                fallThrough = false;

            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 16; i++)
            {
                Vector2 vel = new Vector2(Main.rand.NextFloat(1f, 2f), 0).RotateRandom(6.28f);
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Wraith, vel.X, vel.Y);
                Main.dust[d].noGravity = true;
            }

            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            Vector2 delta = Projectile.velocity;
            float velSpeed = Projectile.ai[1];
            if (velSpeed < 0)
                velSpeed = Projectile.velocity.Length();

            if ((int)Projectile.ai[2] >= 0)
            {
                Player player = Main.player[(int)Projectile.ai[2]];
                if (player.active && !player.dead)
                {
                    delta = player.Center - Projectile.Center;
                }
            }
            delta.Normalize();
            // Arc type
            if (Projectile.ai[0] < 0)
            {
                int amount = (int)(Projectile.ai[0] * -1f);

                for (int i = 0; i < amount; i++)
                {
                    float angle = MathHelper.ToRadians(-45f + 90f * (i / (amount - 1f)));
                    Vector2 vel = (delta * velSpeed).RotatedBy(angle);
                    if (Main.rand.NextBool(3))
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel, ModContent.ProjectileType<SmileBits>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    else
                    {
                        int n = NPC.NewNPC(Projectile.GetSource_FromThis(), (int)Projectile.Center.X, (int)Projectile.Center.Y, ModContent.NPCType<SmileBitsBreakable>());
                        Main.npc[n].velocity = vel;
                        Main.npc[n].netUpdate = true;
                    }
                }
            }
            // Random hotgun type
            else if (Projectile.ai[0] > 0)
            {
                int amount = (int)(Projectile.ai[0]);

                for (int i = 0; i < amount; i++)
                {
                    float speed = velSpeed * Main.rand.NextFloat(0.8f, 1.2f);
                    Vector2 vel = (delta * speed).RotatedByRandom(MathHelper.ToRadians(45));
                    if (Main.rand.NextBool(3))
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel, ModContent.ProjectileType<SmileBits>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    else
                    {
                        int n = NPC.NewNPC(Projectile.GetSource_FromThis(), (int)Projectile.Center.X, (int)Projectile.Center.Y, ModContent.NPCType<SmileBitsBreakable>());
                        Main.npc[n].velocity = vel;
                        Main.npc[n].netUpdate = true;
                    }
                }
            }
        }
    }
}
