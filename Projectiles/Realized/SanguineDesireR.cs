using LobotomyCorp.Misc;
using LobotomyCorp.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static LobotomyCorp.LobotomyCorp;
using static Terraria.Player;

namespace LobotomyCorp.Projectiles.Realized
{
    public class SanguineDesireR : ModProjectile
    {
        public override string Texture => "LobotomyCorp/Items/Ruina/Literature/SanguineDesireR";

        public override void SetStaticDefaults() {
            //DisplayName.SetDefault("Spear");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
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

        //private Vector2 PreviousPosition;

        public override void AI() {
            Player projOwner = Main.player[Projectile.owner];
            Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);
            Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
            projOwner.heldProj = Projectile.whoAmI;
            projOwner.itemTime = projOwner.itemAnimation;

            /*if (Projectile.ai[1] == 0)
            {
                PreviousPosition = projOwner.position;
                Projectile.ai[1] = 1;
            }*/

            float rot = Projectile.velocity.ToRotation() - MathHelper.ToRadians(145) * projOwner.direction ;

            float progress = 1f - (float)projOwner.itemAnimation / (float)projOwner.itemAnimationMax;

            rot += WeaponRotation(projOwner, progress);
            /*if (progress < 0.3f)
            {
                float swing = progress / 0.3f;
                rot += MathHelper.ToRadians(290) * (float)Math.Sin(1.57f * swing) * projOwner.direction;
                Projectile.scale = 1f + 0.2f * (float)Math.Sin(3.14f * swing);
            }
            else if (progress < 0.5f)
            {
                rot += MathHelper.ToRadians(290) * projOwner.direction;
            }
            else if (progress < 0.8f)
            {
                float swing = 1f - ((progress - 0.5f) / 0.3f);
                rot += MathHelper.ToRadians(290) * (float)Math.Sin(1.57f * swing) * projOwner.direction;
                Projectile.scale = 1f + 0.2f * (float)Math.Sin(3.14f * swing);
            }*/

            if (progress < 0.8f)
            {
                if (true)
                {
                    Vector2 position = projOwner.Center + new Vector2(80, 0).RotatedBy(rot);
                    position.X -= Projectile.width / 4;
                    position.Y -= Projectile.height / 4;

                    Dust d = Main.dust[Dust.NewDust(position, Projectile.width / 2, Projectile.height / 2, DustID.Blood)];

                    Vector2 vel = Projectile.velocity;// d.position - projOwner.Center;
                    vel.Normalize();
                    d.velocity = vel * 3;
                    d.noGravity = true;
                    d.fadeIn = 1.1f;
                }
            }

            if (progress < 0.3f)
            {
                if (Projectile.localAI[0] == 0)
                {
                    SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Literature/RedShoes_Strong_Vert") with { Volume = 0.5f, MaxInstances = 3}, Projectile.Center);
                    Projectile.localAI[0] += 0.01f + Main.rand.NextFloat(0.95f);
                }

                progress = (progress) / 0.3f;
                Projectile.scale = 1f + 0.2f * (float)Math.Sin(3.14f * progress);
            }
            else if (progress < 0.4f)
            {
            }
            else if (progress < 0.5f)
            {
            }
            else if (progress < 0.8f)
            {
                if (Projectile.localAI[1] == 0)
                {
                    SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Literature/RedShoes_Strong_Vert") with { Volume = 0.5f, MaxInstances = 3 }, Projectile.Center);
                    Projectile.localAI[1] += 0.01f + Main.rand.NextFloat(0.95f);
                }

                progress = (progress - 0.5f) / 0.3f;
                Projectile.scale = 1f + 0.2f * (float)Math.Sin(3.14f * progress);
            }
            else
            {
            }

            Vector2 velRot = new Vector2(1, 0).RotatedBy(rot);
            //projOwner.itemRotation = (float)Math.Atan2(velRot.Y * Projectile.direction, velRot.X * Projectile.direction);
            projOwner.direction = Projectile.spriteDirection;
            Projectile.rotation = rot;// + MathHelper.ToRadians(Projectile.spriteDirection == 1 ? 45 : 135);
            //projOwner.SetCompositeArmFront(enabled: true, CompositeArmStretchAmount.Full, Projectile.rotation - 1.57f); //- 0.785f);// * owner.direction);

            Items.LobCorpLight.LobItemFrame(projOwner, MathHelper.ToDegrees(rot), projOwner.direction);

            Projectile.Center = ownerMountedCenter + (60 + Projectile.ai[0]) * velRot;

            if (projOwner.itemAnimation == 2)
            {
                if (Projectile.ai[0] < 0)
                    Projectile.Kill();
                else
                {
                    Projectile.ai[1]++;
                    if (Projectile.ai[1] == 10)
                    {
                        Projectile.ai[0] = -1;
                    }
                    projOwner.itemAnimation = 3;
                }
            }
        }

