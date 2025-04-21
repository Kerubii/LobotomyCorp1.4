using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace LobotomyCorp.Items.Tools
{
    public class HeartOfAspiration : ModItem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return ModContent.GetInstance<Configs.LobotomyServerConfig>().TestItemEnable;
        }

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("");
            /* Tooltip.SetDefault("\"Excessive aspiration would bring about unwarranted frenzy.\"\n" +
                               "Increases health, damage, defense, melee speed and movement speed\n" +
							   "Offensive boosts build up over time\n" +
							   "Hitting an enemy reduces Offensive boosts\n" +
							   "When Offensive boosts peak for 10 seconds, defensive boosts deactivate and gives the debuff Heart Attack\n" +
							   "Heart Attack decreases health and life regen, Offensive boosts never dissapears"); */
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
            player.statLifeMax2 = (int)(player.statLifeMax2 * 1.1f);
            player.moveSpeed += 5;
            player.GetAttackSpeed(DamageClass.Melee) += 0.15f;
            //LobotomyModPlayer modPlayer = LobotomyModPlayer.ModPlayer(player);
        }
    }
}