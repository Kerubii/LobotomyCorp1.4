using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.He
{
    public class BearPaw : LobItemBase
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Its adorable appearance makes it something that might even appeal to a child as a gift.\n" +
                               "Do not underestimate the weaponÅ's power because of its fluffy exterior."); */
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.CopperShortsword);
            Item.shoot = ModContent.ProjectileType<Projectiles.BearPaw>();
            Item.damage = 40;
            Item.useAnimation = 12;
            Item.useTime = 12;
            Item.autoReuse = true;
            Item.UseSound = LobotomyCorp.WeaponSounds.Fist;
            Item.rare = ItemRarityID.Yellow;
            EGORiskLevel = RiskLevel.He;
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
            .AddIngredient(ItemID.Silk, 10)
            .AddIngredient(ItemID.Cobweb, 20)
            .AddIngredient(ItemID.BrownDye, 3)
            .AddTile(Mod, "BlackBox")
            .Register();
        }
    }
}