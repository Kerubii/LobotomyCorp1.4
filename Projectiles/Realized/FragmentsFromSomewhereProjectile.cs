using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Dynamic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;	
using Terraria.ID;
using LobotomyCorp.Misc.Dusts;

namespace LobotomyCorp.Projectiles.Realized
{
	public class FragmentsFromSomewhereProjectile : ModProjectile
	{
        public override void SetDefaults() {
			Projectile.width = 22;
			Projectile.height = 22;
			Projectile.aiStyle = -1;
			Projectile.penetrate = -1;

			Projectile.timeLeft = 100;
			Projectile.tileCollide = false;
			Projectile.friendly = true;
			Projectile.extraUpdates = 5;
		}

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            for (int i = -1; i < 2; i++)
			{
				int type = ModContent.DustType<WhiteDust>();// 72 + i;

				Vector2 offset = new Vector2(-11, 11 * (float)Math.Sin((Projectile.localAI[0]/20f) * 6.28f) * i).RotatedBy(Projectile.rotation);

				Dust d = Dust.NewDustPerfect(Projectile.Center + offset, type, Vector2.Zero);
				d.noGravity = true;
				d.color = LobotomyCorp.FragmentShaderColor;
				if (i == 0)
				{
					d.scale = 0.75f;
					d.position.X = Main.rand.Next(-5, 6);
                    d.position.Y = Main.rand.Next(-5, 6);
                }
			}
            Projectile.localAI[0]++;

			if (Projectile.timeLeft < 30)
				Projectile.alpha += 15;

			ModContent.GetInstance<ModSystems.ScreenSystem>().FragmentScreenActivate();
		}

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
			{
				int type = ModContent.DustType<WhiteDust>();// Main.rand.Next(71, 74);
				int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, type, Projectile.velocity.X, Projectile.velocity.Y);
				Main.dust[d].noGravity = true;
				Main.dust[d].fadeIn = 1.5f;
                Main.dust[d].color = LobotomyCorp.FragmentShaderColor;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
			lightColor = LobotomyCorp.FragmentShaderColor;
			//lightColor = Color.White * 0.8f * (1f - Projectile.alpha / 255f);
			//lightColor.A = (byte)(lightColor.A * 0.5f);
			Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
			Rectangle frame = tex.Frame();
			Vector2 position = Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
			Vector2 origin = frame.Size() - new Vector2(0, 11);
            Main.EntitySpriteDraw(tex, position, frame, lightColor, Projectile.rotation, origin, Projectile.scale, 0, 0);
            return false;
        }
	}
}
