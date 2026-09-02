using LobotomyCorp.Misc;
using LobotomyCorp.Players;
using LobotomyCorp.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static tModPorter.ProgressUpdate;

namespace LobotomyCorp.Projectiles.Realized
{
	public class DaCapoClef : ModProjectile
	{
        public override void SetStaticDefaults()
        {
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
		{
			Projectile.width = 24;
			Projectile.height = 24;
			Projectile.aiStyle = -1;
			Projectile.penetrate = -1;

			Projectile.scale = 1f;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.tileCollide = false;
			Projectile.friendly = true;
		}

		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();
			Projectile.direction = Math.Sign(Projectile.velocity.X);
			if (Projectile.direction < 0)
				Projectile.rotation += 3.14f;

            //Janky Wave Projectile
            float rotation = Projectile.velocity.ToRotation();
            float wave = 30 * (float)Math.Sin(Projectile.ai[1] * 0.2f);
            Vector2 wavePosition = new Vector2(0, wave).RotatedBy(rotation);
            Projectile.position -= wavePosition;

            Projectile.ai[1]++;
            wave = 30 * (float)Math.Sin(Projectile.ai[1] * 0.2f);
            wavePosition = new Vector2(0, wave).RotatedBy(rotation);
            Projectile.position += wavePosition;

			if (Projectile.ai[0] < Projectile.ai[1])
			{
				Projectile.Kill();
				for (int i = 0; i < 10; i++)
				{
					int n = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Wraith);
					Main.dust[n].noGravity = true;
				}
			}
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            LobotomyAlephPlayer modPlayer = Main.player[Projectile.owner].GetModPlayer<LobotomyAlephPlayer>();
            if (modPlayer.DaCapoSilentMusic)
                modPlayer.DaCapoTotalDamage += damageDone;
        }

        public override bool PreDraw(ref Color lightColor)
		{
			Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
			Rectangle frame = tex.Frame();
			
            CustomShaderData shader = LobotomyCorp.LobcorpShaders["SwingTrail"].UseOpacity(1f);
            shader.UseImage1(MiscAssets.SheetNote2WA);
            shader.UseImage2(MiscAssets.SheetNote2WA);
            shader.UseImage3(MiscAssets.WindTrail);
            shader.UseCustomShaderDate(0, 0);

            SlashTrail slashTrail = new SlashTrail(30, 0);
            slashTrail.color = Color.Black * 0.4f;
			Vector2[] positions = (Vector2[])Projectile.oldPos.Clone();
			for (int i = 0; i < positions.Length; i++)
			{
				if (positions[i].X <= 0 || positions[i].Y <= 0)
					continue;
				positions[i] += new Vector2(Projectile.width, Projectile.height) / 2;
			}
			float[] rotations = Projectile.oldRot;
            slashTrail.DrawSpecific(positions, rotations, Vector2.Zero, shader);
			
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, frame, Color.Black, Projectile.rotation, new Vector2(22, 60), 0.5f, 0, 0);
			return false;
        }
    }
}
