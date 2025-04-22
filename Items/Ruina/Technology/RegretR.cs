using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using System.Collections.Generic;
using LobotomyCorp.Items.Teth;

namespace LobotomyCorp.Items.Ruina.Technology
{
    //[Autoload(LobotomyCorp.TestMode)]
    public class RegretR : SEgoItem
	{
        public override void Load()
        {
			RegretWhitelist.Add(ModContent.BuffType<Buffs.BindingJacket>());
        }

        public override void Unload()
        {
			RegretWhitelist = null;
        }

        public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Regret"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault(GetTooltip());
		}

		public override void SetDefaults() 
		{
			EgoColor = LobotomyCorp.TethRarity;

			Item.damage = 90;
			Item.DamageType = DamageClass.Melee;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 26;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = ModContent.RarityType<TethR>();
			Item.UseSound = SoundID.Item1;
            Item.noMelee = true;
            Item.noUseGraphic = true;
			Item.autoReuse = true;
            Item.channel = true;

			Item.shoot = ModContent.ProjectileType<Projectiles.Realized.RegretR>();
			Item.shootSpeed = 1f;
		}

        public override bool SafeCanUseItem(Player player)
        {
			return base.SafeCanUseItem(player);
        }

        public override void HoldItem(Player player)
        {
			int bind = ModContent.BuffType<Buffs.BindingJacket>();
			foreach (int buff in player.buffType)
            {
				if (buff > 0 && Main.debuff[buff] && BindingJacketWhitelist(buff))
                {
					player.AddBuff(bind, 300);
                }
            }
        }

		public static List<int> RegretWhitelist = new List<int>();

		private bool BindingJacketWhitelist(int buff)
        {
			switch(buff)
            {
				case BuffID.PotionSickness:
				case BuffID.ManaSickness:
				case BuffID.Werewolf:
				case BuffID.Merfolk:
				case BuffID.WaterCandle:
				case BuffID.Campfire:
				case BuffID.StarInBottle:
				case BuffID.HeartLamp:
				case BuffID.MonsterBanner:
				case BuffID.Sunflower:
				case BuffID.PeaceCandle:
				case BuffID.NoBuilding:
                case BuffID.CatBast:
				case BuffID.BrainOfConfusionBuff:
				case BuffID.NeutralHunger:
					return false;
				default:
					if (RegretWhitelist.Contains(buff))
						return false;
					return true;
            }
        }

        /*
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }*/

        public override void AddRecipes() 
		{
			CreateRecipe()
			.AddIngredient(ModContent.ItemType<Regret>())
			.AddIngredient(ItemID.AdamantiteBar, 5)
			.AddIngredient(ItemID.FlamingMace)
			.AddTile<Tiles.BlackBox3>()
            .AddCondition(RedMistCond)
            .Register();

			CreateRecipe()
			.AddIngredient(ModContent.ItemType<Regret>())
			.AddIngredient(ItemID.TitaniumBar, 5)
			.AddIngredient(ItemID.FlamingMace)
			.AddTile<Tiles.BlackBox3>()
            .AddCondition(RedMistCond)
            .Register();
		}
	}
}