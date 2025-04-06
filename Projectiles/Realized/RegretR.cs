using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using LobotomyCorp.Utils;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using LobotomyCorp.Players;

namespace LobotomyCorp.Projectiles.Realized
{
	public class RegretR : ModProjectile
	{

		public override void SetDefaults()
		{
			Projectile.width = 24;
			Projectile.height = 24;
			Projectile.aiStyle = -1;
			Projectile.penetrate = -1;

			Projectile.scale = 1.5f;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.tileCollide = true;
			Projectile.friendly = true;

			screencrackAttack = false;
			groundPound = false;
		}

		private bool screencrackAttack = false;
		private Bezier chainBezier;
		private bool groundPound = false;

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];//Get Owner
			if (!player.active || player.dead || player.noItems || player.CCed || Vector2.Distance(Projectile.Center, player.Center) > 900f)
			{
				Projectile.Kill();
				return;
			}
			if (Main.myPlayer == Projectile.owner && Main.mapFullscreen)
			{
				Projectile.Kill();
				return;
			}
			Vector2 mountedCenter = player.MountedCenter;
			bool doFastThrowDust = false;
			bool flag = true;
			bool flag2 = false;
			int num = 10;
			float speed = 32;
			float maxDistance = 800f;
			float maxSpeed = 12f;
			float minimumDistance = 16f;
			float postDropMaxSpeed = 6f;
			float postDropDistance = 48f;
			float num20 = 1f;
			float num21 = 14f;
			int num2 = 60;
			int num3 = 10;
			int hitCooldown = 12;
			int hitCooldown2 = 10;
			int num6 = num + 5;

			float num8 = speed * (float)num;
			float droppedDistance = num8 + 160f;
			Projectile.localNPCHitCooldown = num3;

