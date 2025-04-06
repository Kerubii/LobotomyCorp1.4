using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Chat;
using LobotomyCorp;
using LobotomyCorp.Utils;
using System.Collections.Generic;

namespace LobotomyCorp.NPCs.RedMist
{
    //[AutoloadBossHead]
    class HeavenBranch : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 90;
            Projectile.hostile = true;
            Projectile.scale = 0;

            Projectile.hide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                Projectile.frame = Main.rand.Next(4);
                Projectile.spriteDirection = Main.rand.NextBool(2) ? 1 : -1;
            }

            Projectile.ai[2]++;
            if (Projectile.ai[2] <= 5)
            {
                Projectile.scale = Projectile.ai[2] / 5;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(15) * Projectile.ai[1];

            if (Main.netMode != NetmodeID.MultiplayerClient && Projectile.ai[2] == 5 && Projectile.ai[0] > 0)
            {
                float length = 90;
                Vector2 pos = Projectile.Center + new Vector2(length * 0.6f, 0).RotatedBy(Projectile.rotation);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos, Projectile.velocity, Projectile.type, Projectile.damage, Projectile.knockBack, -1, Projectile.ai[0] - 1, Projectile.ai[1] * -1);
            }

            if (Projectile.timeLeft < 30)
            {
                Projectile.alpha += 255/30;
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);

            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float length = 90;
            Vector2 pos = new Vector2(length * 0.6f, 0).RotatedBy(Projectile.rotation);
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + pos, 4, ref length);
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float rotationOffset = MathHelper.ToRadians(45);
            Vector2 origin = new Vector2(3, 61);
            if (Projectile.spriteDirection < 0)
            {
                origin.X = origin.Y;
                rotationOffset += MathHelper.ToRadians(90);
            }
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value,
                             Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                             TextureAssets.Projectile[Projectile.type].Frame(1,4,0,Projectile.frame),
                             lightColor * (1f - Projectile.alpha / 255f),
                             Projectile.rotation + rotationOffset,
                             origin,
                             Projectile.scale,
                             Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                             0);
            return false;
        }
    }

}
