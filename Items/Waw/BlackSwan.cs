using LobotomyCorp.Players;
using LobotomyCorp.Projectiles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Waw
{
    public class BlackSwan : LobCorpLight
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Penitence"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
            /* Tooltip.SetDefault("Believing that it would turn white,\n" +
							   "The black swan wanted to lift the curse by weaving together nettles.\n" +
							   "All that was left is a worn parasol it once treasured.\n" +
							   "Small chance to reflect damage taken"); */
        }

        public override void SetDefaults()
        {
            Item.damage = 42;
            Item.knockBack = 6;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;

            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = 15;

            Item.value = 10000;
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = LobotomyCorp.WeaponSound("blackSwan", false, 2);
            Item.autoReuse = true;

            Item.shoot = ModContent.ProjectileType<BlackSwanGlob>();
            Item.shootSpeed = 12f;
            EGORiskLevel = RiskLevel.Waw;
        }

        public override void HoldItem(Player player)
        {
            player.GetModPlayer<LobotomyWawPlayer>().BlackSwanParryChance += 10;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.TragicUmbrella)
            .AddIngredient(ItemID.Feather, 2)
            .AddTile(Mod, "BlackBox3")
            .Register();
        }
    }
}