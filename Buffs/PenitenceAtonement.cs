using LobotomyCorp.Items.Zayin;
using LobotomyCorp.Players;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Buffs
{
	public class PenitenceAtonement : ModBuff
	{
        //Glorified Timer lmao

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.HeldItem.type == ModContent.ItemType<Penitence>())
            {
                player.aggro += 100;
            }
        }

        public override bool RightClick(int buffIndex)
        {
            return false;
        }
    }
}