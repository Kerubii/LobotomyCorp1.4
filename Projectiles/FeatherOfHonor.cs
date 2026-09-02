using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
    public class FeatherOfHonor : ModProjectile
    {
        public static Asset<Texture2D> FeatherTexture;
        public override void Load()
        {
            if (!Main.dedServ)
            {
                FeatherTexture = Mod.Assets.Request<Texture2D>("Projectiles/FeatherOfHonor", AssetRequestMode.ImmediateLoad);

                Main.QueueMainThreadAction(() =>
                {
                    LobotomyCorp.PremultiplyTexture(FeatherTexture.Value);
                });
            }
        }

        public override void Unload()
        {
            FeatherTexture = null;
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 3;
            Projectile.scale = 1f;
            Projectile.timeLeft = 20;

            Projectile.tileCollide = false;
            //Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.friendly = true;
            Projectile.extraUpdates = 3;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void AI()
        {
            Player projOwner = Main.player[Projectile.owner];
            Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);

            if (Projectile.ai[1] < 3)
            {
                if (!projOwner.channel)
                {
                    Projectile.ai[1] = -1;
                }
                else
                {
                    Projectile.Center = ownerMountedCenter + new Vector2(52, 0).RotatedBy(MathHelper.ToRadians(-30 - 30 * Projectile.ai[0]));
                    Projectile.timeLeft = 20;
                    Projectile.spriteDirection = projOwner.direction;
                    projOwner.itemTime = 24;
                    projOwner.itemAnimation = 24;

                    if (Main.rand.Next(3) == 0)
                    {
                        int i = Dust.NewDust(Projectile.Center - new Vector2(24, 9) * Projectile.scale, (int)(48 * Projectile.scale), (int)(18 * Projectile.scale), DustID.Torch);
                        Main.dust[i].noGravity = true;
                    }
                }
            }

            if (Projectile.ai[1] == 0)
            {
                Projectile.scale = 0;
                Projectile.ai[1] = 1;
            }
            else if (Projectile.ai[1] == 1)
            {
                if (Projectile.scale < 1f)
                    Projectile.scale += 0.02f;
                if (Projectile.scale >= 1f)
                {
                    Projectile.scale = 1f;
                    Projectile.ai[1] ++;
                }
            }
            else if (Projectile.ai[1] == 2)
            {
                if (projOwner.whoAmI == Main.myPlayer)
                {
                    Items.Waw.FeatherOfHonor ModItem = (Items.Waw.FeatherOfHonor)projOwner.HeldItem.ModItem;
                    if (ModItem.FeatherShoot == 0)
                    {
                        SoundEngine.PlaySound(LobotomyCorp.WeaponSound("Firebird"), Projectile.Center);

                        Projectile.ai[1]++;
                        ModItem.FeatherShoot = 24;

                        Vector2 speed = Main.MouseWorld - Projectile.Center;
                        speed.Normalize();
                        speed *= 12f;
                        Projectile.timeLeft = 600;
                        Projectile.velocity = speed;
                        Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0 : 3.14f);

                        Projectile.ai[1]++;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else if (Projectile.ai[1] < 0)
            {
                Projectile.scale -= 0.05f;
                if (Projectile.scale <= 0f)
                    Projectile.Kill();
            }
            else
            {
                int i = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
                Main.dust[i].noGravity = true;

                Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0 : 3.14f);
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5;i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Projectile.velocity.X, Projectile.velocity.Y);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 60 * 5);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.ai[1] == 4)
                modifiers.ArmorPenetration += 8;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.ai[1] >= 3)
                return base.CanHitNPC(target);
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects sp = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D tex = FeatherTexture.Value;
            if (Projectile.ai[1] > 2)
                for (int i = 0; i < 10; i++)
                {
                    Vector2 Pos = Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition;
                    Color color = lightColor * (1f - ((float)(i + 1f) / 10f));

                    Main.EntitySpriteDraw(
                    tex,
                    Pos,
                    TextureAssets.Projectile[Projectile.type].Frame(),
                    lightColor * (1f - i / 10f),
                    Projectile.oldRot[i],
                    TextureAssets.Projectile[Projectile.type].Size() / 2,
                    Projectile.scale,
                    sp, 0);
                }
            Main.EntitySpriteDraw(
                tex,
                Projectile.Center - Main.screenPosition,
                TextureAssets.Projectile[Projectile.type].Frame(),
                lightColor,
                Projectile.rotation,
                TextureAssets.Projectile[Projectile.type].Size() / 2,
                Projectile.scale,
                sp, 0);
            return false;
        }
    }
}
