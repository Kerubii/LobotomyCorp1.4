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
using LobotomyCorp.ModSystems;
using LobotomyCorp.Players;
using LobotomyCorp.Buffs;

namespace LobotomyCorp.Projectiles.Realized
{
	public class Spiderbud : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Spiderbud, Spiderbud, does whatever a Spiderbud does");
		}

		public override void SetDefaults()
		{
			Projectile.width = 50;
			Projectile.height = 92; 
			Projectile.aiStyle = -1;
			Projectile.scale = 1f;

			Projectile.DamageType = DamageClass.Melee;
			Projectile.friendly = true;
			Projectile.tileCollide = false;

			Projectile.netImportant = true;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 75;
		}

		private Vector2 Camera;
		private Vector2 initialPosition;
        public override void AI()
        {
			Player owner = Main.player[Projectile.owner];

            if (Projectile.ai[0] == 0)
            {
                SoundEngine.PlaySound(LobotomyCorp.ItemLobSound("Literature/Spidermom_Down"), Projectile.Center);
                if (Main.myPlayer == Projectile.owner)
				{
                    int target = Items.Ruina.Literature.RedEyesR.NearestCryOutTarget(Main.MouseWorld);
                    Projectile.ai[1] = target + 1;
                    Projectile.netUpdate = true;
                    if (target <= -1 || owner.GetModPlayer<LobotomyTethPlayer>().RedEyesPredator)
                    {
                        Projectile.Kill();
                        owner.itemTime = owner.itemAnimationMax;
                        owner.itemAnimation = 0;
                        owner.AddBuff(ModContent.BuffType<Alertness>(), 300);
                        for (int i = 0; i < 16; i++)
                        {
                            int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Wraith, Projectile.velocity.X, Projectile.velocity.Y);
                            Main.dust[d].velocity *= 0.2f;
                            Main.dust[d].noGravity = true;
                        }
                        return;
                    }
                }

				Camera = owner.Center;
				initialPosition = Camera;
			}
			Projectile.ai[0]++;

			if (Projectile.ai[0] == 30)
            {
                Projectile.position.X = Main.npc[(int)(Projectile.ai[1] - 1)].Center.X - Projectile.width / 2 - 75f * owner.direction;
				Projectile.position.Y = Main.npc[(int)(Projectile.ai[1] - 1)].Center.Y - (1057f / 2 + Projectile.height);
				Projectile.spriteDirection = owner.direction;
				Projectile.netUpdate = true;
			}
			if (Projectile.ai[0] < 30)
			{
				Projectile.Center = Vector2.Lerp(Projectile.Center, Camera - (1057f / 2 + Projectile.height) * Vector2.UnitY, 0.1f);
				owner.itemAnimation = owner.itemAnimationMax / 3 * 2;
				owner.itemTime = owner.itemAnimation;
				Projectile.alpha = (int)(255 * Projectile.ai[0] / 30);
				owner.GetModPlayer<LobotomyTethPlayer>().RedEyesOpacity = Projectile.ai[0] / 30;
			}
			else if (Projectile.ai[0] >= 45)
            {
				if (Projectile.ai[0] == 45)
					for (int i = 0; i < 16; i++)
					{
						int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Wraith, Projectile.velocity.X, Projectile.velocity.Y);
						Main.dust[d].velocity *= 0.2f;
						Main.dust[d].noGravity = true;
					}

				Projectile.hide = true;
				Projectile.alpha = 255;
				Projectile.velocity *= 0;
            }
			else
			{
				//Projectile.Center = Vector2.Lerp(Projectile.Center, Main.npc[(int)Projectile.ai[1] - 1].Center, 0.5f);
				if (Projectile.alpha > 0)
					Projectile.alpha -= 25;
				if (Projectile.alpha < 0)
					Projectile.alpha = 0;

                owner.GetModPlayer<LobotomyTethPlayer>().RedEyesOpacity = Projectile.alpha / 255f;
				Projectile.velocity.Y = 42;// 1057 / 30;
				owner.itemAnimation = owner.itemAnimationMax / 2;
				owner.itemTime = owner.itemAnimation;
			}

			if (Projectile.owner == Main.myPlayer)
            {
				if (Projectile.ai[0] < 30)
				{
					ModContent.GetInstance<ScreenSystem>().RedEyesSpecialCamera(Camera);
				}
				else 
				{
					if (Projectile.ai[1] > 0)
						Camera = ModContent.GetInstance<ScreenSystem>().RedEyesSpecialCamera(Camera, new Vector2(owner.Center.X,Main.npc[(int)Projectile.ai[1] - 1].Center.Y), 0.2f);
					else
						ModContent.GetInstance<ScreenSystem>().RedEyesSpecialCamera(Camera);
				}
			}

			if (Projectile.ai[0] >= 45)
			{
				Projectile.Center = owner.itemLocation;
			}
			else
			{
				Vector2 posOffset = owner.Size / 2 * -1 + new Vector2(-8f, 0f);
				posOffset.X *= owner.direction;
				owner.Center = Projectile.Center + posOffset;

				for (int i = 0; i < 8; i++)
				{
					int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Wraith);
					Main.dust[d].velocity *= 0;
					Main.dust[d].noGravity = true;
				}
			}

			if (Projectile.ai[0] == 50)
            {
				float rot = Projectile.spriteDirection > 0 ? MathHelper.ToRadians(10) : MathHelper.ToRadians(170);
				Vector2 pos = owner.position + new Vector2(82, 0).RotatedBy(rot) + new Vector2(owner.width/2, owner.height + 20f);
				int amount = 24;
				for (int i = 0; i < amount; i++)
				{
					Vector2 velocity = new Vector2();
					velocity.X = 12f * (float)Math.Cos(i / (float)amount * 6.28f);
					velocity.Y = 3f * (float)Math.Sin(i / (float)amount * 6.28f);
					Dust.NewDustPerfect(pos, DustID.Wraith, velocity).noGravity = true;

					if (Main.rand.NextBool(3))
                    {
						Vector2 pos2 = owner.position + new Vector2(82 + Main.rand.Next(-60, 60), 0).RotatedBy(rot) + new Vector2(owner.width / 2, owner.height + 20f);
						Vector2 velocity2 = new Vector2(0, Main.rand.Next(-16, -8));
						Dust.NewDustPerfect(pos2, DustID.Wraith, velocity2).noGravity = true;
					}
				}

				owner.immune = true;
				owner.immuneTime = 45;
				owner.immuneNoBlink = true;
			}
        }

        public override void OnKill(int timeLeft)
        {
            Player owner = Main.player[Projectile.owner];
            if (Collision.SolidCollision(owner.position, owner.width, owner.height))
            {
                owner.Center = initialPosition;
                for (int i = 0; i < 16; i++)
                {
                    int d = Dust.NewDust(owner.position, owner.width, owner.head, DustID.Wraith);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].fadeIn = 1.5f;
                }
            }
            owner.immuneNoBlink = false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
			if (Projectile.ai[0] >= 45)
				overPlayers.Add(index);

            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        }

        public override bool? CanHitNPC(NPC target)
        {
			return false;
        }

        public override bool CanHitPvp(Player target)
        {
			return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
			if (Projectile.ai[0] >= 45)
			{
				float prog = 1f - (Projectile.ai[0] - 45f) / 15f;
				prog = Math.Clamp(prog, 0f, 1f);
				float opacity = 1f;
				if (prog < 0.5f)
					opacity = prog / 0.5f;
				CustomShaderData shader = LobotomyCorp.LobcorpShaders["RedEyesTrail"].UseOpacity(opacity);

				SlashTrail trail = new SlashTrail(60, 1.57f);
				trail.color = Color.White;
				float rotation = -90;
				float length = 30 + 240 * prog;
				if (prog > 0.5f)
				{
					prog = (prog - 0.5f) / 0.5f;
					rotation += 100f * (float)Math.Sin(1.57f + 1.57f * prog);
				}
				else
				{
					prog = 1f - prog / 0.5f;
					rotation += 100f + 10f * prog;
				}
				float rot = Projectile.rotation + MathHelper.ToRadians(rotation) * Projectile.spriteDirection;
				/*if (prog > 0.5f)
					prog = 1f - (prog - 0.5f) / 0.5f;
				else
					prog = 1f;*/
				trail.DrawPartCircle(Projectile.Center, rot + (Projectile.spriteDirection > 0 ? 0f : 3.14f), MathHelper.ToRadians(length), Projectile.spriteDirection, 100, 32, shader);
				return false;
			}

			return base.PreDraw(ref lightColor);
        }
    }
}
