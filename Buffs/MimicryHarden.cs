using LobotomyCorp.Players;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace LobotomyCorp.Buffs
{
	public class MimicryHarden : ModBuff
	{
        public override void Update(Player player, ref int buffIndex)
        {
            LobotomyAlephPlayer modPlayer = player.GetModPlayer<LobotomyAlephPlayer>();
            modPlayer.MimicryProtection = true;
        }

        public override bool RightClick(int buffIndex)
        {
            return false;
        }
    }
}