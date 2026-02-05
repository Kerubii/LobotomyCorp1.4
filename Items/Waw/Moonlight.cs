using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Waw
{
    public class Moonlight : LobCorpLight
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Penitence"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
            /* Tooltip.SetDefault("The snake\'s open mouth represents the endless yearning for music.\n" +
                               "It temporarily invites the user to the world of trance.\n" +
                               "25% chance to apply Black Shields to nearby allies when used"); */
        }

        public override void LobSetDefaults()
        {
            Item.damage = 62;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 26;
            Item.useAnimation = 20;
            Item.useStyle = 15;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<WawB>();
            Item.UseSound = LobotomyCorp.WeaponSound("blackSwan1");
            Item.autoReuse = true;
            EGORiskLevel = RiskLevel.Waw;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(4))
                LobotomyModPlayer.ModPlayer(player).ApplyShield("B", 900, Item.damage * 2);
            foreach (Player teammate in Main.ActivePlayers)
            {
                if (teammate.whoAmI != player.whoAmI && !teammate.dead && teammate.team == player.team && Vector2.Distance(teammate.Center, player.Center) < 1600)
                {
                    LobotomyModPlayer.ModPlayer(teammate).ApplyShield("B", 900, Item.damage * 2);
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.TaxCollectorsStickOfDoom)
            .AddIngredient(ItemID.Piano)
            .AddIngredient(ItemID.Bird)
            .AddTile(Mod, "BlackBox3")
            .Register();

            Recipe recipe = Recipe.Create(ItemID.TaxCollectorsStickOfDoom);
            recipe.AddIngredient(ItemID.IronBar, 10);
            recipe.AddTile(TileID.Hellforge);
            recipe.Register();
        }
    }
}