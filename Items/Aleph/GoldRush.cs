using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Aleph
{
    public class GoldRush : LobItemBase
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Penitence"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
            /* Tooltip.SetDefault("The weapon she used to brandish in her prime, before the greed solidified and became what it is now.\n" +
							   "One can release their primal desires and strike enemies with full force; technical skill is unneeded."); */

        }

        public override void SetDefaults()
        {
            Item.damage = 50;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;

            Item.useTime = 5;
            Item.useAnimation = 16;
            Item.reuseDelay = 18;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<AlephB>();
            Item.shootSpeed = 11f;
            Item.shoot = ModContent.ProjectileType<Projectiles.GoldRushPunches>();

            Item.noUseGraphic = true;
            Item.UseSound = SoundID.Item1;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.channel = true;
            EGORiskLevel = RiskLevel.Aleph;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse != 2)
            {
                /*
                Item.useTime = 28;
                Item.useAnimation = 28;
                Item.shootSpeed = 4f;
                Item.UseSound = LobotomyCorp.WeaponSound("greed2");
                Item.shoot = ModContent.ProjectileType<Projectiles.GoldRushPunch>();*/

                Item.UseSound = null;
                Item.shoot = ModContent.ProjectileType<Projectiles.GoldRushHold>();
                Item.shootSpeed = 11f;
            }
            else
            {
                
                //Item.useTime = 5;
                //Item.useAnimation = 16;
                Item.shootSpeed = 7f;
                Item.UseSound = LobotomyCorp.WeaponSound("greed1_", false, 2);
                Item.shoot = ModContent.ProjectileType<Projectiles.GoldRushPunches>();
            }
            return true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (type == ModContent.ProjectileType<Projectiles.GoldRushHold>())
            {
                damage *= 2;
                return;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.FeralClaws)
            .AddIngredient(ItemID.Amber)
            .AddIngredient(ItemID.GoldBar, 20)
            .AddIngredient(ItemID.FallenStar, 8)
            .AddTile(Mod, "BlackBox3")
            .Register();

            CreateRecipe()
            .AddIngredient(ItemID.FeralClaws)
            .AddIngredient(ItemID.Amber)
            .AddIngredient(ItemID.PlatinumBar, 20)
            .AddIngredient(ItemID.FallenStar, 8)
            .AddTile(Mod, "BlackBox3")
            .Register();
        }
    }
}