using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	public class RedMistMimicryHello : ModProjectile
	{
        public static Asset<Texture2D> ToothballTexture;

        public override void Load()
        {
            ToothballTexture = ModContent.Request<Texture2D>("LobotomyCorp/Projectiles/MimicryToothball");
        }

        public override string Texture => "LobotomyCorp/Projectiles/MimicryHello";
        public override void SetStaticDefaults() {
            // DisplayName.SetDefault("Hello?");
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 540;
            Projectile.extraUpdates = 3;

            Projectile.tileCollide = true;
            Projectile.hostile = true;
        }

        const int TeethActivate = 180;

        public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.rotation = Main.rand.NextFloat(6.28f);
            }
            Projectile.ai[0]++;

            if (Projectile.ai[0] >= TeethActivate)
            {
                if (Projectile.ai[0] == TeethActivate)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        Vector2 vel = new Vector2(3, 0).RotatedBy(0.785f * i + Main.rand.NextFloat(-0.3f, 0.3f));
                        Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Blood, vel);
                        d.noGravity = true;
                    }

                    Projectile.velocity.Normalize();
                    Projectile.velocity *= 16;
                    Projectile.rotation = Projectile.velocity.ToRotation();
                }
            }
            else
            {
                if (Projectile.velocity.Length() > 2)
                {
                    Projectile.velocity *= 0.95f;

                    Projectile.rotation += Projectile.velocity.Length() * Math.Sign(Projectile.velocity.X) * 0.2f;
                }
            }
        }

        public override bool CanHitPlayer(Player target)
        {
            if (Projectile.ai[0] < TeethActivate)
                return false;

            return base.CanHitPlayer(target);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            if (Projectile.ai[0] < TeethActivate)
            {
                tex = ToothballTexture.Value;
                Vector2 endpoint = Projectile.Center + new Vector2(200, 0).RotatedBy(Projectile.velocity.ToRotation());
                Color color = Color.Red * (float)Math.Sin(3.14f * Projectile.ai[0] / 60f);

                Terraria.Utils.DrawLine(Main.spriteBatch, Projectile.Center, endpoint, color, Color.Transparent, 4f);
            }

            Main.EntitySpriteDraw(
                tex,
                Projectile.Center - Main.screenPosition,
                null,
                lightColor,
                Projectile.rotation,
                tex.Size() / 2 + new Vector2(tex.Width/2 - Projectile.width/2, 0),
                Projectile.scale,
                0,
                0);
            return false;
        }
    }
}
