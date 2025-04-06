using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Teth
{
    public class FourthMatchFlame : LobItemBase
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("The fire roars and burns like the first flame.\n" +
                               "The light of the match will not go out.\n" +
                               "Consumes bullets"); */

        }

        public override void SetDefaults()
        {
            Item.damage = 18; // Sets the Item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage damageed together.
            Item.DamageType = DamageClass.Ranged; // sets the damage type to ranged
            Item.width = 40; // hitbox width of the Item
            Item.height = 38; // hitbox height of the Item
            Item.useTime = 38; // The Item's use time in ticks (60 ticks == 1 second.)
            Item.useAnimation = 32; // The length of the Item's use animation in ticks (60 ticks == 1 second.)
            Item.useStyle = ItemUseStyleID.Shoot; // how you use the Item (swinging, holding out, etc)
            Item.noMelee = true; //so the Item's animation doesn't do damage
            Item.knockBack = 4; // Sets the Item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback damageed together.
            Item.value = 10000; // how much the Item sells for (measured in copper)
            Item.rare = ItemRarityID.Blue; // the color that the Item's name will be in-game
            Item.UseSound = LobotomyCorp.WeaponSounds.Cannon; // The sound that this Item plays when used.
            Item.autoReuse = true; // if you can hold click to automatically use it again
            Item.shoot = 10; //idk why but all the guns in the vanilla source have this
            Item.shootSpeed = 8f; // the speed of the projectile (measured in pixels per frame)
            Item.useAmmo = ItemID.Torch; // The "ammo Id" of the ammo Item that this weapon uses. Note that this is not an Item Id, but just a magic value.
            EGORiskLevel = RiskLevel.Teth;
        }

        public override bool? CanChooseAmmo(Item ammo, Player player)
        {
            return ammo.type == ItemID.Torch;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<Projectiles.FourthMatchFlameShot>();
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.Torch, 99)
            .AddIngredient(ItemID.GoldBar, 4)
            .AddTile(Mod, "BlackBox")
            .Register();

            CreateRecipe()
            .AddIngredient(ItemID.Torch, 99)
            .AddIngredient(ItemID.PlatinumBar, 4)
            .AddTile(Mod, "BlackBox")
            .Register();
        }
    }
}
