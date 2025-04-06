using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.ItemTiles
{
    public class DisciplinaryShell : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Discplinary Shell");
            /* Tooltip.SetDefault("Awakens the Red Mist\n" + 
							   "The Boss is highly unfinished I dont recommend fighting it"); */
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 99;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.buyPrice(0, 20, 0, 0);
            Item.createTile = ModContent.TileType<Tiles.DisciplinaryShell>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                var warning = new TooltipLine(Mod, "LobotomyCorp.ShellWarning", $"{Language.GetTextValue("Mods.LobotomyCorp.Items.DisciplinaryShell.Warning")}");
                tooltips.Add(warning);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
               .AddRecipeGroup(RecipeGroupID.IronBar, 10)
               .AddRecipeGroup("LobotomyCorp:DualSoul", 3)
               .AddTile(TileID.DemonAltar)
               .Register();
        }
    }
}