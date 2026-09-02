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
    public class ChristmasCane : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 26;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;

        }

        public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[1] = Projectile.velocity.ToRotation() - 3.14f;
                Projectile.ai[2] = Projectile.direction;
            }

            Projectile.ai[0]++;
            if (Projectile.ai[0] > 30)
            {
                Projectile.velocity += new Vector2(0.16f, 0).RotatedBy(Projectile.ai[1]);
                if (Projectile.velocity.LengthSquared() > 16 * 16)
                {
                    Projectile.velocity.Normalize();
                    Projectile.velocity *= 16;
                }
            }

            Projectile.rotation += MathHelper.ToRadians(24) * (Projectile.ai[2] > 0 ? 1 : -1);

            if (Projectile.soundDelay == 0)
            {
                SoundEngine.PlaySound(SoundID.Item7, Projectile.position);
                Projectile.soundDelay = 8;
            }
        }

        public override void OnKill(int timeLeft)
        {
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
