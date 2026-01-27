using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static LobotomyCorp.LobotomyCorp;

namespace LobotomyCorp.Projectiles
{
    public class CensoredGrab : ModProjectile
    {
        public override string Texture => "LobotomyCorp/Projectiles/Censored";

        public override void SetStaticDefaults() {
        }

        public override void SetDefaults() {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1.3f;
            Projectile.alpha = 0;
            Projectile.timeLeft = 120;

            //Projectile.hide = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.friendly = true;

            DrawHeldProjInFrontOfHeldItemAndArms = true;
        }

        public override void AI() {
            Player projOwner = Main.player[Projectile.owner];
            Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);
            Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
            projOwner.heldProj = Projectile.whoAmI;
            projOwner.itemTime = projOwner.itemAnimation;

            float rot = Projectile.velocity.ToRotation();

            float progress = 1f - (float)projOwner.itemAnimation / (float)projOwner.itemAnimationMax;
            if (progress < 0.25f)
            {
                Projectile.ai[0] = Lerp(0f, 90, progress % 0.25f / 0.25f);
            }
            else if (progress >= 0.75f)
            {
                Projectile.ai[0] = 45 + 45f * (float)Math.Sin(1.57f + 3.14f * ((0.75f - progress) / 0.25f));
            }
            else
                Projectile.ai[0] = 90;

            if (progress > 0.25f)
            {
                if (Projectile.localAI[0] < 0.785f)
                    Projectile.localAI[0] += 0.0898132f;
                if (Projectile.localAI[0] > 0.785f)
                    Projectile.localAI[0] = 0.785f;
            }
            
            Vector2 velRot = new Vector2(1, 0).RotatedBy(rot);
            projOwner.itemRotation = (float)Math.Atan2(velRot.Y * Projectile.direction, velRot.X * Projectile.direction);
            projOwner.direction = Projectile.spriteDirection;
            Projectile.rotation = rot;// + MathHelper.ToRadians(Projectile.spriteDirection == 1 ? 45 : 135);

            Projectile.Center = ownerMountedCenter + (40 + Projectile.ai[0]) * velRot;

            if (Projectile.ai[1] > 0 && progress < 0.70f)
            {
                int index = (int)Projectile.ai[1] - 1;
                NPC n = Main.npc[index];
                bool canGrab = !(n.knockBackResist < (Projectile.knockBack / 40f) || n.boss || (n.width > 180 || n.height > 180) || GrabBlackList(n.type));
                if (n.active && canGrab)
                {
                    n.Center = Projectile.Center;
                }
            }

            if (projOwner.itemAnimation == 1)
                Projectile.Kill();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player projOwner = Main.player[Projectile.owner];
            target.immune[Projectile.owner] = (int)(projOwner.itemAnimationMax * 0.3f);
            if (Projectile.ai[1] <= 0)
            {
                Projectile.ai[1] = target.whoAmI + 1;
                if ((int)(projOwner.itemAnimationMax * 0.25f) < projOwner.itemAnimation && projOwner.itemAnimation < (int)(projOwner.itemAnimationMax * 0.75f))
                    projOwner.itemAnimation = (int)(projOwner.itemAnimationMax * 0.75f);
            }

