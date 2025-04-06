using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.He
{
    public class Laetitia : LobItemBase
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("The crude design appears to have been made a long time ago.\n" +
                               "The small accessory remains like the wishes of a child who yearned for happiness."); */
        }

        public override void SetDefaults()
        {
            Item.damage = 50; // Sets the Item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage damageed together.
            Item.DamageType = DamageClass.Ranged; // sets the damage type to ranged
            Item.width = 40; // hitbox width of the Item
            Item.height = 42; // hitbox height of the Item
            Item.useTime = 24; // The Item's use time in ticks (60 ticks == 1 second.)
            Item.useAnimation = 24; // The length of the Item's use animation in ticks (60 ticks == 1 second.)
            Item.useStyle = ItemUseStyleID.Shoot; // how you use the Item (swinging, holding out, etc)
            Item.noMelee = true; //so the Item's animation doesn't do damage
            Item.knockBack = 4; // Sets the Item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback damageed together.
            Item.value = 10000; // how much the Item sells for (measured in copper)
            Item.rare = ItemRarityID.Yellow; // the color that the Item's name will be in-game
            Item.UseSound = LobotomyCorp.WeaponSounds.Rifle; // The sound that this Item plays when used.
            Item.autoReuse = true; // if you can hold click to automatically use it again
            Item.shoot = 10; //idk why but all the guns in the vanilla source have this
            Item.shootSpeed = 8f; // the speed of the projectile (measured in pixels per frame)
            Item.useAmmo = AmmoID.Bullet; // The "ammo Id" of the ammo Item that this weapon uses. Note that this is not an Item Id, but just a magic value.
            EGORiskLevel = RiskLevel.He;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-16, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.Ruby, 3)
            .AddIngredient(ItemID.TissueSample, 20)
            .AddIngredient(ItemID.CrimtaneBar, 5)
            .AddTile(Mod, "BlackBox2")
            .Register();
        }
    }
}
