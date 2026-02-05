using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;
using System;
using LobotomyCorp.Items.Waw;

namespace LobotomyCorp.Items.Ruina.Art
{
    public class FragmentsFromSomewhereR : SEgoItem
	{
		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Fragmnents from Somewhere"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault(GetTooltip());
		}

		public override void SetDefaults() 
		{
			EgoColor = LobotomyCorp.TethRarity;

			Item.width = 64;
			Item.height = 64;

			Item.damage = 90;
			Item.DamageType = ModContent.GetInstance<Players.DamageType.ExtractorMelee>();
			Item.knockBack = 2.3f;
			Item.useTime = 22;
			Item.useAnimation = 22;
			Item.useStyle = ItemUseStyleID.Shoot;

			Item.value = 10000;
            Item.noMelee = true;
            Item.noUseGraphic = true;
			//Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.channel = true;
			Item.rare = ModContent.RarityType<TethR>();

			Item.shoot = ModContent.ProjectileType<Projectiles.Realized.FragmentsFromSomewhereRSpear>();
			Item.shootSpeed = 5.6f;
		}

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
			if (player.altFunctionUse == 2)
			{
				type = ModContent.ProjectileType<Projectiles.Realized.FragmentsFromSomewhereSong>();
				//damage /= 2;
			}
			base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
        }

        public override bool? UseItem(Player player)
        {
			string Sound = "Cosmos_Stab_Down";
			if (player.altFunctionUse == 2)
				Sound = "Cosmos_HowlingAtk";
			else if (Main.rand.NextBool(2))
				Sound = "Cosmos_Stab_Up";
			SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Art/" + Sound) with { Volume = 0.25f }, player.Center);

			return true;//base.UseItem(player);
        }

        public override float UseTimeMultiplier(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                return 3.4f;
            }
            return base.UseTimeMultiplier(player);
        }

        public override float UseAnimationMultiplier(Player player)
        {
			if (player.altFunctionUse == 2)
			{
				return 3.4f;
			}
			return base.UseAnimationMultiplier(player);
        }

        public override bool AltFunctionUse(Player player)
        {
			return true;
        }

        public override void AddRecipes() 
		{
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<Items.Teth.FragmentsFromSomewhere>())
            .AddIngredient(ItemID.ArcaneCrystal)
            .AddIngredient(ItemID.MeteoriteBar, 8)
            .AddIngredient(ItemID.CrystalShard, 12)
            .AddTile<Tiles.BlackBox3>()
            .AddCondition(RedMistCond)
            .Register();
        }
	}
}