using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Waw
{
    public class InTheNameOfLoveAndHate : LobItemBase
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("A magic wand surging with the lovely energy of a magical girl.\n" +
                               "The holy light can cleanse the body and mind of every villain, and they shall be born anew."); */
            //Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 38; // Sets the Item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage damageed together.
            Item.DamageType = DamageClass.Magic; ; // sets the damage type to ranged
            Item.mana = 5;
            Item.width = 40; // hitbox width of the Item
            Item.height = 20; // hitbox height of the Item
            Item.useTime = 20; // The Item's use time in ticks (60 ticks == 1 second.)
            Item.useAnimation = 20; // The length of the Item's use animation in ticks (60 ticks == 1 second.)
            Item.useStyle = ItemUseStyleID.Shoot; // how you use the Item (swinging, holding out, etc)
            Item.noMelee = true; //so the Item's animation doesn't do damage
            Item.knockBack = 4; // Sets the Item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback damageed together.
            Item.value = 10000; // how much the Item sells for (measured in copper)
            Item.rare = ItemRarityID.Purple; // the color that the Item's name will be in-game
            Item.UseSound = LobotomyCorp.WeaponSound("magicalGirl"); // The sound that this Item plays when used.
            Item.autoReuse = true; // if you can hold click to automatically use it again
            Item.shoot = ModContent.ProjectileType<Projectiles.StarShot>(); //idk why but all the guns in the vanilla source have this
            Item.shootSpeed = 8f; // the speed of the projectile (measured in pixels per frame)
            EGORiskLevel = RiskLevel.Waw;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 pos2 = position + Vector2.Normalize(velocity) * 90f;
            if (Collision.CanHit(position, 0, 0, pos2, 0, 0))
            {
                position = pos2;
            }
            if (Main.myPlayer == player.whoAmI)
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, Main.rand.Next(4));
            return false;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.MagicMissile)
            .AddIngredient(ItemID.Amethyst)
            .AddIngredient(ItemID.FallenStar, 15)
            .AddTile(Mod, "BlackBox3")
            .Register();
        }
    }
}
