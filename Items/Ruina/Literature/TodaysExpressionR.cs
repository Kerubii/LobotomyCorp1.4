using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;
using LobotomyCorp.Items.Teth;
using LobotomyCorp.Players;

namespace LobotomyCorp.Items.Ruina.Literature
{
    public class TodaysExpressionR : SEgoItem
	{
		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Today's Expression"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault(GetTooltip());
		}

		public override void SetDefaults() 
		{
			EgoColor = LobotomyCorp.TethRarity;

			Item.width = 40;
			Item.height = 40;

			Item.damage = 44;
			Item.DamageType = DamageClass.Magic; LobItemBase.ConvertVanillaDamageToExtractor(Item);
            Item.knockBack = 6;
			Item.useTime = 26;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.mana = 8;

			Item.value = 10000;
            Item.noMelee = true;
            Item.noUseGraphic = true;
			//Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.rare = ModContent.RarityType<TethR>();

			Item.shoot = ModContent.ProjectileType<Projectiles.Realized.TodaysExpressionWall>();
			Item.shootSpeed = 16f;
		}

        public override bool SafeCanUseItem(Player player)
        {
			if (player.altFunctionUse != 2)
            {
				return player.GetModPlayer<LobotomyTethPlayer>().TodaysExpressionActive;
            }

			return base.SafeCanUseItem(player);
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool? UseItem(Player player)
        {
			if (player.altFunctionUse == 2)
            {
				player.AddBuff(ModContent.BuffType<Buffs.TodaysLook>(), 10);
                player.GetModPlayer<LobotomyTethPlayer>().TodayExpressionChangeFace(Main.rand.Next(5));
				return true;
            }

            return base.UseItem(player);
        }

		public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
		{
			if (player.altFunctionUse == 2)
			{
				mult *= 2f;
			}
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			if (Main.myPlayer == player.whoAmI && player.altFunctionUse != 2)
            {
				int face = player.GetModPlayer<LobotomyTethPlayer>().TodaysExpressionFace;
				switch (face)
				{
					case 0://Happy
						velocity *= 0.1f;
						knockback *= 3f;
						damage = (int)(damage * 0.05f);
						break;
					case 1://Smile
						velocity *= 0.4f;
						knockback *= 2f;
						damage = (int)(damage * 0.5f);
						break;
					default://Neutral
						break;
					case 3://Sad
						velocity *= 1.5f;
						knockback *= 0.5f;
						damage = (int)(damage * 1.1f); 
						break;
					case 4://Angry
						velocity *= 2.2f;
						knockback *= 0f;
						damage = (int)(damage * 1.6f); 
						break;
				}
				if (face == 4)
					SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Literature/Shy_Strong_Atk") with { Volume = 0.5f, MaxInstances = 2}, player.position);
				else
					SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Literature/Shy_Atk") with { Volume = 0.5f, PitchVariance = 0.2f, MaxInstances = 2}, player.position);

				Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, face);
			}

			return false;
        }

        public override void AddRecipes() 
		{
			CreateRecipe()
			   .AddIngredient(ModContent.ItemType<TodaysExpression>())
			   .AddIngredient(ItemID.SoulofNight, 8)
			   .AddRecipeGroup("LobotomyCorp:BossMasks")
			   .AddTile<Tiles.BlackBox3>()
               .AddCondition(RedMistCond)
               .Register();
		}
	}
}