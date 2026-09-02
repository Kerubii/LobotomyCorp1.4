using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;
using System;
using LobotomyCorp.Items.Waw;
using LobotomyCorp.Players;

namespace LobotomyCorp.Items.Ruina.Literature
{
    public class BlackSwanR : SEgoItem
	{
		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Black Swan"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault(GetTooltip());
		}

		public override void SetDefaults() 
		{
			EgoColor = LobotomyCorp.WawRarity;

			Item.width = 70;
			Item.height = 70;

			Item.damage = 132;
			Item.DamageType = DamageClass.Melee;
            LobItemBase.ConvertVanillaDamageToExtractor(Item);
            Item.knockBack = 2.3f;
			Item.useTime = 28;
			Item.useAnimation = 28;
			Item.useStyle = ItemUseStyleID.Shoot;

			Item.value = 10000;
            Item.noMelee = true;
            Item.noUseGraphic = true;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.channel = true;
			Item.rare = ModContent.RarityType<WawR>();

			Item.shoot = ModContent.ProjectileType<Projectiles.Realized.BlackSwanR>();
			Item.shootSpeed = 4f;
			GooeyWasteProduce = 0;
		}

        public override bool SafeCanUseItem(Player player)
        {
			return base.SafeCanUseItem(player);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
			if (player.altFunctionUse == 2)
			{
				bool shriek = false;
				if (player.GetModPlayer<LobotomyWawPlayer>().BlackSwanNettleClothing >= 4)
				{
					damage = (int)(damage * 0.75f);
					player.GetModPlayer<LobotomyWawPlayer>().BlackSwanNettleRemove(1);
					shriek = true;
				}
				else if (player.GetModPlayer<LobotomyWawPlayer>().BlackSwanBrokenDream)
				{
					damage = (int)(damage * 0.4f);
					shriek = true;
					int bufType = ModContent.BuffType<Buffs.BrokenDreams>();
					int time = player.buffTime[player.FindBuffIndex(bufType)] + 360;
					player.AddBuff(bufType, time);
				}

				if (shriek)
				{
					type = ModContent.ProjectileType<Projectiles.Realized.BlackSwanScream>();
				}
				else
					type = ModContent.ProjectileType<Projectiles.Realized.BlackSwanAlternate>();
			}
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
        }

        public override bool? UseItem(Player player)
        {
			if (!player.GetModPlayer<LobotomyWawPlayer>().BlackSwanBrokenDream)
				player.AddBuff(ModContent.BuffType<Buffs.NettleClothing>(), 60);
			if (player.altFunctionUse == 2)
            {
				Item.useStyle = ItemUseStyleID.Shoot;
			}
			else
            {
				Item.useStyle = ItemUseStyleID.Shoot;
				Item.shoot = ModContent.ProjectileType<Projectiles.Realized.BlackSwanR>();
			}

            return base.UseItem(player);
        }

        public override float UseSpeedMultiplier(Player player)
        {
			if (player.GetModPlayer<LobotomyWawPlayer>().BlackSwanNettleClothing >= 1 || player.GetModPlayer<LobotomyWawPlayer>().BlackSwanBrokenDream)
				return 1.15f;
            return base.UseSpeedMultiplier(player);
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

		private int GooeyWasteProduce = 0;
        public override void HoldItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI && player.GetModPlayer<LobotomyWawPlayer>().BlackSwanNettleClothing >= 2 || player.GetModPlayer<LobotomyWawPlayer>().BlackSwanBrokenDream)
            {
				if (GooeyWasteProduce <= 0)
                {
					Vector2 pos = player.position + new Vector2(Main.rand.Next(player.width), Main.rand.Next(player.height));
					Projectile.NewProjectile(player.GetSource_FromThis(), pos, Vector2.Zero, ModContent.ProjectileType<Projectiles.Realized.BlackSwanGooeyWaste>(), 22, 0, player.whoAmI);
                }
				GooeyWasteProduce--;
            }
        }

        public override void AddRecipes() 
		{
			CreateRecipe()
			.AddIngredient(ModContent.ItemType<BlackSwan>())
			.AddIngredient(ItemID.Feather, 8)
			.AddIngredient(ItemID.Ectoplasm, 2)
			.AddTile<Tiles.BlackBox3>()
            .AddCondition(RedMistCond)
            .Register();
		}
	}
}