using LobotomyCorp.NPCs.RedMist;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Reflection.Metadata;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
    internal class SmileBobsFriendly : ModProjectile
    {
        public static Asset<Texture2D> MouthTexture;

        public override void Load()
        {
            MouthTexture = ModContent.Request<Texture2D>(Texture + "Mouth");
        }

        public override string Texture => "LobotomyCorp/Projectiles/SmileBobs";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Smile");
            Main.projFrames[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 300;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.frame = Main.rand.Next(3);
                Projectile.localAI[0]++;
                Projectile.rotation = Main.rand.NextFloat(6.28f);
            }

            // Delay attack
            if (Projectile.ai[0] > 0)
            {
                Projectile.ai[0]--;

                Projectile.alpha = 255;
                Projectile.Center = Main.player[Projectile.owner].MountedCenter + new Vector2(110 * Main.player[Projectile.owner].direction, 14);
                return;
            }
            else
            {
                Projectile.alpha = 0;
            }

            if (Projectile.ai[1] != 0)
            {
                for (int i = 0; i < 7; i++)
                {
                    Dust d = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Wraith)];
                    d.velocity = -Projectile.velocity / 8;
                    d.noGravity = true;
                }
                return;
            }

            if (Projectile.localAI[0] == 1)
            {
                for (int i = 0; i < 7; i++)
                {
                    Dust d = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Wraith)];
                    d.velocity = (Projectile.velocity / 4).RotatedByRandom(MathHelper.ToRadians(10));
                    d.noGravity = true;
                }
                Projectile.localAI[0]++;
            }

            Projectile.velocity.Y += 0.8f;
            Projectile.velocity.X *= 0.99f;

            if (Projectile.velocity.Y > 0)
            {
                float nearest = 160;
                int target = -1;
                foreach (NPC n in Main.ActiveNPCs)
                {
                    if (n.CanBeChasedBy(Projectile))
                    {
                        float dist = n.Center.Distance(Projectile.Center) - (n.width > n.height ? n.width / 2 : n.height / 2);
                        if (dist < nearest)
                        {
                            dist = nearest;
                            target = n.whoAmI;
                        }
                    }
                }
                if (target != -1)
                {
                    Projectile.timeLeft = 30;
                    Vector2 velocity = 24 * Projectile.Center.DirectionTo(Main.npc[target].Center);
                    Projectile.velocity = velocity;
                    Projectile.ai[1] = 1;
                }
            }
            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Wraith);
            }
            Projectile.ai[2]++;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.ai[2] < 10)
                return false;
            return base.CanHitNPC(target);
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            if (Projectile.velocity.Y < 0)
            {
                return false;
            }

            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle frame = tex.Frame(verticalFrames: Main.projFrames[Projectile.type], frameY: Projectile.frame);
            Vector2 origin = frame.Size() / 2;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, frame, lightColor * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, 0, 0);

            if (Projectile.ai[1] != 0)
            {
                tex = MouthTexture.Value;
                origin = tex.Size() / 2;
                origin.X -= 6;
                Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, tex.Frame(), lightColor * Projectile.Opacity, Projectile.velocity.ToRotation(), origin, Projectile.scale, 0, 0);
            }
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player owner = Main.player[Projectile.owner];
            if (Main.myPlayer == owner.whoAmI)
            {
                Vector2 velocity = target.Center - Projectile.Center;
                velocity.Normalize();
                for (int i = 0; i < 7; i++)
                {
                    Vector2 vel = velocity.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-15, 15))) * Main.rand.Next(10, 14);
                    Vector2 randomPos = new Vector2(Projectile.position.X + Main.rand.Next(Projectile.width), Projectile.position.Y + Main.rand.Next(Projectile.height));
                    Projectile.NewProjectile(owner.GetSource_FromThis(), randomPos, vel, ModContent.ProjectileType<Projectiles.SmileBitsFriendly>(), Projectile.damage / 3, 0, owner.whoAmI, target.whoAmI);
                }
            }
            SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.Center);
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
        }
    }
}
