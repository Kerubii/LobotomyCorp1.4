using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.ItemTiles
{
    public class BlackBox : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Absorbs an unknown energy around itself\n" +
							   "Allows the creation of ZAYIN and TETH E.G.O"); */
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.BlackBox>());
            Item.value = Item.buyPrice(0, 0, 0, 35);
            Item.rare = ItemRarityID.Red;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.WorkBench)
            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}