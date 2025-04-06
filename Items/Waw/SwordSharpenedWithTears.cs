using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Waw
{
    public class SwordSharpenedWithTears : LobItemBase
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Penitence"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
            /* Tooltip.SetDefault("A sword suitable for swift thrusts.\n" +
                               "Even someone unskilled in dueling can rapidly puncture an enemy using this E.G.O with remarkable agility.\n" +
                               "Alternate attack to perform a flurry of stabs"); */
        }

        public override void SetDefaults()
        {
            Item.damage = 38;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;

            Item.useTime = 16;
            Item.useAnimation = 16;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ItemRarityID.Purple;
            Item.shootSpeed = 3.2f;
            Item.shoot = ModContent.ProjectileType<Projectiles.SwordSharpenedWithTearsProj>();

            Item.noUseGraphic = true;
            Item.UseSound = SoundID.Item1;
            Item.noMelee = true;
            Item.autoReuse = true;
            EGORiskLevel = RiskLevel.Waw;
        }

        private bool AlternateAttack;

        /*public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = velocity.RotatedByRandom((float)MathHelper.ToRadians(player.altFunctionUse == 2 ? 30 : 15));
            if (player.altFunctionUse == 2)
            {
                knockback = 6f;
                damage = (int)(damage * 0.75f);
            }
        }*/

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (AlternateAttack)
            {
                knockback = 6f;
                damage = (int)(damage * 0.9f);
                if (player.itemAnimation != player.itemAnimationMax)
                {
                    type = ModContent.ProjectileType<Projectiles.SpearExtender>();
                    velocity *= 6f;
                    if (Main.myPlayer == player.whoAmI)
                    {
                        Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 2, -4);
                    }
                    return false;
                }
                return true;
            }

            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }

        /*public override bool AltFunctionUse(Player player)
        {
            return true;
        }*/

        public override bool CanUseItem(Player player)
        {
            /*
            if (player.altFunctionUse == 2)
            {
                Item.UseSound = LobotomyCorp.WeaponSound("rapier1", false);
                Item.reuseDelay = 4;
            }
            else
            {
                Item.UseSound = LobotomyCorp.WeaponSound("rapier2", false);
                Item.reuseDelay = 0;
            }*/
            if (Main.rand.NextBool(8))
            {
                Item.UseSound = LobotomyCorp.WeaponSound("rapier1", false);
                Item.reuseDelay = 4;
                AlternateAttack = true;
            }
            else
            {
                Item.UseSound = LobotomyCorp.WeaponSound("rapier1", false);
                Item.reuseDelay = 4;
                AlternateAttack = false;
            }

            return base.CanUseItem(player);
        }

        public override float UseSpeedMultiplier(Player player)
        {
            if (AlternateAttack)
            {
                return 16 / 30f;
            }
            return base.UseSpeedMultiplier(player);
        }

        public override float UseTimeMultiplier(Player player)
        {
            if (AlternateAttack)
            {
                return 2 / 16f;
            }
            return base.UseTimeMultiplier(player);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.Gladius)
            .AddIngredient(ItemID.Sapphire)
            .AddIngredient(ItemID.FallenStar, 8)
            .AddTile(Mod, "BlackBox3")
            .Register();
        }
    }
}