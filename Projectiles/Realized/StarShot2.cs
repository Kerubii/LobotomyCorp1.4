using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using LobotomyCorp.Utils;
using Terraria.GameContent;
using System.Collections.Generic;
using Terraria.Audio;
using LobotomyCorp.Players;

namespace LobotomyCorp.Projectiles.Realized
{
	public class StarShot2 : ModProjectile
	{
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 660;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (Projectile.localAI[1] == 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 58, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 150, default(Color), 1.2f);
                    Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position, new Vector2(Projectile.velocity.X * 0.05f, Projectile.velocity.Y * 0.05f), Main.rand.Next(16, 18), 1f);
                }
                Projectile.rotation += Main.rand.NextFloat(3.14f);
            }

            if (Projectile.localAI[1] < 20)
                Projectile.localAI[1]++;

            if (Main.rand.NextBool(4))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 58, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 150, default(Color), 1.2f);
                Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position, new Vector2(Projectile.velocity.X * 0.05f, Projectile.velocity.Y * 0.05f), Main.rand.Next(16, 18), 1f);
            }
            Projectile.rotation += MathHelper.ToRadians(5) * Math.Sign(Projectile.velocity.X);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            LobotomyWawPlayer wawPlayer = Main.player[Projectile.owner].GetModPlayer<LobotomyWawPlayer>();
            if (wawPlayer.LoveAndHateVillain == target.whoAmI)
             wawPlayer.LoveAndHateArcanaCost -= 5;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle frame = tex.Frame(1, 2);
            Vector2 pos = Projectile.Center - Main.screenPosition + Projectile.gfxOffY * Vector2.UnitY;
            Vector2 origin = new Vector2(97, 30);
            Color color = Color.White * (Projectile.localAI[1] / 25f);
            Main.EntitySpriteDraw(tex, pos, new Rectangle(frame.X, frame.Height, frame.Width, frame.Height), color, Projectile.velocity.ToRotation(), origin, new Vector2(1f, 0.25f), 0);
            Main.EntitySpriteDraw(tex, pos, frame, Color.White, Projectile.rotation, origin, 1f, 0);

            return false;
        }
    }
}
