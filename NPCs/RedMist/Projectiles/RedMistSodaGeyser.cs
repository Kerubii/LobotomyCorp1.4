using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Chat;
using LobotomyCorp;
using Terraria.GameContent;
using LobotomyCorp.Buffs;
using LobotomyCorp.Visuals.LobEffects;
using static LobotomyCorp.Misc.MiscAssets;


namespace LobotomyCorp.NPCs.RedMist
{
	class RedMistSodaGeyser : ModProjectile
    {
        public override string Texture => "LobotomyCorp/Items/Zayin/Soda";

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 100;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 1;

            Projectile.hostile = true;
        }

        public override void AI()
        {
            if (Projectile.ai[0]++ == 40)
            {
                WeaponSmearLine smear = new WeaponSmearLine();
                smear.Setup(Projectile, Vector2.Zero, -Projectile.velocity.ToRotation(), 30, 1);
                smear.SetupLine(14, 10, Projectile.velocity.Length() * 40);
                smear.SetShaderImage(SodaTexture, WindTrail, Worley);
                smear.SetShaderTexOffset(Main.rand.NextFloat(1f), Main.rand.NextFloat(1f));
                smear.AddEffect();
            }
            else if (Projectile.ai[0] < 20)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 normal = Vector2.Normalize(Projectile.velocity) * 4;
                    int d = Dust.NewDust(Projectile.position, 16, 16, DustID.GemAmethyst, normal.X, normal.Y);
                    Main.dust[d].noGravity = true;
                }
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    int d = Dust.NewDust(Projectile.position, 24, 24, DustID.GemAmethyst, 0, 0);
                    Main.dust[d].noGravity = true;
                }
            }
        }

        public override bool ShouldUpdatePosition()
        {
            return Projectile.ai[0] >= 40;
        }
    }
}
