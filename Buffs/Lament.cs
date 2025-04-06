using LobotomyCorp.Players;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Buffs
{
	public class Lament : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Lament");
			// Description.SetDefault("A Solemn Lament gun is disabled");
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
		
        public override void Update(Player player, ref int buffIndex)
        {
            if (player.buffTime[buffIndex] == 1)
                player.GetModPlayer<LobotomyWawPlayer>().SolemnLamentDisable = 0;
        }
    }
}