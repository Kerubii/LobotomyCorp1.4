using Microsoft.Xna.Framework;
using Terraria;
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

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.CopperShortsword);
            Item.shoot = ModContent.ProjectileType<Projectiles.WristCutter>();
            Item.damage = 16;
            Item.rare = ModContent.RarityType<TethB>();
            Item.UseSound = LobotomyCorp.WeaponSounds.Dagger;
            EGORiskLevel = RiskLevel.Teth;
        }

        public override void AddRecipes()
        {
        }
    }
}