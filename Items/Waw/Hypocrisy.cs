using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Waw
{
    public class Hypocrisy : LobItemBase
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("The tree turned out to be riddled with hypocrisy and deception;\n" +
                               "Those who wear its blessing act in the name of bravery and faith.\n" +
							   "However, be warned that nature does not know the difference between a blessing and a curse.\n" +
							   "Sold by the Dryad after Queen Bee"); */
        }

        public override void LobSetDefaults()
        {
            Item.damage = 40;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 20;
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<WawB>();
            Item.UseSound = LobotomyCorp.WeaponSounds.BowGun;
            Item.autoReuse = true;
            Item.shoot = 10;
            Item.shootSpeed = 10f;
            Item.useAmmo = AmmoID.Arrow;
            Item.scale = 0.8f;
            EGORiskLevel = RiskLevel.Waw;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            Main.projectile[p].GetGlobalProjectile<LobotomyGlobalProjectile>().HypocrisyArrow = true;
            return false;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }

        public override void AddRecipes()
        {
        }
    }
}
