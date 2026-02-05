using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;

namespace LobotomyCorp.Items.Ruina.Literature
{
    public class LaetitiaR : SEgoItem
	{
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<Configs.LobotomyServerConfig>().TestItemEnable;
		}

		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Laetitia"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault(GetTooltip());
		}

		public override void SetDefaults() 
		{
			EgoColor = LobotomyCorp.HeRarity;

			Item.width = 40;
			Item.height = 40;

			Item.damage = 44;
			Item.DamageType = ModContent.GetInstance<Players.DamageType.ExtractorSummon>();
			Item.knockBack = 6;
			Item.useTime = 26;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.mana = 8;

			Item.value = 10000;
            Item.noMelee = true;
            Item.noUseGraphic = true;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.rare = ModContent.RarityType<HeR>();

			Item.shoot = ModContent.ProjectileType<Projectiles.Realized.LaetitiaR>();
			Item.shootSpeed = 6f;
		}

        public override void AddRecipes() 
		{
		}
	}
}