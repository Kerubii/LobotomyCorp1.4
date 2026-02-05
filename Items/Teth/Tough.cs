using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Teth
{
    public class Tough : LobItemBase
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("A glock reminiscent of a certain detective who fought evil for 25 years, losing hair as time went by.\n" +
                               "Only those who maintain a clean \'hairstyle\' with no impurities on their head will be deemed worthy of equipping this weapon."); */
        }

        public override void LobSetDefaults()
        {
            Item.damage = 16;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 20;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<TethB>();
            Item.UseSound = LobotomyCorp.WeaponSounds.Gun;
            Item.autoReuse = true;
            Item.shoot = 10;
            Item.shootSpeed = 14f;
            Item.useAmmo = AmmoID.Bullet;
            Item.scale = 0.8f;
            EGORiskLevel = RiskLevel.Teth;
        }

        public override bool CanUseItem(Player player)
        {
            return player.hair == 15 || player.hair == 76;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(RecipeGroupID.IronBar, 10)
            .AddIngredient(ItemID.Glass, 5)
            .AddIngredient(ItemID.StoneSlab, 10)
            .AddIngredient(ItemID.Lens, 2)
            .AddTile(Mod, "BlackBox")
            .Register();
        }
    }
}
