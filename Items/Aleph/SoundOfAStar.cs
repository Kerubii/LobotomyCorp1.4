using LobotomyCorp.Projectiles;
using Microsoft.Xna.Framework;
using rail;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Aleph
{
    public class SoundOfAStar : LobItemBase
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Penitence"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
            /* Tooltip.SetDefault("The star shines brighter as our despair gathers.\n" +
                               "The weapon's small, evocative sphere fires a warm ray.\n" +
                               "In the light, everything is equal."); */
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
            ItemID.Sets.StaffMinionSlotsRequired[Item.type] = 0f;
        }

        public override void LobSetDefaults()
        {
            Item.damage = 27;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 6;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 10;
            Item.useAnimation = 30;
            Item.reuseDelay = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.4f;
            Item.value = 5000;
            Item.rare = ModContent.RarityType<AlephB>();
            Item.UseSound = LobotomyCorp.WeaponSound("blueStar");
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Projectiles.SoundOfAStarBall>();
            Item.shootSpeed = 4f;
            Item.noUseGraphic = true;
            Item.buffType = ModContent.BuffType<Buffs.SoundOfAStarBallBuff>();
        }

        public override float UseTimeMultiplier(Player player)
        {
            if (player.altFunctionUse != 2)
                return 3;
            return base.UseTimeMultiplier(player);
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                player.AddBuff(Item.buffType, 2);
            }
            else
            {
                type = ModContent.ProjectileType<Projectiles.SoundOfAStar>();
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                Vector2 speed = velocity.RotatedByRandom(MathHelper.ToRadians(30)) * Main.rand.NextFloat(0.8f, 1f);
                Projectile.NewProjectile(source, position, -speed, type, damage, knockback, player.whoAmI);
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.FallenStar, 10)
            .AddIngredient(ItemID.SoulofLight, 4)
            .AddIngredient(ItemID.SoulofNight, 4)
            .AddIngredient(ItemID.SapphireGemsparkBlock, 25)
            .AddTile(Mod, "BlackBox3")
            .Register();
        }
    }
}