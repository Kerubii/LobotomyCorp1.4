using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Teth
{
    public class CherryBlossoms : LobItemBase
    {
        public override void LobSetDefaults()
        {
            Item.damage = 18;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 4;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = 1;
            Item.noMelee = true;
            Item.knockBack = 2.4f;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<TethB>();
            Item.UseSound = LobotomyCorp.WeaponSound("sakura");
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Projectiles.CherryBlossomsPetal>();
            Item.shootSpeed = 14f;
            EGORiskLevel = RiskLevel.Teth;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                damage = (int)(damage * 0.6f);
                int amount = RedMistMaskUpgrade(player) ? 6 : 3;
                for (int i = 0; i < amount; i++)
                {
                    Vector2 speed = velocity.RotatedByRandom(MathHelper.ToRadians(15));
                    Projectile.NewProjectile(source, position, speed, type, damage, knockback, player.whoAmI);
                }
            }
            return false;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            if (RedMistMaskUpgrade(player))
            {
                damage += 3.5f;
            }
            base.ModifyWeaponDamage(player, ref damage);
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            Dust dust;
            dust = Main.dust[Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, ModContent.DustType<Misc.Dusts.BlossomDust>(), 0f, 0f, 0, new Color(255, 255, 255), 1f)];
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.Acorn, 3)
            .AddIngredient(ItemID.DynastyWood, 10)
            .AddTile(Mod, "BlackBox")
            .Register();

            CreateRecipe()
            .AddIngredient(ItemID.Acorn, 3)
            .AddIngredient(ItemID.VanityTreeSakuraSeed, 4)
            .AddTile(Mod, "BlackBox")
            .Register();
        }
    }
}