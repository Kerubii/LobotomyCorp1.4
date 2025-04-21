using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.He
{
    public class Harvest : LobItemBase
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Penitence"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
            /* Tooltip.SetDefault("The last legacy of the man who sought wisdom.\n" +
							   "The rake tilled the human brain instead of farmland."); */

        }

        public override void SetDefaults()
        {
            Item.damage = 45;
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
            Item.shoot = ModContent.ProjectileType<Projectiles.Harvest>();

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
            .AddIngredient(ItemID.Sickle)
            .AddIngredient(ItemID.Hay, 100)
            .AddTile(Mod, "BlackBox2")
            .Register();
        }
    }
}