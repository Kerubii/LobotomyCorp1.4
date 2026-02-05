using LobotomyCorp.Items;
using LobotomyCorp.NPCs.RedMist;
using LobotomyCorp.Players;
using LobotomyCorp.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Security.Policy;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static LobotomyCorp.LobotomyCorp;
using static Terraria.Player;

namespace LobotomyCorp.Projectiles.Realized
{
    public class SmileRSword : ModProjectile
    {
        public override void SetDefaults() {
            Projectile.width = 92;
            Projectile.height = 92;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1.3f;
            Projectile.alpha = 0;
            Projectile.timeLeft = 600;

            //Projectile.hide = true;
            Projectile.ownerHitCheck = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
        }

        public override void AI() 
        {
            Player owner = Main.player[Projectile.owner];
            owner.heldProj = Projectile.whoAmI;
            float rotation = Projectile.velocity.ToRotation();
            float prog = (float)owner.itemAnimation / owner.itemAnimationMax;
            if (Projectile.ai[0] < 2)
            {
                // Swing Motion
                if (prog > 0.5f)
                {
                    if (Projectile.ai[2] == 0)
                    {
                        Projectile.ai[2]++;
                        SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Danggo_Lv3_Atk") with { Volume = 0.25f }, Projectile.Center);
                        if (Main.myPlayer == Projectile.owner)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), owner.MountedCenter, Projectile.velocity, ModContent.ProjectileType<SmileREffectSlash>(), 0, 0, Projectile.owner, Main.rand.Next(1, 5), owner.itemAnimationMax * 0.6f, 25);
                        }
                    }
                    prog = 1f - ((prog - 0.5f) / 0.5f);
                    rotation += MathHelper.ToRadians(-135 + 270 * (float)Math.Sin(1.57f * prog)) * owner.direction;

                    Projectile.scale = 1f + (float)Math.Sin(3.14f * prog);

