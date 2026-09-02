using LobotomyCorp.Buffs;
using LobotomyCorp.Players;
using LobotomyCorp.Util;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using ReLogic.Content;
using System;
using System.Data;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;

namespace LobotomyCorp.Projectiles
{
    public class FeatherOfHonorMinion : ModProjectile
    {
        public override string Texture => "LobotomyCorp/Projectiles/FeatherOfHonor";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Spear");
            Main.projPet[Projectile.type] = true;

            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 2;

            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.friendly = true;

            Projectile.minion = true;
            Projectile.minionSlots = 1;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = Main.rand.Next(120);
            }

            Player projOwner = Main.player[Projectile.owner];
            Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);

            if (!CheckActive(projOwner))
                return;

            float order = getOrder();
            int total = projOwner.ownedProjectileCounts[Projectile.type] - 1;
            if (total <= 0)
            {
                if (total < 0)
                    order = 0.5f;
                total = 1;
            }
            float angle = (order / total);

            Vector2 targetPos = projOwner.MountedCenter + new Vector2(52, 0).RotatedBy(MathHelper.ToRadians(-150 + 120 * angle));
            Projectile.localAI[0]++;
            targetPos.Y += (float)Math.Sin(6.28f * Projectile.localAI[0] / 120f);
            float targetRot = projOwner.direction > 0 ? 0 : 3.14f;

            int target = -1;
            int range = 1000;
            Projectile.Minion_FindTargetInRange(range, ref target, false);

            // Turn to look at target
            if (target > 0) targetRot = Projectile.Center.AngleTo(Main.npc[target].Center);
            // Follow player velocity when no target
            else targetRot += Math.Clamp(projOwner.velocity.Y / 12f, - 0.262f, 0.262f) * projOwner.direction;

            if (Projectile.ai[0] < 0)
            {
                Projectile.ai[0]++;
                Projectile.ai[1] = 0;

                // Creates dust to appear
                if (Projectile.ai[0] > -60 && Projectile.ai[0] <= -30)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, new Vector2(2 + Main.rand.NextFloat(3), 0).RotatedByRandom(6.28f));
                        d.noGravity = true;
                    }
                }
            }
            else if (Projectile.ai[1] > 0)
            {
                Projectile.ai[1]--;
                if (Projectile.owner == Main.myPlayer)
                {
                    targetRot = Projectile.Center.AngleTo(Main.MouseWorld);
                }

                if (Projectile.ai[1] == 0 && Projectile.ai[1] >= 0)
                {
                    if (Projectile.owner == Main.myPlayer)
                        ShootFeatherTo(Main.MouseWorld, -30 + Main.rand.Next(60) * -1, order, true);

                    Projectile.netUpdate = true;
                }

                if (projOwner.itemAnimation < projOwner.itemAnimationMax)
                {
                    projOwner.itemAnimation = projOwner.itemAnimationMax - 1;
                }
            }
            else if (Projectile.ai[0] > 0)
            {
                if (Projectile.ai[0] > 0)
                {
                    Projectile.ai[0]--;
                    if (target == -1)
                        Projectile.ai[0] = 0;
                }

                if (Projectile.ai[0] == 1)
                {
                    ShootFeatherTo(Main.npc[target].Center, -30 + Main.rand.Next(60) * -1, order);
                }
            }
            else
            {
                if (target > 0)
                {
                    Projectile.ai[0] = 15 + Main.rand.Next(60);
                }

                if (Main.rand.NextBool(5))
                {
                    int i = Dust.NewDust(Projectile.Center - new Vector2(24, 9) * Projectile.scale, (int)(48 * Projectile.scale), (int)(18 * Projectile.scale), DustID.Torch);
                    Main.dust[i].noGravity = true;
                }
            }

            Projectile.Center = Vector2.Lerp(targetPos, Projectile.Center, 0.35f);
            Projectile.rotation = Terraria.Utils.AngleLerp(Projectile.rotation, targetRot, MathHelper.ToRadians(4f));
        }

        private void ShootFeatherTo(Vector2 target, int cooldown, float order, bool pen = false)
        {
            Player projOwner = Main.player[Projectile.owner];
            Vector2 shoot = target - Projectile.Center;
            shoot.Normalize();
            shoot *= 12f;

            if (Projectile.owner == Main.myPlayer)
            {
                int p = Projectile.NewProjectile(projOwner.GetSource_FromThis(), Projectile.Center, shoot, ModContent.ProjectileType<FeatherOfHonor>(), Projectile.damage, Projectile.knockBack, projOwner.whoAmI, order, pen ? 3 : 4);
                Main.projectile[p].timeLeft = 600;
                Main.projectile[p].netUpdate = true;
            }

            Projectile.ai[0] = cooldown;
            for (int i = 0; i < 16; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, new Vector2(2, 0).RotatedBy(6.28f * (i / 16f)));
                d.noGravity = true;
            }

            SoundEngine.PlaySound(LobotomyCorp.WeaponSound("Firebird"), Projectile.Center);
        }

        private int getOrder()
        {
            int order = 0;
            if (Projectile.whoAmI > 0)
            {
                for (int i = Projectile.whoAmI - 1; i >= 0; i--)
                {
                    if (Main.projectile[i].active && Main.projectile[i].type == Projectile.type && Main.projectile[i].owner == Projectile.owner) order++;
                }
            }
            return order;
        }
        
        private bool CheckActive(Player owner)
        {
            int bufType = ModContent.BuffType<FeatherOfHonorM>();

            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(bufType);

                return false;
            }

            if (owner.HasBuff(bufType) && Projectile.timeLeft < 2)
            {
                Projectile.timeLeft = 3;
            }
            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] < -30)
                return false;

            float msc = 1f;
            if (Projectile.ai[0] < 0)
            {
                msc = 1f - Projectile.ai[0] / -45f;
                //msc = Easing.EaseInCubic(msc);
            }

            SpriteEffects sp = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D tex = FeatherOfHonor.FeatherTexture.Value;
            Vector2 position = Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;

            Main.EntitySpriteDraw(
                tex,
                position,
                TextureAssets.Projectile[Projectile.type].Frame(),
                lightColor,
                Projectile.rotation,
                TextureAssets.Projectile[Projectile.type].Size() / 2,
                Projectile.scale * msc,
                sp, 0);

            if (Projectile.ai[0] < 30 && Projectile.ai[0] > 0)
            {
                float scale = Projectile.ai[0] / 30f;

                Color glow = Color.White * 0.5f;

                scale = 0.4f * Util.Easing.EaseInCubic(scale);

                Main.EntitySpriteDraw(
                tex,
                position,
                TextureAssets.Projectile[Projectile.type].Frame(),
                glow,
                Projectile.rotation,
                TextureAssets.Projectile[Projectile.type].Size() / 2,
                Projectile.scale + scale,
                sp, 0);
            }
            return false;
        }
    }
}
