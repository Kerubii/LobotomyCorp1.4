using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Zayin
{
    public class Soda : LobItemBase
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("A pistol painted in a refreshing purple.\n" +
                               "Whenever this E.G.O. is used, a faint scent of grapes wafts through the air."); */
        }

        public override void LobSetDefaults()
        {
            Item.damage = 9;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 20;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<ZayinB>();
            Item.UseSound = LobotomyCorp.WeaponSounds.Gun;
            Item.autoReuse = true;
            Item.shoot = 10;
            Item.shootSpeed = 14f;
            Item.useAmmo = AmmoID.Bullet;
            Item.scale = 0.8f;
            EGORiskLevel = RiskLevel.Zayin;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            if (RedMistMaskUpgrade(player))
                damage += 3.5f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (RedMistMaskUpgrade(player))
            {
                //Soder Shoot
                int n = Projectile.NewProjectile(player.GetSource_FromThis(), position, velocity, type, damage, knockback, player.whoAmI);
                Main.projectile[n].GetGlobalProjectile<LobotomyGlobalProjectile>().SodaSpecial = true;
                Main.projectile[n].netUpdate = true;
                return false;
            }
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            if (RedMistMaskUpgrade(player))
            {
                int rand = Main.rand.Next(3);
                if (rand == 0)
                {
                    player.Heal(1);
                }
                if (rand == 1)
                {
                    player.statMana += 20;
                    player.ManaEffect(20);
                }
                else
                {
                    return false;
                }
            }
            return base.CanConsumeAmmo(ammo, player);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(RecipeGroupID.IronBar, 10)
            .AddIngredient(ItemID.LesserHealingPotion)
            .AddIngredient(ItemID.LesserManaPotion)
            .AddIngredient(ItemID.BottledHoney)
            .AddTile(Mod, "BlackBox")
            .Register();
        }
    }
}
