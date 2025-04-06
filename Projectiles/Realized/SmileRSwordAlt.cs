using LobotomyCorp.Items;
using LobotomyCorp.NPCs.RedMist;
using LobotomyCorp.Players;
using LobotomyCorp.Utils;
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
    public class SmileRSwordAlt : ModProjectile
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
            int time = owner.itemAnimationMax / 9;
            
            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[0]++;
                owner.GetModPlayer<LobotomyAlephPlayer>().SmileReduceCorpse((int)(owner.statLifeMax2 * 0.01f));
                SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Language/Danggo_Lv3_Special") with { Volume = 0.25f, MaxInstances = 1, SoundLimitBehavior = SoundLimitBehavior.IgnoreNew }, Projectile.Center);
            }

            if (Main.myPlayer == Projectile.owner)
            {
                float targetRotation = (Main.MouseWorld - owner.Center).ToRotation();
                rotation = Terraria.Utils.AngleLerp(rotation, targetRotation, .2f);
                Projectile.velocity = new Vector2(1, 0).RotatedBy(rotation);
                owner.direction = Math.Sign(Projectile.velocity.X);
                Vector2 Offset = new Vector2(70 * Projectile.scale, 0).RotatedBy(Projectile.rotation);
                if (owner.itemAnimation % time == 0)
                {
                    // Spawn Scream Projectiles
                    Projectile.NewProjectile(owner.GetSource_FromThis(), Projectile.Center + Offset, Projectile.velocity * 12, ModContent.ProjectileType<SmileRVomit>(), Projectile.damage, Projectile.knockBack / 2, Projectile.owner, 1, 2, 1);
                }

                for (int i = 0; i < 4; i++)
                {
                    Vector2 vel = Projectile.velocity * Main.rand.Next(10, 12);
                    vel = vel.RotatedBy(Main.rand.NextFloat(-MathHelper.ToRadians(15), MathHelper.ToRadians(15)));
                    int id = DustID.Wraith;
                    if (Main.rand.NextBool(3))
                    {
                        id = DustID.Blood;
                        vel = Projectile.velocity * Main.rand.Next(5, 12);
                    }
                    Dust d = Dust.NewDustPerfect(Projectile.Center + Offset, id, vel);
                    d.velocity = vel;
                    //d.noGravity = gravity;
                    d.fadeIn = 1.2f;
                }
                if (Main.rand.NextBool(100))
                {
                    Vector2 vel = Projectile.velocity * Main.rand.Next(10, 12);
                    Gore.NewGorePerfect(owner.GetSource_FromThis(), Projectile.Center + Offset, vel.RotatedBy(Main.rand.NextFloat(-0.01f, 0.01f)), Main.rand.NextBool(2) ? GoreID.BloodZombieChunk : GoreID.BloodZombieChunk2);
                }
            }
            Projectile.localAI[0] = Lerp(MathHelper.ToRadians(-10), MathHelper.ToRadians(45), (1f - prog) * 5);
            if (prog < 0.1f)
            {
                Projectile.localAI[0] = Lerp(MathHelper.ToRadians(45), MathHelper.ToRadians(-10), (1f - (prog / 0.1f)));
            }

            float rand = MathHelper.ToRadians(3);
            rotation += Main.rand.NextFloat(-rand, rand);

            Projectile.Center = LobCorpLight.LobItemLocation(owner, TextureAssets.Item[owner.HeldItem.type].Value.Frame(), rotation, Projectile.spriteDirection);
            LobCorpLight.LobItemFrame(owner, rotation, Projectile.spriteDirection);
            Projectile.rotation = rotation;
            Projectile.spriteDirection = owner.direction;
            if (owner.ItemAnimationEndingOrEnded)
                Projectile.Kill();
        }

        public override bool? CanHitNPC(NPC target)
        {
            return false;
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

            Main.EntitySpriteDraw(tex, position, frame, lightColor, rotation, origin, Projectile.scale, spriteEffect, 0);
            return false;
        }
    }
}
