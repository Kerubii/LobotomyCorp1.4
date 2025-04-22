using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace LobotomyCorp.Items.Tools
{
    public class BehaviorAdjustment : ModItem
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
            Item.width = 28;
            Item.height = 28;
            Item.accessory = true;
            Item.value = 1000;
            Item.rare = RarityType<TethB>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //player.statLifeMax2 = (int)(player.statLifeMax2 * 1.1f);
            player.moveSpeed += 0.18f;
            player.GetAttackSpeed(DamageClass.Melee) += 0.18f;
            //LobotomyModPlayer modPlayer = LobotomyModPlayer.ModPlayer(player);
        }
    }
}