using LobotomyCorp.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Teth
{
    public class EngulfingDream : LobItemBase
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("We must be awake at all times.\n" +
                               "Not even sweet dreams in a sound sleep are allowed here; this weapon shall wake those who swim in such illusions.\n" +
                               "And when the crying stops, dawn will break."); */
        }

        public override void SetDefaults()
        {
            Item.damage = 18;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 3;
            Item.width = 26;
            Item.height = 20;
            Item.useTime = 9;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 0;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<TethB>();
            Item.UseSound = new SoundStyle("LobotomyCorp/Sounds/Item/LWeapons/Dreamy", 2) with { Volume = 0.5f, MaxInstances = 1, SoundLimitBehavior = SoundLimitBehavior.IgnoreNew };
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Projectiles.EngulfingDreamCall>();
            Item.shootSpeed = 0.1f;
            EGORiskLevel = RiskLevel.Teth;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(8, 0);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int ai1 = 0;
            if (RedMistMaskUpgrade(player))
            {
                ai1 = 1;
                if (Main.rand.NextBool(6))
                {
                    int amount = 3 + Main.rand.Next(3);
                    for (int i = 0; i < amount; i++)
                    {
                        Vector2 starVel = velocity * 40;
                        starVel = starVel.RotatedByRandom(MathHelper.ToRadians(360));
                        Projectile.NewProjectile(source, position, starVel, ModContent.ProjectileType<EngulfingDreamStar>(), damage, knockback, player.whoAmI);
                    }
                }
            }
            
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0, ai1);
            return false;
        }

        public override float UseTimeMultiplier(Player player)
        {
            return base.UseTimeMultiplier(player);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.Cloud, 30)
            .AddIngredient(ItemID.Feather)
            .AddIngredient(ItemID.FallenStar, 2)
            .AddTile(Mod, "BlackBox")
            .Register();
        }
    }
}
