using Terraria.ID;
using Terraria.ModLoader;
using LobotomyCorp.Items.Teth;

namespace LobotomyCorp.Items.Armor.Teth
{
    [AutoloadEquip(EquipType.Body)]
    public class FourthMatchFlameSuit : LobItemBase
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return ModContent.GetInstance<Configs.LobotomyServerConfig>().TestItemEnable;
        }

        public override void SetStaticDefaults()
        {
            // Links the ego weapon to the armor, used for temporary vanity set acquiration
            PEBox.EgoSets.Add(
                ModContent.ItemType<FourthMatchFlame>(),
                new int[2] { ModContent.ItemType<FourthMatchFlameSuit>(), ModContent.ItemType<FourthMatchFlamePants>() }
                );
        }

        public override void LobSetDefaults()
        {
            Item.vanity = true;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 56;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<TethB>();
            Item.defense = 6;
            EGORiskLevel = RiskLevel.Teth;
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class FourthMatchFlamePants : LobItemBase
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return ModContent.GetInstance<Configs.LobotomyServerConfig>().TestItemEnable;
        }

        public override void LobSetDefaults()
        {
            Item.vanity = true;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 56;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<TethB>();
            Item.defense = 6;
            EGORiskLevel = RiskLevel.Teth;
        }
    }
}