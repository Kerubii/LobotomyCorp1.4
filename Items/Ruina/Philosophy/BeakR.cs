using LobotomyCorp.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Ruina.Philosophy
{
    public class BeakR : SEgoItem
	{
        public override bool IsLoadingEnabled(Mod mod)
        {
            return ModContent.GetInstance<Configs.LobotomyServerConfig>().TestItemEnable;
        }

        public override void SetStaticDefaults() 
		{
            // DisplayName.SetDefault("Penitence"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("People have been committing sins since long ago. \"Why do they commit sins, knowing it is wrong?\"");

            EgoColor = LobotomyCorp.TethRarity;
		}

		public override void SetDefaults() 
		{
            PassiveText = "Fluttering Wings - Right click to reduce the next incoming damage by half and counterattack\n" +
                          "Punishing Beak - Mark enemies and enable punishment mode when getting hit" +
                          "Punishment! - Deal 3 times more damage against marked enemies\n" +
                          "|Misdeeds Not Allowed! - Can only hit marked enemies during punishment mode\n" +
                          "This Item is incomplete and unobtainable";

            Item.damage = 1;
			Item.DamageType = DamageClass.Melee;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 8;
			Item.useAnimation = 8;
			Item.useStyle = 1;
			Item.knockBack = 0.1f;
			Item.value = 10000;
			Item.rare = ModContent.RarityType<TethR>();
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
		}

        public override bool AltFunctionUse(Player player)
        {
            return player.itemTime == 0;
        }

        public override bool? CanHitNPC(Player player, NPC target)
        {
            return player.GetModPlayer<LobotomyTethPlayer>().BeakPunish > 0 || LobotomyGlobalNPC.LNPC(target).BeakTarget > 0;
        }

        public override bool SafeCanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                player.GetModPlayer<LobotomyTethPlayer>().BeakParry = 30;
                Item.useAnimation = 30;
                Item.useTime = 45;
            }
            else
            {
                Item.useAnimation = 8;
                Item.useTime = 8;
            }

            return true;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            if (player.GetModPlayer<LobotomyTethPlayer>().BeakPunish > 0)
                damage += 75f;
        }

        public override void AddRecipes() 
		{
		}
	}
}