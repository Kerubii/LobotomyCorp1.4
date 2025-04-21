using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.He
{
    public class Lumber : LobCorpHeavy
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Penitence"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
            /* Tooltip.SetDefault("A versatile equipment made to cut down trees and people alike.\n" +
                               "Perhaps sharpening the axe was the one thing it didn't neglect. The blade is always shiny."); */
        }

        public override void SetDefaults()
        {
            Item.damage = 96;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 56;
            Item.useAnimation = 56;
            Item.useStyle = 15;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<HeB>();
            SwingSound = new SoundStyle("LobotomyCorp/Sounds/Item/Lumberjack_Atk2") with { Volume = 0.5f, PitchVariance = 0.1f }; ;
            Item.autoReuse = true;
            EGORiskLevel = RiskLevel.He;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.life < 0 && target.lifeMax > 20)
            {
                int number = Item.NewItem(target.GetSource_FromThis(), target.position, target.Size, ItemID.Heart);
                if (Main.netMode == 1)
                {
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, number, 1f);
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.IronAxe)
            .AddIngredient(ItemID.IronBar, 20)
            .AddIngredient(ItemID.Wood, 50)
            .AddTile(Mod, "BlackBox2")
            .Register();

            CreateRecipe()
            .AddIngredient(ItemID.LeadAxe)
            .AddIngredient(ItemID.LeadBar, 20)
            .AddIngredient(ItemID.Wood, 50)
            .AddTile(Mod, "BlackBox2")
            .Register();
        }
    }
}