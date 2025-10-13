using LobotomyCorp.Items.Ruina.Language;
using LobotomyCorp.Players;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace LobotomyCorp.Buffs
{
	public class Blessing : ModBuff
	{
		public override void SetStaticDefaults()
		{
            Main.buffNoTimeDisplay[Type] = true;
		}

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            tip += $"\n{Language.GetTextValue("Mods.LobotomyCorp.Buffs.Blessing.Description2")}";
        }

        public override bool RightClick(int buffIndex)
        {
            Main.LocalPlayer.GetModPlayer<LobotomyWawPlayer>().SwordSharpenedBlessingBestower = -1;
            return base.RightClick(buffIndex);
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<LobotomyWawPlayer>().SwordSharpenedBlessing = true;
        }
    }
}