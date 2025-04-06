using LobotomyCorp.Items.He;
using LobotomyCorp.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Ruina.Technology
{
    public class HarmonyR : SEgoItem
	{
		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Harmony"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault(GetTooltip());
			ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
			ItemID.Sets.IsChainsaw[Item.type] = true;
		}

		public override void SetDefaults() 
		{
			EgoColor = LobotomyCorp.HeRarity;

            Item.damage = 38;
			Item.DamageType = DamageClass.Melee;
			Item.width = 40;
			Item.height = 40;

			Item.useTime = 5;
			Item.useAnimation = 20;

			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = 3;
            Item.shootSpeed = 3.7f;
            Item.shoot = ModContent.ProjectileType<Projectiles.HarmonyS>();

            Item.noUseGraphic = true;
			Item.UseSound = SoundID.Item1;
            Item.noMelee = true;
			Item.autoReuse = true;
            Item.channel = true;

			Item.tileBoost = 3;
			Item.axe = 22;
		}

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
			if (player.altFunctionUse == 2)
				damage = (int)(damage * 0.7f);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			if (Main.myPlayer == player.whoAmI && type == ModContent.ProjectileType<Projectiles.HarmonyShotR>() && player.GetModPlayer<LobotomyHePlayer>().HarmonyAddiction)
            {
				for (int i = -1; i < 2; i += 2)
				{
					Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, i);
				}
			}
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }

        public override float UseTimeMultiplier(Player player)
        {
			if (player.altFunctionUse == 2)
				return 6;

            return base.UseTimeMultiplier(player);
        }

        public override bool SafeCanUseItem(Player player)
        {
			if (player.altFunctionUse == 2)
			{
				Item.shootSpeed = 16;
				Item.shoot = ModContent.ProjectileType<Projectiles.HarmonyShotR>();
				Item.UseSound = new SoundStyle("LobotomyCorp/Sounds/Item/Singing_Shot") with { Volume = 0.5f, MaxInstances = 1, SoundLimitBehavior = SoundLimitBehavior.IgnoreNew};
				Item.useStyle = ItemUseStyleID.Guitar;
				Item.noUseGraphic = false;
				return true;
			}
			else
			{
				Item.shootSpeed = 3.7f;
				Item.shoot = ModContent.ProjectileType<Projectiles.HarmonyS>();
				Item.UseSound = SoundID.Item1;
				Item.useStyle = ItemUseStyleID.Shoot;
				Item.noUseGraphic = true;
			}
			return player.ownedProjectileCounts[Item.shoot] < 1;
		}

        public override bool AltFunctionUse(Player player)
        {
			return true;
        }

        public override void AddRecipes() 
		{
			CreateRecipe()
			.AddIngredient(ModContent.ItemType<Harmony>())
			.AddIngredient(ItemID.HallowedBar, 5)
			.AddIngredient(ItemID.CarbonGuitar)
			.AddIngredient(ItemID.Chain, 12)
			.AddTile<Tiles.BlackBox3>()
            .AddCondition(RedMistCond)
            .Register();
		}
	}
}