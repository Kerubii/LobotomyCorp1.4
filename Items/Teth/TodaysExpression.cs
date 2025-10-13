using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Teth
{
    public class TodaysExpression : LobItemBase
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Today's Expression");
            /* Tooltip.SetDefault("Many different expressions are pdamageed on the equipment like patches.\n" +
                               "The inability to show one's face is perhaps a form of shyness."); */
        }

        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 20;
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<TethB>();
            Item.UseSound = LobotomyCorp.WeaponSounds.Gun;
            Item.autoReuse = true;
            Item.shoot = 10;
            Item.shootSpeed = 14f;
            Item.useAmmo = AmmoID.Bullet;
            Item.scale = 0.8f;
            EGORiskLevel = RiskLevel.Teth;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (RedMistMaskUpgrade(player))
            {
                int amount = Main.rand.Next(5);
                if (amount > 0)
                {
                    int floor = (int)Math.Floor(amount / 2f);
                    int start = floor * -1;
                    for (int i = start; i <= Math.Ceiling(amount / 2f); i++)
                    {
                        if (i == 0)
                            continue;
                        float rotation = velocity.ToRotation();
                        Vector2 offset = new Vector2(0, 4 * i).RotatedBy(velocity.ToRotation());
                        Projectile.NewProjectile(source, position + offset, velocity, type, damage, knockback, player.whoAmI);
                    }
                }
            }            
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            if (RedMistMaskUpgrade(player))
                damage += 3f;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.Silk, 5)
            .AddIngredient(ItemID.GoldBar, 5)
            .AddIngredient(ItemID.IllegalGunParts)
            .AddTile(Mod, "BlackBox")
            .Register();

            CreateRecipe()
            .AddIngredient(ItemID.Silk, 5)
            .AddIngredient(ItemID.PlatinumBar, 5)
            .AddIngredient(ItemID.IllegalGunParts)
            .AddTile(Mod, "BlackBox")
            .Register();
        }
    }
}
