using Microsoft.Xna.Framework;
using System.Globalization;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.He
{
    public class OurGalaxy : LobCorpLight
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Penitence"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
            /* Tooltip.SetDefault("\"There's a universe inside a pebble.\n" + 
                               "When the child cries, the stars in the galaxy light the void.\n" +
                               "In your universe, am I to be found?\""); */
        }

        public override void SetDefaults()
        {
            Item.damage = 30;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.useStyle = 15;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<HeB>();
            Item.shoot = ModContent.ProjectileType<Projectiles.OurGalaxySparkle>();
            Item.shootSpeed = 10f;
            Item.UseSound = LobotomyCorp.WeaponSound("Galaxy");
            Item.autoReuse = true;
            EGORiskLevel = RiskLevel.He;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int num = 3 + Main.rand.Next(2);
            for (int i = 0; i < num; i++)
            {
                Vector2 vel = velocity.RotatedByRandom(MathHelper.ToRadians(30));
                Projectile.NewProjectile(player.GetSource_FromThis(), position, vel, type, (int)(damage * 0.6f), knockback, player.whoAmI);
            }
            return false;
            //return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.FallenStar, 4)
            .AddIngredient(ItemID.Glass, 5)
            .AddIngredient(ItemID.MeteoriteBar, 8)
            .AddTile(Mod, "BlackBox2")
            .Register();
        }
    }
}