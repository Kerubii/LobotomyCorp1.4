using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using LobotomyCorp.Utils;
using Terraria.GameContent;
using ReLogic.Content;
using Terraria.Graphics.Shaders;
using LobotomyCorp.Players;

namespace LobotomyCorp.Projectiles.Realized
{
	public class MagicBulletRGun : ModProjectile
	{
		public static Asset<Texture2D> MagicBulletPortal;
		public override void Load()
		{
			if (!Main.dedServ)
			{
				MagicBulletPortal = Mod.Assets.Request<Texture2D>("Projectiles/Realized/MagicBulletGunCircle", AssetRequestMode.ImmediateLoad);

				Main.QueueMainThreadAction(() =>
				{
					LobotomyCorp.PremultiplyTexture(MagicBulletPortal.Value);
				});
			}
		}

		public override string Texture => "LobotomyCorp/Items/Ruina/Technology/MagicBulletR";

        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Magic Bullet");
		}

		public override void SetDefaults()
		{
			Projectile.width = 8;
			Projectile.height = 8;
			Projectile.aiStyle = -1;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged; 
			Projectile.penetrate = -1;
			Projectile.timeLeft = 1000;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
		}

		//Style 75
        public override void AI()
        {
			Player player = Main.player[Projectile.owner];

			if (Projectile.ai[0] == 0)
			{
				Projectile.localAI[0] = Main.rand.NextFloat(6.28f);
			}

			if (Main.myPlayer == Projectile.owner && Projectile.ai[0] % player.itemAnimationMax == (int)(player.itemAnimationMax * 0.1f))
            {
				LobotomyWawPlayer modPlayer = player.GetModPlayer<LobotomyWawPlayer>();
				if (modPlayer.MagicBulletRequest >= 0)
                {
					if (modPlayer.MagicBulletNthShot < 6)
                    {
						int bullet = SpawnBulletOnNPC(player, modPlayer.MagicBulletRequest, Projectile.velocity.ToRotation());
						if (Main.npc[modPlayer.MagicBulletRequest].townNPC)
                        {
							Main.projectile[bullet].friendly = false;
							Main.projectile[bullet].hostile = true;
						}
						modPlayer.MagicBulletNthShot++;
                    }
					else
                    {
						int target = SeventhBullet(player);
						int bullet;

						if (target >= 0)
							bullet = SpawnBulletOnNPC(player, target, Projectile.velocity.ToRotation());
						else
						{
							target = SeventhBulletAlly(player);
							if (target >= 0)
								bullet = SpawnBulletOnPlayer(Main.player[target], Projectile.velocity.ToRotation());
							else
								bullet = SpawnBulletOnPlayer(player, Projectile.velocity.ToRotation());
						}
						Main.projectile[bullet].friendly = false;
						Main.projectile[bullet].hostile = true;
						player.AddBuff(ModContent.BuffType<Buffs.DarkFlame>(), 900);
						modPlayer.MagicBulletNthShot = 0;
					}
                }
				else
				{
					int shotType = modPlayer.MagicBulletNthShot;

					switch (shotType + 1)
					{
						case 1:
							int target = FirstSixthBullet(player);

							if (target >= 0)
							{
								SpawnBulletOnNPC(player, target, Projectile.velocity.ToRotation());

								modPlayer.MagicBulletNthShot++;
							}
							break;
						case 2:
							int[] targets = SecondBullet(player);

							bool shoot = false;
							for (int i = 0; i < targets.Length; i++)
							{
								if (targets[i] >= 0)
								{
									SpawnBulletOnNPC(player, targets[i], Projectile.velocity.ToRotation());

									shoot = true;
								}
							}
							if (shoot)
								modPlayer.MagicBulletNthShot++;
							break;
						case 3:
							targets = ThirdBullet(player);

							shoot = false;
							for (int i = 0; i < targets.Length; i++)
							{
								if (targets[i] >= 0)
								{
									SpawnBulletOnNPC(player, targets[i], Projectile.velocity.ToRotation());

									shoot = true;
								}
							}
							if (shoot)
								modPlayer.MagicBulletNthShot++;
							break;
						case 4:
							targets = FourthFifthBullet(player);

							shoot = false;
							for (int i = 0; i < targets.Length; i++)
							{
								if (targets[i] >= 0)
								{
									SpawnBulletOnNPC(player, targets[i], Projectile.velocity.ToRotation());

									shoot = true;
								}
							}
							if (shoot)
								modPlayer.MagicBulletNthShot++;
							break;
						case 5:
							targets = FourthFifthBullet(player);

							shoot = false;
							for (int i = 0; i < targets.Length; i++)
							{
								if (targets[i] >= 0)
								{
									SpawnBulletOnNPC(player, targets[i], Projectile.velocity.ToRotation());

									shoot = true;
								}
							}
							if (shoot)
								modPlayer.MagicBulletNthShot++;
							break;
						case 6:
							target = FirstSixthBullet(player);

							if (target >= 0)
							{
								for (int i = 0; i < 5; i++)
									SpawnBulletOnNPC(player, target, Projectile.velocity.ToRotation() + MathHelper.ToRadians(72 * i));

								modPlayer.MagicBulletNthShot++;
							}

							break;
						case 7:
							target = SeventhBullet(player);
							int bullet;

							if (target >= 0)
								bullet = SpawnBulletOnNPC(player, target, Projectile.velocity.ToRotation());
							else
							{
								target = SeventhBulletAlly(player);
								if (target >= 0)
									bullet = SpawnBulletOnPlayer(Main.player[target], Projectile.velocity.ToRotation());
								else
									bullet = SpawnBulletOnPlayer(player, Projectile.velocity.ToRotation());
							}
							Main.projectile[bullet].friendly = false;
							Main.projectile[bullet].hostile = true;
							player.AddBuff(ModContent.BuffType<Buffs.DarkFlame>(), 900);
							modPlayer.MagicBulletNthShot = 0;
							break;
					}
				}
			}
			else if (player.ownedProjectileCounts[ModContent.ProjectileType<MagicBulletSpawner>()] > 0 && Projectile.ai[0] % player.itemAnimationMax == (int)(player.itemAnimationMax * 0.1f) + 15)
            {
				Vector2 positionOffset = new Vector2(1, 0).RotatedBy(Projectile.velocity.ToRotation());
				Vector2 position = Projectile.Center + positionOffset * 120;

				Dust.NewDustPerfect(position, DustID.GemSapphire, positionOffset.RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f)) * 6).noGravity = true;
            }

			if ((!player.channel && Projectile.ai[0] % player.itemAnimationMax == 0) || player.dead)
			{
				Projectile.Kill();
			}
			Projectile.ai[0]++;
			Projectile.localAI[0] += MathHelper.ToRadians(5) * Projectile.direction;

			//Holding Projectile
			float num = (float)Math.PI;
			if (Projectile.direction == 1)
				num = 0;

			Projectile.direction = Math.Sign(Projectile.velocity.X);
			Projectile.position = player.RotatedRelativePoint(player.MountedCenter, false, false) - Projectile.Size / 2f;
			Projectile.rotation = Projectile.velocity.ToRotation() + num;
			Projectile.spriteDirection = Projectile.direction;
			Projectile.timeLeft = 2;

			player.ChangeDir(Projectile.direction);
			player.heldProj = Projectile.whoAmI;
			player.SetDummyItemTime(2);
			player.itemRotation = MathHelper.WrapAngle((float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction));
        }

		private int SpawnBulletOnNPC(Player player, int target, float rotation)
        {
			NPC npc = Main.npc[target];
			Vector2 velocityRotation = new Vector2(1, 0).RotatedBy(rotation);
			Vector2 spawnerPosition = npc.Center - velocityRotation * 128f;

			return Projectile.NewProjectile(player.GetSource_FromThis(), spawnerPosition, velocityRotation * 12f, ModContent.ProjectileType<MagicBulletSpawner>(), Projectile.damage, Projectile.knockBack, Projectile.owner, target + 1);
		}

		private int SpawnBulletOnPlayer(Player player, float rotation)
        {
			Vector2 velocityRotation = new Vector2(1, 0).RotatedBy(rotation);
			Vector2 spawnerPosition = player.Center - velocityRotation * 128f;

			return Projectile.NewProjectile(player.GetSource_FromThis(), spawnerPosition, velocityRotation * 12f, ModContent.ProjectileType<MagicBulletSpawner>(), Projectile.damage, Projectile.knockBack, player.whoAmI);
		}

		//Magic Bullet - Targets Nearest to Cursor
		//Inevitable Bullet - Targets 4 nearby enemies except nearest to Cursor, when none near target nearest
		//Captivating Bullet - Targets 3 Random enemies
		//Ruthless Bullet - Targets 5 nearest enemies ignoring defense
		//Silent Bullet - Targets 5 nearest enemies inflicting debuffs
		//Flooding Bullets - Target Nearest to Cursor and shoots them 5 times
		//Bullet of Despair - Targets Town NPCs and yourself
		private readonly int EFFECTIVEDISTANCE = 2500;

		private int FirstSixthBullet(Player player)
		{
			int target = -1;
			float distance = EFFECTIVEDISTANCE;
			Vector2 compareTo = Main.MouseWorld;
			foreach (NPC n in Main.npc)
			{
				if (n.active && !n.friendly && !n.dontTakeDamage && n.Center.Distance(compareTo) < distance && n.CanBeChasedBy(this))
				{
					distance = n.Center.Distance(compareTo);
					target = n.whoAmI;
				}
			}

			return target;
		}

		private int[] SecondBullet(Player player)
		{
			Dictionary<int, int> validTargetList = new Dictionary<int, int>();
			float distance = EFFECTIVEDISTANCE;
			Vector2 compareTo = Main.MouseWorld;
			foreach (NPC n in Main.npc)
			{
				if (n.active && !n.friendly && !n.dontTakeDamage && n.Center.Distance(compareTo) < distance && n.CanBeChasedBy(this))
				{
					int localDistance = (int)n.Center.Distance(compareTo);
					validTargetList.Add(n.whoAmI, localDistance);
				}
			}
			if (validTargetList.Count > 1)
			{
				var SortedList = from x in validTargetList orderby x.Value ascending select x;
				validTargetList = SortedList.ToDictionary(x => x.Key, x => x.Value);
			}
			int[] targets = new int[4] { -1, -1, -1, -1 };
			for (int i = 0; i < 4; i++)
            {
				if (i > validTargetList.Count - 2)
					targets[i] = -1;
				else
					targets[i] = validTargetList.ElementAt(i + 1).Key;
            }
			if (targets[0] < 0)
				targets[0] = FirstSixthBullet(player);
			return targets;
		}

		private int[] ThirdBullet(Player player)
        {
			int[] target = new int[3] {-1, -1, -1};
			List<int> validTargetList = new List<int>();
			float distance = EFFECTIVEDISTANCE;
			foreach (NPC n in Main.npc)
			{
				if (n.active && !n.friendly && !n.dontTakeDamage && n.Center.Distance(player.Center) < distance && n.CanBeChasedBy(this))
				{
					validTargetList.Add(n.whoAmI);
				}
			}
			if (validTargetList.Count > 0)
			{
				for (int i = 0; i < 3; i++)
				{
					int randomSelect = validTargetList[Main.rand.Next(validTargetList.Count)];
					validTargetList.Remove(randomSelect);
					target[i] = randomSelect;
					if (validTargetList.Count == 0)
						break;
				}
			}

			return target;
		}

		private int[] FourthFifthBullet(Player player)
		{
			Dictionary<int, int> validTargetList = new Dictionary<int, int>();
			float distance = EFFECTIVEDISTANCE;
			Vector2 compareTo = Main.MouseWorld;
			foreach (NPC n in Main.npc)
			{
				if (n.active && !n.friendly && !n.dontTakeDamage && n.Center.Distance(compareTo) < distance && n.CanBeChasedBy(this))
				{
					int localDistance = (int)n.Center.Distance(player.Center);
					validTargetList.Add(n.whoAmI, localDistance);
				}
			}
			if (validTargetList.Count > 1)
			{
				var SortedList = from x in validTargetList orderby x.Value ascending select x;
				validTargetList = SortedList.ToDictionary(x => x.Key, x => x.Value);
			}
			int[] targets = new int[5] {-1, -1, -1, -1, -1};
			for (int i = 0; i < targets.Length; i++)
			{
				if (i > validTargetList.Count - 1)
					targets[i] = -1;
				else
					targets[i] = validTargetList.ElementAt(i).Key;
			}
			return targets;
		}

		private int SeventhBullet(Player player)
        {
			int target = -1;
			foreach (NPC n in Main.npc)
			{
				if (n.active && n.friendly && !n.dontTakeDamage && n.townNPC && !NPCBlackList(n.type))
				{
					target = n.whoAmI;
				}
			}

			return target;
		}

		private int SeventhBulletAlly(Player player)
        {
			int target = -1;
			float distance = EFFECTIVEDISTANCE;
			if (Main.netMode == NetmodeID.SinglePlayer)
				return -1;
			foreach (Player p in Main.ActivePlayers)
            {
				if (!p.dead && player.whoAmI != p.whoAmI && p.team == player.team && p.Center.Distance(player.Center) < distance)
                {
					distance = p.Center.Distance(player.Center);
					target = p.whoAmI;
				}
            }
			return target;
        }

		private void FocusedBullet(Player player)
        {

        }

		private bool NPCBlackList(int type)
        {
			if (type == NPCID.OldMan)
				return true;
			else if (type == NPCID.SkeletonMerchant)
				return true;
			return false;
        }

		private static Vector2 GunOffset = new Vector2(-50, 0);

        public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
			Rectangle frame = texture.Frame();
			Vector2 origin = frame.Size() / 2f + GunOffset * Projectile.spriteDirection;
			Vector2 position = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
			Color color = lightColor;
			Main.EntitySpriteDraw(texture, position, frame, color, Projectile.rotation, origin, Projectile.scale, Projectile.spriteDirection >= 0 ? 0 : SpriteEffects.FlipHorizontally, 0);

			Texture2D tex = MagicBulletPortal.Value;
			float rot = Projectile.velocity.ToRotation();
			Vector2 positionOffset = new Vector2(1, 0).RotatedBy(rot);

			position = Projectile.Center + positionOffset * 124;
			frame = tex.Frame();
			origin = frame.Size() / 2;
			Vector2 scale = new Vector2(0.3f, 1f) * Projectile.scale;
			color = Color.White;

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

			float rotate = Projectile.localAI[0] / (2f * (float)Math.PI);

			var resizeShader = GameShaders.Misc["LobotomyCorp:Rotate"];
			resizeShader.UseOpacity(1f);
			resizeShader.UseShaderSpecificData(LobotomyCorp.ShaderRotation(rotate));
			resizeShader.Apply(null);

			scale *= 0.75f;

			Main.EntitySpriteDraw(tex, position - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, (Rectangle?)(frame), color, rot, origin, scale * 0.5f, SpriteEffects.None, 0);

			position += positionOffset * 20f;
			Main.EntitySpriteDraw(tex, position - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, (Rectangle?)(frame), color, rot, origin, scale, SpriteEffects.None, 0);

			position += positionOffset * 13f;
			Main.EntitySpriteDraw(tex, position - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, (Rectangle?)(frame), color, rot, origin, scale * 0.8f, SpriteEffects.None, 0);

			position += positionOffset * 16f;
			Main.EntitySpriteDraw(tex, position - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, (Rectangle?)(frame), color, rot, origin, scale * 0.3f, SpriteEffects.None, 0);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

			return false;
		}

        public override bool? CanHitNPC(NPC target)
        {
			return false;
        }

        public override bool CanHitPvp(Player target)
        {
			return false;
        }
    }
}
