using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Teth
{
    public class Horn : LobItemBase
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Penitence"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
            /* Tooltip.SetDefault("The green - eyed beauty's favorite flower was the dahlia; \"Your love makes me happy.\"\n" +
                               "The lady's happiness came to an end with the budding of those unsightly horns.\n" +
                               "The dahlia's unfulfilled meaning was borne as a seed in this E.G.O, carrying a lingering emotion."); */

        }

        public override void SetDefaults()
        {
            Item.damage = 28;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;

            Item.useTime = 26;
            Item.useAnimation = 24;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ItemRarityID.Blue;
            Item.shootSpeed = 3.7f;
            Item.shoot = ModContent.ProjectileType<Projectiles.Horn>();

            Item.noUseGraphic = true;
            Item.UseSound = LobotomyCorp.WeaponSounds.Spear;
            Item.noMelee = true;
            Item.autoReuse = true;
            EGORiskLevel = RiskLevel.Teth;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (RedMistMaskUpgrade(player) && player.altFunctionUse != 2)
            {
                velocity *= 8f;
                type = ModContent.ProjectileType<Projectiles.HornThrown>();
            }
        }

        public override bool AltFunctionUse(Player player)
        {
            return RedMistMaskUpgrade(player);
        }

        public override bool CanUseItem(Player player)
        {
            // Ensures no more than one spear can be thrown out, use this when using autoReuse
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.JungleRose)
            .AddIngredient(ItemID.Lens)
            .AddIngredient(ItemID.JungleSpores, 4)
            .AddTile(Mod, "BlackBox")
            .Register();
        }
    }
}