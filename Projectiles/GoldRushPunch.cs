﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	public class GoldRushPunch : ModProjectile
	{
        public override string Texture => "LobotomyCorp/Projectiles/GoldRushPunches";

        public override void SetStaticDefaults() {
            // DisplayName.SetDefault("ORA");
        }

		public override void SetDefaults() {
			Projectile.width = 24;
			Projectile.height = 24;
			Projectile.aiStyle = -1;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
            Projectile.timeLeft = 12;
            
			Projectile.DamageType = DamageClass.Melee;
			Projectile.tileCollide = false;
			Projectile.friendly = true;

			Projectile.localNPCHitCooldown = 8;
			Projectile.usesLocalNPCImmunity = true;

			DrawHeldProjInFrontOfHeldItemAndArms = true;
		}

        public override void AI()
        {
			if (Projectile.ai[0] == 0)
            {
				Projectile.ai[0] = Main.rand.NextFloat(1.50f);
				Projectile.ai[1] = Main.rand.Next(-10, 11);
            }
			Player projOwner = Main.player[Projectile.owner];
			Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);
			Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
			if (Projectile.timeLeft < 6)
			{
				//Projectile.ai[0] -= 0.5f;
				Projectile.alpha += 40;
			}
			else
			{
				projOwner.heldProj = Projectile.whoAmI;
				Projectile.ai[0]++;
			}

            if (Projectile.timeLeft == 4 && Main.myPlayer == Projectile.owner)
            {
                int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 2, ModContent.ProjectileType<GoldRushFlurry>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
			if (Projectile.spriteDirection < 0)
				Projectile.rotation += MathHelper.ToRadians(180);

			float rot = Projectile.velocity.ToRotation();
			Vector2 length = (Projectile.ai[0] * Projectile.velocity);

			Projectile.Center = ownerMountedCenter + new Vector2(0, Projectile.ai[1]).RotatedBy(rot) + length;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
			Vector2 position = Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Vector2 origin = new Vector2(18, 12);

			Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, position, TextureAssets.Projectile[Projectile.type].Frame(), lightColor * (1f - Projectile.alpha / 255f), Projectile.rotation + 0.785f * Projectile.spriteDirection, origin, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
			return false;
        }
    }
}
