using System;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Audio;
using LobotomyCorp.Players;

namespace LobotomyCorp.Buffs
{
	public class GrinderMk52Activated : ModBuff
	{
		public override void SetStaticDefaults()
        {
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override bool RightClick(int buffIndex)
        {
            Main.LocalPlayer.GetModPlayer<LobotomyHePlayer>().GrinderMk2Battery = 0;
            return base.RightClick(buffIndex);
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.moveSpeed += 0.40f;
            player.dashType = -1;

            LobotomyHePlayer modPlayer = player.GetModPlayer<LobotomyHePlayer>();
            modPlayer.GrinderMk2Active = true;
            player.GetModPlayer<LobotomyDashPlayer>().SpecialDash = true;

            if (player.buffTime[buffIndex] > 0)
                player.buffTime[buffIndex] = modPlayer.GrinderMk2Battery/4;
        }
    }
}