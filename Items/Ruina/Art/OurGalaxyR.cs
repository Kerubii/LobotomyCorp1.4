using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;
using System;

namespace LobotomyCorp.Items.Ruina.Art
{
    public class OurGalaxyR : SEgoItem
	{
		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Our Galaxy");
			// Tooltip.SetDefault(GetTooltip());
		}

		public override void SetDefaults() 
		{
			EgoColor = LobotomyCorp.HeRarity;

			Item.width = 64;
			Item.height = 64;

			Item.damage = 90;
			Item.DamageType = DamageClass.Magic;
			Item.knockBack = 1f;
			Item.useTime = 11;
			Item.reuseDelay = 48;
			Item.useAnimation = 33;
			Item.useStyle = ItemUseStyleID.Shoot;

			Item.value = 100000;
            Item.noMelee = true;
            Item.noUseGraphic = true;
			Item.UseSound = new SoundStyle("LobotomyCorp/Sounds/Item/Art/Galaxy_Strong_Big_Shot") with { Volume = 0.25f };
            Item.autoReuse = true;
			Item.rare = ModContent.RarityType<HeR>();

			Item.shoot = ModContent.ProjectileType<Projectiles.Realized.OurGalaxyComet>();
			Item.shootSpeed = 14;
		}

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
			if (player.whoAmI == Main.myPlayer)
            {
				int rotation = Main.rand.Next(80, 100);
				Vector2 speed = new Vector2(velocity.Length(), 0).RotatedBy(MathHelper.ToRadians(rotation));

				position = Main.MouseWorld + new Vector2(Main.rand.Next(-16, 17), Main.rand.Next(-16, 17)) - speed * 60;
				velocity = speed;
			}

            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<He.OurGalaxy>())
			.AddIngredient(ItemID.PearlstoneBlock, 50)
			.AddIngredient(ItemID.FallenStar, 6)
            .AddIngredient(ItemID.HallowedBar, 2)
            .AddTile<Tiles.BlackBox3>()
            .AddCondition(RedMistCond)
            .Register();
        }
    }
}