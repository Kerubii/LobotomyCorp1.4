using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Waw
{
    public class Hornet : LobItemBase
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("The kingdom needed to stay prosperous, and more bees were required for that task.\n" +
							   "The name of that kingdom may go down in history.\n" +
							   "But who will be there to remember the bees that gave their lives to the cause?" +
							   "The weapon's bullets selectively fly toward threats, so skillful aiming isn't required; all that's needed is the will to hit the target.\n" +
							   "Bullets are converted to bees"); */

        }

        public override void SetDefaults()
        {
            Item.damage = 38; // Sets the Item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage damageed together.
            Item.DamageType = DamageClass.Ranged; // sets the damage type to ranged
            Item.width = 40; // hitbox width of the Item
            Item.height = 42; // hitbox height of the Item
            Item.useTime = 42; // The Item's use time in ticks (60 ticks == 1 second.)
            Item.useAnimation = 42; // The length of the Item's use animation in ticks (60 ticks == 1 second.)
            Item.useStyle = ItemUseStyleID.Shoot; // how you use the Item (swinging, holding out, etc)
            Item.noMelee = true; //so the Item's animation doesn't do damage
            Item.knockBack = 4; // Sets the Item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback damageed together.
            Item.value = 10000; // how much the Item sells for (measured in copper)
            Item.rare = ItemRarityID.Purple; // the color that the Item's name will be in-game
            Item.UseSound = LobotomyCorp.WeaponSounds.Rifle; // The sound that this Item plays when used.
            Item.autoReuse = true; // if you can hold click to automatically use it again
            Item.shoot = 10; //idk why but all the guns in the vanilla source have this
            Item.shootSpeed = 8f; // the speed of the projectile (measured in pixels per frame)
            Item.useAmmo = AmmoID.Bullet; // The "ammo Id" of the ammo Item that this weapon uses. Note that this is not an Item Id, but just a magic value.
            EGORiskLevel = RiskLevel.Waw;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            damage = player.beeDamage(damage);
            knockback = player.beeKB(knockback);
            type = player.beeType();
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-16, 0);
        }
    }
}
