using LobotomyCorp.Players;
using LobotomyCorp.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles.Realized
{
	public class GoldRushEffect : ModProjectile
	{
		public override string Texture => "LobotomyCorp/Projectiles/Realized/BlackSwanR";

		public override void SetStaticDefaults()
        {
			Main.projFrames[Projectile.type] = 2;
			//ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
			//ProjectileID.Sets.TrailingMode[Projectile.type] = 4;
		}

        public override void SetDefaults() {
			Projectile.width = 18;
			Projectile.height = 18;
			Projectile.aiStyle = -1;
			Projectile.penetrate = -1;
			Projectile.scale = 1.3f;
			Projectile.alpha = 0;
			Projectile.timeLeft = 14;

			Projectile.hide = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.tileCollide = false;
			Projectile.friendly = true;
		}

        public override void AI()
        {

        }

        public override bool? CanHitNPC(NPC target)
        {
			return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overWiresUI.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
		{
            float opacity = Math.Clamp(Projectile.timeLeft / 14f, 0f, 1f);
            SlashTrail trail = new SlashTrail(0, 0);
			trail.color = Color.Yellow * 0.8f;

			Player projOwner = Main.player[Projectile.owner];
			float trailScale = Projectile.scale * opacity;
			if (Projectile.timeLeft < 5)
			{
				//trail.color *= opacity;
			}
			CustomShaderData shader = LobotomyCorp.LobcorpShaders["SwingTrail"].UseOpacity(opacity);
			shader.UseImage1(Mod, "Misc/FlatColor");
			shader.UseImage2(Mod, "Misc/Trail70");
			shader.UseImage3(Mod, "Misc/Trail70");

			int max = Projectile.timeLeft > 4 ? Projectile.timeLeft - 4 : 0;
			max = (int)(max * 0.7f);
			if (max < 2)
				max = 2;
			Vector2[] trail1 = new Vector2[max];
			Vector2[] trail2 = new Vector2[max];
			float[] rot = new float[max];
			for (int i = 0; i < max; i++)
			{
				Vector2 center = Projectile.Center + Projectile.velocity * 3 * i - Main.screenPosition;
				rot[i] = Projectile.rotation;
				float distance = 120 - 60 * trailScale;
				trail1[i] = center + new Vector2(0, distance).RotatedBy(Projectile.rotation);
				trail2[i] = center + new Vector2(0, -distance).RotatedBy(Projectile.rotation);
			}

			trail.DrawManual(trail1, trail2, shader);
			return false;
			/*
			SlashTrail trail = new SlashTrail(24, MathHelper.ToRadians(0));//, 0.785f - MathHelper.ToRadians(Projectile.direction == 1 ? 0 : 90));
			trail.color = new Color(0, 100, 0);

			Player projOwner = Main.player[Projectile.owner];
			float prog = (float)Math.Sin(1.57f * (Projectile.timeLeft / 14f));
			if (prog < 0)
				prog = 0;
			CustomShaderData windTrail = LobotomyCorp.LobcorpShaders["WindTrail"].UseOpacity(prog);

			int max = Projectile.timeLeft > 4 ? 10 - (Projectile.timeLeft - 4) : 10;
			Vector2[] posTrail = new Vector2[max];
			float[] rot = new float[max];
			for (int i = 0; i < posTrail.Length; i++)
            {
				posTrail[i] = Projectile.Center - Projectile.velocity * i * 6;
				rot[i] = Projectile.rotation;
            }

			trail.DrawSpecific(posTrail, rot, Vector2.Zero, windTrail);
			
			return false;*/
        }
    }
}
