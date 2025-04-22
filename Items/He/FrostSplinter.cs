using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.He
{
    public class FrostSplinter : LobItemBase
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Penitence"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
            /* Tooltip.SetDefault("The edge of the spear is both straight and icy.\n" +
                               "Anyone damaged by it will lose themselves for a moment.\n" +
                               "As the equipment was forged from snow, it shall disappear without a trace someday.\n" +
                               "Inflicts Slow"); */

        }

        public override void SetDefaults()
        {
            Item.damage = 26;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;

            Item.useTime = 26;
            Item.useAnimation = 24;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<HeB>();
            Item.shootSpeed = 4.7f;
            Item.shoot = ModContent.ProjectileType<Projectiles.FrostSplinter>();

            Item.noUseGraphic = true;
            Item.UseSound = LobotomyCorp.WeaponSounds.Spear;
            Item.noMelee = true;
            Item.autoReuse = true;
            EGORiskLevel = RiskLevel.He;
        }

        public override bool CanUseItem(Player player)
        {
            // Ensures no more than one spear can be thrown out, use this when using autoReuse
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.Spear)
            .AddIngredient(ItemID.SnowBlock, 25)
            .AddIngredient(ItemID.IceBlock, 75)
            .AddTile(Mod, "BlackBox2")
            .Register();
        }
    }
}