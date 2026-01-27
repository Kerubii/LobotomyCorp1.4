using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Aleph
{
    public class Pink : LobItemBase
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Pink is considered to be the color of warmth and love, but is that true?\n" +
                               "Can guns really bring peace and love?\n" +
                               "Can only be used while standing absolutely still"); */
        }

        public override void SetDefaults()
        {
            Item.damage = 184; // Sets the Item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage damageed together.
            Item.DamageType = DamageClass.Ranged; // sets the damage type to ranged
            Item.width = 40; // hitbox width of the Item
            Item.height = 42; // hitbox height of the Item
            Item.useTime = 36; // The Item's use time in ticks (60 ticks == 1 second.)
            Item.useAnimation = 36; // The length of the Item's use animation in ticks (60 ticks == 1 second.)
            Item.useStyle = ItemUseStyleID.Shoot; // how you use the Item (swinging, holding out, etc)
            Item.noMelee = true; //so the Item's animation doesn't do damage
            Item.knockBack = 4; // Sets the Item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback damageed together.
            Item.value = 10000; // how much the Item sells for (measured in copper)
            Item.rare = ModContent.RarityType<AlephB>(); // the color that the Item's name will be in-game
            Item.UseSound = LobotomyCorp.WeaponSounds.Rifle; // The sound that this Item plays when used.
            Item.autoReuse = true; // if you can hold click to automatically use it again
            Item.shoot = 10; //idk why but all the guns in the vanilla source have this
            Item.shootSpeed = 16f; // the speed of the projectile (measured in pixels per frame)
            Item.useAmmo = AmmoID.Bullet; // The "ammo Id" of the ammo Item that this weapon uses. Note that this is not an Item Id, but just a magic value.
            EGORiskLevel = RiskLevel.Aleph;
        }

        /*public override bool CanUseItem(Player player)
        {
            return player.velocity.X == 0 && player.velocity.Y == 0;
        }*/

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.itemAnimation == 2 * player.itemAnimationMax / 3)
                SoundEngine.PlaySound(LobotomyCorp.WeaponSound("PinkArmy2", false), player.Center);

            base.UseStyle(player, heldItemFrame);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            float velVar = velocityVariance(player);
            //Main.NewText(velVar);
            //velocity = velocity.RotatedBy(velVar * MathHelper.ToRadians(Main.rand.Next(-30, 31)));
            if (velVar > 0.1f)
                damage = (int)(damage * 0.8f);
            damage = (int)(damage * (1f - 0.9f * velVar));

            if (velVar < 0.1f)
                type = ProjectileID.BulletHighVelocity;
        }

        public override float UseSpeedMultiplier(Player player)
        {
            float baseMult = 1f + 1f * velocityVariance(player);
            return baseMult;
        }

        private float velocityVariance(Player player)
        {
            float maxVel = 16;
            float vel = player.velocity.Length();
            return (float)Math.Min(1f, vel / maxVel);
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-16, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.Musket)
            .AddIngredient(ItemID.ClockworkAssaultRifle)
            .AddIngredient(ItemID.PinkDye)
            .AddIngredient(ItemID.PinkThread, 3)
            .AddTile(Mod, "BlackBox3")
            .Register();
        }
    }
}
