using LobotomyCorp.Misc;
using LobotomyCorp.Visuals.LobEffects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	public class FullBloomArrowProjectile : ModProjectile
	{
        public override string Texture => "LobotomyCorp/Items/NonEgo/FullBloomArrow";

        public override void SetStaticDefaults()
        {
            // If this arrow would have strong effects (like Holy Arrow pierce), we can make it fire fewer projectiles from Daedalus Stormbow for game balance considerations like this:
            //ProjectileID.Sets.FiresFewerFromDaedalusStormbow[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10; // The width of projectile hitbox
            Projectile.height = 10; // The height of projectile hitbox

            Projectile.arrow = true;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 1200;

            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            // The code below was adapted from the ProjAIStyleID.Arrow behavior. Rather than copy an existing aiStyle using Projectile.aiStyle and AIType,
            // like some examples do, this example has custom AI code that is better suited for modifying directly.
            // See https://github.com/tModLoader/tModLoader/wiki/Basic-Projectile#what-is-ai for more information on custom projectile AI.

            // Apply gravity after a quarter of a second
            /*
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] >= 15f)
            {
                Projectile.ai[0] = 15f;
                Projectile.velocity.Y += 0.1f;
            }
            */
            // The projectile is rotated to face the direction of travel
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

            // Cap downward velocity
            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }

            int time = 60;

            if (Projectile.localAI[0] != 0)
                return;
            Projectile.localAI[0]++;
            for (int i = 0; i < 4; i++)
            {
                int waveTime = time + time / 2 + Main.rand.Next(10);
                int waveSpeed = -10 - Main.rand.Next(20);
                int angleDir = (Main.rand.NextBool(2) ? 1 : -1);

                WeaponSmearSineWave test = new();
                test.Setup(Projectile, Projectile.velocity, Projectile.velocity.ToRotation(), Projectile.timeLeft, 1);
                test.SetupWave(15, 4, 0, 0, MathHelper.ToRadians(20) * angleDir, Main.rand.NextFloat(1f, 1.2f));
                test.SetShaderImage(
                    MiscAssets.FlatColor,
                    MiscAssets.WindTrail,
                    MiscAssets.WindTrail
                    );
                test.Color = new Color(249, 159, 253);
                if (Main.rand.NextBool(3))
                    test.Color = new Color(64, 255, 243);

                test.AddEffect();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                Vector2 vel = new Vector2(8, 0).RotatedByRandom(3.14f);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, vel, ModContent.ProjectileType<FaintAromaShot>(), Projectile.damage / 2, Projectile.knockBack / 2, Projectile.owner, 30, target.whoAmI + 1);
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position); // Plays the basic sound most projectiles make when hitting blocks.
            for (int i = 0; i < 5; i++) // Creates a splash of dust around the position the projectile dies.
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.VenomStaff);
                dust.noGravity = true;
                dust.velocity *= 1.5f;
                dust.scale *= 0.9f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, null, lightColor, Projectile.rotation,
                new Vector2(tex.Width / 2, tex.Height - (Projectile.height / 2)),
                Projectile.scale, Projectile.spriteDirection > 0 ? SpriteEffects.FlipHorizontally : 0, 0);

            return false;
        }
    }
}