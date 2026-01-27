using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Aleph
{
    public class Censored : LobItemBase
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("[CENSORED]"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
            /* Tooltip.SetDefault("[CENSORED] has the ability to [CENSORED], but this is a horrendous sight for those watching.\n" +
							   "Looking at the E.G.O for more than 3 seconds will make you sick.\n" +
							   "Heal 40% damage taken\n"); */
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 42;
            Item.knockBack = 6;
            Item.scale = 1.3f;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;

            Item.useTime = 32;
            Item.useAnimation = 32;
            Item.useStyle = 5;

            Item.value = 10000;
            Item.rare = ModContent.RarityType<AlephB>();
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.shoot = ModContent.ProjectileType<Projectiles.CensoredGrab>();
            Item.shootSpeed = 1f;
            EGORiskLevel = RiskLevel.Aleph;
        }

        public override bool CanUseItem(Player player)
        {
            // Ensures no more than one spear can be thrown out, use this when using autoReuse
            if (player.altFunctionUse == 2)
            {
                Item.UseSound = LobotomyCorp.WeaponSound("Censored2_1");
                Item.shoot = ModContent.ProjectileType<Projectiles.CensoredSpike>();
            }
            else
            {
                Item.UseSound = LobotomyCorp.WeaponSound("Censored1");
                Item.shoot = ModContent.ProjectileType<Projectiles.CensoredGrab>();
            }

            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override float UseSpeedMultiplier(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                return 32f / 42f;
            }
            return base.UseSpeedMultiplier(player);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                damage = (int)(damage * 0.8f);
            }
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override void AddRecipes()
        {
        }
    }
}