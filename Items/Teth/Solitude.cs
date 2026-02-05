using LobotomyCorp.Projectiles.RedMist;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Teth
{
    public class Solitude : LobItemBase
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("A strong sense of loneliness still lingers, even in the form of an E.G.O.\n" +
                               "Its bullets create a void that cannot be filled on the victim's soul.\n" +
                               "It was a rusty weapon from the beginning."); */
        }

        public override void LobSetDefaults()
        {
            Item.damage = 20;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 18;
            Item.height = 20;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<TethB>();
            Item.UseSound = LobotomyCorp.WeaponSounds.Revolver;
            Item.autoReuse = true;
            Item.shoot = 10;
            Item.shootSpeed = 14f;
            Item.useAmmo = AmmoID.Bullet;
            Item.scale = 0.8f;
            EGORiskLevel = RiskLevel.Teth;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.Revolver)
            .AddRecipeGroup("LobotomyCorp:EvilPowder", 5)
            .AddTile(Mod, "BlackBox")
            .Register();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (RedMistMaskUpgrade(player))
            {
                int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                Main.projectile[p].GetGlobalProjectile<LobotomyGlobalProjectile>().SolitudeSpecial = true;
                return false;
            }
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            if (RedMistMaskUpgrade(player))
                damage += 3.5f;
        }
    }
}
