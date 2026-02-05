using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Waw
{
    public class Discord : LobItemBase
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Penitence"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
            /* Tooltip.SetDefault("A falchion edge shadowed by the world's discord.\n" +
							   "Every life is trapped in the cycle of reincarnation."); */

        }

        public override void LobSetDefaults()
        {
            Item.damage = 84;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.scale = 1.2f;

            Item.useTime = 62;
            Item.useAnimation = 58;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<WawB>();
            Item.shootSpeed = 10.2f;
            Item.shoot = ModContent.ProjectileType<Projectiles.Discord2>();

            Item.noUseGraphic = true;
            Item.UseSound = SoundID.Item1;
            Item.noMelee = true;
            Item.autoReuse = true;
            EGORiskLevel = RiskLevel.Waw;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                type = ModContent.ProjectileType<Projectiles.DiscordSlash>();
            }
            else
            {
                damage /= 2;
            }
        }

        public override bool CanUseItem(Player player)
        {
            // Ensures no more than one spear can be thrown out, use this when using autoReuse
            if (player.altFunctionUse == 2)
            {
                //Item.useStyle = ItemUseStyleID.Shoot;
                Item.shootSpeed = 10.2f;
                Item.useTime = 62;
                Item.useAnimation = 58;
                Item.UseSound = LobotomyCorp.WeaponSound("YinYang2");
                //Item.noUseGraphic = true;
                //Item.noMelee = true;
            }
            else
            {
                //Item.useStyle = ItemUseStyleID.Swing;
                Item.shootSpeed = 1f;
                Item.useTime = 26;
                Item.useAnimation = 26;
                Item.UseSound = LobotomyCorp.WeaponSound("YinYang1");
                //Item.noUseGraphic = false;
                //Item.noMelee = false;
            }
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.DarkLance)
            .AddIngredient(ItemID.BlackDye)
            .AddIngredient(ItemID.EbonsandBlock, 100)
            .AddTile(Mod, "BlackBox3")
            .Register();

            CreateRecipe()
            .AddIngredient(ItemID.DarkLance)
            .AddIngredient(ItemID.BlackDye)
            .AddIngredient(ItemID.CrimsandBlock, 100)
            .AddTile(Mod, "BlackBox3")
            .Register();
        }
    }
}