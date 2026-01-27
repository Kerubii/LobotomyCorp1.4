using LobotomyCorp.Misc.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Humanizer.In;

namespace LobotomyCorp.Projectiles
{
	public class GrinderMk4 : ModProjectile
	{
        public static Asset<Texture2D> SawHead;

        public override void Load()
        {
            SawHead = ModContent.Request<Texture2D>(Texture + "Head");
        }

        protected virtual float HoldoutRangeMin => 24f;
        protected virtual float HoldoutRangeMax => 96f;

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
            Texture2D tex = SawHead.Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, tex.Frame(), lightColor, Projectile.localAI[1], tex.Size()/2 , 1f, 0f, 0);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Projectile.owner] = 5;
            int num208 = Realized.GrinderMk2Cleaner2.SpawnTrailDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Misc.Dusts.ElectricTrail>(), Projectile.velocity, 0.5f, 5);

            if (Main.myPlayer == Projectile.owner)
            {
                Vector2 vector3 = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                vector3 += Projectile.velocity * 48 * 3f;
                vector3.Normalize();
                vector3 *= Main.rand.Next(35, 81) * 0.1f;
                int num15 = (int)(damageDone * 0.5f);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, vector3.X, vector3.Y, ModContent.ProjectileType<GrinderMk4Spark>(), num15, Projectile.knockBack * 0.2f, Projectile.owner);
            }
        }
    }
    class GrinderMk4Spark : ModProjectile
    {
        public override string Texture => "LobotomyCorp/Projectiles/GrinderMk4";

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Spark);
        }

        public override bool PreAI()
        {
            Projectile.alpha = 255;
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] > 3f)
            {
                int num174 = 100;
                if (Projectile.ai[0] > 20f)
                {
                    int num186 = 40;
                    float num197 = Projectile.ai[0] - 20f;
                    num174 = (int)(100f * (1f - num197 / (float)num186));
                    if (num197 >= (float)num186)
                    {
                        Projectile.Kill();
                    }
                }
                if (Projectile.ai[0] <= 10f)
                {
                    num174 = (int)Projectile.ai[0] * 10;
                }
                if (Main.rand.Next(200) < num174)
                {
                    int i = Realized.GrinderMk2Cleaner2.SpawnTrailDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Misc.Dusts.ElectricTrail>(), Projectile.velocity, 0.5f, 5);
                    Main.dust[i].noGravity = true;
                }
            }
            if (Projectile.ai[0] >= 20f)
            {
                Projectile.velocity.X *= 0.99f;
                Projectile.velocity.Y += 0.1f;
            }
            return false;
        }
    }
}
