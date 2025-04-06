using LobotomyCorp.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{

    public class RealizedHornet : ModProjectile
    {
        public override void SetStaticDefaults() {
            //DisplayName.SetDefault("Spear");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

		public override void SetDefaults() {
			Projectile.width = 18;
			Projectile.height = 18;
			Projectile.aiStyle = -1;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
			Projectile.alpha = 0;
            Projectile.timeLeft = 60;

			//Projectile.hide = true;
			Projectile.ownerHitCheck = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.tileCollide = false;
			Projectile.friendly = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;

		}
        
		public override void AI() {
			Player projOwner = Main.player[Projectile.owner];
			Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);
            projOwner.direction = Math.Sign(Projectile.velocity.X);
			Projectile.direction = projOwner.direction;
			projOwner.heldProj = Projectile.whoAmI;

            float positionOffset = 1.2f;
            projOwner.itemTime = projOwner.itemAnimation;
            projOwner.itemAnimation = projOwner.itemAnimationMax;
            Vector2 positionShake = new Vector2(0, 0);
            if (projOwner.HeldItem.type == ModContent.ItemType<Items.Ruina.History.HornetR>() && projOwner.channel)
            {
                Projectile.ai[0] += 0.26f;
                if (Projectile.ai[0] > 16)
                {
                    Projectile.ai[0] = 16;

                    positionShake.X = Main.rand.NextFloat(-1f, 1f);
                    positionShake.Y = Main.rand.NextFloat(-1f, 1f);
                }
                Projectile.timeLeft = 60;
                positionOffset = 1.2f - Projectile.ai[0] / 16f * 0.4f;

                for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
                {
                    Projectile.oldPos[i] = Vector2.Zero;
                }
            }

            Projectile.position.X = ownerMountedCenter.X - (float)(Projectile.width / 2);
			Projectile.position.Y = ownerMountedCenter.Y - (float)(Projectile.height / 2);
			Projectile.position += Projectile.velocity * positionOffset + positionShake;
			
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(135f);

            if (Projectile.spriteDirection == -1)
            {
                Projectile.rotation -= MathHelper.ToRadians(90);
            }

            if (projOwner.channel)
                return;

            if (Projectile.ai[1] == 0)
            {
                SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/QueenBee_Queen_Stab") with { Volume = 0.25f }, projOwner.Center);
            }

            Projectile.ai[1]++;

            //Main.NewText(Projectile.ai[1]);


            if (Projectile.ai[1] > Projectile.ai[0] + 30)
                Projectile.Kill();

            if (Projectile.ai[1] > (int)Projectile.ai[0])
                return;

            if (Projectile.ai[1] == 8 && Projectile.ai[0] >= 12)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        float x = -20 * i;
                        Vector2 offset = new Vector2(x, 0).RotatedBy(Projectile.velocity.ToRotation());
                        Vector2 vel = Projectile.velocity;
                        vel.Normalize();
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + offset, -vel * (6 + 0.4f * i), ModContent.ProjectileType<HornetEffect>(), 0, 0, Projectile.owner, Main.rand.Next(10), i % 2 == 0 ? -1 : 1);
                    }
                }
            }

            if (Projectile.ai[0] >= 8)
            {
                if (Projectile.ai[1] % 3 == 0)
                {
                    Vector2 dustPos = Projectile.position + new Vector2(Main.rand.NextFloat((float)Projectile.width), Main.rand.NextFloat((float)Projectile.height));
                    for (int i = 0; i < 5; i++)
                    {
                        Vector2 offset = new Vector2(60 + 4 * i, 0).RotatedBy(Projectile.velocity.ToRotation() + 3.14f);
                        Dust d = Dust.NewDustPerfect(dustPos + offset, 87, Projectile.velocity);
                        d.velocity.Normalize();
                        d.velocity *= -14f;
                        d.fadeIn = 1.5f;
                        d.noGravity = true;
                    }
                }

                if (Projectile.ai[1] > 9 && Projectile.ai[1] % 4 == 0)
                {
                    Vector2 vel = Vector2.Normalize(Projectile.velocity) * 4f;
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, -vel, ModContent.ProjectileType<HornetEffectCircle>(), 0, 0, Projectile.owner);
                }
            }

            if (Main.rand.Next(3) == 0)
            {
                int d= Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Misc.Dusts.HornetDust>());
                Main.dust[d].noLight = false;
                Main.dust[d].fadeIn = 1.8f;
            }

            projOwner.velocity = 28f * Vector2.Normalize(Projectile.velocity);
            projOwner.immune = true;
            projOwner.immuneTime = 10;


            if (Projectile.ai[1] == (int)Projectile.ai[0])
            {
                Main.player[Projectile.owner].velocity *= 0.6f;

                for (int i = 0; i < 5; i++)
                {
                    int d = Dust.NewDust(Projectile.position, Projectile.width, projOwner.height, 87);
                    Dust dust = Main.dust[d];
                    dust.velocity = Vector2.Normalize(Projectile.velocity);
                    dust.velocity *= 20f;
                    dust.fadeIn = 1.5f;
                    dust.noGravity = true;
                }

                //Vector2 vel = Vector2.Normalize(Projectile.velocity) * 4f;
                //Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, -vel, ModContent.ProjectileType<HornetEffectCircle>(), 0, 0, Projectile.owner);
            }
        }

        public override void OnKill(int timeLeft)
        {
            Main.player[Projectile.owner].itemAnimation = 30;
            Main.player[Projectile.owner].itemTime = 30;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (!Main.player[Projectile.owner].channel && Projectile.ai[1] <= Projectile.ai[0])
                return base.CanHitNPC(target);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Buffs.BeeSpore>(), 120 + (int)(240 * (Projectile.ai[1] / Projectile.ai[0])));
            if (target.life <= 0)
            {
                LobotomyGlobalNPC.SpawnHornet(target, Projectile.owner, hit.Damage, hit.Knockback);
            }
            if (Projectile.ai[0] < 10)
                return;
            /*
            int dustAmount = 16;
            for (int i = 0; i < dustAmount;i++)
            {
                Vector2 dustPos = new Vector2(
                    (float)Math.Cos((float)i / dustAmount * 6.48f) * 5f,
                    (float)Math.Sin((float)i / dustAmount * 6.48f) * 12f);

                dustPos = dustPos.RotatedBy(Projectile.velocity.ToRotation());
                Dust d = Dust.NewDustPerfect(Projectile.Center + dustPos, 87, dustPos * 0.4f);
                d.noGravity = true;
                d.fadeIn = 1.3f;

                if (Main.rand.Next(4) == 0)
                {
                    int n = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, 87);
                    d = Main.dust[n];
                    d.velocity = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(20));
                    d.velocity.Normalize();
                    d.velocity *= Main.rand.Next(4);
                    d.noGravity = true;
                }
            }*/
            Vector2 vel = Vector2.Normalize(Projectile.velocity) * 4f;
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, -vel, ModContent.ProjectileType<HornetEffectCircle>(), 0, 0, Projectile.owner);
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 position = Projectile.Center - Main.screenPosition;
            /*
            if (!Main.player[Projectile.owner].channel)
            {
                for (int i = 0; i < 4; i++)
                {
                    position = Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition;
                    Color color = lightColor;
                    //color.A = (byte)(color.A * 0.15f);
                    color *= (4 - i) / 4f;

                    Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, position, new Microsoft.Xna.Framework.Rectangle?
                                        (
                                            new Rectangle
                                            (
                                                0, 0, TextureAssets.Projectile[Projectile.type].Width(), TextureAssets.Projectile[Projectile.type].Height()
                                            )
                                        ),
                    color, Projectile.rotation, Vector2.Zero, Projectile.scale, SpriteEffects.None, 0);
                }

            }*/
            position = Projectile.Center - Main.screenPosition;
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, position, new Microsoft.Xna.Framework.Rectangle?
                                    (
                                        new Rectangle
                                        (
                                            0, 0, TextureAssets.Projectile[Projectile.type].Width(), TextureAssets.Projectile[Projectile.type].Height()
                                        )
                                    ),
                lightColor * ((float)(255 - Projectile.alpha) / 255f), Projectile.rotation, Vector2.Zero, Projectile.scale, SpriteEffects.None, 0);

            if (!Main.player[Projectile.owner].channel && Projectile.ai[0] >= 12 && Projectile.ai[1] <= Projectile.ai[0] + 10)
            {
                SlashTrail trail = new SlashTrail(24, MathHelper.ToRadians(45));//, 0.785f - MathHelper.ToRadians(Projectile.direction == 1 ? 0 : 90));
                trail.color = new Color(255,255,89);

                Player projOwner = Main.player[Projectile.owner];
                float prog = 1f - (float)Projectile.ai[1] / (Projectile.ai[0] + 10);
                if (prog < 0)
                    prog = 0;
                CustomShaderData windTrail = LobotomyCorp.LobcorpShaders["WindTrail"].UseOpacity(prog);

                trail.DrawTrail(Projectile, windTrail);
            }
            return false;
        }
    }

    public class HornetEffect : ModProjectile
    {
        public override string Texture => "LobotomyCorp/Projectiles/RealizedHornet";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Spear");
            //ProjectileID.Sets.TrailCacheLength[Projectile.type] = 32;
            //ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.alpha = 255;
            Projectile.timeLeft = 60;
            //Projectile.hide = true;

            //Projectile.hide = true;
            Projectile.ownerHitCheck = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.friendly = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public Trailhelper trailhelp;

        public override void AI()
        {
            if (trailhelp == null)
            {
                trailhelp = new Trailhelper(12);

                for (int i = 0; i < 12; i++)
                {
                    float startDdistance = (80 - 32 * (Projectile.timeLeft / 60f)) * (float)Math.Sin((Projectile.ai[0] - i) * 0.174f) * Projectile.ai[1];
                    Vector2 NewPos = Projectile.Center - Projectile.velocity * -i + new Vector2(0, startDdistance).RotatedBy(Projectile.rotation);

                    trailhelp.TrailPos[i] = NewPos;
                    trailhelp.TrailRotation[i] = Projectile.velocity.ToRotation();
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation();// - 1.57f;
            Projectile.ai[0]++;
            float distance = (80 - 32 * (Projectile.timeLeft / 60f)) * (float)Math.Sin(Projectile.ai[0] * 0.174f) * Projectile.ai[1];
            Vector2 newPos = Projectile.Center + new Vector2(0, distance).RotatedBy(Projectile.rotation);
            
            if (trailhelp != null)
                trailhelp.TrailUpdate(newPos, Projectile.rotation);

            if (Main.rand.Next(3) == 0)
            {
                Dust d = Dust.NewDustPerfect(newPos, 87, Projectile.velocity);
                d.velocity.Normalize();
                d.velocity *= 4f;
                d.noGravity = true;
            }
        }

        /*public override bool ShouldUpdatePosition()
        {
            return false;
        }*/

        public override bool PreDraw(ref Color lightColor)
        {
            SlashTrail trail = new SlashTrail(10, 1.57f);// MathHelper.ToRadians(45));
            trail.color = new Color(255,255,89);

            Player projOwner = Main.player[Projectile.owner];
            float prog = Projectile.timeLeft / 60f;
            if (prog < 0)
                prog = 0;
            CustomShaderData windTrail = LobotomyCorp.LobcorpShaders["WindTrail"].UseOpacity(prog * 0.6f);
            if (trail != null && trailhelp != null)
                trail.DrawSpecific(trailhelp.TrailPos, trailhelp.TrailRotation, Vector2.Zero, windTrail);

            return false;
        }
    }

    public class HornetEffectCircle : ModProjectile
    {
        public override string Texture => "LobotomyCorp/Projectiles/RealizedHornet";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Spear");
            //ProjectileID.Sets.TrailCacheLength[Projectile.type] = 32;
            //ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.alpha = 255;
            Projectile.timeLeft = 30;

            //Projectile.hide = true;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            Projectile.direction = Math.Sign(Projectile.velocity.X);
            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[0] = 30;
                Projectile.ai[1] = 16;
                Projectile.localAI[0] = Main.rand.NextFloat(1.00f);
            }
            else
            {
                Projectile.ai[0] += 2;//Circle Radius
                Projectile.ai[1]++;
            }

            Projectile.rotation = (-Projectile.velocity).ToRotation();
        }

        /*public override bool ShouldUpdatePosition()
        {
            return false;
        }*/

        public override bool PreDraw(ref Color lightColor)
        {
            SlashTrail trail = new SlashTrail(10, 1.57f);// MathHelper.ToRadians(45));
            trail.color = new Color(255, 255, 89);

            Player projOwner = Main.player[Projectile.owner];
            float prog = Projectile.timeLeft / 30f;
            if (prog < 0)
                prog = 0;
            CustomShaderData windTrail = LobotomyCorp.LobcorpShaders["WindTrail"].UseOpacity(prog * 0.6f);
            Vector2[] firstPos = new Vector2[32];
            Vector2[] secondPos = new Vector2[32];
            for (int i = 0; i < 32; i++)
            {
                float angle = MathHelper.ToRadians(90f - (360f/32f) * i * Projectile.direction);
                float radius = Projectile.ai[0];
                firstPos[i] = Projectile.Center + new Vector2(radius * 0.25f * (float)Math.Cos(angle), radius * (float)Math.Sin(angle)).RotatedBy(Projectile.rotation) - Main.screenPosition;
                float increase = 1f + .1f * prog;
                float distance = Projectile.ai[1];
                secondPos[i] = Projectile.Center + new Vector2(radius * 0.25f * (float)Math.Cos(angle) - distance, radius * increase * (float)Math.Sin(angle)).RotatedBy(Projectile.rotation) - Main.screenPosition;
            }

            trail.DrawManual(firstPos, secondPos, windTrail, Projectile.localAI[0] + 1f * prog * Projectile.spriteDirection);

            return false;
        }
    }
}
