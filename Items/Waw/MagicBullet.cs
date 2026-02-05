using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Waw
{
    public class MagicBullet : LobItemBase
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Though the original's power couldn't be fully extracted, the magic this holds is still potent.\n" +
                               "The weapon's bullets travel across the corridor, along the horizon.\n" +
                               "Pierces through foes and friends alike"); */
        }

        public override void LobSetDefaults()
        {
            Item.damage = 76; // Sets the Item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage damageed together.
            Item.DamageType = DamageClass.Ranged; // sets the damage type to ranged
            Item.width = 40; // hitbox width of the Item
            Item.height = 42; // hitbox height of the Item
            Item.useTime = 48; // The Item's use time in ticks (60 ticks == 1 second.)
            Item.useAnimation = 48; // The length of the Item's use animation in ticks (60 ticks == 1 second.)
            Item.useStyle = ItemUseStyleID.Shoot; // how you use the Item (swinging, holding out, etc)
            Item.noMelee = true; //so the Item's animation doesn't do damage
            Item.knockBack = 1.5f; // Sets the Item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback damageed together.
            Item.value = 10000; // how much the Item sells for (measured in copper)
            Item.rare = ModContent.RarityType<WawB>(); // the color that the Item's name will be in-game
            Item.UseSound = new SoundStyle("LobotomyCorp/Sounds/Item/Freischutz_Shot") with { Volume = 0.25f }; // The sound that this Item plays when used.
            Item.autoReuse = true; // if you can hold click to automatically use it again
            Item.shoot = 10; //idk why but all the guns in the vanilla source have this
            Item.shootSpeed = 8f; // the speed of the projectile (measured in pixels per frame)
            Item.useAmmo = AmmoID.Bullet; // The "ammo Id" of the ammo Item that this weapon uses. Note that this is not an Item Id, but just a magic value.
            EGORiskLevel = RiskLevel.Waw;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 normVel = velocity;
            velocity.Normalize();
            position += velocity * 54f;
            velocity *= 12f;
            type = ModContent.ProjectileType<Projectiles.MagicBulletBullet>();
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-16, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.Sapphire, 3)
            .AddIngredient(ItemID.ShadowScale, 20)
            .AddIngredient(ItemID.DemoniteBar, 5)
            .AddTile(Mod, "BlackBox2")
            .Register();
        }
    }
}