			switch ((int)Projectile.ai[0])//ProjectileStates
			{
				case 0://If player is channeling, Move Flail Around the Player
					{
						flag2 = true;
						if (Projectile.owner == Main.myPlayer)//Checks if player is mainPlayer for mouse
						{
							Vector2 mouseWorld = Main.MouseWorld;
							Vector2 direction = mountedCenter.DirectionTo(mouseWorld).SafeNormalize(Vector2.UnitX * (float)player.direction);
							player.ChangeDir((direction.X > 0f) ? 1 : (-1));
							if (!player.channel)//If Mouse is released, Flail is sent out
							{
								Projectile.ai[0] = 1f;
								Projectile.ai[1] = 0f;
								Projectile.velocity = direction * speed + player.velocity;
								Projectile.Center = mountedCenter;
								Projectile.netUpdate = true;
								Projectile.ResetLocalNPCHitImmunity();
								Projectile.localNPCHitCooldown = hitCooldown2;
								break;
							}
						}
						Projectile.localAI[1] += 1f;
						Vector2 circlePosition = Terraria.Utils.RotatedBy(new Vector2((float)player.direction), (float)Math.PI * 7f * (Projectile.localAI[1] / 60f) * (float)player.direction);//Circle
						circlePosition.Y *= 0.8f;
						if (circlePosition.Y * player.gravDir > 0f)
						{
							circlePosition.Y *= 0.5f;
						}
						Projectile.Center = mountedCenter + circlePosition * 30f;//Circle + Distance
						Projectile.velocity = Vector2.Zero;
						Projectile.localNPCHitCooldown = hitCooldown;
						break;
					}
				case 1://Flail is sent away from the player, maintaining velocity
					{
						doFastThrowDust = true;
						bool flag4 = Projectile.ai[1]++ >= (float)num;//Check if [num] frames has paassed
						flag4 |= Projectile.Distance(mountedCenter) >= maxDistance;//Or if our distance is bigger than max Distance
						if (player.controlUseItem)//If item is held again, Go to Drop State
						{
							Projectile.ai[0] = 6f;
							Projectile.ai[1] = 0f;
							Projectile.netUpdate = true;
							//Projectile.velocity.X *= 0.6f;
							Projectile.velocity.Y *= 0.2f;
							break;
						}
						if (flag4)//If 10 frames or too far, return to player
						{
							Projectile.ai[0] = 4f;
							Projectile.ai[1] = 0f;
							Projectile.netUpdate = true;
							Projectile.velocity *= 0.3f;
						}
						player.ChangeDir((player.Center.X < Projectile.Center.X) ? 1 : (-1));
						Projectile.localNPCHitCooldown = hitCooldown2;
						break;
					}
				case 2://Flail attempts to return to player
					{
						Vector2 playerCenterTarget = Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero);
						if (Projectile.Distance(mountedCenter) <= minimumDistance)//If Flail is near player, Kill
						{
							Projectile.Kill();
							return;
						}
						if (player.controlUseItem)//Iif item is held again, Go to Drop State
						{
							Projectile.ai[0] = 6f;
							Projectile.ai[1] = 0f;
							Projectile.netUpdate = true;
							Projectile.velocity *= 0.2f;
						}
						else//Return to player
						{
							Projectile.velocity *= 0.98f;
							Projectile.velocity = Projectile.velocity.MoveTowards(playerCenterTarget * minimumDistance, maxSpeed);
							player.ChangeDir((player.Center.X < Projectile.Center.X) ? 1 : (-1));
						}
						break;
					}
				case 3://Unknown, Old Held?
					{
						if (!player.controlUseItem)
						{
							Projectile.ai[0] = 4f;
							Projectile.ai[1] = 0f;
							Projectile.netUpdate = true;
							break;
						}
						float num10 = Projectile.Distance(mountedCenter);
						Projectile.tileCollide = Projectile.ai[1] == 1f;
						bool flag3 = num10 <= num8;
						if (flag3 != Projectile.tileCollide)
						{
							Projectile.tileCollide = flag3;
							Projectile.ai[1] = (Projectile.tileCollide ? 1 : 0);
							Projectile.netUpdate = true;
						}
						if (num10 > (float)num2)
						{
							if (num10 >= num8)
							{
								Projectile.velocity *= 0.5f;
								Projectile.velocity = Projectile.velocity.MoveTowards(Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero) * num21, num21);
							}
							Projectile.velocity *= 0.98f;
							Projectile.velocity = Projectile.velocity.MoveTowards(Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero) * num21, num20);
						}
						else
						{
							if (Projectile.velocity.Length() < 6f)
							{
								Projectile.velocity.X *= 0.96f;
								Projectile.velocity.Y += 0.2f;
							}
							if (player.velocity.X == 0f)
							{
								Projectile.velocity.X *= 0.96f;
							}
						}
						player.ChangeDir((player.Center.X < Projectile.Center.X) ? 1 : (-1));
						break;
					}
				case 4://After dropstate return to player
					{
						Projectile.tileCollide = false;
						Vector2 playerCenterTarget = Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero); //Go to Player
						if (Projectile.Distance(mountedCenter) <= postDropDistance) //If near Player, Kill
						{
							Projectile.Kill();
							return;
						}
						Projectile.velocity *= 0.98f;
						Projectile.velocity = Projectile.velocity.MoveTowards(playerCenterTarget * postDropDistance, postDropMaxSpeed);
						Vector2 target = Projectile.Center + Projectile.velocity;
						Vector2 value = mountedCenter.DirectionFrom(target).SafeNormalize(Vector2.Zero);
						if (Vector2.Dot(playerCenterTarget, value) < 0f)
						{
							Projectile.Kill();
							return;
						}
						player.ChangeDir((player.Center.X < Projectile.Center.X) ? 1 : (-1));
						break;
					}
				case 5://Post hit something state
					if (Projectile.ai[1]++ >= (float)num6)
					{
						Projectile.ai[0] = 6f;
						Projectile.ai[1] = 0f;
						Projectile.netUpdate = true;
					}
					else
					{
						Projectile.localNPCHitCooldown = hitCooldown2;
						Projectile.velocity.Y += 0.6f;
						Projectile.velocity.X *= 0.95f;
						player.ChangeDir((player.Center.X < Projectile.Center.X) ? 1 : (-1));
					}
					break;
				case 6://Drop State
					if (!player.controlUseItem || Projectile.Distance(mountedCenter) > droppedDistance) //If stop holding m1 or too far change to retract
					{
						Projectile.ai[0] = 4f;
						Projectile.ai[1] = 0f;
						Projectile.netUpdate = true;
					}
					else
					{
						if (Projectile.velocity.Y < 0)
							Projectile.velocity.Y = 0;
						Projectile.velocity.Y += 1.2f;
						Projectile.velocity.X *= 0.95f;
						//if (!groundPound)
						//{
							//groundPound = true;
							//Projectile.velocity.Y = 16;
						//}
						player.ChangeDir((player.Center.X < Projectile.Center.X) ? 1 : (-1));
					}
					break;
			}
			Projectile.direction = ((Projectile.velocity.X > 0f) ? 1 : (-1));
			//Projectile.spriteDirection = Projectile.direction;
			//Vector2 delt = Projectile.Center - mountedCenter;
			//Projectile.rotation = delt.ToRotation() + 0.875f;
			Projectile.ownerHitCheck = flag2;
			Projectile.timeLeft = 2;
			player.heldProj = Projectile.whoAmI;
			player.SetDummyItemTime(2);
			player.itemRotation = Projectile.DirectionFrom(mountedCenter).ToRotation();
			if (Projectile.Center.X < mountedCenter.X)
			{
				player.itemRotation += (float)Math.PI;
			}
			player.itemRotation = MathHelper.WrapAngle(player.itemRotation);

			if (chainBezier == null)
			{
				chainBezier = new Bezier(Projectile.Center, mountedCenter);
			}

			chainBezier.SetStartEnd(Projectile.Center + Projectile.velocity, mountedCenter);

			Vector2 chainDelta = Projectile.Center - mountedCenter;
			Vector2 cOffset1 = chainDelta / 3;
			Vector2 cOffset2 = Projectile.velocity * 1.5f;
			if (Projectile.ai[0] == 1)
            {
				float prog = 1f - Projectile.ai[1] / 10;
				cOffset1 = cOffset1.RotatedBy(MathHelper.ToRadians(45 * prog)) * 5;
				cOffset2 = cOffset2.RotatedBy(MathHelper.ToRadians(-45 * prog)) * 5;
			}
			if (Projectile.ai[0] == 6)
            {
				cOffset2 = Projectile.velocity * 5f;
				cOffset1 = Projectile.velocity * 5f;
            }
			chainBezier.CPoint2Move(mountedCenter + cOffset1, 24f);
			chainBezier.CPoint1Move(Projectile.Center + cOffset2, 24f);
			if (Projectile.ai[0] < 2)
			{
				Vector2 delt = Projectile.Center - mountedCenter;
				Projectile.rotation = delt.ToRotation() + 0.875f;
			}
			else
				Projectile.rotation = (chainBezier.BezierPoint(0f) - chainBezier.BezierPoint(0.05f)).ToRotation() + MathHelper.ToRadians(45);
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			if (!screencrackAttack && Projectile.ai[0] != 0)
			{
				Player player = Main.player[Projectile.owner];
				LobotomyTethPlayer modPlayer = player.GetModPlayer<LobotomyTethPlayer>();
				if (modPlayer.RegretShockwave < 2)
				{
					modPlayer.RegretShockwave++;
					screencrackAttack = true;
					SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Abandoned_Strong_Hori") with { Volume = 0.2f }, Projectile.Center);
				}
				else
				{
					int type = ModContent.ProjectileType<Projectiles.Realized.RegretShockwave>();
					if (Main.player[Projectile.owner].ownedProjectileCounts[type] == 0)
					{
						modPlayer.RegretShockwave = 0;
						Projectile.NewProjectile(player.GetSource_FromThis(), target.Center, Vector2.Zero, type, hit.Damage, 0, player.whoAmI, 0, target.whoAmI);
						screencrackAttack = true;
					}
				}
			}
		}

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
			if (Projectile.ai[0] == 0)
				modifiers.FinalDamage *= 0.7f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
			int num = 10;
			int num2 = 0;
			Vector2 velocity = Projectile.velocity;
			float num3 = 0.2f;
			if (Projectile.ai[0] == 1f || Projectile.ai[0] == 5f)
            {
				num3 = 0.4f;
            }
			if (Projectile.ai[0] == 6f)
            {
				num3 = 0f;
            }
			if (oldVelocity.X != Projectile.velocity.X)
			{
				if (Math.Abs(oldVelocity.X) > 4f)
				{
					num2 = 1;
				}
				Projectile.velocity.X = (0f - oldVelocity.X) * num3;
			}
			if (oldVelocity.Y != Projectile.velocity.Y)
			{
				if (Math.Abs(oldVelocity.Y) > 4f)
				{
					num2 = 1;
				}
				Projectile.position.Y += Projectile.velocity.Y;
				Projectile.velocity.Y = (0f - oldVelocity.Y) * num3;
			}
			if (Projectile.ai[0] == 1f)
			{
				Projectile.ai[0] = 5f;
				Projectile.localNPCHitCooldown = num;
				Projectile.netUpdate = true;
				Point scanAreaStart = Projectile.TopLeft.ToTileCoordinates();
				Point scanAreaEnd = Projectile.BottomRight.ToTileCoordinates();
				num2 = 2;
				Projectile.CreateImpactExplosion(2, Projectile.Center, ref scanAreaStart, ref scanAreaEnd, Projectile.width, out var causedShockwaves);
				Projectile.CreateImpactExplosion2_FlailTileCollision(Projectile.Center, causedShockwaves, velocity);
				Projectile.position -= velocity;
			}
			if (Projectile.ai[0] == 6f && !groundPound)
            {
				groundPound = true;
				Projectile.velocity.X *= 0.2f;
				Projectile.netUpdate = true;
				Point scanAreaStart = Projectile.TopLeft.ToTileCoordinates();
				Point scanAreaEnd = Projectile.BottomRight.ToTileCoordinates();
				num2 = 2;
				Projectile.CreateImpactExplosion(2, Projectile.Center, ref scanAreaStart, ref scanAreaEnd, Projectile.width, out var causedShockwaves);
				Projectile.CreateImpactExplosion2_FlailTileCollision(Projectile.Center, causedShockwaves, velocity * 12f);
			}
			if (num2 > 0)
			{
				Projectile.netUpdate = true;
				for (int i = 0; i < num2; i++)
				{
					Collision.HitTiles(Projectile.position, velocity, Projectile.width, Projectile.height);
				}
				SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
			}
			if (Projectile.ai[0] != 3f && Projectile.ai[0] != 0f && Projectile.ai[0] != 5f && Projectile.ai[0] != 6f && Projectile.localAI[0] >= 10f)
			{
				Projectile.ai[0] = 4f;
				Projectile.netUpdate = true;
			}
			return false;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			if (chainBezier != null)
			{
				float accuracy = 150;
				float steps = 0.001f;

				float BezierLength = chainBezier.SegmentBezierLength(accuracy);

				Texture2D tex = Mod.Assets.Request<Texture2D>("Projectiles/Realized/RegretRChain").Value;
				int chainWidth = 8;
				Rectangle frame = Terraria.Utils.Frame(tex, 1, 2);
				Vector2 origin = new Vector2(5, 7);

				int segments = (int)(BezierLength / chainWidth);
				bool draw = true;
				if (segments <= 0)
					draw = false;

				if (draw)
				{
					for (int j = 1; j >= 0; j--)
					{
						float t = 0;
						for (int i = 0; i < segments + 1; i++)
						{
							Vector2 Pos = chainBezier.BezierPoint(t);
							float t1 = t;
							chainBezier.NextApproximatePoint(chainWidth, ref t, steps);
							float t2 = t;

							Color color = Lighting.GetColor((int)(Pos.X / 16f), (int)(Pos.Y / 16f));

							if (i % 2 == j)
							{
								frame.Y = ((i + 1) % 2) * frame.Height;

								Main.EntitySpriteDraw(chainBezier.DrawCurveRotatedtoNext(tex, frame, color, 1f, 0f, origin, 0, t1, t2));
							}
						}
					}
				}
			}
			Vector2 position = Projectile.Center - Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;

			Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, position, null, lightColor, Projectile.rotation, new Vector2(16,16), Projectile.scale, 0, 0);//new Vector2(5, 27)

			return false;
		}
	}
}
