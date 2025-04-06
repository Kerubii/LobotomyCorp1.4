using System;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Audio;
using LobotomyCorp.Players;

namespace LobotomyCorp.Buffs
{
	public class BrokenDreams : ModBuff
	{
		public override void SetStaticDefaults()
        {
			// DisplayName.SetDefault("Broken Dreams");
			// Description.SetDefault("30% increased damage taken");
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            tip += "\n15% increased attack speed";
            tip += "\n20% increased movement speed";
            tip += "\n100% increased true melee damage and enhances melee range";
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<LobotomyWawPlayer>().BlackSwanBrokenDream = true;
            player.GetModPlayer<LobotomyWawPlayer>().BlackSwanNettleClothing = 0;
            player.endurance -= 0.3f;
            player.moveSpeed += 0.2f;
        }
    }
}