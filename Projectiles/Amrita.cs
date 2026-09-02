using LobotomyCorp.Items;
using LobotomyCorp.Items.Accessories;
using LobotomyCorp.Projectiles.RedMist;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	public class Amrita : ModProjectile
	{
        protected virtual float HoldoutRangeMin => 24f;
        protected virtual float HoldoutRangeMax => 112f;

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Spear); // Clone the default values for a vanilla spear. Spear specific values set for width, height, aiStyle, friendly, penetrate, tileCollide, scale, hide, ownerHitCheck, and melee.
            Projectile.width = 26;
            Projectile.height = 26;
        }

        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner]; // Since we access the owner player instance so much, it's useful to create a helper local variable for this
            int duration = player.itemAnimationMax; // Define the duration the projectile will exist in frames

            player.heldProj = Projectile.whoAmI; // Update the player's held projectile id

            // Reset projectile time left if necessary
            if (Projectile.timeLeft > duration)
            {
                Projectile.timeLeft = duration;
            }

            Projectile.velocity = Vector2.Normalize(Projectile.velocity); // Velocity isn't used in this spear implementation, but we use the field to store the spear's attack direction.

            float QuarterDuration = duration / 4;
            float progress = 1f;

            // Here 'progress' is set to a value that goes from 0.0 to 1.0 and back during the item use animation.
            if (Projectile.timeLeft < QuarterDuration)
            {
                progress = Projectile.timeLeft / QuarterDuration;
            }
            else if (Projectile.timeLeft > QuarterDuration * 3)
            {
                progress = (duration - Projectile.timeLeft) / QuarterDuration;
            }

            if (Projectile.timeLeft == QuarterDuration)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 velNorm = Vector2.Normalize(Projectile.velocity);
                    Vector2 projPos = Projectile.Center - velNorm * 22;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), projPos, velNorm * 5, ModContent.ProjectileType<AmritaLight>(), Projectile.damage * 2 / 3, Projectile.knockBack, Projectile.owner);

                    if (LobItemBase.RedMistMaskUpgrade(Main.LocalPlayer, RiskLevel.Waw))
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), projPos, velNorm * 5, ModContent.ProjectileType<AmritaSarira>(), Projectile.damage * 2 / 3, Projectile.knockBack, Projectile.owner);
                    }
                }
            }

            // Move the projectile from the HoldoutRangeMin to the HoldoutRangeMax and back, using SmoothStep for easing the movement
            Projectile.Center = player.MountedCenter + Vector2.SmoothStep(Projectile.velocity * HoldoutRangeMin, Projectile.velocity * HoldoutRangeMax, progress);

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

            //Head rotation
            Projectile.localAI[1] += MathHelper.ToRadians(24) * player.direction;
            return false; // Don't execute vanilla AI.
        }
    }
}
