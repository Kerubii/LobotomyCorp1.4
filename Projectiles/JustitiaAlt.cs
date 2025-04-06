using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static LobotomyCorp.LobotomyCorp;

namespace LobotomyCorp.Projectiles
{
    public class JustitiaAlt : ModProjectile
    {
        public override string Texture => "LobotomyCorp/Items/Aleph/Justitia";

        public override void SetStaticDefaults() {
            //DisplayName.SetDefault("Spear");
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
        }

        public override void SetDefaults() {
            Projectile.width = 82;
            Projectile.height = 82;
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
            //projOwner.itemTime = projOwner.itemAnimation;

            float rot = Projectile.velocity.ToRotation();

            float progress = 1f - (float)projOwner.itemAnimation / (float)projOwner.itemAnimationMax;
            if (progress < 0.33f)
            {
                if (Projectile.ai[1] < 1)
                {
                    Projectile.ai[1] = 1;
                    SoundEngine.PlaySound(LobotomyCorp.WeaponSound("judgement2_1"), Projectile.Center);
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), ownerMountedCenter, Projectile.velocity * 22, ModContent.ProjectileType<JustitiaExtended>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 1);
                    }
                }
                if (progress < 0.05f)
                    rot += MathHelper.ToRadians(-160) * Projectile.spriteDirection;
                else if (progress < 0.2f)
                    rot += MathHelper.ToRadians(Lerp(-160, 45, (progress - 0.05f) % 0.15f / 0.15f)) * Projectile.spriteDirection;
                else if (progress < 0.25f)
                    rot += MathHelper.ToRadians(45) * Projectile.spriteDirection;
                else if (progress < 0.3f)
                {
                    if (Main.myPlayer == projOwner.whoAmI)
                    {
                        Projectile.velocity = Vector2.Normalize(Main.MouseWorld - ownerMountedCenter);
                    }

                    rot += MathHelper.ToRadians(Lerp(45, 0, progress % 0.05f / 0.05f)) * Projectile.spriteDirection;
                    Projectile.ai[0] = Lerp(0, -30, progress % 0.05f / 0.05f);
                }
                Projectile.localAI[1] = Main.rand.Next(-10, 10);
            }
            else if (progress < 0.66f)
            {
                if (progress < 0.43f)
                    Projectile.ai[0] = Lerp(-30, 10, (progress - 0.03f) % 0.1f / 0.1f);
                else if (progress < 0.46f)
                {
                    if (Main.myPlayer == projOwner.whoAmI)
                    {
                        Projectile.velocity = Vector2.Normalize(Main.MouseWorld - ownerMountedCenter);
                    }
                    Projectile.ai[0] = Lerp(10, -30, (progress - 0.01f) % 0.03f / 0.03f);
                }
                else if (progress < 0.56f)
                {
                    rot += MathHelper.ToRadians(Projectile.localAI[1]) * Projectile.spriteDirection;
                    Projectile.ai[0] = Lerp(-30, 10, (progress - 0.06f) % 0.1f / 0.1f);
                }
                else if (progress < 0.60f)
                {
                    rot += MathHelper.ToRadians(Projectile.localAI[1]) * Projectile.spriteDirection;
                    Projectile.ai[0] = 10;
                }
                else
                {
                    if (Main.myPlayer == projOwner.whoAmI)
                    {
                        Projectile.velocity = Vector2.Normalize(Main.MouseWorld - ownerMountedCenter);
                    }

                    rot += MathHelper.ToRadians(Lerp(Projectile.localAI[1], -160, progress % 0.06f / 0.06f)) * Projectile.spriteDirection;
                    Projectile.ai[0] = Lerp(10, 0, progress % 0.06f / 0.06f);
                }

                if (Projectile.ai[1] < 2)
                {
                    Projectile.ai[1] = 2;
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), ownerMountedCenter, new Vector2(20f, 0).RotatedBy(rot), ModContent.ProjectileType<SpearExtender>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 1, 3);
                    }
                    SoundEngine.PlaySound(LobotomyCorp.WeaponSound("judgement2_2"), Projectile.Center);
                }
                if (Projectile.ai[1] < 3 && progress > 0.46f)
                {
                    Projectile.ai[1] = 3;
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), ownerMountedCenter, new Vector2(20f, 0).RotatedBy(rot), ModContent.ProjectileType<SpearExtender>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 1, 3);
                    }
                    SoundEngine.PlaySound(LobotomyCorp.WeaponSound("judgement2_3"), Projectile.Center);
                }
            }
            else
            {
                if (Projectile.ai[1] < 4)
                {
                    Projectile.ai[1] = 4;
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), ownerMountedCenter, Projectile.velocity * 16, ModContent.ProjectileType<JustitiaExtended>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 2);
                    }
                    SoundEngine.PlaySound(LobotomyCorp.WeaponSound("judgement2_2"), Projectile.Center);
                }

                if (progress < 0.7f)
                {
                    Projectile.ai[0] = 0;
                    rot += MathHelper.ToRadians(-160);
                }
                else if (progress < 0.95f)
                {
                    rot += MathHelper.ToRadians(Lerp(-160, 45, (float)Math.Sin(1.57f * (progress - 0.7f) / 0.25f))) * Projectile.spriteDirection;
                }
                else
                {
                    rot += MathHelper.ToRadians(45) * Projectile.spriteDirection;
                }
            }

            /*
            if (((0.33f < progress && progress < 0.4f) ||
                (0.46f < progress && progress < 0.50f) ||
                (0.7f < progress && progress < 0.8f)) && Projectile.localAI[0] == 0)
            {
                SoundEngine.PlaySound(SoundID.Item1, ownerMountedCenter);
                Projectile.localAI[0]++;
            }
            else if (!((0.33f < progress && progress < 0.4f) ||
                (0.46f < progress && progress < 0.50f) ||
                (0.7f < progress && progress < 0.8f)))
                Projectile.localAI[0] = 0;
            */

            Vector2 velRot = new Vector2(1, 0).RotatedBy(rot);
            projOwner.itemRotation = (float)Math.Atan2(velRot.Y * Projectile.direction, velRot.X * Projectile.direction);
            projOwner.direction = Projectile.spriteDirection;
            Projectile.rotation = rot + MathHelper.ToRadians(Projectile.spriteDirection == 1 ? 45 : 135);

            if ((0.05f < progress && progress < 0.25f) ||
                (0.66f < progress && progress < 0.95f))
            {
                for (int i = 0; i < 3; i++)
                {
                    Dust.NewDustPerfect(ownerMountedCenter + new Vector2(30 + Main.rand.Next(100), 0).RotatedBy(rot), 15).noGravity = true;
                }
            }

            if ((0.33f < progress && progress < 0.43f) ||
                (0.46f < progress && progress < 0.6f))
            {
                for (int i = 0; i < 3; i++)
                {
                    Dust.NewDustPerfect(ownerMountedCenter + new Vector2(30 + Main.rand.Next(100), Main.rand.Next(-5, 5)).RotatedBy(rot), 15, -Projectile.velocity * 4).noGravity = true;
                }
            }

            Projectile.Center = ownerMountedCenter + (80 + Projectile.ai[0]) * velRot;

            if (projOwner.itemAnimation == 1)
                Projectile.Kill();
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.ScalingArmorPenetration += 1f;
            modifiers.Knockback *= 0.1f;
        }

        public override bool? CanHitNPC(NPC target)
        {
            float progress = 1f - (float)Main.player[Projectile.owner].itemAnimation / (float)Main.player[Projectile.owner].itemAnimationMax;
            if (!(0.25f < progress && progress < 0.33f) ||
                    !(0.56f < progress && progress < 0.7f))
                return null;    
            else
                return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player projOwner = Main.player[Projectile.owner];
            float progress = 1f - (float)projOwner.itemAnimation / (float)projOwner.itemAnimationMax;
            if (progress < 0.66f)
            {
                if (progress < 0.33f)
                {
                    target.immune[Projectile.owner] = projOwner.itemAnimation - (int)(projOwner.itemAnimationMax * 0.666f);
                }
                else
                {
                    target.immune[Projectile.owner] = 6;
                }
            }
            else
                target.immune[Projectile.owner] = 5;
        }

        public override void OnKill(int timeLeft)
        {
            Main.player[Projectile.owner].itemRotation = 0;
            Main.player[Projectile.owner].heldProj = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player projOwner = Main.player[Projectile.owner];
            if (projOwner.itemAnimation == 3)
                return false;

            Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);
            //Dust.NewDustPerfect(ownerMountedCenter, 14, Vector2.Zero);
            Vector2 position = ownerMountedCenter - Main.screenPosition;
            Vector2 originOffset = new Vector2(Projectile.ai[0] + 12, 0).RotatedBy(MathHelper.ToRadians(Projectile.direction == 1 ? 135 : 45));
            Vector2 origin = new Vector2((Projectile.spriteDirection == 1 ? 9 : 74), 74) + originOffset;
            SpriteEffects spriteEffect = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, position, new Microsoft.Xna.Framework.Rectangle?
                                    (
                                        new Rectangle
                                        (
                                            0, 0, TextureAssets.Projectile[Projectile.type].Width(), TextureAssets.Projectile[Projectile.type].Height()
                                        )
                                    ),
                lightColor * ((float)(255 - Projectile.alpha) / 255f), Projectile.rotation, origin, Projectile.scale, spriteEffect, 0);
            return false;
        }
    }
}
