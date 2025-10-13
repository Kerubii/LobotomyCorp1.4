using LobotomyCorp.Items.Waw;
using LobotomyCorp.Players;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace LobotomyCorp.Buffs
{
	public class Vengeance : ModBuff
	{
		public override void SetStaticDefaults()
		{
		}

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            int boost = (int)(Main.LocalPlayer.GetModPlayer<LobotomyWawPlayer>().CrimsonScarVengeanceBoost * 100f);
            tip += $"\n{Language.GetTextValue("Mods.LobotomyCorp.Buffs.Vengeance.Description2", boost)}";
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.HeldItem.type == ModContent.ItemType<CrimsonScar>())
                player.GetDamage(DamageClass.Generic) += player.GetModPlayer<LobotomyWawPlayer>().CrimsonScarVengeanceBoost;
        }
    }
}