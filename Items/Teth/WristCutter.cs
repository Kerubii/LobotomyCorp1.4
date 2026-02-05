using LobotomyCorp.Projectiles.RedMist;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Teth
{
    public class WristCutter : LobItemBase
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Its sharp blade can make a clean cut through bone like a hot knife through butter,\n" +
                               "Leaving a wound that will never heal.\n" +
                               "Sold by the Merchant during Blood Moon"); */
        }

        public override void LobSetDefaults()
        {
            Item.CloneDefaults(ItemID.CopperShortsword);
            Item.shoot = ModContent.ProjectileType<Projectiles.WristCutter>();
            Item.damage = 16;
            Item.rare = ModContent.RarityType<TethB>();
            Item.UseSound = LobotomyCorp.WeaponSounds.Dagger;
            EGORiskLevel = RiskLevel.Teth;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                type = ModContent.ProjectileType<WristCutterThrown>();
                velocity *= 14;
            }
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<WristCutterThrown>()] == 0;
        }

        public override bool AltFunctionUse(Player player)
        {
            return RedMistMaskUpgrade(player);
        }

        public override void AddRecipes()
        {
        }
    }
}