using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static LobotomyCorp.LobotomyCorp;

namespace LobotomyCorp.Projectiles
{
	public class DaCapo : ModProjectile
	{
        public override string Texture => "LobotomyCorp/Items/Aleph/DaCapo";

        public override void SetStaticDefaults() {
            //DisplayName.SetDefault("Spear");
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
        }

		public override void SetDefaults() {
			Projectile.width = 82;
			Projectile.height = 82;
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
			Projectile.spriteDirection = projOwner.direction;
			projOwner.heldProj = Projectile.whoAmI;
			projOwner.itemTime = projOwner.itemAnimation;

            float rot = Projectile.velocity.ToRotation();

            int AnimationMax = projOwner.itemAnimationMax / 3;
            int AnimationRest = (int)(AnimationMax * 0.5f);

            if (projOwner.itemAnimation > AnimationMax * 2)
            {
                if (Projectile.ai[1] < 1)
                {
                    Projectile.ai[1]++;
                    if (Main.myPlayer == Projectile.owner)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), ownerMountedCenter, Projectile.velocity * 8f, ModContent.ProjectileType<FirstMovement>(), Projectile.damage * 4 / 5, Projectile.knockBack * 0.2f, Projectile.owner, Main.rand.NextFloat(1f, 5.5f) * projOwner.direction * -1);
                }
                float progress = ((float)projOwner.itemAnimation - ((float)AnimationMax * 2)) / ((float)AnimationMax - AnimationRest) - 0.5f;
                rot += MathHelper.ToRadians(Lerp(-90, 90, progress, Projectile.spriteDirection == 1));
            }
            else if (projOwner.itemAnimation > AnimationMax)
            {
                if (Projectile.ai[1] < 2)
                {
                    Projectile.ai[1] = 2;
                    SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/LWeapons/silent2_2") with { Volume = 0.25f }, Projectile.Center);
                    if (Main.myPlayer == Projectile.owner)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), ownerMountedCenter, Projectile.velocity * 8f, ModContent.ProjectileType<FirstMovement>(), Projectile.damage * 4 / 5, Projectile.knockBack * 0.2f, Projectile.owner, Main.rand.NextFloat(1f, 5.5f) * projOwner.direction);
                }
                float progress = ((float)projOwner.itemAnimation - ((float)AnimationMax)) / ((float)AnimationMax - AnimationRest) - 0.5f;
                rot += MathHelper.ToRadians(Lerp(90, -110, progress, Projectile.spriteDirection == 1));
                Projectile.spriteDirection *= -1;
            }
            else
            {
                if (Projectile.ai[1] < 3)
                {
                    Projectile.ai[1] = 3;
                    SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/LWeapons/silent2_3") with { Volume = 0.25f }, Projectile.Center);
                    if (Main.myPlayer == Projectile.owner)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), ownerMountedCenter, Projectile.velocity * 8f, ModContent.ProjectileType<FirstMovement>(), Projectile.damage * 4 / 5, Projectile.knockBack * 0.2f, Projectile.owner, Main.rand.NextFloat(1f, 5.5f) * projOwner.direction * -1);
                }
                float progress = ((float)projOwner.itemAnimation) / ((float)AnimationMax - AnimationRest) - 0.5f;
                rot += MathHelper.ToRadians(Lerp(-110, 120, progress, Projectile.spriteDirection == 1));
            }

            Vector2 velRot = new Vector2(1, 0).RotatedBy(rot);
            projOwner.itemRotation = (float)Math.Atan2(velRot.Y * Projectile.direction, velRot.X * Projectile.direction);
            Projectile.rotation = rot + MathHelper.ToRadians(Projectile.spriteDirection == 1 ? 45 : 135);

            Projectile.ai[0] = 20 * ( 1f - (float)projOwner.itemAnimation / (float)projOwner.itemAnimationMax);

            Projectile.Center = ownerMountedCenter + (42 + Projectile.ai[0]) * velRot;

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
            modifiers.FinalDamage *= 1.2f;
            if (projOwner.itemAnimation > projOwner.itemAnimationMax / 3)
                modifiers.Knockback *= 0.1f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Projectile.owner] = (Main.player[Projectile.owner].itemAnimation % Main.player[Projectile.owner].itemAnimationMax /3);
            if (target.immune[Projectile.owner] <= 5)
                target.immune[Projectile.owner] = 8;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player projOwner = Main.player[Projectile.owner];
            Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);
            //Dust.NewDustPerfect(ownerMountedCenter, 14, Vector2.Zero);
            Vector2 position = ownerMountedCenter - Main.screenPosition;// - new Vector2(Projectile.direction == 1 ? 8 : 16, 10) + new Vector2(10, 0).RotatedBy(Projectile.rotation);
            Vector2 originOffset = new Vector2(Projectile.ai[0], 0).RotatedBy(MathHelper.ToRadians(Projectile.direction == 1 ? 135 : 45));
            Vector2 origin = new Vector2((Projectile.spriteDirection == 1 ? 31 : 78), 78) + originOffset;
            SpriteEffects spriteEffect = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
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
}
