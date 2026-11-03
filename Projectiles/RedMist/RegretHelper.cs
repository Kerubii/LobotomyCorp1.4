using LobotomyCorp.Misc;
using LobotomyCorp.ModSystems;
using LobotomyCorp.Projectiles.Realized;
using LobotomyCorp.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles.RedMist
{
	public class RegretHelper : ModProjectile
	{
        public override string Texture => "LobotomyCorp/Items/Teth/Regret";

        public override void SetStaticDefaults() {
            // DisplayName.SetDefault("Wingbeat");
        }

		public override void SetDefaults() {
			Projectile.width = 60;
			Projectile.height = 60;
			Projectile.aiStyle = -1;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
            Projectile.timeLeft = 600;

			Projectile.ownerHitCheck = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.friendly = true;
		}

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.dead)
                Projectile.Kill();

            owner.direction = Projectile.velocity.X > 0 ? 1 : -1;
            Projectile.spriteDirection = owner.direction;
            if (Projectile.timeLeft > owner.itemAnimationMax)
            {
                Projectile.timeLeft = owner.itemAnimationMax;
                Projectile.ai[0] = owner.itemAnimationMax;
                Projectile.ai[1] = owner.altFunctionUse;
            }

            Vector2 targetPos = owner.Center;

            if (Projectile.ai[1] != 2)
            {
                float half = Projectile.ai[0] / 2;
                if (Projectile.timeLeft == half)
                {
                    if (Main.myPlayer == Projectile.owner)
                        Projectile.velocity = owner.Center.DirectionTo(Main.MouseWorld);
                    Projectile.rotation = Projectile.velocity.ToRotation();

                    SoundEngine.PlaySound(LobotomyCorp.WeaponSounds.Hammer, owner.Center);
                }
                
                if (Projectile.timeLeft <= half)
                {
                    float time = (1f - ((Projectile.timeLeft) / half));
                    targetPos += Projectile.velocity * (Easing.EaseOutExpo(time) * 82);
                }                
            }
            else
            {
                int ImpactDist = 74 * owner.direction;
                int hitTimes = (int)(Projectile.ai[0] * 0.15f);
                if ((Projectile.ai[0] - Projectile.timeLeft == hitTimes) ||
                    (Projectile.ai[0] - Projectile.timeLeft == hitTimes * 2) ||
                    (Projectile.ai[0] - Projectile.timeLeft == hitTimes * 3) ||
                    (Projectile.ai[0] - Projectile.timeLeft == hitTimes * 6))
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), targetPos + new Vector2(ImpactDist, 0), Vector2.Zero, ModContent.ProjectileType<RegretShock>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Main.rand.NextFloat(6.28f));
                }
            }

            Projectile.Center = targetPos;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.ai[1] != 2 && Projectile.timeLeft < Projectile.ai[0] / 2)
                return null;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[1] == 2 || (Projectile.timeLeft > Projectile.ai[0] / 2))
                return false;

            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            float rotation = Projectile.rotation + MathHelper.ToRadians(Projectile.spriteDirection > 0 ? 45 : 135);
            Vector2 origin = new Vector2(Projectile.spriteDirection > 0 ? 54 : tex.Width - 54, 16);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, null, lightColor, rotation, origin, Projectile.scale, Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
            return false;
        }
    }
}
