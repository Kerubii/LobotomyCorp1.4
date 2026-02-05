using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Teth
{
    public class SoCute : LobItemBase
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("SO CUTE!!!");
            /* Tooltip.SetDefault("Beware that the beast inside you may awaken if you use this weapon too much...\n" +
                               "Oh but the soft jelly-like pawbs feel vewwy nice to touch."); */
        }

        public override void LobSetDefaults()
        {
            Item.CloneDefaults(ItemID.CopperShortsword);
            Item.shoot = ModContent.ProjectileType<Projectiles.SoCute>();
            Item.damage = 28;
            Item.useAnimation = 12;
            Item.useTime = 12;
            Item.autoReuse = true;
            Item.rare = ModContent.RarityType<TethB>();
            Item.UseSound = LobotomyCorp.WeaponSounds.Fist;
            EGORiskLevel = RiskLevel.Teth;

        }
        /*
        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            player.immune = true;
            player.immuneTime = 15;
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            player.immune = true;
            player.immuneTime = 15;
        }
        */
        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.Silk, 25)
            .AddTile(Mod, "BlackBox")
            .Register();
        }
    }
}