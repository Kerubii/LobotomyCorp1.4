using LobotomyCorp.Items.Zayin;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class SodaGift : LobItemBase
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return ModContent.GetInstance<Configs.LobotomyServerConfig>().TestItemEnable;
        }

        public override void SetStaticDefaults()
        {
            ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
            // Links the ego weapon to the armor, used for temporary vanity set acquiration
            PEBox.EgoSets.Add(
                ModContent.ItemType<Soda>(),
                new int[3] { ModContent.ItemType<SodaGift>(), ModContent.ItemType<SodaSuit>(), ModContent.ItemType<SodaPants>() }
                );
        }

        public override void SetDefaults()
        {
            Item.vanity = true;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 56;
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;
            EGORiskLevel = RiskLevel.Zayin;
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class SodaSuit : LobItemBase
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return ModContent.GetInstance<Configs.LobotomyServerConfig>().TestItemEnable;
        }

        public override void SetDefaults()
        {
            Item.vanity = true;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 56;
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;
            EGORiskLevel = RiskLevel.Zayin;
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class SodaPants : LobItemBase
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return ModContent.GetInstance<Configs.LobotomyServerConfig>().TestItemEnable;
        }

        public override void SetDefaults()
        {
            Item.vanity = true;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 56;
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;
            EGORiskLevel = RiskLevel.Zayin;
        }
    }
}