using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	public class TwilightStrikes : ModProjectile
	{
		public override void SetDefaults() {
			Projectile.height = 12;
			Projectile.width = 12;
			Projectile.aiStyle = -1;
			Projectile.penetrate = -1;

			Projectile.tileCollide = false;
			Projectile.timeLeft = 30;
			Projectile.friendly = true;
			Projectile.alpha = 255;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = Projectile.timeLeft;

			Projectile.extraUpdates = 3;
		}

        public override void AI()
        {
            if (Projectile.ai[0] >= 0 && !Main.npc[(int)Projectile.ai[0]].active)
            {
                Projectile.ai[0] = -1;
            }

			Projectile.localAI[0]++;
			Projectile.rotation = Projectile.velocity.ToRotation();
			if (Projectile.timeLeft < 5)
				Projectile.alpha -= 51;

			if (Projectile.ai[1] > 0 && Projectile.localAI[0] == 15)
            {
				float angle = Main.rand.NextFloat(6.28f);
				Vector2 velocity = new Vector2(16f, 0f).RotatedBy(angle) * Main.rand.NextFloat(0.5f, 1f);
				Vector2 position = -velocity * 15;
                int offsetX = 5;
                int offsetY = 5;

                if (Projectile.ai[0] >= 0)
                {
                    position += Main.npc[(int)Projectile.ai[0]].Center;
                    offsetX = Main.npc[(int)Projectile.ai[0]].width / 2;
                    offsetY = Main.npc[(int)Projectile.ai[0]].height / 2;
                }
                else
                    position += Projectile.Center;

				position.X += Main.rand.Next(-offsetX, offsetX);
				position.Y += Main.rand.Next(-offsetY, offsetY);
				int type = ModContent.ProjectileType<Projectiles.TwilightStrikes>();
				if (Main.rand.NextBool(5))
				{
					position += velocity * 15;
					velocity *= 0;
					type = ModContent.ProjectileType<Projectiles.TwilightSlashes>();
				}
                if (Main.myPlayer == Projectile.owner)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), position, velocity, type, Projectile.damage, 0, Projectile.owner, Projectile.ai[0], Projectile.ai[1] - 1);
			}
		}

        public override bool? CanHitNPC(NPC target)
        {
			if (Projectile.ai[0] > -1 && target.whoAmI != (int)Projectile.ai[0])
				return false;
            if (Projectile.localNPCImmunity[target.whoAmI] == 0)
                return null;
            return null;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.ScalingArmorPenetration += 1f;
            modifiers.DisableCrit();
        }

        public override bool PreDraw(ref Color lightColor)
        {
			Texture2D tex = SpearExtender.SpearTrail;
			Vector2 pos = Projectile.Center - Main.screenPosition + Projectile.gfxOffY * Vector2.UnitY;
			Rectangle frame = tex.Frame();
			float length = Projectile.velocity.Length() * Projectile.localAI[0];
			Vector2 origin = new Vector2(287, 53);
			float nana = 287f;
			Vector2 scale = new Vector2(length / nana, 0.2f);
			Color color = Color.DarkRed * (Projectile.alpha / 255f);
			color.A = (byte)(color.A * 0.7f);
			color *= 0.9f;
			Main.EntitySpriteDraw(tex, pos, frame, color, Projectile.rotation, origin, scale, 0, 0);
			/*
			if (Projectile.alpha > 100)
			{
				Vector2 scale = new Vector2(1f * (frame.Width * 2 / 124f), 1f * (frame.Height * 2 / 124f));
				tex = LobotomyCorp.CircleGlow.Value;
				color = Color.Yellow * ((Projectile.alpha - 100) / 155f);
				color.A = (byte)(color.A * 0.7f);
				color *= 0.9f;
				frame = tex.Frame();
				origin = frame.Size() / 2;
				Main.EntitySpriteDraw(tex, pos, frame, color, Projectile.velocity.ToRotation() + Projectile.rotation + 1.57f, origin, scale, 0, 0);
			}*/

			return false;
        }
    }

    public class TwilightSlashes : ModProjectile
    {
        //public override string Texture => "LobotomyCorp/Projectiles/RedMistSlashes";
        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                TwilightSlashTex = ModContent.Request<Texture2D>("LobotomyCorp/Projectiles/TwilightSlashes", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

                Main.QueueMainThreadAction(() =>
                {
                    LobotomyCorp.PremultiplyTexture(TwilightSlashTex);
                });
            }
        }

        public override void Unload()
        {
            TwilightSlashTex = null;
        }

        public static Texture2D TwilightSlashTex = null;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Twilight Slashes");
        }

        public override void SetDefaults()
        {
            Projectile.width = 96;
            Projectile.height = 96;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 15;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = Projectile.timeLeft;
        }

        public override void AI()
        {
            if (Projectile.ai[0] >= 0 && !Main.npc[(int)Projectile.ai[0]].active)
            {
                Projectile.ai[0] = -1;
            }

            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[1] = Main.rand.NextFloat(6.28f);

                Projectile.scale = Main.rand.NextFloat(0.6f, 1.2f);
                Projectile.rotation = Projectile.localAI[1] - MathHelper.ToRadians(270f) * (Main.rand.NextBool(2) ? -1 : 1);
            }
            else
                Projectile.rotation = Projectile.rotation + (Projectile.localAI[1] - Projectile.rotation) * 0.2f;

            Projectile.localAI[0]++;

            if (Projectile.ai[1] > 0 && Projectile.localAI[0] == 15)
            {
                float angle = Main.rand.NextFloat(6.28f);
                Vector2 velocity = new Vector2(16f, 0f).RotatedBy(angle) * Main.rand.NextFloat(0.5f, 1f);
                Vector2 position = -velocity * 15;
                int offsetX = 5;
                int offsetY = 5;

                if (Projectile.ai[0] >= 0)
                {
                    position += Main.npc[(int)Projectile.ai[0]].Center;
                    offsetX = Main.npc[(int)Projectile.ai[0]].width / 2;
                    offsetY = Main.npc[(int)Projectile.ai[0]].height / 2;
                }
                else
                    position += Projectile.Center;

                position.X += Main.rand.Next(-offsetX, offsetX);
                position.Y += Main.rand.Next(-offsetY, offsetY);
                int type = ModContent.ProjectileType<Projectiles.TwilightStrikes>();
                if (Main.rand.NextBool(5))
                {
                    position += velocity * 15;
                    velocity *= 0;
                    type = ModContent.ProjectileType<Projectiles.TwilightSlashes>();
                }
                if (Main.myPlayer == Projectile.owner)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), position, velocity, type, Projectile.damage, 0, Projectile.owner, Projectile.ai[0], Projectile.ai[1] - 1);
            }

            if (Projectile.timeLeft <= 5)
                Projectile.alpha -= 50;
        }

        public override bool? CanDamage()
        {
            if (Projectile.timeLeft >= 10 && Projectile.timeLeft < 15)
                return base.CanDamage();
            return false;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.ScalingArmorPenetration += 1f;
            modifiers.DisableCrit();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TwilightSlashTex;//TextureAssets.Projectile[Projectile.type].Value;
            Vector2 pos = Projectile.Center + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
            Rectangle frame = tex.Frame();
            Vector2 origin = frame.Size() / 2;
            Color color = Color.DarkRed * (float)(Projectile.alpha / 255f);

            Main.EntitySpriteDraw(tex, pos, frame, color, Projectile.rotation, origin, Projectile.scale, 0f, 0);
            return false;
        }
    }
}