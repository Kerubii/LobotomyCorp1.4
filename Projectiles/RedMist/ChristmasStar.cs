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
    public class ChristmasStar : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 26;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;

            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;

        }

        public override void AI()
        {
            if (Projectile.localAI[0]++ == 0)
            {
                Projectile.rotation = Main.rand.NextFloat(3.14f);
                SoundEngine.PlaySound(SoundID.Item9, Projectile.position);
            }

            if (Main.rand.NextBool(20))
                Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(Main.rand.Next(4), 0).RotatedByRandom(6.28f), 16 + Main.rand.Next(2));

            Projectile.rotation += MathHelper.ToRadians(12f) * (Projectile.direction > 0 ? 1 : -1);
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
