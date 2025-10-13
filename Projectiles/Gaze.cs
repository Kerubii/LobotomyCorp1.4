using LobotomyCorp.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	public class Gaze : ModProjectile
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

            float ThirdDuration = (int)(duration / 3f);
            float progress = 1f;

            // Here 'progress' is set to a value that goes from 0.0 to 1.0 and back during the item use animation.
            if (Projectile.timeLeft < ThirdDuration)
            {
                progress = Projectile.timeLeft / ThirdDuration;
            }
            else if (Projectile.timeLeft > ThirdDuration * 2)
            {
                progress = (duration - Projectile.timeLeft) / ThirdDuration;
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

        public override void PostDraw(Color lightColor)
        {
            Texture2D tex = Mod.Assets.Request<Texture2D>("Projectiles/GazeHead").Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, tex.Frame(), lightColor, Projectile.localAI[1], tex.Size()/2 , 1f, 0f, 0);
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (LobItemBase.RedMistMaskUpgrade(Main.player[Projectile.owner], RiskLevel.He) && Collision.CanHit(Projectile, target))
                return true;
            return base.CanHitNPC(target);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			Projectile.ai[1]++;
            target.immune[Projectile.owner] = 5;
            Player player = Main.player[Projectile.owner];
            int duration = player.itemAnimationMax;
            float ThirdDuration = (int)(duration / 3f);
            if (player.itemAnimation >= ThirdDuration && player.itemAnimation <= ThirdDuration * 2)
            {
                player.itemAnimation = (int)(ThirdDuration * 2);
                player.itemTime = player.itemAnimation;
                Projectile.timeLeft = player.itemAnimation;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
			float mult = Projectile.ai[1] / 10;
			if (mult > 1f)
				mult = 1f;
			modifiers.FinalDamage += 1f * mult;
            if (LobItemBase.RedMistMaskUpgrade(Main.player[Projectile.owner], RiskLevel.Waw) && !Projectile.getRect().Intersects(target.getRect()))
            {
                modifiers.FinalDamage -= 0.5f;
                modifiers.DisableKnockback();
            }
        }
    }
}
