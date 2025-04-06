using LobotomyCorp.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	public class MimicryS : ModProjectile
	{
		public override void SetStaticDefaults() {
            //DisplayName.SetDefault("Spear");
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
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
		}
        
		public override void AI() {
			Player projOwner = Main.player[Projectile.owner];
			Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);
			Projectile.direction = projOwner.direction;
			//projOwner.heldProj = Projectile.whoAmI;
			projOwner.itemTime = projOwner.itemAnimation;

            float rot = Projectile.velocity.ToRotation();

            int rest = 180 * Projectile.direction;
            int peak = 100 * Projectile.direction;
            int between = rest + peak;

            Projectile.direction = Math.Sign(Projectile.velocity.X);

            if (projOwner.itemAnimation > 22)
            {
                float windUp = projOwner.itemAnimationMax - 20;
                float anim = projOwner.itemAnimation - 20;
                rot += (MathHelper.ToRadians(rest) - (MathHelper.ToRadians(between) * (1 - (anim / windUp))));
                if (Projectile.ai[0] < 25)
                    Projectile.ai[0] += 1.56f;
            }
            else if (projOwner.itemAnimation <= 6)
            {
                rot += MathHelper.ToRadians(rest);
            }
            else if (projOwner.itemAnimation <= 19)
            {
                float Slash = 19 - 6;
                float anim = projOwner.itemAnimation - 6;
                rot -= (MathHelper.ToRadians(peak) - (MathHelper.ToRadians(between) * (1 - (anim / Slash))));
                if (projOwner.itemAnimation <= 14 && Projectile.ai[0] > 0)
                    Projectile.ai[0] -= 4.8f;
                else
                    Projectile.ai[0] += 7.23f;
            }
            else
            {
                rot -= MathHelper.ToRadians(peak);
                for (int i = 0; i < 6; i++)
                {
                    Projectile.oldRot[i] = -1000;//rot + MathHelper.ToRadians(Projectile.direction == 1 ? 45 : 135);
                }
            }

            Vector2 velRot = new Vector2(1, 0).RotatedBy(rot);
            projOwner.itemRotation = (float)Math.Atan2(velRot.Y * Projectile.direction, velRot.X * Projectile.direction);
            Projectile.rotation = rot + MathHelper.ToRadians(Projectile.direction == 1 ? 45 : 135);

            Projectile.position = ownerMountedCenter;

            if (projOwner.itemAnimation == 1)
                Projectile.Kill();
		}

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            Player projOwner = Main.player[Projectile.owner];
            //Sweetspot
            if (projOwner.itemAnimation <= 19 && projOwner.itemAnimation > 6)
            {
                Vector2 Center = new Vector2(84, 0).RotatedBy(Projectile.velocity.ToRotation());
                
                if (projOwner.itemAnimation <= 11)
                    Center = new Vector2(48, 0).RotatedBy(Projectile.velocity.ToRotation() + MathHelper.ToRadians(160) * Projectile.direction);
                else if (projOwner.itemAnimation <= 15)
                    Center = new Vector2(55, 0).RotatedBy(Projectile.velocity.ToRotation() + MathHelper.ToRadians(90) * Projectile.direction);
                
                Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true) + Center;
                hitbox = new Rectangle((int)ownerMountedCenter.X - 40, (int)ownerMountedCenter.Y - 40, 80, 80);
            }
            if ( projOwner.itemAnimation > 22 )
            {
                Vector2 Center = new Vector2(55, 0).RotatedBy(Projectile.velocity.ToRotation());

                if (projOwner.itemAnimation > 28)
                    Center = new Vector2(55, 0).RotatedBy(Projectile.velocity.ToRotation() + MathHelper.ToRadians(90) * Projectile.direction);

                Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true) + Center;
                hitbox = new Rectangle((int)ownerMountedCenter.X - 40, (int)ownerMountedCenter.Y - 40, 80, 80);
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Player projOwner = Main.player[Projectile.owner];
            if (projOwner.itemAnimation <= 19 && projOwner.itemAnimation > 15)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<MimicrySEffect>(), 0, 0, Projectile.owner, Projectile.direction);
                modifiers.FinalDamage *= 3;
                target.immune[Projectile.owner] = projOwner.itemAnimation;
            }
            else if (projOwner.itemAnimation > 22)
            {
                modifiers.Knockback.Base = 0.2f;
                target.immune[Projectile.owner] = projOwner.itemAnimation - 20;
            }
        }

        public Vector2[] trailPos = new Vector2[6];

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 position = Projectile.Center - Main.screenPosition;
            position = Projectile.Center - Main.screenPosition - new Vector2(Projectile.direction == 1 ? 8 : 16, 10) + new Vector2(10, 0).RotatedBy(Projectile.rotation);
            Vector2 originOffset = new Vector2(Projectile.ai[0], 0).RotatedBy(MathHelper.ToRadians(Projectile.direction == 1 ? 135 : 45));
            Vector2 origin = new Vector2(56 + (Projectile.direction == 1 ? 0 : 35), 60) + originOffset;
            SpriteEffects spriteEffect = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (Main.player[Projectile.owner].itemAnimation <= 19)
            {
                /*for (int i = 0; i < 10; i++)
                {
                    position = Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition - new Vector2(Projectile.direction == 1 ? 8 : 16, 10) + new Vector2(10, 0).RotatedBy(Projectile.rotation);
                    Texture2D tex = Mod.Assets.Request<Texture2D>("Projectiles/MimicrySBlur").Value;
                    Color color = lightColor;
                    //color.A = (byte)(color.A * 0.15f);
                    color *= (1 - ((float)i / 6f)) * 0.4f;

                    Main.EntitySpriteDraw(tex, position, new Microsoft.Xna.Framework.Rectangle?
                                        (
                                            new Rectangle
                                            (
                                                0, 0, TextureAssets.Projectile[Projectile.type].Width(), TextureAssets.Projectile[Projectile.type].Height()
                                            )
                                        ),
                    color, Projectile.oldRot[i], origin, Projectile.scale, spriteEffect, 0);
                }*/
                SlashTrail trail = new SlashTrail(24, 0.785f - MathHelper.ToRadians(Projectile.direction == 1 ? 0 : 90));
                Vector2[] trailPos = new Vector2[Projectile.oldPos.Length];
                for (int i = 0; i < trailPos.Length; i++)
                {
                    if (Projectile.oldPos[i].Length() > 0)
                        trailPos[i] = Projectile.position + new Vector2(44 + Projectile.ai[0], 0).RotatedBy(Projectile.oldRot[i] - 0.785f - MathHelper.ToRadians(Projectile.direction == 1 ? 0 : 90));
                    if (Projectile.oldRot[i] <= -1000)
                        trailPos[i] = Vector2.Zero;
                }

                Player projOwner = Main.player[Projectile.owner];
                float prog = 1f - (float) projOwner.itemAnimation / 19f;
                CustomShaderData mimicry = LobotomyCorp.LobcorpShaders["MimicrySlash"].UseOpacity(prog);

                trail.DrawSpecific(trailPos, Projectile.oldRot, Vector2.Zero, mimicry);
            }                     

            position = Projectile.Center - Main.screenPosition - new Vector2(Projectile.direction == 1 ? 8 : 16, 10) + new Vector2(10, 0).RotatedBy(Projectile.rotation);
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, position, new Microsoft.Xna.Framework.Rectangle?
                                    (
                                        new Rectangle
                                        (
                                            0, 0, TextureAssets.Projectile[Projectile.type].Width(), TextureAssets.Projectile[Projectile.type].Height()
                                        )
                                    ),
                lightColor * ((float)(255 - Projectile.alpha) / 255f), Projectile.rotation, origin, Projectile.scale, spriteEffect, 0);
            return false;
        }
    }

    class MimicrySEffect : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.alpha = 255;
            Projectile.timeLeft = 15;
            Projectile.tileCollide = false;
            Projectile.friendly = true;

        }

        public override void AI()
        {
            for (int i = 0; i < 3; i++)
            {
                Vector2 velocity = new Vector2(0, Main.rand.NextFloat(-8.00f, 8.00f)).RotatedBy(MathHelper.ToRadians(45 * Projectile.ai[0]));
                Dust dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 5, velocity.X, velocity.Y)];
                dust.fadeIn = Main.rand.NextFloat(1f, 2f);
                dust.noGravity = true;
            }
            float n = (Projectile.ai[1] - 7) * 4;
            Vector2 offset = new Vector2(0, n).RotatedBy(MathHelper.ToRadians(45) * Projectile.ai[0]);
            Vector2 pos = Projectile.Center + offset;
            Dust dust2 = Dust.NewDustPerfect(pos, 87, Vector2.Zero, 0, default(Color), 1.2f *(Projectile.ai[1] / 15f));
            dust2.noGravity = true;
            dust2.fadeIn = 0.2f + 1.2f * (Projectile.ai[1] / 15f);
            Projectile.ai[1]++;
        }
    }

    class MimicrySHello : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.extraUpdates = 60;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    Vector2 dustVel = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.47f, 0.47f));
                    Dust dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 5, dustVel.X, dustVel.Y)];
                    dust.fadeIn = 1.4f;
                }
                Projectile.ai[0]++;
            }
            if (Main.rand.Next(10) == 0)
            {
                Dust dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 5, Projectile.velocity.X, Projectile.velocity.Y)];
                dust.fadeIn = 1.4f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                Vector2 splosh = new Vector2(Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f));
                Dust dust2 = Dust.NewDustPerfect(Projectile.Center, 133, splosh, 0, default(Color), 0.1f);
                dust2.fadeIn = Main.rand.NextFloat(0.5f, 1.2f);
            }
            for (int i = 0; i < 10; i++)
            {
                Vector2 splosh = new Vector2(Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f));
                Dust dust2 = Dust.NewDustPerfect(Projectile.Center, 5, splosh, 0, default(Color), 1f);
                dust2.fadeIn = Main.rand.NextFloat(1f, 1.6f);
            }
        }
    }
}
