using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;
using System;
using LobotomyCorp.Items.Teth;
using LobotomyCorp.Players;
using LobotomyCorp.Buffs;

namespace LobotomyCorp.Items.Ruina.Literature
{
    public class RedEyesR : SEgoItem
	{
		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Red Eyes"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault(GetTooltip());
		}

		public override void SetDefaults() 
		{
			EgoColor = LobotomyCorp.TethRarity;

			Item.width = 70;
			Item.height = 70;

			Item.damage = 62;
			Item.DamageType = DamageClass.Melee;
			Item.knockBack = 2.3f;
			Item.useTime = 26;
			Item.useAnimation = 26;
			Item.useStyle = 1;

			Item.value = 10000;
            Item.noMelee = false;
            Item.noUseGraphic = false;
			Item.UseSound = null;//SoundID.Item1;
			Item.autoReuse = true;
			Item.rare = ModContent.RarityType<TethR>();

			Item.shoot = ModContent.ProjectileType<Projectiles.Realized.RedEyesSlash>();
			Item.shootSpeed = 1f;
			angle = 0f;
		}

        public override bool AltFunctionUse(Player player)
        {
			return true;
        }

		public static int NearestCryOutTarget(Vector2 position, float distance = 10000)
        {
			LobotomyTethPlayer tethPlayer = Main.LocalPlayer.GetModPlayer<LobotomyTethPlayer>();
            int target = -1;
			bool isMeal = false;
			foreach(NPC n in Main.ActiveNPCs)
            {
				if (!n.dontTakeDamage)
                {
					LobotomyGlobalNPC modNPC = n.GetGlobalNPC<LobotomyGlobalNPC>();
					float dist = Vector2.Distance(n.Center, position);
					bool isMealTarget = modNPC.RedEyesMealAmount > tethPlayer.RedEyesMealMax;
					if (!isMeal && isMealTarget)
					{
						isMeal = true;
						distance = dist;
						target = n.whoAmI;
					}
					else if ((isMealTarget || (!isMeal && tethPlayer.RedEyesAlerted)) && dist < distance)
					{
                        distance = dist;
                        target = n.whoAmI;
                    }
                }
            }
			return target;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			if (player.altFunctionUse != 2)
			{
				int slashType = 1;
				if (Item.useStyle == 15)
				{
					slashType = 0;
					velocity = new Vector2(1f * Math.Sign(velocity.X), 0);
					damage = 0;
				}
				Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, slashType);

				return false;
			}           
			return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
			if (player.altFunctionUse == 2)
                type = ModContent.ProjectileType<Projectiles.Realized.Spiderbud>();
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
        }

        private float angle = 0f;
        public override bool SafeCanUseItem(Player player)
        {
			if (player.MountedCenter.Distance(Main.MouseWorld) > 240)
            {
				angle = (Main.MouseWorld - player.MountedCenter).ToRotation();
				Item.useStyle = 16;
            }
			else
            {
				Item.useStyle = 15;
            }

            return base.SafeCanUseItem(player);
        }

        public override bool? UseItem(Player player)
        {
			return player.ItemTimeIsZero;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
			if (player.altFunctionUse == 2)
			{
				if (player.itemAnimation <= player.itemAnimationMax / 2)
				{
					if (player.itemAnimation == player.itemAnimationMax / 2 - 2)
					{
						SoundEngine.PlaySound(LobotomyCorp.ItemLobSound("Literature/Spidermom_Strong_Hori", 1, 1, 0, 0.5f), player.Center);
					}
					float progress = 1f - (player.itemAnimation + 1) / (player.itemAnimationMax * 0.5f);
					player.itemLocation.X = player.position.X + player.width * 0.5f + (heldItemFrame.Width * 0.5f - 36) * (float)player.direction;
					player.itemLocation.Y = player.position.Y + 26f + player.HeightOffsetHitboxCenter;
					if (progress < 0.25f)
					{
						progress = progress / 0.25f;
						player.itemRotation = (-0.785f + 1.658f * (float)Math.Sin(1.57f * progress)) * player.direction;
					}
					else
						player.itemRotation = 0.872f * player.direction;
				}
				else
				{
					player.itemLocation.X = player.position.X + (float)player.width * 0.5f - ((float)heldItemFrame.Width * 0.5f - 36f) * (float)player.direction;
					player.itemLocation.Y = player.position.Y + 6f + player.HeightOffsetHitboxCenter;
					player.itemRotation = 1.57f * player.direction;
				}
			}
			else
            {
				if (player.itemAnimation == player.itemAnimationMax - 1)
				{
					SoundEngine.PlaySound(LobotomyCorp.ItemLobSound("Literature/Spidermom_Hit", 2, 1, 0, 0.5f), player.Center);
				}
				float rotation = LobCorpLight.ItemRotation(player);
				if (Item.useStyle == 16)
				{
					rotation = AltRotation(player);
				}
				LobCorpLight.PseudoUseStyleSwing(player, heldItemFrame, rotation);
            }
			LobCorpLight.ResetPlayerAttackCooldown(player, 0.05);
			base.UseStyle(player, heldItemFrame);
        }

		public float AltRotation(Player player)
        {
			float prog = 1f - player.itemAnimation / (float)player.itemAnimationMax;
			float rotation = 0;

			if (prog < 0.4f)
			{
				prog = prog / 0.4f;
				rotation = (-60 + 285 * (float)Math.Sin(1.57f * prog));// * player.direction;
			}
			else if (prog < 0.5f)
			{
				rotation = 255;// * player.direction;
			}
			else
			{
				prog = (prog - 0.5f) / 0.5f;
				rotation = (255 - 45 * prog);// * player.direction;
			}
			float additionalAngle = angle;
				additionalAngle = (float)Math.Atan2(Math.Sin(angle), Math.Cos(angle) * player.direction);

			rotation += MathHelper.ToDegrees(additionalAngle);
			//if (player.direction < 0)
				//rotation -= 180;
			return rotation;
		}

        public override void UseItemFrame(Player player)
        {
			if (player.altFunctionUse == 2)
			{
				if (player.itemAnimation <= player.itemAnimationMax / 2)
				{
					player.bodyFrame.Y = player.bodyFrame.Height * 4;
				}
				else
				{
					player.SetCompositeArmFront(enabled: true, Player.CompositeArmStretchAmount.Full, MathHelper.ToRadians(45 - (player.direction < 0 ? 90 : 0)));
				}
			}
			else
			{
				float rotation = LobCorpLight.ItemRotation(player);
				if (Item.useStyle == 16)
				{
					rotation = AltRotation(player);
				}
				LobCorpLight.LobItemFrame(player, rotation - 90);
			}
		}

        public override void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
			if (player.altFunctionUse == 2)
			{
				if (player.itemAnimation <= player.itemAnimationMax / 4)
				{
					if (player.direction == -1)
					{
						hitbox.X -= (int)((double)hitbox.Width * 1.4 - (double)hitbox.Width);
					}
					hitbox.Width = (int)((double)hitbox.Width * 1.4);
					hitbox.Y += (int)((double)hitbox.Height * 0.4 * (double)player.gravDir);
					hitbox.Height = (int)((double)hitbox.Height * 0.8);
				}
				else
					noHitbox = true;
			}
			else if (Item.useStyle == 15)
			{
				hitbox = new Rectangle((int)player.itemLocation.X, (int)player.itemLocation.Y, 32, 32);
				if (!Main.dedServ)
				{
					Rectangle hitboxSize = Item.GetDrawHitbox(Item.type, player);
					hitbox = new Rectangle((int)player.itemLocation.X, (int)player.itemLocation.Y, hitboxSize.Width, hitboxSize.Height);
				}
				float adjustedItemScale = player.GetAdjustedItemScale(Item);
				hitbox.Width = (int)((float)hitbox.Width * adjustedItemScale);
				hitbox.Height = (int)((float)hitbox.Height * adjustedItemScale);
				if (player.direction == -1)
				{
					hitbox.X -= hitbox.Width;
				}
				if (player.gravDir == 1f)
				{
					hitbox.Y -= hitbox.Height;
				}

				float prog = 1f - player.itemAnimation / (float)player.itemAnimationMax;
				if (prog < .2f)
				{
					if (player.direction == 1)
					{
						hitbox.X -= (int)(hitbox.Width * 1f);
					}
					hitbox.Width *= 2;
					hitbox.Y -= (int)((hitbox.Height * 1.7 - hitbox.Height) * player.gravDir);
					hitbox.Height = (int)(hitbox.Height * 1.7);
				}
				else if (prog < .4f)
				{
					if (player.direction == -1)
					{
						hitbox.X -= (int)((double)hitbox.Width * 1.95 - (double)hitbox.Width);
					}
					hitbox.Width = (int)((double)hitbox.Width * 1.95);
					hitbox.Y += (int)((double)hitbox.Height * 0.5 * (double)player.gravDir);
					hitbox.Height = (int)((double)hitbox.Height * 1.4);
				}
				else
					noHitbox = true;
			}
			else
				noHitbox = true;

            //base.UseItemHitbox(player, ref hitbox, ref noHitbox);
        }

        public override void HoldItem(Player player)
        {
			if (Main.myPlayer == player.whoAmI)
			{
				foreach (NPC n in Main.ActiveNPCs)
				{
					if (!(n.life > 0 && n.GetGlobalNPC<LobotomyGlobalNPC>().RedEyesCocoonPlayer == player.whoAmI && n.GetGlobalNPC<LobotomyGlobalNPC>().RedEyesCocoonCooldown <= 0))
						continue;
					n.GetGlobalNPC<LobotomyGlobalNPC>().RedEyesCocoonCooldown = 20;
					Vector2 vel = new Vector2(6, 0).RotatedByRandom(6.28f);
					Vector2 spawnArea = n.Center - vel * 120;

					Projectile.NewProjectile(player.GetSource_FromThis(), spawnArea, vel, ModContent.ProjectileType<Projectiles.Realized.Spiderling>(), Item.damage / 3 * 2, Item.knockBack, player.whoAmI, n.whoAmI);
				}
			}
		}

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
		{
			bool predator = player.GetModPlayer<LobotomyTethPlayer>().RedEyesPredator;
			LobotomyGlobalNPC modNPC = target.GetGlobalNPC<LobotomyGlobalNPC>();
			if (player.altFunctionUse == 2)
			{
                bool alert = player.GetModPlayer<LobotomyTethPlayer>().RedEyesAlerted;
                if (modNPC.RedEyesMealAmount > player.GetModPlayer<LobotomyTethPlayer>().RedEyesMealMax)
				{
                    target.AddBuff(ModContent.BuffType<Buffs.Cocoon>(), 600);
                    modNPC.RedEyesCocoon = true;
                    modNPC.RedEyesCocoonPlayer = player.whoAmI;
                }
				if (alert)
				{
					// Give Predator Buff if Alertness is active
					player.AddBuff(ModContent.BuffType<Predator>(), 60 * 30);
				}
			}
			if (target.realLife >= 0)
				Main.npc[target.realLife].GetGlobalNPC<LobotomyGlobalNPC>().RedEyesApplyMeal(60, predatorBoosted: predator);
			else
				modNPC.RedEyesApplyMeal(60, predatorBoosted: predator);

			int dustAmount = Main.rand.Next(4, 8);
			for (int i = 0; i < dustAmount; i++)
			{
				Vector2 speed = target.Center - player.Center;
				speed.Normalize();
				speed *= Main.rand.Next(3, 6);
				Dust d = Dust.NewDustPerfect(target.Center, DustID.Wraith, speed.RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)), 0, default, Main.rand.NextFloat(1f, 2f));
				d.noGravity = true;
			}
        }

        public override bool? CanHitNPC(Player player, NPC target)
        {
			if (player.altFunctionUse == 2)
            {
				LobotomyGlobalNPC modNPC = target.GetGlobalNPC<LobotomyGlobalNPC>();
				bool alert = player.GetModPlayer<LobotomyTethPlayer>().RedEyesAlerted;
                if ((modNPC.RedEyesMealAmount > player.GetModPlayer<LobotomyTethPlayer>().RedEyesMealMax || alert) && target.immune[player.whoAmI] <= 0)
					return true;
				else
					return false;
            }
            return base.CanHitNPC(player, target);
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            if (player.altFunctionUse == 2)
            {
				damage *= 3;
            }
        }

        public override void AddRecipes() 
		{
			CreateRecipe()
			.AddIngredient(ModContent.ItemType<RedEyes>())
			.AddIngredient(ItemID.SpiderFang, 8)
			.AddIngredient(ItemID.CobaltSword)
			.AddTile<Tiles.BlackBox3>()
            .AddCondition(RedMistCond)
            .Register();

			CreateRecipe()
			.AddIngredient(ModContent.ItemType<RedEyes>())
			.AddIngredient(ItemID.SpiderFang, 8)
			.AddIngredient(ItemID.PalladiumSword)
			.AddTile<Tiles.BlackBox3>()
            .AddCondition(RedMistCond)
            .Register();
		}
	}
}