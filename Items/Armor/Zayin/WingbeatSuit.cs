using LobotomyCorp.Items.Armor.Zayin;
using LobotomyCorp.Items.Zayin;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Armor.Zayin
{
    [AutoloadEquip(EquipType.Head)]
    public class WingbeatGift : LobItemBase
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
                ModContent.ItemType<Wingbeat>(),
                new int[3] { ModContent.ItemType<WingbeatGift>(), ModContent.ItemType<WingbeatSuit>(), ModContent.ItemType<WingbeatPants>() }
                );
        }

        public override void LobSetDefaults()
        {
            Item.vanity = true;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 56;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<ZayinB>();
            EGORiskLevel = RiskLevel.Zayin;
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class WingbeatSuit : LobItemBase
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
            Item.rare = ModContent.RarityType<ZayinB>();
            EGORiskLevel = RiskLevel.Zayin;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return legs.type == ModContent.ItemType<WingbeatPants>();
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class WingbeatPants : LobItemBase
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
            Item.rare = ModContent.RarityType<ZayinB>();
            EGORiskLevel = RiskLevel.Zayin;
        }
    }
}