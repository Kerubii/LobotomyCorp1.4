using LobotomyCorp.Projectiles;
using LobotomyCorp.Projectiles.RedMist;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static LobotomyCorp.Items.LobItemBase;

namespace LobotomyCorp.Items.Zayin
{
    public class Wingbeat : LobCorpLight
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Penitence"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
            /* Tooltip.SetDefault("Graced by the fairies, the weapon radiates a pale light.\n" +
                               "Despite its cute shape, the E.G.O. itself is rather heavy."); */
        }

        public override void SetDefaults()
        {
            Item.damage = 18;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = 15;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = LobotomyCorp.WeaponSounds.Mace;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<WingbeatSlash>();
            Item.shootSpeed = 14;
            EGORiskLevel = RiskLevel.Zayin;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            if (RedMistMaskUpgrade(player))
                damage += 3.5f;
        }

        public override bool CanShoot(Player player)
        {
            return RedMistMaskUpgrade(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (RedMistMaskUpgrade(player))
            {
                //Soder Shoot//Shoot Wingbeat Fairy, Fairye slashes forward then it sticks to the first enemy hit, releases then comes back
                //Also creates a wing beat slash projectile
                type = ModContent.ProjectileType<WingbeatFairy2>();
                if (player.ownedProjectileCounts[type] < 3)
                {
                    int n = Projectile.NewProjectile(player.GetSource_FromThis(), position, velocity * 1.1f, type, damage / 3, knockback, player.whoAmI);
                }
                return true;
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.Firefly, 3)
            .AddIngredient(ItemID.Bottle)
            .AddIngredient(ItemID.Sunflower)
            .AddTile(Mod, "BlackBox")
            .Register();
        }
    }
}