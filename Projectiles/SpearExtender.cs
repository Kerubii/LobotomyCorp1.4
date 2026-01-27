using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	public class SpearExtender : ModProjectile
	{
		public override void Load()
		{
			if (Main.netMode != NetmodeID.Server)
			{
				SpearTrail = ModContent.Request<Texture2D>("LobotomyCorp/Projectiles/SpearTrail", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
				GlowSpear = ModContent.Request<Texture2D>("LobotomyCorp/Projectiles/SpearExtenderGlow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
				GlowTip = ModContent.Request<Texture2D>("LobotomyCorp/Projectiles/SpearExtenderTipGlow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
				Tip = ModContent.Request<Texture2D>(Texture + "Tip").Value;

                Main.QueueMainThreadAction(() =>
				{
					LobotomyCorp.PremultiplyTexture(SpearTrail);
					LobotomyCorp.PremultiplyTexture(GlowSpear);
					LobotomyCorp.PremultiplyTexture(GlowTip);
				});
			}
			base.Load();
		}

		public static Texture2D SpearTrail;
		public static Texture2D GlowSpear;
		public static Texture2D GlowTip;
		public static Texture2D Tip;

		public override void SetDefaults() {
			Projectile.width = 18;
			Projectile.height = 18;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 15;

			Projectile.tileCollide = false;
			Projectile.friendly = true;
			Projectile.hide = true;

			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;
		}

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
			overPlayers.Add(index);
        }

		private float[] ExtraAI;

		public enum SEExtra
        {
			None = 1,
			SSWT,
			SSWTNoExtra,
			BlackSwan
        }

        public override void AI()
        {
			ExtraAICheck();

			Player projOwner = Main.player[Projectile.owner];
			Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);
			Projectile.direction = projOwner.direction;
			Projectile.position.X = ownerMountedCenter.X - (float)(Projectile.width / 2);
			Projectile.position.Y = ownerMountedCenter.Y - (float)(Projectile.height / 2);

			if (Projectile.ai[0] == 2)
			{
				Projectile.position.X += ExtraAI[0];
				Projectile.position.Y += ExtraAI[1];
			}

			Projectile.ai[1] += 1f;

			if (Projectile.timeLeft > 5)
				Projectile.localAI[0] += Projectile.velocity.Length();
			else
				Projectile.localAI[0] -= Projectile.velocity.Length();

			Projectile.position += Projectile.velocity * Projectile.ai[1];

			Vector2 dustVel = Vector2.Normalize(Projectile.velocity);
			for (int i = 0; i < 3; i++)
			{
				int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.MagicMirror, -dustVel.X, -dustVel.Y);
				Main.dust[d].noGravity = true;
			}

			Projectile.alpha = (int)(255 * (float)Math.Sin(3.14f * (1f - (Projectile.timeLeft) / 15f)));
			Projectile.scale = (float)Math.Sin(1.57f + 1.57f * (1f - (Projectile.timeLeft) / 15f));
			Projectile.rotation = Projectile.velocity.ToRotation();
		}

		private void ExtraAICheck()
        {
			if (ExtraAI != null)
				return;

			if (Projectile.ai[0] == (int)SEExtra.SSWT)
            {
				ExtraAI = new float[2];
				ExtraAI[0] = Main.rand.NextFloat(-32f, 32f);
				ExtraAI[1] = Main.rand.NextFloat(-32f, 32f);
			}

			if (Projectile.ai[0] <= 0)
            {

            }
        }

		//-1 = darker, +1 = lighter
		private Color GetColor(int range = 0)
        {
			if (Projectile.ai[0] == (int)SEExtra.BlackSwan)
			{
				if (range == 0)
					return new Color(125, 239, 156);//CyanLime
				return range < 0 ? new Color(24, 45, 26) : new Color(84, 212, 51);
			}
			else
			{
				if (range == 0)
					return new Color(103, 232, 192);//Pale
				return range < 0 ? new Color(55, 166, 133) : new Color(72, 180, 206);
			}
		}

        public override bool PreDraw(ref Color lightColor)
        {
			Color BaseColor = GetColor();
			float Opacity = (Projectile.alpha / 255f);

			Texture2D tex = SpearTrail;
			Vector2 pos = Projectile.Center - Main.screenPosition + Projectile.gfxOffY * Vector2.UnitY;
			Rectangle frame = tex.Frame();
			Vector2 origin = frame.Size() / 2;
			origin.X += frame.Width / 4;
			Vector2 scale = new Vector2((Projectile.localAI[0]/192f), Projectile.scale);
			if (Projectile.ai[0] == (int)SEExtra.SSWT || Projectile.ai[0] == (int)SEExtra.SSWTNoExtra)
			{
				scale.Y *= 0.5f;
			}

			Main.EntitySpriteDraw(tex, pos, frame, BaseColor * Opacity, Projectile.rotation, origin, scale, 0, 0);

			Vector2 positionOffset = Vector2.Normalize(Projectile.velocity) * -14;
			Color LightColor = GetColor(1);
			Color DarkColor = GetColor(-1);

			List<DrawData> drawList = new List<DrawData>();

			float RotationOffset = 30;
			if (Projectile.timeLeft < 5)
				RotationOffset = 30f - 30f * (1f - Projectile.timeLeft / 5);

			DrawSpear(drawList, Projectile.Center, LightColor, MathHelper.ToRadians(RotationOffset), Opacity, Opacity);
			DrawSpear(drawList, Projectile.Center, DarkColor, -MathHelper.ToRadians(RotationOffset), Opacity, Opacity);
			DrawSpear(drawList, Projectile.Center, BaseColor, 0f, Opacity, Opacity);

			for (int i = 0; i < drawList.Count; i++)
            {
				Main.EntitySpriteDraw(drawList[i]);
            }

			scale.X *= 0.66f;
			scale.Y *= 0.33f;
			Main.EntitySpriteDraw(tex, pos, frame, Color.White * Opacity, Projectile.rotation, origin, scale, 0, 0);
			DrawTip(Opacity);

			return false;
        }

		private void DrawSpear(List<DrawData> drawlist, Vector2 position, Color color, float rotationOffset, float SpearOpacity = 1f, float GlowOpacity = 1f)
		{
			Vector2 scale = new Vector2(1f, Projectile.scale);
			if (GlowOpacity > 0f)
			{
				Texture2D tex = GlowSpear;
				Vector2 pos = position - Main.screenPosition + Projectile.gfxOffY * Vector2.UnitY;
				Rectangle frame = tex.Frame();
				Vector2 origin = frame.Size() / 2;
				if (drawlist.Count > 0)
					drawlist.Insert(0, new DrawData(tex, pos, frame, color * GlowOpacity, Projectile.rotation + rotationOffset, origin, scale, 0, 0));
				else
					drawlist.Add(new DrawData(tex, pos, frame, color * GlowOpacity, Projectile.rotation + rotationOffset, origin, scale, 0, 0));
			}
			if (SpearOpacity > 0f)
			{
				Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
				Vector2 pos2 = position - Main.screenPosition + Projectile.gfxOffY * Vector2.UnitY;
				Rectangle frame2 = tex.Frame();
				Vector2 origin2 = frame2.Size() / 2;
				drawlist.Add(new DrawData(tex, pos2, frame2, color * GlowOpacity, Projectile.rotation + rotationOffset, origin2, scale, 0, 0));
			}
		}

		private void DrawTip(float Opacity)
        {
			Texture2D tex = GlowTip;
			Vector2 pos = Projectile.Center - Main.screenPosition + Projectile.gfxOffY * Vector2.UnitY;
			Rectangle frame = tex.Frame();
			Vector2 origin = frame.Size() / 2;
			Vector2 scale = new Vector2(1f, Projectile.scale);
			Main.EntitySpriteDraw(tex, pos, frame, Color.White * Opacity, Projectile.rotation, origin, scale, 0, 0);

			tex = Tip;
			Vector2 pos2 = Projectile.Center - Main.screenPosition + Projectile.gfxOffY * Vector2.UnitY;
			Rectangle frame2 = tex.Frame();
			Vector2 origin2 = frame2.Size() / 2;
			Main.EntitySpriteDraw(tex, pos2, frame2, Color.White * Opacity, Projectile.rotation, origin2, scale, 0, 0);
		}

        public override void SendExtraAI(BinaryWriter writer)
        {
			if (ExtraAI != null)
			{
				for (int i = 0; i < ExtraAI.Length; i++)
				{
					writer.Write(ExtraAI[i]);
				}
			}
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
			if (ExtraAI != null)
			{
				for (int i = 0; i < ExtraAI.Length; i++)
				{
					ExtraAI[i] = reader.ReadSingle();
				}
			}
		}
    }
}
