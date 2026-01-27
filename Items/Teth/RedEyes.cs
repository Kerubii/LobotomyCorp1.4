using LobotomyCorp.ModSystems;
using LobotomyCorp.Visuals.PrimEffects;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Teth
{
    public class RedEyes : LobCorpLight
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Penitence"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
            /* Tooltip.SetDefault("The Spider Bud had dozens of eyes, and its children were always hungry.\n" +
                               "This tenacity carried over to the E.G.O., demonstrating an outstanding ability to track down targets.\n" +
                               "Grants swiftness buff for 3 seconds on hit"); */
        }

        public override void SetDefaults()
        {
            Item.damage = 22;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = 15;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<TethB>();
            Item.UseSound = LobotomyCorp.WeaponSounds.Mace;
            Item.autoReuse = true;
            Item.scale = 1.3f;
            EGORiskLevel = RiskLevel.Teth;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            player.AddBuff(BuffID.Swiftness, 180);
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            player.AddBuff(BuffID.Swiftness, 180);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.Cobweb, 100)
            .AddIngredient(ItemID.Ruby, 3)
            .AddTile(Mod, "BlackBox")
            .Register();
        }
    }
}