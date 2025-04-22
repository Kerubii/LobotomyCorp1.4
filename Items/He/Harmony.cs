using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.He
{
    public class Harmony : LobItemBase
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("It may look like a deteriorating machine at first glance,\n" +
                               "But the music it makes captures its audience more than any other instrument could.\n" +
                               "The wielder must dedicate himself in return.\n" +
                               "After all, art is a devil's gift, born from despair and suffering.\n" +
                               "30% increased damage when consuming 2% hp while having above 2% maximum health"); */
        }

        public override void SetDefaults()
        {
            Item.damage = 77; // Sets the Item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage damageed together.
            Item.DamageType = DamageClass.Magic; ; // sets the damage type to ranged
            Item.width = 40; // hitbox width of the Item
            Item.height = 38; // hitbox height of the Item
            Item.useTime = 38; // The Item's use time in ticks (60 ticks == 1 second.)
            Item.useAnimation = 32; // The length of the Item's use animation in ticks (60 ticks == 1 second.)
            Item.useStyle = ItemUseStyleID.Shoot; // how you use the Item (swinging, holding out, etc)
            Item.noMelee = true; //so the Item's animation doesn't do damage
            Item.knockBack = 4; // Sets the Item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback damageed together.
            Item.value = 10000; // how much the Item sells for (measured in copper)
            Item.rare = ModContent.RarityType<HeB>(); // the color that the Item's name will be in-game
            Item.UseSound = LobotomyCorp.WeaponSounds.Cannon; // The sound that this Item plays when used.
            Item.autoReuse = true; // if you can hold click to automatically use it again
            Item.shoot = 10; //idk why but all the guns in the vanilla source have this
            Item.shootSpeed = 8f; // the speed of the projectile (measured in pixels per frame)
            Item.mana = 10;
            EGORiskLevel = RiskLevel.He;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 pos2 = position + Vector2.Normalize(velocity) * 90f;
            if (Collision.CanHit(position, 0, 0, pos2, 0, 0))
            {
                position = pos2;
                for (int i = 0; i < 7; i++)
                {
                    int n = Dust.NewDust(position, 1, 1, ModContent.DustType<Misc.Dusts.NoteDust>(), velocity.X * 0.5f, velocity.Y * 0.5f);
                    Main.dust[n].fadeIn = 1.2f;
                }
            }

            type = ModContent.ProjectileType<Projectiles.HarmonyShot>();
            if (player.statLife > player.statLifeMax2 * 0.02f)
            {
                damage = (int)(damage * 1.3f);
                player.statLife -= (int)(player.statLifeMax2 * 0.02f);
                CombatText.NewText(player.getRect(), CombatText.DamagedFriendly, (int)(player.statLifeMax2 * 0.02f));
            }
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-15, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.Sawmill)
            .AddIngredient(ItemID.SilverBar, 8)
            .AddIngredient(ItemID.Chain, 10)
            .AddIngredient(ItemID.RottenChunk, 2)
            .AddTile(Mod, "BlackBox2")
            .Register();

            CreateRecipe()
            .AddIngredient(ItemID.Sawmill)
            .AddIngredient(ItemID.TungstenBar, 8)
            .AddIngredient(ItemID.Chain, 10)
            .AddIngredient(ItemID.Vertebrae, 2)
            .AddTile(Mod, "BlackBox2")
            .Register();
        }
    }
}
