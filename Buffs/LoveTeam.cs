using LobotomyCorp.Items.Ruina.Natural;
using LobotomyCorp.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace LobotomyCorp.Buffs
{
	public class LoveTeam : ModBuff
	{
		public override void SetStaticDefaults()
		{
            Main.buffNoTimeDisplay[Type] = true;
        }


        public override void Update(Player player, ref int buffIndex)
        {
            LobotomyWawPlayer modPlayer = player.GetModPlayer<LobotomyWawPlayer>();
            modPlayer.LoveAndHateRegenBuff = true;
        }
    }
}