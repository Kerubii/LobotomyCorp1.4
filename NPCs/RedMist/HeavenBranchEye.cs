using LobotomyCorp;
using LobotomyCorp.Projectiles;
using LobotomyCorp.Util;
using LobotomyCorp.Visuals.DeathAnimations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Chat;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.NPCs.RedMist
{
    //[AutoloadBossHead]
    class HeavenBranchEye : LobProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 150;
            Projectile.hostile = true;
            Projectile.scale = 0;

            LobotomyGlobalProjectile.SetDeathAnimation(Projectile, ModContent.GetInstance<LobDeathBHeaven>().Type);
            //DeathAnimation = ModContent.GetInstance<LobDeathBHeaven>().Type;
        }

        public override void AI()
        {
            Projectile.ai[2]++;
            if (Projectile.ai[2] < 10)
            {
                Projectile.scale = Projectile.ai[2] / 10;
            }

            if (Projectile.ai[2] < 60)
            {
                if (Projectile.ai[2] > 30)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        //Dust.NewDustPerfect();
                    }
                    Projectile.localAI[0]++;
                }
            }
            else if (Projectile.ai[2] == 60)
            {
                Projectile.localAI[0] = 0;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = -1; i < 2; i += 2)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.RotatedBy(MathHelper.ToRadians(90 * i)), ModContent.ProjectileType<HeavenBranch>(), Projectile.damage, Projectile.knockBack, -1, Projectile.ai[0], i);
                    }
                }
            }
            if (Projectile.ai[2] > 60 && Projectile.ai[2] < 90)
            {
                float prog = (Projectile.ai[2] - 60) / 30f;
                Projectile.scale = 1f + 0.5f * (float)Math.Sin(3.14f * prog);
            }


            if (Projectile.timeLeft < 30)
            {
                Projectile.alpha += 255 / 30;
            }
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 posOffset = Vector2.Zero;
            float intensity = Projectile.localAI[0] * 0.05f;
            if (Projectile.ai[2] < 60 && Projectile.ai[2] > 30)
            {
                posOffset.X += Main.rand.NextFloat(-intensity, intensity);
                posOffset.Y += Main.rand.NextFloat(-intensity, intensity);
            }

            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value,
                             Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) + posOffset,
                             TextureAssets.Projectile[Projectile.type].Frame(),
                             lightColor * (1f - Projectile.alpha / 255f),
                             Projectile.rotation,
                             TextureAssets.Projectile[Projectile.type].Value.Size() / 2,
                             Projectile.scale,
                             Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                             0);
            return false;
        }
    }

}
