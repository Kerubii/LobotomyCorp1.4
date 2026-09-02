using LobotomyCorp.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static LobotomyCorp.LobotomyCorp;

namespace LobotomyCorp.Projectiles.RedMist
{
    public class DiscordSlashInv : ModProjectile
    {
        public override void SetStaticDefaults() {
            //DisplayName.SetDefault("Spear");
        }

        public override void SetDefaults() {
            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1.3f;
            Projectile.alpha = 0;
            Projectile.timeLeft = 120;

            //Projectile.hide = true;
            Projectile.ownerHitCheck = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
        }

        public override void AI() {
            Player projOwner = Main.player[Projectile.owner];
            Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);
            Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
            projOwner.heldProj = Projectile.whoAmI;
            projOwner.itemTime = projOwner.itemAnimation;

            float rot = Projectile.velocity.ToRotation() + 3.14f;

            float progress = 1f - (float)projOwner.itemAnimation / (float)projOwner.itemAnimationMax;
            int slashAmount = 6;
            float dustSpawnUntil = 0.63f;

            rot += MathHelper.ToRadians(340f * (float)Math.Sin(1.6f * progress) - 170f) * Projectile.spriteDirection;
            slashAmount = 12;
            Projectile.ai[0] = DiscordSlash.SlashOffset * (float)Math.Sin(1.6f * progress);
            dustSpawnUntil = 0.98f;

            Vector2 velRot = new Vector2(1, 0).RotatedBy(rot);
            projOwner.itemRotation = (float)Math.Atan2(velRot.Y * Projectile.direction, velRot.X * Projectile.direction);
            projOwner.direction = Projectile.spriteDirection;
            Projectile.rotation = rot + MathHelper.ToRadians(Projectile.spriteDirection == 1 ? 45 : 135);

            Vector2 offset = (120 + Projectile.ai[0]) * velRot;
            Projectile.Center = ownerMountedCenter + offset;

            if (projOwner.itemAnimation == 1)
                Projectile.Kill();

            if (projOwner.itemAnimation % (projOwner.itemAnimationMax / slashAmount) == 0 && Main.myPlayer == Projectile.owner)
            {
                Vector2 delta = Projectile.Center - projOwner.Center;
                delta.Normalize();
                delta *= 20;

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, delta, ModContent.ProjectileType<DiscordLingeringSlash>(), 0, Projectile.knockBack, Projectile.owner, 1);
            }

            
            if (0.14f < progress && progress < dustSpawnUntil)
            {
                for (int i = 0; i < 16; i++)
                {
                    Vector2 size = new Vector2(Projectile.width * (0.5f + 0.5f * progress), Projectile.height * (0.5f + 0.5f * progress));

                    Dust d = Main.dust[Dust.NewDust(Projectile.Center - size / 2, (int)size.X, (int)size.Y, DustID.SilverFlame)];
                    d.noGravity = true;
                    d.fadeIn = 1.2f;
                    d.scale = 2;
                    d.velocity *= 0;
                }
            }

            /*if (Projectile.ai[1] == 0 && projOwner.itemAnimation < projOwner.itemAnimationMax / 2)
            {
                Projectile.NewProjectile(Projectile.Center, Projectile.velocity, ModContent.ProjectileType<DiscordLingeringSlash>(), Projectile.damage, 0f, Projectile.owner);
                Projectile.ai[1]++;
            }*/
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player projOwner = Main.player[Projectile.owner];
            Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);
            //Dust.NewDustPerfect(ownerMountedCenter, 14, Vector2.Zero);
            Vector2 position = ownerMountedCenter - Main.screenPosition;
            Vector2 originOffset = new Vector2(Projectile.ai[0] - 20, 0).RotatedBy(MathHelper.ToRadians(Projectile.direction == 1 ? 135 : 45));
            Vector2 origin = new Vector2((Projectile.spriteDirection == 1 ? (128 - 95) : 95), 99) + originOffset;
            SpriteEffects spriteEffect = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            lightColor *= 0.8f;
            lightColor.A = 160;
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, position, null,
                lightColor * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, spriteEffect, 0);

            return false;
        }
    }
}
