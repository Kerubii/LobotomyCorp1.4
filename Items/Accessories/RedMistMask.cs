using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace LobotomyCorp.Items.Accessories
{
    public class RedMistMask : ModItem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return ModContent.GetInstance<Configs.LobotomyServerConfig>().TestItemEnable;
        }

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("");
            /* Tooltip.SetDefault("\"Eventually, intellect loses all meaning as they forget even how to exist.\"\n" +
                               "18% increased melee speed\n" +
							   "18% increased movement speed\n" +
							   "15% decreased damage reduction\n"); */
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.accessory = true;
            Item.value = 1000;
            Item.rare = ModContent.RarityType<AlephB>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            LobotomyModPlayer modPlayer = LobotomyModPlayer.ModPlayer(player);
            modPlayer.RedMistMask = true;
        }
    }
}