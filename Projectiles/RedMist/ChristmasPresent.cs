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
    public class ChristmasPresent : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 20;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;

            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;

        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.12f;

            Projectile.rotation += MathHelper.ToRadians(15) * (Projectile.direction > 0 ? 1 : -1);
        }

        public override void OnKill(int timeLeft)
        {
            for(int i = 0; i < 5; i++)
            {
                Vector2 vel = new Vector2(Main.rand.NextFloat(1, 4f), 0).RotatedByRandom(6.28f);

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel, ModContent.ProjectileType<ExuviaeMist>(), Projectile.damage, 0, Projectile.owner);
            }
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

            for (int num444 = 0; num444 < 20; num444++)
            {
                int num445 = Main.rand.Next(89, 91);
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