            float scale = target.width > target.height ? target.width : target.height * 0.8f;
            scale /= 76f;
            if (Main.myPlayer == Projectile.owner)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<Censored>(), 0, 0, Projectile.owner, scale, target.whoAmI);
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
            Vector2 position = ownerMountedCenter - Main.screenPosition + RandomizeTexture();
            Vector2 origin = new Vector2(Projectile.spriteDirection == 1 ? 8 : 68, 14);
            float rotation = Projectile.rotation + (Projectile.spriteDirection == 1 ? 0 : 3.14f);
            float scale = Projectile.scale * 0.33f;

            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, position, TextureAssets.Projectile[Projectile.type].Frame(), lightColor, rotation, origin, scale, 0f, 0);

            position += (Projectile.Center - ownerMountedCenter) / 5 + RandomizeTexture();
            scale = Projectile.scale * 0.50f;
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, position, TextureAssets.Projectile[Projectile.type].Frame(), lightColor, rotation, origin, scale, 0f, 0);
            
            for (int i = -1; i < 2; i += 2)
            {
                position = Projectile.Center + new Vector2(-14, -5 * i).RotatedBy(Projectile.rotation) - Main.screenPosition;
                scale = Projectile.scale * 0.33f;
                float rotation2 = Projectile.rotation - 1.047f * i + Projectile.localAI[0] * i;
                Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, position + RandomizeTexture(), TextureAssets.Projectile[Projectile.type].Frame(), lightColor, rotation2 + (Projectile.spriteDirection == 1 ? 0 : 3.14f), origin, scale, 0f, 0);

                position += new Vector2(25, 0).RotatedBy(rotation2) + RandomizeTexture();
                rotation2 += 0.78f * i + (Projectile.spriteDirection == 1 ? 0 : 3.14f);
                Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, position, TextureAssets.Projectile[Projectile.type].Frame(), lightColor, rotation2, origin, scale, 0f, 0);
            }

            position = Projectile.Center + new Vector2(-60, 0).RotatedBy(Projectile.rotation) - Main.screenPosition + RandomizeTexture();
            scale = Projectile.scale * 0.66f;
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, position, TextureAssets.Projectile[Projectile.type].Frame(), lightColor, rotation, origin, scale, 0f, 0);

            return false;
        }

        private Vector2 RandomizeTexture()
        {
            return new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
        }

        private bool GrabBlackList(int i)
        {
            switch (i)
            {
                case NPCID.TargetDummy:
                    return true;
                default:
                    return false;
            }
        }
    }

    public class CensoredSpike : ModProjectile
    {
        public override string Texture => "LobotomyCorp/Projectiles/Censored";

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1.3f;
            Projectile.alpha = 0;
            Projectile.timeLeft = 120;

            //Projectile.hide = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.friendly = true;

            DrawHeldProjInFrontOfHeldItemAndArms = true;
        }

        public override void AI()
        {
            Player projOwner = Main.player[Projectile.owner];
            Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);
            Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
            projOwner.heldProj = Projectile.whoAmI;
            projOwner.itemTime = projOwner.itemAnimation;

            float rot = Projectile.velocity.ToRotation();

            if (Projectile.localAI[1] == 0)
            {
                SoundEngine.PlaySound(WeaponSound("Censored2_2", false), projOwner.Center);
                Projectile.localAI[1] = 1;
            }

            float progress = 1f - (float)projOwner.itemAnimation / (float)projOwner.itemAnimationMax;
            if (progress < 0.15f)
            {
                float prog = progress % 0.15f / 0.15f;
                prog *= prog * prog;
                Projectile.ai[0] = Lerp(0f, 360, prog);
            }
            else if (progress >= 0.5f)
            {
                if (progress < 0.8f)
                    Projectile.ai[0] = 360 - 360 * (float)Math.Sin(1.57f * ((progress - 0.5f) / 0.3f));
                else
                    Projectile.ai[0] = 0;
            }
            else
                Projectile.ai[0] = 360;

            if (progress > 0.0f)
            {
                if (Projectile.localAI[0] < 1.0472)
                    Projectile.localAI[0] += 0.1298132f;
            }

            Vector2 velRot = new Vector2(1, 0).RotatedBy(rot);
            projOwner.itemRotation = (float)Math.Atan2(velRot.Y * Projectile.direction, velRot.X * Projectile.direction);
            projOwner.direction = Projectile.spriteDirection;
            Projectile.rotation = rot;// + MathHelper.ToRadians(Projectile.spriteDirection == 1 ? 45 : 135);

            Projectile.Center = ownerMountedCenter + (40 + Projectile.ai[0]) * velRot;

            if (projOwner.itemAnimation == 1)
                Projectile.Kill();
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Player projOwner = Main.player[Projectile.owner];
            float progress = 1f - (float)projOwner.itemAnimation / (float)projOwner.itemAnimationMax;
            if (progress > 0.13f && progress < 0.3f)
                modifiers.SourceDamage *= 2f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.player[Projectile.owner].itemAnimation < Main.player[Projectile.owner].itemAnimationMax / 2)
                target.immune[Projectile.owner] = Main.player[Projectile.owner].itemAnimation;
            else
                target.immune[Projectile.owner] = Main.player[Projectile.owner].itemAnimationMax / 3;

            float scale = target.width > target.height ? target.width : target.height * 0.8f;
            scale /= 76f;
            if (Main.myPlayer == Projectile.owner)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<Censored>(), 0, 0, Projectile.owner, scale, target.whoAmI);
        }

        public override bool? CanHitNPC(NPC target)
        {
            Player projOwner = Main.player[Projectile.owner];
            float prog = 1f - (float)projOwner.itemAnimation / (float)projOwner.itemAnimationMax;
            if (0.3f < prog && prog < 0.5f)
                return false;
            return base.CanHitNPC(target);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Player projOwner = Main.player[Projectile.owner];
            Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);
            if (Collision.CheckAABBvLineCollision2(targetHitbox.TopLeft(), targetHitbox.Size(), ownerMountedCenter, Projectile.Center))
                return true;
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player projOwner = Main.player[Projectile.owner];
            Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);
            Vector2 position = ownerMountedCenter - Main.screenPosition;
            Vector2 origin = new Vector2(Projectile.spriteDirection == 1 ? 8 : 68, 14);
            float rotation = Projectile.rotation + (Projectile.spriteDirection == 1 ? 0 : 3.14f);
            float scale = Projectile.scale * 0.33f;
            lightColor = Color.White;

            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, position + RandomizeTexture(), TextureAssets.Projectile[Projectile.type].Frame(), lightColor, rotation, origin, scale, 0f, 0);

            position += (Projectile.Center - ownerMountedCenter);
            position += new Vector2(-12, 0).RotatedBy(Projectile.rotation);
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, position + RandomizeTexture(), TextureAssets.Projectile[Projectile.type].Frame(), lightColor, rotation, origin, scale, 0f, 0);

            for (int i = 0; i < 7; i++)
            {
                position -= (Projectile.Center - ownerMountedCenter) / 14;
                if (i == 3)
                    position -= (Projectile.Center - ownerMountedCenter) / 26;
                if (i > 2)
                    scale = Projectile.scale * 0.5f;
                Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, position + RandomizeTexture(), TextureAssets.Projectile[Projectile.type].Frame(), lightColor, rotation, origin, scale, 0f, 0);
            }

            position = ownerMountedCenter - Main.screenPosition + (Projectile.Center - ownerMountedCenter) / 12;
            scale = Projectile.scale * 0.50f;
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, position + RandomizeTexture(), TextureAssets.Projectile[Projectile.type].Frame(), lightColor, rotation, origin, scale, 0f, 0);

            position += (Projectile.Center - ownerMountedCenter) / 8;

            for (int i = -1; i < 2; i += 2)
            {
                float length = 40;
                if (projOwner.itemAnimation < projOwner.itemAnimationMax / 2)
                    length = Lerp(-20, 40, (float)projOwner.itemAnimation / ((float)projOwner.itemAnimationMax / 2f));

                Vector2 position2 = position + new Vector2(length, -5 * i).RotatedBy(Projectile.rotation);
                scale = Projectile.scale * 0.33f;
                float rotation2 = Projectile.rotation - 1.047f * i + Projectile.localAI[0] * i;
                Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, position2 + RandomizeTexture(), TextureAssets.Projectile[Projectile.type].Frame(), lightColor, rotation2 + (Projectile.spriteDirection == 1 ? 0 : 3.14f), origin, scale, 0f, 0);

                position2 += new Vector2(25, 0).RotatedBy(rotation2) + RandomizeTexture();
                rotation2 += 0.78f * i + (Projectile.spriteDirection == 1 ? 0 : 3.14f) - (Projectile.localAI[0] - 0.261799f) * i;
                Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, position2 + RandomizeTexture(), TextureAssets.Projectile[Projectile.type].Frame(), lightColor, rotation2, origin, scale, 0f, 0);
            }

            scale = Projectile.scale * 0.66f;
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, position + RandomizeTexture(), TextureAssets.Projectile[Projectile.type].Frame(), lightColor, rotation, origin, scale, 0f, 0);

            return false;
        }

        private Vector2 RandomizeTexture()
        {
            return new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
        }
    }

    class Censored : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1.3f;
            Projectile.alpha = 0;
            Projectile.timeLeft = 20;

            Projectile.hide = true;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        }

        public override void AI()
        {
            if (Projectile.localAI[1] == 0)
            {
                Projectile.rotation = Main.rand.NextFloat(-0.785f, 0.785f);
                Projectile.localAI[1]++;
            }
            Projectile.scale = Projectile.ai[0];

            if (Projectile.ai[1] >= 0 && Main.npc[(int)Projectile.ai[1]].active)
            {
                Projectile.Center = Main.npc[(int)Projectile.ai[1]].Center;
            }
            else
                Projectile.ai[1] = -1;

            if (Projectile.timeLeft < 10)
                Projectile.alpha += 25;
            base.AI();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 position = Projectile.Center - Main.screenPosition + RandomizeTexture();
            Vector2 origin = tex.Size() / 2;
            Color color = Color.White * (1f - Projectile.alpha / 255f);

            Main.EntitySpriteDraw(tex, position, tex.Frame(), color, Projectile.rotation, origin, Projectile.scale, 0f, 0);

            return false;
        }

        private Vector2 RandomizeTexture()
        {
            return new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
        }
    }
}