        private float WeaponRotation(Player player, float progress)
        {
            float rot = 0;
            if (progress < 0.3f)
            {
                progress = (progress) / 0.3f;
                rot += MathHelper.ToRadians(190) * (float)Math.Sin(1.57f * progress) * player.direction;
            }
            else if (progress < 0.4f)
            {
                progress = (progress - 0.3f) / 0.1f;
                rot += MathHelper.ToRadians(190 + 10 * progress) * player.direction;
            }
            else if (progress < 0.5f)
            {
                progress = (progress - 0.4f) / 0.1f;
                rot += MathHelper.ToRadians(200 - 200 * progress) * player.direction;
            }
            else if (progress < 0.8f)
            {
                progress = (progress - 0.5f) / 0.3f;
                rot += MathHelper.ToRadians(210) * (float)Math.Sin(1.57f * progress) * player.direction;
            }
            else
            {
                progress = (progress - 0.8f) / 0.2f;
                rot += MathHelper.ToRadians(210 + 10 * progress) * player.direction;
            }

            return rot;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Player projOwner = Main.player[Projectile.owner];
            Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);

            int length = 40;
            float progress = 1f - (float)projOwner.itemAnimation / (float)projOwner.itemAnimationMax;

            if (0.5f < progress && progress < 0.9f)
            {
                progress = (progress - 0.5f) / 0.5f;
                length += (int)(75 * progress);
            }

            projHitbox.X += (int)(length * (float)Math.Cos(Projectile.rotation));
            projHitbox.Y += (int)(length * (float)Math.Sin(Projectile.rotation));

            if (targetHitbox.Intersects(projHitbox))
                return true;

            return base.Colliding(projHitbox, targetHitbox);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            int itemTime = Main.player[Projectile.owner].itemAnimation;
            int itemMax = Main.player[Projectile.owner].itemAnimationMax;
            if (((int)(itemMax * 0.95f) >= itemTime && itemTime >= (int)(itemMax * 0.7f)) ||
                ((int)(itemMax * 0.45f) >= itemTime && itemTime >= (int)(itemMax * 0.2f)))
            {
                return;
            }
            modifiers.FinalDamage *= 0.6f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.ai[0] = -1;
            int maximumDamage = target.damage * 20;
            if (maximumDamage < Projectile.damage * 20)
                maximumDamage = Projectile.damage * 20;
            LobotomyGlobalNPC.SanguineDesireApplyBleed(target, damageDone / 2f, maximumDamage, 60, 600);

            int amount = 5 + Main.rand.Next(5);
            for (int i = 0; i < amount; i++)
            {
                Dust d = Main.dust[Dust.NewDust(target.position, target.width, target.height, DustID.Blood)];
                float rotation = (d.position - target.Center).ToRotation(); 

                Vector2 dustVel = new Vector2(2f, 0).RotatedBy(rotation);
                d.velocity = dustVel * Main.rand.NextFloat(2f);
                d.noGravity = true;
                d.fadeIn = 1f + Main.rand.NextFloat(0.4f);
            }
            Player projOwner = Main.player[Projectile.owner];
            if (projOwner.itemAnimation >= (int)(projOwner.itemAnimationMax * 0.5f))
            {
                target.immune[Projectile.owner] = projOwner.itemAnimation - (int)(projOwner.itemAnimationMax * 0.5f);
            }
            else
            {
                target.immune[Projectile.owner] = projOwner.itemAnimation;
            }
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player projOwner = Main.player[Projectile.owner];
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);
            //Dust.NewDustPerfect(ownerMountedCenter, 14, Vector2.Zero);
            Vector2 position = Items.LobCorpLight.LobItemLocation(projOwner, tex.Frame(), MathHelper.ToDegrees(Projectile.rotation), projOwner.direction, 2) - Main.screenPosition;
            Vector2 originOffset = new Vector2(Projectile.ai[0] + 12, 0).RotatedBy(MathHelper.ToRadians(Projectile.direction == 1 ? 135 : 45));
            Vector2 origin = new Vector2((Projectile.spriteDirection == 1 ? 14 : 70), 70) + originOffset;
            float rotation = Projectile.rotation + MathHelper.ToRadians(Projectile.spriteDirection == 1 ? 45 : 135);
            SpriteEffects spriteEffect = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(tex, position, tex.Frame(), lightColor * ((float)(255 - Projectile.alpha) / 255f), rotation, origin, Projectile.scale, spriteEffect, 0);
            /*
            SlashTrail trail = new SlashTrail(80, 1.57f);
            trail.DrawTrail(Projectile, LobcorpShaders["TwilightSlash"]);*/

