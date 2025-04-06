using LobotomyCorp.Items.He;
using LobotomyCorp.Items.Teth;
using LobotomyCorp.Players;
using LobotomyCorp.Projectiles.Realized;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Ruina
{
    public class LifeForADaredevilR : SEgoItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Life For a Daredevil"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault(GetTooltip());
		}

		public override void SetDefaults()
		{
			EgoColor = LobotomyCorp.HeRarity;

			Item.damage = 72;
			Item.DamageType = DamageClass.Melee;
			Item.width = 40;
			Item.height = 40;

			Item.useTime = 42;
			Item.useAnimation = 42;

			Item.useStyle = 1;// ItemUseStyleID.Shoot;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = 3;

			Item.shootSpeed = 1f;
			Item.shoot = ModContent.ProjectileType<Projectiles.Realized.LifeForADaredevilR>();

			Item.noUseGraphic = true;
			Item.UseSound = null;// new SoundStyle("LobotomyCorp/Sounds/Item/Armor_Down");
			//Item.noMelee = true;
			Item.channel = true;
			//Item.holdStyle = 7;

			PreviousTarget = -1;
		}

        public override float UseAnimationMultiplier(Player player)
        {
			if (player.altFunctionUse == 2)
				return 1.75f;

            return base.UseAnimationMultiplier(player);
        }

        public override float UseTimeMultiplier(Player player)
        {
            if (player.altFunctionUse == 2)
                return 1.75f;

            return base.UseTimeMultiplier(player);
        }

        public override bool AltFunctionUse(Player player)
        {
			return player.GetModPlayer<LobotomyHePlayer>().LifeForADareDevilGiftActive;
        }

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			if (player.altFunctionUse == 2)
				type = ModContent.ProjectileType<LifeForADaredevilRAlt>();
		}

        public override bool SafeCanUseItem(Player player)
        {
            player.GetModPlayer<LobotomyHePlayer>().LifeForADareDevilCounterStance = false;

			return base.SafeCanUseItem(player);
        }

        public override void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
			noHitbox = player.itemAnimation >= player.itemAnimationMax || player.ownedProjectileCounts[Item.shoot] == 0 || player.GetModPlayer<LobotomyHePlayer>().LifeForADareDevilCounterStance;

			hitbox = new Rectangle((int)player.itemLocation.X, (int)player.itemLocation.Y, 32, 32);
			if (!Main.dedServ)
			{
				Rectangle hitboxSize = Item.GetDrawHitbox(Item.type, player);
				hitbox = new Rectangle((int)player.position.X, (int)player.position.Y, hitboxSize.Width, hitboxSize.Height);
			}
			float adjustedItemScale = player.GetAdjustedItemScale(Item);
			hitbox.Width = (int)((float)hitbox.Width * adjustedItemScale);
			hitbox.Height = (int)((float)hitbox.Height * adjustedItemScale);
			if (player.direction == -1)
			{
				hitbox.X -= (hitbox.Width - player.width);
			}
			if (player.gravDir == 1f)
			{
				hitbox.Y -= hitbox.Height;
			}

			if (player.itemAnimation < player.itemAnimationMax / 2)
			{
				float prog = 1f - player.itemAnimation / (player.itemAnimationMax / 2f);
				if (prog < 0.1f)
				{
					hitbox.Y += (int)(hitbox.Height * 0.33f);
					hitbox.Height = (int)(hitbox.Height * 0.66f);
				}
				else if (prog < 0.4f)
				{
					if (player.direction == -1)
					{
						hitbox.X -= hitbox.Width;
					}
					hitbox.Y += hitbox.Height / 2;
					hitbox.X += 16 * player.direction;
					hitbox.Width *= 2;
				}
				else
                {
					if (player.direction == -1)
					{
						hitbox.X -= hitbox.Width/2;
					}
					hitbox.Y += hitbox.Height;
					hitbox.X += 16 * player.direction;
					hitbox.Width = (int)(hitbox.Width * 1.5f);
					if (prog > 0.66f)
						noHitbox = true;
				}
			}
			else
            {
				if (player.direction == -1)
				{
					hitbox.X -= hitbox.Width;
				}
				hitbox.Y += hitbox.Height / 2;
				hitbox.X += 16 * player.direction;
				hitbox.Width *= 2;
			}

            base.UseItemHitbox(player, ref hitbox, ref noHitbox);
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
			if (player.itemAnimation > player.itemAnimationMax / 2)
			{
				PreviousTarget = target.whoAmI;
			}
			else
			{
                player.GetModPlayer<LobotomyHePlayer>().LifeForADareDevilPierceEffect(target.Center, 20, 20);
			}
        }

        private int PreviousTarget = -1;

        public override void HoldItem(Player player)
        {
			if (PreviousTarget >= 0)
			{
				LobCorpLight.ResetPlayerImmuneHit(player, ref PreviousTarget, player.itemAnimationMax / 2);
			}
		}

		

        public override void UseItemFrame(Player player)
		{
			if (player.channel)
				player.bodyFrame.Y = player.bodyFrame.Height * 2;
		}

        public override void HoldStyle(Player player, Rectangle heldItemFrame)
        {
			if (player.altFunctionUse != 2)
				player.itemLocation = player.position;
			//heldItemFrame.X = (int)player.position.X;

			//heldItemFrame.Y = (int)player.position.Y;
			base.HoldStyle(player, heldItemFrame);
        }

        public override bool? UseItem(Player player)
        {
            return base.UseItem(player);
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
		{
			if (player.altFunctionUse == 2)
				Item.useStyle = ItemUseStyleID.Shoot;
			else
			{
                if (player.ownedProjectileCounts[Item.shoot] != 0)
                {
                    if (player.itemAnimation == player.itemAnimationMax - 2)
                        SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Armor_Cut") with { Pitch = Main.rand.NextFloat(0.05f, 0.15f) }, player.position);
                    else if (player.itemAnimation == player.itemAnimationMax / 2 - 1)
                        SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Armor_Cut") with { Pitch = Main.rand.NextFloat(-0.15f, -0.05f) }, player.position);

                }
                Item.useStyle = 1;
			}
            base.UseStyle(player, heldItemFrame);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
               .AddIngredient(ModContent.ItemType<LifeForADaredevil>())
			   .AddIngredient(ItemID.OrichalcumBar)
               .AddIngredient(ItemID.SoulofNight)
			   .AddIngredient(ItemID.SoulofLight)
			   .AddIngredient(ItemID.SoulofFlight)
               .AddTile<Tiles.BlackBox3>()
               .AddCondition(RedMistCond)
               .Register();

            CreateRecipe()
               .AddIngredient(ModContent.ItemType<LifeForADaredevil>())
               .AddIngredient(ItemID.MythrilBar)
               .AddIngredient(ItemID.SoulofNight)
               .AddIngredient(ItemID.SoulofLight)
               .AddIngredient(ItemID.SoulofFlight)
               .AddTile<Tiles.BlackBox3>()
			   .AddCondition(RedMistCond)
               .Register();
        }
    }
}