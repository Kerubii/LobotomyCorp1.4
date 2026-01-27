using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Chat;
using LobotomyCorp;
using Terraria.GameContent;
using LobotomyCorp.Projectiles;
using Terraria.Localization;
using Terraria.DataStructures;
using LobotomyCorp.Visuals.DeathAnimations;

namespace LobotomyCorp.NPCs.RedMist
{
    class HeavenBoss : LobProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Heaven");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 120;

            Projectile.extraUpdates = 5;
            Projectile.hostile = true;
            Projectile.friendly = false;

            LobotomyGlobalProjectile.SetDeathAnimation(Projectile, ModContent.GetInstance<LobDeathBHeaven>().Type);
            //DeathAnimation = ModContent.GetInstance<LobDeathBHeaven>().Type;
        }

        public override void AI()
        {
            Projectile.ai[2]++;
            if (Main.netMode != NetmodeID.MultiplayerClient && Main.expertMode && Projectile.ai[2] % 15 == 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<HeavenBranchEye>(), Projectile.damage, Projectile.knockBack, -1, Projectile.ai[0]);
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
        }

        public override bool PreDraw(ref Color lightColor)
		{
            for (int i = 0; i < 12; i++)
            {
                Color color = lightColor * (0.7f * (1f - i / 12f));
                Vector2 origin2 = new Vector2(75, 13);
                float rotation2 = Projectile.oldRot[i] + 0.785398f;
                if (Projectile.spriteDirection == -1)
                {
                    origin2.X = 13;
                    rotation2 += 1.57f;
                }
                Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value,
                                 Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                                TextureAssets.Projectile[Projectile.type].Frame(),
                                 color,
                                 rotation2,
                                 origin2,
                                 Projectile.scale,
                                 Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                                 0);
            }

            Vector2 origin = new Vector2(75, 13);
            float rotation = Projectile.rotation + 0.785398f;
            if (Projectile.spriteDirection == -1)
            {
                origin.X = 13;
                rotation += 1.57f;
            }
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value,
                             Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                             TextureAssets.Projectile[Projectile.type].Frame(),
                             lightColor,
                             rotation,
                             origin,
                             Projectile.scale,
                             Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                             0);
            return false;
		}
	}
}