            float progress = 1f - (float)projOwner.itemAnimation / (float)projOwner.itemAnimationMax;
            bool attack1 = progress < 0.4f;
            bool attack2 = 0.5f < progress && progress < 0.9f;
            if (attack1 || attack2)
            {
                SlashTrail trail = new SlashTrail(40, 1.57f);

                float opacity = 1f;
                float length = 0.4f;// + 0.6f * pprog;
                float distance = 100;
                if (attack1)
                {
                    progress = progress / 0.4f;
                    distance += 15 * progress;
                }
                else if (attack2)
                {
                    progress = (progress - 0.5f) / 0.4f;
                    distance += 75 * progress;
                }
                length += 1.2f * progress;
                if (length > 1f)
                    length = 1f;

                float rot = Projectile.velocity.ToRotation() - MathHelper.ToRadians(125) * projOwner.direction;
                rot += MathHelper.ToRadians(220) * (float)Math.Sin(1.57f * progress) * projOwner.direction;

                if (progress > 0.5f)
                {
                    progress = (progress - 0.5f) / 0.5f;
                    opacity -= progress;
                }

                CustomShaderData shader = LobcorpShaders["SwingTrail"].UseOpacity(opacity);
                shader.UseImage1(MiscAssets.NoiseTexture);
                shader.UseImage2(MiscAssets.BloodTrail);
                shader.UseImage3(MiscAssets.Worley);
                //shader.UseCustomShaderDate(Projectile.localAI[0], Projectile.localAI[1]);
                trail.color = Color.Red;// * opacity;

                trail.DrawPartCircle(ownerMountedCenter, rot, MathHelper.ToRadians(180f) * length, projOwner.direction, distance, 60, shader);
            }

