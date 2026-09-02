using LobotomyCorp.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static LobotomyCorp.LobotomyCorp;

namespace LobotomyCorp.Projectiles.RedMist
{
    public class ChristmasBall : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 26;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;

            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;

        }

        public override void AI()
        {
            Projectile.ai[0]++;
            if (Projectile.ai[0] > 60 * 5)
            {
                Projectile.velocity.Y += 0.12f;
                Projectile.rotation += MathHelper.ToRadians(1f) * Projectile.velocity.Y;
                return;
            }

            Projectile.velocity *= 0.95f;

            Projectile.rotation = MathHelper.ToRadians(15) * (float)Math.Sin(Projectile.ai[0] * 0.1f);

            Dust d = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SparkForLightDisc, Projectile.velocity.X, Projectile.velocity.Y)];
            d.noGravity = true;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.position);

            for (int num444 = 0; num444 < 20; num444++)
            {
                int num445 = Main.rand.Next(89, 92);
                int num446 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, num445);
                Main.dust[num446].noLight = true;
                Main.dust[num446].scale = 0.8f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(Items.He.Christmas.GetRandomDebuff(), 60 * 3);
        }
    }
}
