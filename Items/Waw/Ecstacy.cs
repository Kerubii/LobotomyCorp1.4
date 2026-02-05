using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Waw
{
    public class Ecstacy : LobItemBase
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Penitence"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("The colorful pattern is vivid, similar to a child's plaything.");

        }

        public override void LobSetDefaults()
        {
            Item.damage = 24;
            Item.DamageType = DamageClass.Magic; ;
            Item.mana = 6;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.useStyle = 5;
            Item.noMelee = true;
            Item.knockBack = 2.4f;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<WawB>();
            Item.UseSound = LobotomyCorp.WeaponSound("Shark");
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Projectiles.Candy>();
            Item.shootSpeed = 8f;
            EGORiskLevel = RiskLevel.Waw;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, Main.rand.Next(5));
                for (int i = 0; i < 5; i++)
                {
                    Vector2 speed = velocity.RotatedByRandom(MathHelper.ToRadians(15));
                    Projectile.NewProjectile(source, position, speed * Main.rand.NextFloat(0.3f, 0.7f), type, (int)(damage * 0.2f), knockback, player.whoAmI, Main.rand.Next(5));
                }
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.WaterGun)
            .AddIngredient(ItemID.Bubble, 30)
            .AddIngredient(ItemID.SharkFin)
            .AddTile(Mod, "BlackBox3")
            .Register();

            CreateRecipe()
            .AddIngredient(ItemID.SlimeGun)
            .AddIngredient(ItemID.Bubble, 30)
            .AddIngredient(ItemID.SharkFin)
            .AddTile(Mod, "BlackBox3")
            .Register();
        }
    }
}