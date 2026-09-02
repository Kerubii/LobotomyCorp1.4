using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles.RedMist
{
	class Discord2Inv : ModProjectile
	{
        protected virtual float HoldoutRangeMin => 24f;
        protected virtual float HoldoutRangeMax => 184;

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Spear); // Clone the default values for a vanilla spear. Spear specific values set for width, height, aiStyle, friendly, penetrate, tileCollide, scale, hide, ownerHitCheck, and melee.
            Projectile.hide = false;
        }

        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner]; // Since we access the owner player instance so much, it's useful to create a helper local variable for this
            int duration = player.itemAnimationMax; // Define the duration the projectile will exist in frames

            //player.heldProj = Projectile.whoAmI; // Update the player's held projectile id

            // Reset projectile time left if necessary
            if (Projectile.timeLeft > duration)
            {
                Projectile.timeLeft = duration;

                Projectile.velocity *= -1;
            }

            Projectile.velocity = Vector2.Normalize(Projectile.velocity); // Velocity isn't used in this spear implementation, but we use the field to store the spear's attack direction.

            float limit = duration / 4f;
			float halfLimit = limit / 2;
            float progress;

            // Here 'progress' is set to a value that goes from 0.0 to 1.0 and back during the item use animation.
            if (Projectile.timeLeft % limit < limit / 2)
            {
                progress = (Projectile.timeLeft % limit) / halfLimit;
            }
            else
            {
				if (Projectile.timeLeft % (int)limit == 0)
				{
                    if (Main.LocalPlayer.whoAmI == Projectile.owner)
                    {
                        Projectile.velocity = new Vector2(1, 0).RotatedBy((Main.MouseWorld - player.Center).ToRotation()) * -1;
						Projectile.netUpdate = true;
                    }
				}
				progress = (limit - Projectile.timeLeft % limit) / halfLimit;
            }

            // Move the projectile from the HoldoutRangeMin to the HoldoutRangeMax and back, using SmoothStep for easing the movement
            Projectile.Center = player.MountedCenter + Vector2.SmoothStep(Projectile.velocity * (HoldoutRangeMin + Projectile.ai[2]), Projectile.velocity * (HoldoutRangeMax + Projectile.ai[2]), 1f - progress);

            // Apply proper rotation to the sprite.
            if (Projectile.spriteDirection == -1)
            {
                // If sprite is facing left, rotate 45 degrees
                Projectile.rotation += MathHelper.ToRadians(45f);
            }
            else
            {
                // If sprite is facing right, rotate 135 degrees
                Projectile.rotation += MathHelper.ToRadians(135f);
            }
            return false; // Don't execute vanilla AI.
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 4; i++)
            {
                Dust d = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Wraith)];
                d.noGravity = true;
                d.color = Color.Black;
                d.fadeIn = 1.1f;
                d.scale = 1.2f;
                d.velocity = Projectile.velocity * Main.rand.NextFloat(1f, 2f);
            }
            base.OnHitNPC(target, hit, damageDone);
        }
    }
}
