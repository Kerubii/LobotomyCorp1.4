using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using LobotomyCorp.Items.He;
using LobotomyCorp.Players;

namespace LobotomyCorp.Items.Ruina.Technology
{
    public class GrinderMk52R : SEgoItem
	{
		public override void SetStaticDefaults() 
		{
            // DisplayName.SetDefault("Grinder Mk52"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault(GetTooltip());
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
		}

		public override void SetDefaults() 
		{
            EgoColor = LobotomyCorp.HeRarity;

            Item.damage = 63;
			Item.DamageType = DamageClass.Summon;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 2;
			Item.value = 10000;
			Item.rare = ItemRarityID.Yellow;
			Item.UseSound = SoundID.Item1;
            Item.noMelee = true;
            Item.noUseGraphic = true;
			Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Projectiles.Realized.GrinderMk2Cleaner2>();
            GrinderWeaponOrder = 0;
		}
        public override bool CanShoot(Player player)
        {
            return false;
        }
        public int GrinderWeaponOrder = 0;

        public override bool SafeCanUseItem(Player player)
        {
            if (player.GetModPlayer<LobotomyHePlayer>().GrinderMk2Recharging)
                return false;

            if (player.ownedProjectileCounts[Item.shoot] == 0)
            {
                if (Main.myPlayer == player.whoAmI)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, Item.shoot, Item.damage, Item.knockBack, player.whoAmI, i);
                    }
                }
                SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Helper_On") with {Volume = 0.25f}, player.Center);
            }
            if (player.altFunctionUse == 2)
            {
                Item.useAnimation = 60;
                Item.useTime = 60;
            }
            else
            {
                Item.useTime = 20;
                Item.useAnimation = 20;
            }

            GrinderWeaponOrder++;
            if (GrinderWeaponOrder > 3)
                GrinderWeaponOrder = 0;

            return true;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override void AddRecipes() 
		{
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<GrinderMk4>())
            .AddIngredient(ItemID.SoulofFright, 2)
            .AddIngredient(ItemID.IronBroadsword, 4)
            .AddIngredient(ItemID.Wire, 150)
            .AddTile<Tiles.BlackBox3>()
            .AddCondition(RedMistCond)
            .Register();

            CreateRecipe()
            .AddIngredient(ModContent.ItemType<GrinderMk4>())
            .AddIngredient(ItemID.SoulofFright, 2)
            .AddIngredient(ItemID.LeadBroadsword, 4)
            .AddIngredient(ItemID.Wire, 150)
            .AddTile<Tiles.BlackBox3>()
            .AddCondition(RedMistCond)
            .Register();
        }
	}
}