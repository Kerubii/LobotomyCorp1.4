using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Ruina.Natural
{
    public class SwordSharpenedWithTearsR : SEgoItem
	{
        public override bool IsLoadingEnabled(Mod mod)
        {
            return ModContent.GetInstance<Configs.LobotomyServerConfig>().TestItemEnable;
        }

        public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Penitence"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
			/* Tooltip.SetDefault("A complete E.G.O.\n" +
                               "\"All that remains is the hollow pride of a weathered knight.\"\n"); */
            
            EgoColor = LobotomyCorp.WawRarity;
		}

		public override void SetDefaults() 
		{
            PassiveText = "Heart-piercing sword - Summon three swords that floats around you\n" +
                          "Sharpened with Tears - Always deals a flat amount of damage and ignores all defense\n" +
                          "Blessing - When a teammate stands near you while synchronized with this weapon, they gain Blessed buff\n" +
                          "|Despair - The swords pierce back after missing a target\n" +
                          "This Item is incomplete and unobtainable";

            Item.damage = 63;
			Item.DamageType = DamageClass.Melee;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 26;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = ModContent.RarityType<WawR>();
			Item.UseSound = SoundID.Item1;
            Item.noMelee = true;
            Item.noUseGraphic = true;
			Item.autoReuse = true;
		}

        public override bool SafeCanUseItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI && player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.RealizedSwordSharpenedWithTearsProj>()] == 0)
            {
                for (int i = -1; i < 2; i++)
                {
                    Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.RealizedSwordSharpenedWithTearsProj>(), Item.damage, Item.knockBack, player.whoAmI, i);
                }
            }
            if (player.altFunctionUse == 2)
            {
                Item.useAnimation = 40;
                Item.useTime = 52;
            }
            else
            {
                Item.useTime = 26;
                Item.useAnimation = 20;
            }

            return base.SafeCanUseItem(player);
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override void AddRecipes() 
		{
		}
	}
}