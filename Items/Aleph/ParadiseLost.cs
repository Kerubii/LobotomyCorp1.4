using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Aleph
{
    public class ParadiseLost : LobItemBase
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Penitence"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
            /* Tooltip.SetDefault("Behold: you stood at the door and knocked, and it was opened to you.\n" +
                               "I come from the end, and I am here to stay for but a moment.\n" +
                               "At the same time, I am the one who kindled the light to face the world.\n" +
                               "My loved ones, who now eagerly desire the greater gifts; I will show you the most excellent way."); */

        }

        public override void SetDefaults()
        {
            Item.damage = 66;
            Item.DamageType = DamageClass.Magic; ;
            Item.mana = 15;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 34;
            Item.useAnimation = 34;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.knockBack = 0.8f;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<AlephB>();
            Item.UseSound = LobotomyCorp.WeaponSound("DeathAngel1");
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Projectiles.ParadiseLostBase>();
            Item.shootSpeed = 14f;
            Item.noUseGraphic = true;
            EGORiskLevel = RiskLevel.Aleph;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            /*damage = (int)(damage * 0.6f);
            for (int i = 0; i < 3; i++)
            {
                Vector2 speed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(15));
                Projectile.NewProjectile(position, speed, type, damage, knockBack, player.whoAmI);
            }*/
            position = Main.MouseWorld;
            int x = (int)(position.X / 16), y = (int)(position.Y / 16);
            int add = Main.rand.Next(3);
            int dir = Main.rand.NextBool(2)? -1 : 1;
            for (int i = -1; i < 2; i++)
            {
                if (Main.tile[x + 2 * i, y].HasTile && Main.tileSolid[Main.tile[x + 2 * i, y].TileType] && Main.tileSolidTop[Main.tile[x + 2 * i, y].TileType])
                {
                    continue;
                }
                for (int j = 0; j < 20; j++)
                {
                    Tile tile = Main.tile[x + 2 * i, y + j];
                    if (tile.HasTile && Main.tileSolid[tile.TileType])
                    {
                        position = new Vector2(Main.MouseWorld.X + 32 * i + Main.rand.Next(-8, 9), (y + j) * 16 - 8);
                        Vector2 speed = Main.MouseWorld - position;
                        if (Main.myPlayer == player.whoAmI)
                            Projectile.NewProjectile(source, position, speed, Item.shoot, damage, knockback, player.whoAmI, (1 + i + add) * dir);
                        break;
                    }
                }
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.AngelWings)
            .AddIngredient(ItemID.SoulofNight, 5)
            .AddIngredient(ItemID.SoulofLight, 10)
            .AddIngredient(ItemID.LightShard)
            .AddTile(Mod, "BlackBox3")
            .Register();
        }
    }
}