                    for (int i = 0; i < 4; i++)
                    {
                        Rectangle box = Projectile.getRect();
                        ModifyDamageHitbox(ref box);
                        int d = Dust.NewDust(box.TopLeft(), box.Height, box.Width, DustID.Wraith);
                        Main.dust[d].noGravity = true;
                    }
                }
                // Bite Motion
                else
                {
                    if (Projectile.ai[2] == 1)
                    {
                        Projectile.ai[2] = 2;
                        SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Language/Danggo_Lv1_Atk1") with { Volume = 0.25f }, Projectile.Center);
                        if (Main.myPlayer == Projectile.owner)
                        {
                            rotation = (Main.MouseWorld - owner.Center).ToRotation();
                            Projectile.velocity = new Vector2(1, 0).RotatedBy(rotation);
                            owner.direction = Math.Sign(Projectile.velocity.X);

                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), owner.MountedCenter, Projectile.velocity, ModContent.ProjectileType<SmileRBite>(), Projectile.damage, Projectile.knockBack, Projectile.owner, (int)(owner.itemAnimationMax * .2f), 140 * Projectile.scale, 1);
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), owner.MountedCenter, Projectile.velocity, ModContent.ProjectileType<SmileRBite>(), Projectile.damage, Projectile.knockBack, Projectile.owner, (int)(owner.itemAnimationMax * .2f), 140 * Projectile.scale, -1);
                        }
                    }

                    Projectile.ai[0] = 1;
                    prog = (prog / 0.5f);
                    Projectile.localAI[0] = Lerp(MathHelper.ToRadians(80), MathHelper.ToRadians(-10), (1f - prog) * 3);
                    Projectile.scale = 1f + Lerp(0, 0.5f, (1f - prog) * 3);
                    Vector2 offset = new Vector2(10 * prog, 0).RotatedBy(rotation);

                    if (owner.itemAnimation <= 2 && owner.channel && owner.GetModPlayer<LobotomyAlephPlayer>().SmileMountain > owner.statLifeMax2 * 0.3f)
                    {
                        Projectile.ai[0]++;
                        owner.itemTime = owner.itemAnimation = owner.itemAnimationMax;
                        Projectile.localAI[0] = MathHelper.ToRadians(-10);
                    }
                }
            }
            // Scream Charge
            else if (Projectile.ai[0] == 2)
            {
                owner.itemTime = owner.itemAnimation = owner.itemAnimationMax;
                if (Main.myPlayer == Projectile.owner)
                {
                    float targetRotation = (Main.MouseWorld - owner.Center).ToRotation(); 
                    rotation = Terraria.Utils.AngleLerp(rotation, targetRotation, .2f);
                    Projectile.velocity = new Vector2(1, 0).RotatedBy(rotation);
                    owner.direction = Math.Sign(Projectile.velocity.X);
                }                
                Projectile.ai[1]++;
                // Scream when fully charged
                if (Projectile.ai[1] > 180 || (Projectile.ai[1] > 90 && !owner.channel))
                {
                    owner.channel = false;
                    Projectile.ai[0] = 3;
                    SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/LWeapons/Danggo_Lv2") with { Volume = 0.25f }, Projectile.Center);
                    //owner.GetModPlayer<LobotomyAlephPlayer>().SmileReduceCorpse((int)(owner.statLifeMax2 * 0.05f));
                }
                else // Cancel Scream
                if (!owner.channel)
                {
                    Projectile.ai[0] = 4;
                }
                //Mouth Quivers
                if (Projectile.ai[1] > 60)
                {
                    prog = (Projectile.ai[1] - 30) / 30;
                    Projectile.localAI[0] = MathHelper.ToRadians(-10 + Main.rand.NextFloat(5 * prog));
                }
                
            }
            // Screaming
            else if (Projectile.ai[0] == 3)
            {
                int time = owner.itemAnimationMax / 5;
                Projectile.localAI[0] = Lerp(MathHelper.ToRadians(-10), MathHelper.ToRadians(45), (1f - prog) * 3);
                // Screams Periodically
                if (Main.myPlayer == Projectile.owner && owner.itemAnimation % time == 0)
                {
                    // Spawn Scream Projectiles
                    Vector2 Offset = new Vector2(110 * Projectile.scale, 0).RotatedBy(Projectile.rotation);
                    Projectile.NewProjectile(owner.GetSource_FromThis(), Projectile.Center + Offset, Vector2.Zero, ModContent.ProjectileType<SmileRScream>(), (Projectile.damage * 2 / 3), Projectile.knockBack / 2, Projectile.owner, 1, 2, 2.5f);
                    float random = Main.rand.NextFloat(1.00f);
                    for (int i = 0; i < 28; i++)
                    {
                        Vector2 vel = new Vector2(16, 0).RotatedBy(random + MathHelper.ToRadians((360 / 28f) * i));
                        int type = DustID.Wraith;
                        if (Main.rand.NextBool(4))
                            type = DustID.Blood;
                        Dust d = Dust.NewDustPerfect(Projectile.Center + Offset, type, vel);
                        if (Main.rand.NextBool(3))
                            d.noGravity = true;
                    }
                }
                float rand = MathHelper.ToRadians(10);
                rotation += Main.rand.NextFloat(-rand, rand);
                if (owner.itemAnimation == 1)
                {
                    owner.itemTime = owner.itemAnimation = owner.itemAnimationMax / 2;
                    Projectile.ai[0]++;
                }
            }
            // Cooldown
            else if (Projectile.ai[0] == 4)
            {
                if (owner.itemAnimation > owner.itemAnimationMax / 2)
                    owner.itemTime = owner.itemAnimation = owner.itemAnimationMax / 2;
            }
            Projectile.Center = LobCorpLight.LobItemLocation(owner, TextureAssets.Item[owner.HeldItem.type].Value.Frame(), rotation, Projectile.spriteDirection);
            LobCorpLight.LobItemFrame(owner, rotation, Projectile.spriteDirection);
            Projectile.rotation = rotation;
            Projectile.spriteDirection = owner.direction;
            
            if (owner.ItemAnimationEndingOrEnded)
                Projectile.Kill();
        }

        public override bool? CanHitNPC(NPC target)
        {
            Player owner = Main.player[Projectile.owner];
            if (Projectile.ai[0] > 1 || (owner.itemAnimation <= owner.itemAnimationMax / 2 && owner.itemAnimation >= owner.itemAnimationMax * 0.4f))
                return false;
            return null;
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            Vector2 Offset = new Vector2(80 * Projectile.scale, 0).RotatedBy(Projectile.rotation);
            hitbox.X += (int)Offset.X;
            hitbox.Y += (int)Offset.Y;
            base.ModifyDamageHitbox(ref hitbox);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.itemAnimation > owner.itemAnimationMax / 2)
            {
                target.immune[Projectile.owner] = owner.itemAnimation - (owner.itemAnimationMax / 2);
            }
            else if (owner.itemAnimation < owner.itemAnimationMax * 0.4f)
            {
                target.immune[Projectile.owner] = owner.itemAnimation;
            }

            if (Projectile.owner == Main.myPlayer)
            {
                if (target.boss)
                {
                    owner.GetModPlayer<LobotomyAlephPlayer>().SmileCreateCorpseBoss(target);
                }
                if (target.life <= 0)
                {
                    owner.GetModPlayer<LobotomyAlephPlayer>().SmileCreateCorpse(target);
                }
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
            Rectangle frame = tex.Frame(1, 4);
            Vector2 position = Projectile.Center - Main.screenPosition;
            Vector2 origin = new Vector2((Projectile.spriteDirection == 1 ? 0 : frame.Width), frame.Height);
            float rotation = Projectile.rotation + MathHelper.ToRadians(Projectile.spriteDirection == 1 ? 45 : 135);
            SpriteEffects spriteEffect = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (Projectile.ai[0] > 0)
            {
                frame.Y = frame.Height;
                Rectangle jawFrame = frame;
                float jawRot = Projectile.localAI[0];
                Vector2 jawLower = new Vector2(55, 45);
                Vector2 jawLowerPosition = position + new Vector2(jawLower.X, -jawFrame.Height + jawLower.Y).RotatedBy(rotation) * Projectile.scale;

                Vector2 jawUpper = new Vector2(47, 41);
                Vector2 jawUpperPosition = position + new Vector2(jawUpper.X, -jawFrame.Height + jawUpper.Y).RotatedBy(rotation) * Projectile.scale;

                if (Projectile.spriteDirection < 0)
                {
                    jawRot *= -1;
                    jawLowerPosition = position + new Vector2(-jawLower.X, -jawFrame.Height + jawLower.Y).RotatedBy(rotation) * Projectile.scale;
                    jawLower.X = jawFrame.Width - jawLower.X;
                    jawUpperPosition = position + new Vector2(-jawUpper.X, -jawFrame.Height + jawUpper.Y).RotatedBy(rotation) * Projectile.scale;
                    jawUpper.X = jawFrame.Width - jawUpper.X;
                }

                jawFrame.Y += frame.Height;
                Main.EntitySpriteDraw(tex, jawLowerPosition, jawFrame, lightColor, rotation + jawRot, jawLower, Projectile.scale, spriteEffect, 0);

                jawFrame.Y += frame.Height;
                Main.EntitySpriteDraw(tex, jawUpperPosition, jawFrame, lightColor, rotation - jawRot, jawUpper, Projectile.scale, spriteEffect, 0);
            }
            
            Main.EntitySpriteDraw(tex, position, frame, lightColor, rotation, origin, Projectile.scale, spriteEffect, 0);
            return false;
        }
    }
}
