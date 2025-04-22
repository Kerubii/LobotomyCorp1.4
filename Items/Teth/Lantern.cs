using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Teth
{
    public class Lantern : LobCorpHeavy
    {
        public override void SetDefaults()
        {
            Item.damage = 62;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 56;
            Item.useAnimation = 56;
            Item.useStyle = 15;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<TethB>();
            SwingSound = LobotomyCorp.WeaponSounds.Hammer;
            Item.autoReuse = true;
            EGORiskLevel = RiskLevel.Teth;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.FlinxFur, 2)
            .AddIngredient(ItemID.Emerald, 1)
            .AddIngredient(ItemID.AntlionMandible, 4)
            .AddTile<Tiles.BlackBox>()
            .Register();
        }
    }
}