            return false;
        }
    }

    public class SanguineDesireRHeavy : ModProjectile
    {
        public override string Texture => "LobotomyCorp/Items/Ruina/Literature/SanguineDesireR";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Spear");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1.3f;
            Projectile.alpha = 0;
            Projectile.timeLeft = 120;
            Projectile.extraUpdates = 3;

            //Projectile.hide = true;
            Projectile.ownerHitCheck = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
        }

        //private Vector2 PreviousPosition;

        public override void AI()
        {
            Player projOwner = Main.player[Projectile.owner];
            Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);
            Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
            projOwner.heldProj = Projectile.whoAmI;
            projOwner.itemTime = projOwner.itemAnimation;

            /*if (Projectile.ai[1] == 0)
            {
                PreviousPosition = projOwner.position;
                Projectile.ai[1] = 1;
            }*/

            float rot = Projectile.velocity.ToRotation() - MathHelper.ToRadians(135) * projOwner.direction;

            float progress = Projectile.ai[1] / ((float)projOwner.itemAnimationMax * Projectile.extraUpdates);

            if (progress < 0.5f)
            {
                if (Projectile.localAI[0] == 0 && projOwner.itemAnimation == (int)(projOwner.itemAnimationMax * 0.95f))
                {
                    SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Literature/RedShoes_Strong_Finish") with { Volume = 0.5f }, Projectile.Center);
                    Projectile.localAI[0]++;
                }

                rot += MathHelper.ToRadians(2) * Main.rand.NextFloat(-1, 1);
            }
            else if (progress < 0.8f)
            {
                float time = (float)Math.Sin(1.57f * ((progress - 0.5f) / 0.3f));
                rot += MathHelper.ToRadians(290) * time * projOwner.direction;
            }
            else
            {
                rot += MathHelper.ToRadians(290) * projOwner.direction;
            }
            if (Projectile.ai[0] < 4)
                Projectile.ai[1]++;

            if (Projectile.ai[0] > 0 || (Projectile.ai[0] == 1 && Projectile.ai[1] < 86))
                Projectile.ai[0]++;

            Vector2 velRot = new Vector2(1, 0).RotatedBy(rot);
            projOwner.itemRotation = (float)Math.Atan2(velRot.Y * Projectile.direction, velRot.X * Projectile.direction);
            projOwner.direction = Projectile.spriteDirection;
            Projectile.rotation = rot;// + MathHelper.ToRadians(Projectile.spriteDirection == 1 ? 45 : 135);
            projOwner.SetCompositeArmFront(enabled: true, CompositeArmStretchAmount.Full, Projectile.rotation - 1.57f); //- 0.785f);// * owner.direction);

            Projectile.Center = ownerMountedCenter + 86 * velRot;

            if (projOwner.itemAnimation == 1)
            {
                /*
                projOwner.velocity *= 0;
                if (!projOwner.mount.Active)
                    projOwner.velocity.Y -= 6f;*/
                Projectile.Kill();
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            float progress = Projectile.ai[1] / ((float)Main.player[Projectile.owner].itemAnimationMax * Projectile.extraUpdates);
            if (progress < 0.5f || progress > 0.8f)
                return false;
            return base.CanHitNPC(target);
        }

        /*
        public override void ModifyDamageScaling(ref float damageScale)
        {
            float progress = Projectile.ai[1] / ((float)Main.player[Projectile.owner].itemAnimationMax * Projectile.extraUpdates);
            if (0.65f >= progress && progress >= 0.55f)
            {
                return;
            }
            damageScale = 0.6f;
        }*/

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            float progress = Projectile.ai[1] / ((float)Main.player[Projectile.owner].itemAnimationMax * Projectile.extraUpdates);
            if (0.65f >= progress && progress >= 0.55f)
            {
                Projectile.ai[0]++;
            }
            target.immune[Projectile.owner] = Main.player[Projectile.owner].itemAnimation;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            int amount = (int)(LobotomyGlobalNPC.LNPC(target).SanguineDesireExtensiveBleedAmount / 15);
            if (amount > 30)
                amount = 30;
            for (int i = 0; i < amount; i++)
            {
                Vector2 dustVel = new Vector2(4f, 0).RotatedByRandom(3.14f);
                dustVel *= Main.rand.NextFloat(10 * (amount / 30f));

                Dust d = Dust.NewDustPerfect(target.Center, DustID.Blood, dustVel);
                d.noGravity = true;
                d.fadeIn = 1f + Main.rand.NextFloat(0.4f);
            }
            for (int i = 0; i < amount/2; i++)
            {
                float dustVel = -4 * Main.rand.NextFloat(10 * (amount / 30f));

                int d = Dust.NewDust(target.position + new Vector2(target.width * 0.25f, target.height * 0.25f), target.width/2, target.height/2, DustID.Blood, 0, dustVel);
                Main.dust[d].noGravity = true;
                Main.dust[d].fadeIn = 1f + Main.rand.NextFloat(0.4f);
            }
            if (Main.myPlayer == Projectile.owner)
                LobotomyCorp.ScreenShake(15, amount * 0.5f, 0.05f, true);
            int damage = LobotomyGlobalNPC.SanguineDesireConsumeBleed(target) * 2;
            modifiers.SourceDamage.Base += damage;
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
            Vector2 originOffset = new Vector2(36, 0).RotatedBy(MathHelper.ToRadians(Projectile.direction == 1 ? 135 : 45));
            Vector2 origin = new Vector2((Projectile.spriteDirection == 1 ? 25 : 51), 51) + originOffset;
            float rotation = Projectile.rotation + MathHelper.ToRadians(Projectile.spriteDirection == 1 ? 45 : 135);
            SpriteEffects spriteEffect = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, position, new Microsoft.Xna.Framework.Rectangle?
                                    (
                                        new Rectangle
                                        (
                                            0, 0, TextureAssets.Projectile[Projectile.type].Width(), TextureAssets.Projectile[Projectile.type].Height()
                                        )
                                    ),
                lightColor * ((float)(255 - Projectile.alpha) / 255f), rotation, origin, Projectile.scale, spriteEffect, 0);
            /*
            SlashTrail trail = new SlashTrail(80, 1.57f);
            trail.DrawTrail(Projectile, LobcorpShaders["TwilightSlash"]);*/

            return false;
        }
    }
}
