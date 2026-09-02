using LobotomyCorp.Items;
using LobotomyCorp.Items.Teth;
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
    public class LanternSwing : ModProjectile
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
            Projectile.localNPCHitCooldown = 15;

            //DrawHeldProjInFrontOfHeldItemAndArms = true;
        }

        private bool isCurentlyBiting => Projectile.ai[0] > 0 && Projectile.ai[0] < 50;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (Projectile.timeLeft > player.itemAnimationMax)
                Projectile.timeLeft = player.itemAnimationMax;
            player.heldProj = Projectile.whoAmI;
            player.direction = Projectile.direction;
            Projectile.spriteDirection = Projectile.direction;

            Projectile.rotation = -90 - 70 * Projectile.spriteDirection;
            float prog = (float)Projectile.timeLeft / player.itemAnimationMax;
            float range = 78 * Projectile.scale;
            if (player.channel && Projectile.ai[1] > 0 && Projectile.ai[0] < 50)
            {
                NPC target = Main.npc[(int)Projectile.ai[1] - 1];
                if (target.life > 0)
                {
                    //target.Center = player.Center + Projectile.velocity;
                    player.GetModPlayer<LobotomyModPlayer>().GrabEnemy(target);

                    if (Projectile.ai[0] % 20 < 10)
                    {
                        Projectile.frame = 1;
                    }
                    else
                    {
                        Projectile.frame = 0;
                    }

                    if (Projectile.ai[0] % 20 == 10)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            Dust d = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood)];
                            d.noGravity = true;
                            d.velocity *= 1.8f;
                            d.fadeIn = 1.2f;
                        }
                        SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Entity/MeatLantern/Bunny_StartSmall") with { Volume = 0.2f }, Projectile.Center);

                        if (Main.myPlayer == Projectile.owner && !target.dontTakeDamage)
                        {
                            player.ApplyDamageToNPC(target, Projectile.damage, 0, 1, damageType: DamageClass.Melee, damageVariation: true);
                        }
                    }

                    Projectile.ai[0]++;
                    Projectile.timeLeft++;
                    player.itemAnimation = Projectile.timeLeft;
                    player.itemTime = player.itemAnimation;
                }
                else
                {
                    Projectile.ai[1] = 0;
                    Projectile.ai[0] = 50;
                    player.channel = false;
                }                
            }
            else
            {
                if (Projectile.ai[0] > 0 && Projectile.ai[0] < 51)
                {
                    if (Projectile.ai[2] == 0 && Main.myPlayer == Projectile.owner && Projectile.ai[1] > 0)
                    {
                        Vector2 dirTo = -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(10));
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, dirTo, ModContent.ProjectileType<LanternNom>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: Projectile.ai[1] - 1, ai2: Projectile.timeLeft - 5);
                        Projectile.ai[2]++;
                    }

                    WeaponSmearLine line = new WeaponSmearLine();
                    line.Setup(Projectile.Center, Vector2.Zero, Projectile.rotation - MathHelper.ToRadians(90) * Projectile.direction, 15, 1);
                    line.SetupLine(32, 8, 60);
                    line.Color = Color.Pink * 0.6f;
                    line.SetShaderImage(FlatColor, TrailSmoke, PlasmaNoise);
                    line.AddEffect();

                    for (int i = 0; i < 12; i++)
                    {
                        Vector2 vel = new Vector2(Main.rand.NextFloat(1, 4f), 0).RotatedBy(Projectile.rotation - MathHelper.ToRadians(90) * Projectile.direction);
                        Dust d = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Snow, vel.X, vel.Y)];
                        d.noGravity = true;
                        if (Main.rand.NextBool(2))
                        {
                            d = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, vel.X, vel.Y)];
                            d.noGravity = true;
                            d.fadeIn = 1.2f;
                        }
                    }

                    for (int i = 0; i < 8; i++)
                    {
                        Dust d = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood)];
                        d.noGravity = true;
                        d.velocity *= 1.8f;
                        d.fadeIn = 1.2f;
                    }

                    Projectile.ai[0] = 51;
                }
                Projectile.frame = 0;

                
            }

            if (prog > 0.8f)
            {
                prog = (prog - 0.8f) / .2f;
                Projectile.rotation += 40 * Easing.EaseOutCubic(prog) * Projectile.direction;
            }
            else
            {
                if (prog > 0.4f && !isCurentlyBiting)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Vector2 vel = new Vector2(Main.rand.NextFloat(1, 4f), 0).RotatedBy(Projectile.rotation - MathHelper.ToRadians(90) * Projectile.direction);
                        Dust d = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Snow, vel.X, vel.Y)];
                        d.noGravity = true;
                    }
                }

                prog = 1f - (prog - 0.2f) / .6f;
                if (prog > 1f)
                    prog = 1f;
                Projectile.rotation += 240 * Easing.EaseOutCubic(prog) * Projectile.direction;
            }
            Projectile.rotation = MathHelper.ToRadians(Projectile.rotation);
            LobCorpLight.LobItemFrame(player, MathHelper.ToDegrees(MathHelper.WrapAngle(Projectile.rotation)));
            Projectile.Center = player.MountedCenter + new Vector2(range, 0).RotatedBy(Projectile.rotation);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];

            if (target.knockBackResist > .1f && player.channel && Projectile.ai[1] == 0 && Projectile.ai[0] == 0)
            {
                Projectile.ai[1] = target.whoAmI + 1;
                //Projectile.velocity = target.Center - player.Center;

                float rot = Projectile.rotation - MathHelper.ToRadians(90) * Projectile.direction;
                WeaponSmearLine line = new WeaponSmearLine();
                line.Setup(Projectile, Vector2.Zero, rot, 20, 1);
                line.SetupLine(28, 8, 80);
                line.Color = Color.Pink * 0.8f;
                line.SetShaderImage(FlatColor, BloodTrailStraight, PlasmaNoise);
                line.AddEffect();

                for (int i = 0; i < 12; i++)
                {
                    Vector2 vel = new Vector2(Main.rand.NextFloat(1, 4f), 0).RotatedBy(rot);
                    Dust d = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Snow, vel.X, vel.Y)];
                    d.noGravity = true;
                }
            }
            else if (Projectile.ai[2] == 0 && Main.myPlayer == Projectile.owner)
            {
                Vector2 dirTo = -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(10));
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, dirTo, ModContent.ProjectileType<LanternNom>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: target.whoAmI, ai2: Projectile.timeLeft - 5);
                Projectile.ai[2]++;
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            int animTime = Main.player[Projectile.owner].itemAnimationMax;
            if ((Projectile.timeLeft > animTime * 0.2f && animTime * 0.8f > Projectile.timeLeft) && !isCurentlyBiting)
                return null;
            return false;
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
            if (Projectile.frame != 0)
                tex = Lantern.AltTex.Value;
            Vector2 pos = LobCorpLight.LobItemLocation(Main.player[Projectile.owner], tex.Frame(), MathHelper.ToDegrees(MathHelper.WrapAngle(Projectile.rotation))) - Main.screenPosition + Projectile.gfxOffY * Vector2.UnitY;
            bool flip = Projectile.spriteDirection == 1;
            Vector2 origin = new Vector2(flip ? 0 : tex.Width, tex.Height);
            Main.EntitySpriteDraw(tex, pos, null, lightColor, Projectile.rotation + MathHelper.ToRadians(flip ? 45 : 135), origin, Projectile.scale, flip ? SpriteEffects.None : SpriteEffects.FlipHorizontally);

            return false;
        }
    }
}
