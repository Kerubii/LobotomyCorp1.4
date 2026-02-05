using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.He
{
    public class Syrinx : LobItemBase
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("What cry could be more powerful than one spurred by primal instinct?\n" +
                               "As if everything else were hollow and pointless,\nThe wailing numbs even the brain, making it impossible to think.\n" +
							   "Can be dropped by Eye of Cthulhu"); */
        }

        public override void LobSetDefaults()
        {
            Item.damage = 12;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 20;
            Item.useTime = 3;
            Item.useAnimation = 9;
            Item.reuseDelay = 14;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<HeB>();
            Item.UseSound = new SoundStyle("LobotomyCorp/Sounds/Item/LWeapons/fetus", 3) with { Volume = 0.5f, MaxInstances = 1, SoundLimitBehavior = SoundLimitBehavior.IgnoreNew };
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Projectiles.SyrinxSound>();
            Item.shootSpeed = 8f;
            EGORiskLevel = RiskLevel.He;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, player.itemAnimation / 9f);
            return false;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
    }
}
