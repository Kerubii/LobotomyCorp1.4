using LobotomyCorp.Items;
using LobotomyCorp.Projectiles.RedMist;
using LobotomyCorp.Util;
using LobotomyCorp.Visuals.LobEffects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static LobotomyCorp.Misc.MiscAssets;

namespace LobotomyCorp.Projectiles
{
    public class LanternSwingAround : ModProjectile
    {
        public override string Texture => "LobotomyCorp/Items/Teth/Lantern";

        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;

            Projectile.hide = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.friendly = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;

            //DrawHeldProjInFrontOfHeldItemAndArms = true;
        }

        public override void AI()
        {

            Player player = Main.player[Projectile.owner];

            if (Projectile.timeLeft > player.itemAnimationMax)
                Projectile.timeLeft = player.itemAnimationMax;
            player.heldProj = Projectile.whoAmI;
            player.direction = Projectile.direction;
            Projectile.spriteDirection = Projectile.direction;

            Projectile.rotation = -90 - 50 * Projectile.spriteDirection;
            float prog = (float)player.itemAnimation / player.itemAnimationMax;
            float range = 78 * Projectile.scale;
            // 5 Swings
            float rotamnt = (360 * 4 + 270);
            if (prog > 0.9f)
            {
                prog = (prog - 0.9f) / .1f;
                Projectile.rotation += 10 * Easing.EaseOutCubic(prog) * Projectile.direction;
            }
            else if (prog > 0.2f)
            {
                prog = 1f - (prog - 0.2f) / .7f;
                Projectile.rotation += rotamnt * Easing.EaseOutSine(prog) * Projectile.direction;

                if (Projectile.soundDelay == 0 && prog < 0.8f)
                {
                    SoundEngine.PlaySound(LobotomyCorp.WeaponSounds.Hammer, player.position);
                    Projectile.soundDelay = 15;
                }

                for (int i = 0; i < 3; i++)
                {
                    int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood);
                    Main.dust[d].noGravity = true;
                }

                if (Projectile.localAI[0] == 0)//(int)(prog * player.itemAnimationMax) % (int)(player.itemAnimationMax * 0.2f) == 0 && prog < 0.8f)
                {
                    WeaponSmearCircle smear = new WeaponSmearCircle();
                    float randSync = MathHelper.ToRadians(Projectile.rotation);// Main.rand.NextFloat(6.28f);
                    rotamnt = MathHelper.ToRadians(rotamnt);
                    smear.Setup(player, Vector2.Zero, randSync, (int)(player.itemAnimationMax * 0.8f), Projectile.direction);
                    smear.SetupCircle(24, range, rotamnt, 0.4f);
                    smear.Color = Color.Pink * 0.8f;
                    smear.SetShaderImage(FlatColor, TexTrail1, PlasmaNoise);
                    smear.AddEffect();

                    smear = new WeaponSmearCircle();
                    smear.Setup(player, Vector2.Zero, randSync, (int)(player.itemAnimationMax * 0.8f), Projectile.direction);
                    smear.SetupCircle(24, range + 5, rotamnt, 0.4f);
                    smear.Color = Color.White * 0.6f;
                    smear.SetShaderImage(BloodTexture, PlasmaNoiseForArcs, Worley);
                    smear.AddEffect();
                    Projectile.localAI[0]++;
                }
            }
            else if (prog > 0.15f)
            {
                Projectile.rotation += rotamnt * Projectile.direction;
                Projectile.soundDelay = 0;
            }
            else
            {
                if (Projectile.soundDelay == 0)
                {
                    SoundEngine.PlaySound(LobotomyCorp.WeaponSounds.Hammer, player.position);
                    Projectile.soundDelay = 100;
                }

                prog = prog / 0.15f;
                Projectile.rotation += 270 * Easing.EaseInSine(prog) * Projectile.direction;
            }
            Projectile.rotation = MathHelper.ToRadians(Projectile.rotation);
            LobCorpLight.LobItemFrame(player, MathHelper.ToDegrees(MathHelper.WrapAngle(Projectile.rotation)));
            Projectile.Center = player.MountedCenter + new Vector2(range, 0).RotatedBy(Projectile.rotation);
        }

        public override bool? CanHitNPC(NPC target)
        {
            int animTime = Main.player[Projectile.owner].itemAnimationMax;
            if ((Projectile.timeLeft > animTime * 0.2f && animTime * 0.9f > Projectile.timeLeft) || Projectile.timeLeft <= animTime * 0.15f)
                return null;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[1] == 0)
            {
                Vector2 dirTo = -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(10)); //player.DirectionTo(Main.MouseWorld);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, dirTo, ModContent.ProjectileType<LanternNom>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: target.whoAmI, ai2: Projectile.timeLeft - 10);
                Projectile.ai[1]++;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.timeLeft >= Main.player[Projectile.owner].itemAnimationMax * 0.15f)
            modifiers.Knockback *= 0.6f;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 pos = LobCorpLight.LobItemLocation(Main.player[Projectile.owner], tex.Frame(), MathHelper.ToDegrees(MathHelper.WrapAngle(Projectile.rotation))) - Main.screenPosition + Projectile.gfxOffY * Vector2.UnitY;
            bool flip = Projectile.spriteDirection == 1;
            Vector2 origin = new Vector2(flip ? 0 : tex.Width, tex.Height);
            Main.EntitySpriteDraw(tex, pos, null, lightColor, Projectile.rotation + MathHelper.ToRadians(flip ? 45 : 135), origin, Projectile.scale, flip ? SpriteEffects.None : SpriteEffects.FlipHorizontally);

            return false;
        }
    }
}
