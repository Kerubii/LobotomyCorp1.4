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

namespace LobotomyCorp.NPCs.RedMist.Projectiles
{
    class RedMistRedEyesPortal : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;

            Projectile.hostile = true;
            Projectile.timeLeft = 215;
        }

        public override void AI()
        {
            if (Projectile.ai[1] > 0)
            {
                Projectile.ai[1]--;
                return;
            }
            if (Projectile.alpha == 255)
                Projectile.alpha = 0;

            if (Projectile.ai[0] == 0)
            {
                Projectile.rotation = Main.rand.NextFloat(6.28f);
                Projectile.alpha = 100;
                Projectile.ai[0]--;
            }

            if (Projectile.ai[0] > 0)
            {
                Projectile.ai[0]++;
                if (Projectile.ai[0] == 90)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Vector2 dir = new Vector2(4 * i, 0);
                        Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.GemRuby, dir);
                        d.noGravity = true;
                        if (i == 0)
                            continue;
                        d = Dust.NewDustPerfect(Projectile.Center, DustID.GemRuby, -dir);
                        d.noGravity = true;

                        dir = new Vector2(0, 1 * i);
                        d = Dust.NewDustPerfect(Projectile.Center, DustID.GemRuby, dir);
                        d.noGravity = true;
                        d = Dust.NewDustPerfect(Projectile.Center, DustID.GemRuby, -dir);
                        d.noGravity = true;
                    }
                }
                if (Projectile.ai[0] == 150)
                {
                    // Red Mist RedEyes tp function
                    int whoami = NPC.FindFirstNPC(ModContent.NPCType<RedMist>());
                    if (whoami != -1)
                    {
                        NPC n = Main.npc[whoami];
                        if (n.ModNPC is RedMist rm)
                        {
                            Vector2 vel = new Vector2(12f, 0).RotatedBy(Projectile.rotation);
                            float dist = 10000;
                            foreach (Player p in Main.ActivePlayers)
                            {
                                if (!p.dead && p.Center.Distance(Projectile.Center) < dist)
                                {
                                    dist = p.Center.Distance(Projectile.Center);
                                    vel = p.Center.DirectionFrom(Projectile.Center) * 12f;
                                }
                            }
                            rm.RedEyesTeleportRedMistTo(Projectile.Center, vel);
                        }
                    }
                }
            }

            if (Projectile.timeLeft < 10)
                Projectile.alpha += (255 / 10);

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 12)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame > 3)
                    Projectile.frame = 3;
            }
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle frame = tex.Frame(1, Main.projFrames[Projectile.type], frameY: Projectile.frame);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition + Projectile.gfxOffY * Vector2.UnitY, frame, Color.White * Projectile.Opacity, Projectile.rotation, frame.Size() / 2, Projectile.scale * 1.5f, 0);
            return false;
        }
    }
}
