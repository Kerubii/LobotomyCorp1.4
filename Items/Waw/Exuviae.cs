using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Waw
{
    public class Exuviae : LobItemBase
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("The slick sensation when holding this E.G.O may disturb some employees.");
        }

        public override void SetDefaults()
        {
            Item.damage = 24; // Sets the Item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage damageed together.
            Item.DamageType = DamageClass.Magic; ; // sets the damage type to ranged
            Item.width = 40; // hitbox width of the Item
            Item.height = 38; // hitbox height of the Item
            Item.useTime = 38; // The Item's use time in ticks (60 ticks == 1 second.)
            Item.useAnimation = 32; // The length of the Item's use animation in ticks (60 ticks == 1 second.)
            Item.useStyle = ItemUseStyleID.Shoot; // how you use the Item (swinging, holding out, etc)
            Item.noMelee = true; //so the Item's animation doesn't do damage
            Item.knockBack = 4; // Sets the Item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback damageed together.
            Item.value = 10000; // how much the Item sells for (measured in copper)
            Item.rare = ItemRarityID.Purple; // the color that the Item's name will be in-game
            Item.UseSound = LobotomyCorp.WeaponSound("Viscus"); // The sound that this Item plays when used.
            Item.autoReuse = true; // if you can hold click to automatically use it again
            Item.shoot = ModContent.ProjectileType<Projectiles.ExuviaeShot>(); //idk why but all the guns in the vanilla source have this
            Item.shootSpeed = 8f; // the speed of the projectile (measured in pixels per frame)
            Item.mana = 6;
            EGORiskLevel = RiskLevel.Waw;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 pos2 = position + Vector2.Normalize(velocity) * 18f;
            if (Collision.CanHit(position, 0, 0, pos2, 0, 0))
            {
                position = pos2;
            }
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-8, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.Hive, 25)
            .AddIngredient(ItemID.Worm, 3)
            .AddTile(Mod, "BlackBox3")
            .Register();
        }
    }
}
