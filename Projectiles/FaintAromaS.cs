using LobotomyCorp.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static LobotomyCorp.LobotomyCorp;

namespace LobotomyCorp.Projectiles
{
	public class FaintAromaS : ModProjectile
	{
		public override void SetStaticDefaults() {
            //DisplayName.SetDefault("Spear");
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
        }

		public override void SetDefaults() {
			Projectile.width = 26;
			Projectile.height = 26;
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
            LobotomyWawPlayer modPlayer = projOwner.GetModPlayer<LobotomyWawPlayer>();
            Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);
			Projectile.direction = projOwner.direction;
			projOwner.heldProj = Projectile.whoAmI;
			projOwner.itemTime = projOwner.itemAnimation;
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[1] = projOwner.direction > 0 ? MathHelper.ToRadians(165) : MathHelper.ToRadians(15);

                if (projOwner.itemAnimation >= projOwner.itemAnimationMax / 2)
                {
                    float progress = (float)(projOwner.itemAnimation - projOwner.itemAnimationMax / 2) / (float)(projOwner.itemAnimationMax - projOwner.itemAnimationMax / 2);
                    Projectile.rotation += Lerp(-MathHelper.ToRadians(90), MathHelper.ToRadians(60), progress) * projOwner.direction;
                    for (int i = 0; i < 3; i++)
                    {
                        Dust dust;
                        dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 205, Projectile.velocity.X, Projectile.velocity.Y, 0, new Color(255, 255, 255), 1f)];
                        dust.velocity = Projectile.velocity * 4f;
                        dust.noGravity = true;
                        dust.fadeIn = 1.5f * (1f - progress);
                    }

                    projOwner.velocity = Projectile.velocity * 12f;
                }
                else
                {
                    if (projOwner.itemAnimation == (projOwner.itemAnimationMax / 2) - 1)
                        projOwner.velocity *= 0.2f;
                    Projectile.rotation += -MathHelper.ToRadians(90) * projOwner.direction;
                }
            }
            else if (Projectile.ai[0] == 1)
            {
                Projectile.ai[1] = Projectile.rotation;
                Projectile.rotation = projOwner.direction > 0 ? MathHelper.ToRadians(135) : MathHelper.ToRadians(45);
                if (projOwner.itemAnimation >= projOwner.itemAnimationMax / 2)
                {
                    Vector2 DustPos = ownerMountedCenter + new Vector2(64, 0).RotatedBy(Projectile.ai[1]) - Projectile.Size / 2;
                    for (int i = 0; i < 3; i++)
                    {
                        Dust dust;
                        dust = Main.dust[Dust.NewDust(DustPos, Projectile.width, Projectile.height, 205, Projectile.velocity.X, Projectile.velocity.Y, 0, new Color(255, 255, 255), 1f)];
                        dust.velocity = Projectile.velocity * 4f;
                        dust.noGravity = true;
                        dust.fadeIn = 1.2f;
                    }

                    projOwner.velocity = Projectile.velocity * 16f;
                }
                else
                {
                    if (projOwner.itemAnimation == (projOwner.itemAnimationMax / 2) - 1)
                        projOwner.velocity *= 0.2f;
                }
            }
            else if (Projectile.ai[0] >= 2)
            {
                if (projOwner.itemAnimation >= projOwner.itemAnimationMax / 2)
                {
                    float progress = (float)(projOwner.itemAnimation - projOwner.itemAnimationMax / 2) / (float)(projOwner.itemAnimationMax - projOwner.itemAnimationMax / 2);
                    Projectile.ai[1] = Projectile.rotation - MathHelper.ToRadians(120) * (1f - progress) * projOwner.direction;
                    Projectile.rotation += MathHelper.ToRadians(220) * (1f - progress) * projOwner.direction;

                    Vector2 DustPos = ownerMountedCenter + new Vector2(64, 0).RotatedBy(Projectile.ai[1]) - Projectile.Size/2;
                    for (int i = 0; i < 3; i++)
                    {
                        Dust dust;
                        dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 205, Projectile.velocity.X, Projectile.velocity.Y, 0, new Color(255, 255, 255), 1f)];
                        dust.velocity = Projectile.velocity * 4f;
                        dust.noGravity = true;
                        dust.fadeIn = 1.2f;
                        
                        dust = Main.dust[Dust.NewDust(DustPos, Projectile.width, Projectile.height, 205, Projectile.velocity.X, Projectile.velocity.Y, 0, new Color(255, 255, 255), 1f)];
                        dust.velocity = Projectile.velocity * 4f;
                        dust.noGravity = true;
                        dust.fadeIn = 1.2f;
                    }
                }
                else
                {
                    Projectile.rotation += MathHelper.ToRadians(220) * projOwner.direction;
                }
            }

            Projectile.Center = ownerMountedCenter + new Vector2(64, 0).RotatedBy(Projectile.rotation);
            projOwner.itemRotation = (float)Math.Atan2((float)Math.Sin(Projectile.rotation) * projOwner.direction, (float)Math.Cos(Projectile.rotation) * projOwner.direction);

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
            if (Projectile.ai[0] == 1 || (Projectile.ai[0] == 2 && projOwner.itemAnimation % 2 == 1))
            {
                Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);
                Vector2 location = ownerMountedCenter + new Vector2(64, 0).RotatedBy(Projectile.ai[1]);
                hitbox = new Rectangle((int)location.X - Projectile.width / 2, (int)location.Y - Projectile.height / 2, Projectile.width, Projectile.height);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player projOwner = Main.player[Projectile.owner];
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            float rot = Projectile.rotation;
            Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true) + new Vector2(8, 0).RotatedBy(rot);
            Vector2 position = ownerMountedCenter - Main.screenPosition;
            Vector2 origin = new Vector2(2, 42);

            Main.EntitySpriteDraw(tex, position, tex.Frame(), lightColor, rot + MathHelper.ToRadians(45), origin, Projectile.scale * 1.2f, 0, 0);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            LobotomyWawPlayer modPlayer = player.GetModPlayer<LobotomyWawPlayer>();
            if (Projectile.ai[0] >= 2 && player.ownedProjectileCounts[ModContent.ProjectileType<AlriuneDeathAnimation>()] == 0)
            {
                modPlayer.FaintAromaPetal = 0;
                for (int i = 0; i < 32; i++)
                {
                    Dust dust;
                    Vector2 dustVel = new Vector2(8f, 0f).RotatedBy(MathHelper.ToRadians(11.25f * i));
                    dust = Main.dust[Dust.NewDust(player.Center, 1, 1, 205, dustVel.X, dustVel.Y, 0, new Color(255, 255, 255), 1f)];
                    //dust.velocity = Projectile.velocity * 4f;
                    dust.noGravity = true;
                    dust.fadeIn = 1.2f;
                }
                foreach (NPC n in Main.npc)
                {
                    if (n.active && n.chaseable && n.CanBeChasedBy(ModContent.ProjectileType<AlriuneDeathAnimation>()) && (n.Center - player.Center).Length() < 800 && Main.myPlayer == Projectile.owner)
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), n.Center, Vector2.Zero, ModContent.ProjectileType<AlriuneDeathAnimation>(), (int)(Projectile.damage * 3f), 0, player.whoAmI, n.whoAmI);
                }
            }
            target.immune[Projectile.owner] = player.itemAnimation;
            modPlayer.FaintAromaPetal += 30f;
            if (modPlayer.FaintAromaPetal > modPlayer.FaintAromaPetalMax * 3 + 30)
                modPlayer.FaintAromaPetal = modPlayer.FaintAromaPetalMax * 3 + 30;
        }
    }
}
