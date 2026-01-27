using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	public class FirstMovement : ModProjectile
	{
		public override void SetStaticDefaults() {
            // DisplayName.SetDefault("Song of Apocalypse");
        }

        public override void SetDefaults()
        {
            Projectile.width = 250;
            Projectile.height = 250;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 8;
            Projectile.scale = 1f;
            Projectile.timeLeft = 30;

            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.friendly = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }    
            
        public override void AI() {
            if (Projectile.ai[1] == 0)
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
                //Projectile.velocity *= 0;
                Projectile.scale = 0f;
            }
            Projectile.ai[1]++;
            Projectile.scale = 1.5f * Projectile.ai[1] / 30f;
            if (Projectile.penetrate <= 1 && Projectile.timeLeft > 8)
            {
                Projectile.timeLeft = 8;
            }
            if (Projectile.timeLeft < 8)
            {
                Projectile.alpha = (int)(255 * (1f - Projectile.timeLeft / 8f));
            }

            Projectile.ai[0] += 0.08f * Math.Sign(Projectile.ai[0]);
            Projectile.Center = Main.player[Projectile.owner].RotatedRelativePoint(Main.player[Projectile.owner].MountedCenter) + Projectile.velocity * Projectile.ai[1];
            //Dust.NewDustPerfect(Projectile.Center + new Vector2(100, 0).RotatedBy(Projectile.ai[0]), 5, Vector2.Zero).noGravity = true;
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            int hitboxSize = (int)(220 * Projectile.scale);
            hitbox.X += hitbox.Width / 2 - hitboxSize / 2;
            hitbox.Y += hitbox.Height / 2 - hitboxSize / 2;
            hitbox.Width = hitboxSize;
            hitbox.Height = hitboxSize;
            base.ModifyDamageHitbox(ref hitbox);
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.penetrate > 1)
                return null;
            return false;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage *= Projectile.penetrate / 16f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 position = Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
            Rectangle frame = texture.Frame(1, 9, 0, 0);
            Vector2 origin = frame.Size() / 2;
            float currentRotation = Projectile.ai[0];
            Color color = Color.White;
            for (int i = 0; i < 8; i++)
            {
                frame = texture.Frame(1, 9, 0, i + 1);

                float equivalentRotation = MathHelper.ToRadians(45 * i - 90);
                float opacity = 0f;
                float anglediff = (currentRotation - equivalentRotation + 3.14f + 6.28f) % 6.28f - 3.14f;

                if (-1.9634954f <= anglediff && anglediff <= 1.9634954f)
                {
                    opacity = (1.9634954f * Math.Sign(anglediff) - anglediff) / 0.785f * Math.Sign(anglediff);
                    if (opacity > 1f)
                        opacity = 1f;
                }

                if (opacity <= 0)
                    continue;

                Main.EntitySpriteDraw(texture, position, frame, color * opacity * (1f - Projectile.alpha / 255f), Projectile.rotation - Projectile.ai[0] - MathHelper.ToRadians(22.5f), origin, Projectile.scale, 0, 0);
            }
            return false;
        }

        public bool IsBetween(float min, float max, float targetAngle)
        {
            var normalisedMin = min > 0 ? min : 2 * Math.PI + min;
            var normalisedMax = max > 0 ? max : 2 * Math.PI + max;
            var normalisedTarget = targetAngle > 0 ? targetAngle : 2 * Math.PI + targetAngle;

            return normalisedMin <= normalisedTarget && normalisedTarget <= normalisedMax;
        }
    }
}
