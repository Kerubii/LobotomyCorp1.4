using LobotomyCorp.Buffs;
using LobotomyCorp.Items.Ruina.Art;
using LobotomyCorp.Misc;
using LobotomyCorp.ModSystems;
using LobotomyCorp.Players;
using LobotomyCorp.Util;
using LobotomyCorp.Visuals.LobEffects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles.Realized
{
	public class FaintAromaRAlt : ModProjectile
	{
		public override string Texture => "LobotomyCorp/Items/Ruina/Art/FaintAromaS";

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.alpha = 0;
            Projectile.timeLeft = 600;

            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.friendly = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;

            Projectile.hide = true;
        }

        public override void AI()
        {
            Player projOwner = Main.player[Projectile.owner];
            Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);
            projOwner.direction = Math.Sign(Projectile.velocity.X);
            Projectile.direction = projOwner.direction;
            projOwner.heldProj = Projectile.whoAmI;

            int half = projOwner.itemAnimationMax / 2;
            int limit = projOwner.itemAnimationMax + half;
            if (Projectile.timeLeft > limit)
            {
                Projectile.timeLeft = limit;
                Projectile.ai[0] = projOwner.GetModPlayer<LobotomyWawPlayer>().FaintAromaPetal;

                Projectile.rotation = Projectile.velocity.ToRotation();
            }

            Projectile.position.X = ownerMountedCenter.X - (float)(Projectile.width / 2);
            Projectile.position.Y = ownerMountedCenter.Y - (float)(Projectile.height / 2);
            Projectile.position += Projectile.velocity * 2;
            
            int realTime = Projectile.timeLeft - half;
            if (realTime <= projOwner.itemAnimationMax / 2)
            {
                if (realTime == projOwner.itemAnimationMax / 2)
                {
                    if (Projectile.ai[1] <= 0 && projOwner.HeldItem.ModItem is FaintAromaR)
                    {
                        FaintAromaR item = projOwner.HeldItem.ModItem as FaintAromaR;
                        if (Main.rand.NextBool(2))
                        {
                            FaintAromaR.SmearCircle(projOwner, item.SwingDirection);
                        }
                        else
                        {
                            FaintAromaR.SmearEllipse(projOwner, item.SwingDirection);
                        }
                        SoundEngine.PlaySound(projOwner.HeldItem.UseSound, projOwner.position);
                    }
                    projOwner.velocity *= 0.1f;
                }

                if (Projectile.ai[1] > 0)
                {
                    if (realTime == projOwner.itemAnimationMax / 4)
                    {
                        if (Projectile.ai[0] >= projOwner.GetModPlayer<LobotomyWawPlayer>().FaintAromaPetalMax * 3)
                        {
                            SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Ali_StrongAtk_Finish"), projOwner.Center);

                            //User gains 'Unwithering Flower on activation'
                            projOwner.AddBuff(ModContent.BuffType<FaintAromaUnwithering>(), 10 * 60);

                            projOwner.GetModPlayer<LobotomyWawPlayer>().FaintAromaPetal = 0;
                            for (int i = 0; i < 32; i++)
                            {
                                Dust dust;
                                Vector2 dustVel = new Vector2(8f, 0f).RotatedBy(MathHelper.ToRadians(11.25f * i));
                                dust = Main.dust[Dust.NewDust(projOwner.Center, 1, 1, 205, dustVel.X, dustVel.Y, 0, new Color(255, 255, 255), 1f)];
                                //dust.velocity = Projectile.velocity * 4f;
                                dust.noGravity = true;
                                dust.fadeIn = 1.2f;
                            }
                            foreach (NPC n in Main.npc)
                            {
                                if (n.active && n.chaseable && n.CanBeChasedBy(ModContent.ProjectileType<AlriuneDeathAnimation>()) && (n.Center - projOwner.Center).Length() < 800 && Main.myPlayer == Projectile.owner && n.GetGlobalNPC<LobotomyGlobalNPC>().FaintAromaLaurelWreath > 0f)
                                {
                                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), n.Center, Vector2.Zero, ModContent.ProjectileType<AlriuneDeathAnimation>(), (int)(Projectile.damage * 3f), 0, projOwner.whoAmI, n.whoAmI);
                                    //n.GetGlobalNPC<LobotomyGlobalNPC>().FaintAromaLaurelWreath = 0f;
                                }
                            }
                        }

                        //Doubled for Bloom Purposes
                        for (int i = 0; i < 2; i++)
                        {
                            WeaponSmearCircle smear = new WeaponSmearCircle();

                            int dir = -1;
                            float start = projOwner.direction > 0 ? -140 : -50;

                            smear.Setup(projOwner, new Vector2(12 * projOwner.direction, 0), MathHelper.ToRadians(start * dir), projOwner.itemAnimationMax / 2, projOwner.direction * dir, true);
                            smear.SetupSemiCircle(45, 120, MathHelper.ToRadians(50), MathHelper.ToRadians(180), MathHelper.ToRadians(270), 0.3f);
                            smear.SetShaderImage(
                                MiscAssets.FlatColor,
                                MiscAssets.TexTrail2,
                                MiscAssets.Gradient
                                );
                            smear.Color = new Color(249, 159, 253);

                            smear.AddEffect();

                            WeaponSmearEllipse smear2 = new WeaponSmearEllipse();
                            dir = -1;
                            start = -140 * projOwner.direction;

                            smear2.Setup(projOwner, new Vector2(12 * projOwner.direction, 0), (projOwner.direction > 0 ? 0 : 3.14f), projOwner.itemAnimationMax / 2, projOwner.direction * dir, true);
                            smear2.SetupPartEllipse(90, 65, 200, 100, MathHelper.ToRadians(start * dir), MathHelper.ToRadians(50), MathHelper.ToRadians(180), MathHelper.ToRadians(270), 0.4f);
                            smear2.Color = new Color(249, 159, 253);
                            smear2.SetShaderImage(
                                MiscAssets.FlatColor,
                                MiscAssets.TexTrail2,
                                MiscAssets.Gradient
                                );

                            smear2.AddEffect();
                        }

                        if (projOwner.HeldItem.ModItem is FaintAromaR)
                        {
                            FaintAromaR item = projOwner.HeldItem.ModItem as FaintAromaR;
                            Projectile.rotation = MathHelper.ToRadians(item.SwingRotation2(projOwner));
                        }
                    }                    
                }                
            }
            else if (realTime <= (int)(projOwner.itemAnimationMax * 0.8f))
            {
                if (realTime == (int)(projOwner.itemAnimationMax * 0.8f))
                {
                    WeaponSmearLine smear = new();
                    int time = (int)(projOwner.itemAnimationMax * 0.33f);
                    float dist = Projectile.velocity.Length();
                    smear.Setup(Projectile, Projectile.velocity * 2, Projectile.velocity.ToRotation() + 3.14f, time, 1);
                    smear.SetupLine(16, 32, dist * time);
                    smear.SetShaderImage(
                        MiscAssets.FlatColor,
                        MiscAssets.TrailSmoke,
                        MiscAssets.TrailPierce
                        );
                    smear.Color = new Color(249, 159, 253);

                    smear.AddEffect();

                    int amount = projOwner.GetModPlayer<LobotomyWawPlayer>().FaintAromaPetalNum() * 3;

                    if (amount > 0)
                    {
                        for (int i = 0; i < amount; i++)
                        {
                            int waveTime = time + time / 2 + Main.rand.Next(10);
                            int waveSpeed = -10 - Main.rand.Next(20);
                            int angleDir = (Main.rand.NextBool(2) ? 1 : -1);

                            WeaponSmearSineWave test = new();
                            test.Setup(Projectile, Projectile.velocity * 2, Projectile.velocity.ToRotation(), waveTime, 1);
                            test.SetupWave(30, 8, 20 * angleDir, waveSpeed, MathHelper.ToRadians(20) * angleDir, 100 + Main.rand.Next(30), waveSpeed * waveTime);
                            test.SetShaderImage(
                                MiscAssets.FlatColor,
                                MiscAssets.WindTrail,
                                MiscAssets.WindTrail
                                );
                            test.Color = new Color(249, 159, 253);
                            if (amount >= 6 && Main.rand.NextBool(3))
                                test.Color = new Color(64, 255, 243);

                            test.AddEffect();
                        }
                    }

                    SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Ali_StrongAtk"), projOwner.Center);
                }

                projOwner.maxFallSpeed = 50;
                //projOwner.velocity = Projectile.velocity;
                projOwner.immune = true;
                projOwner.immuneTime = 10;

                projOwner.GetModPlayer<LobotomyModPlayer>().FallSpeedMult = 3;
                projOwner.GetModPlayer<LobotomyModPlayer>().ForcePlayerVelocity(Projectile.velocity);

                for (int i = 0; i < 4; i++)
                {
                    Dust dust = Main.dust[Dust.NewDust(Projectile.Center - new Vector2(16, 16) + Projectile.velocity * 2, 16, 16, DustID.VenomStaff)];
                    dust.fadeIn = 1.2f;
                    dust.noGravity = true;
                }
            }
            if (realTime < 0)
            {
                projOwner.itemAnimation = 2;
                projOwner.itemTime = 2;
            }
                
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            LobotomyWawPlayer wawPlayer = Main.player[Projectile.owner].GetModPlayer<LobotomyWawPlayer>();
            if (Projectile.ai[1] >= 0 && !wawPlayer.FaintAromaUnwitheringFlower && Projectile.ai[0] >= wawPlayer.FaintAromaPetalMax * 3)
                Projectile.ai[1]++;
            else
                Projectile.ai[1] = -1;

            wawPlayer.FaintAromaAddPetal(8f);

            target.GetGlobalNPC<LobotomyGlobalNPC>().FaintAromaLaurelWreath = 0.15f;

            target.immune[Projectile.owner] = Projectile.timeLeft - Main.player[Projectile.owner].itemAnimationMax;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return base.CanHitNPC(target);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Player player = Main.player[Projectile.owner];

            Vector2 position;
            position.X = player.position.X + (float)player.width * 0.5f - (float)(player.direction * 2); // forward port from 1.4.5
            position.Y = player.MountedCenter.Y;// - (float)player.heldItemFrame.Height * 0.5f;

            float rotation = Projectile.rotation;
            int tMax = (int)(player.itemAnimationMax * 0.3f);
            float prog = 1f - (float)(Projectile.timeLeft - (player.itemAnimationMax/2) - (int)(player.itemAnimationMax * 0.6f)) / tMax;
            prog = Math.Clamp(prog, 0f, 1f);
            Vector2 offset = new Vector2(-16 + (16 * Easing.EaseOutCubic(prog)), 0).RotatedBy(rotation);

            Main.EntitySpriteDraw(tex, position + offset - Main.screenPosition, tex.Frame(), lightColor, rotation + 0.785f, new Vector2(0, tex.Height), player.GetAdjustedItemScale(player.HeldItem), SpriteEffects.None);

            return false;
        }
    }
}
