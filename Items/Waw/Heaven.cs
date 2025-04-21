using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Waw
{
    public class Heaven : LobItemBase
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Penitence"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
            /* Tooltip.SetDefault("Just contain it in your sight.\n" +
                               "As it spreads its wings for an old god, a heaven just for you burrows its way.") ; */

        }

        public override void SetDefaults()
        {
            Item.damage = 42;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;

            Item.useTime = 32;
            Item.useAnimation = 32;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<WawB>();
            Item.shootSpeed = 3.7f;
            Item.shoot = ModContent.ProjectileType<Projectiles.Heaven>();

            Item.noUseGraphic = true;
            Item.UseSound = LobotomyCorp.WeaponSounds.Spear;
            Item.noMelee = true;
            Item.autoReuse = true;
            EGORiskLevel = RiskLevel.Waw;
        }

        public override bool CanUseItem(Player player)
        {
            // Ensures no more than one spear can be thrown out, use this when using autoReuse
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.Javelin)
            .AddIngredient(ItemID.TheRottedFork)
            .AddIngredient(ItemID.CrimtaneBar, 5)
            .AddTile(Mod, "BlackBox3")
            .Register();

            CreateRecipe()
            .AddIngredient(ItemID.Javelin)
            .AddIngredient(ItemID.BallOHurt)
            .AddIngredient(ItemID.DemoniteBar, 5)
            .AddTile(Mod, "BlackBox3")
            .Register();
        }
    }
}