using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Waw
{
    public class FeatherOfHonor : LobItemBase
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Penitence"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
            /* Tooltip.SetDefault("The feather strikes with vivid flame. It is not weak, nor faint.\n" +
                               "The flame pierces the body and melts the frost of the heart."); */

        }

        public int FeatherShoot = 0;

        public override void SetDefaults()
        {
            Item.damage = 24;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 6;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 32;
            Item.useAnimation = 32;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.4f;
            Item.value = 5000;
            Item.rare = ItemRarityID.Purple;
            //Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Projectiles.FeatherOfHonor>();
            Item.shootSpeed = 1f;
            Item.noUseGraphic = true;
            Item.channel = true;
            FeatherShoot = 0;
            EGORiskLevel = RiskLevel.Waw;
        }

        public override void HoldItem(Player player)
        {
            if (FeatherShoot > 0)
                FeatherShoot--;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 5; i++)
            {
                int order = i;
                if (i >= 2)
                    order++;
                if (i == 4)
                    order = 2;
                if (Main.myPlayer == player.whoAmI)
                    Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI, order);
            }

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.Feather, 5)
            .AddIngredient(ItemID.InfernoPotion)
            .AddIngredient(ItemID.Fireblossom, 4)
            .AddTile(Mod, "BlackBox3")
            .Register();
        }
    }
}