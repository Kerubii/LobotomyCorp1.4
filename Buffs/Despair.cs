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
	public class Despair : ModBuff
	{
		public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Festival");
            // Description.SetDefault("Everything will be peaceful");
            //BuffID.Sets.TimeLeftDoesNotDecrease[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            LobotomyWawPlayer modPlayer = player.GetModPlayer<LobotomyWawPlayer>();
            player.statLifeMax2 = (int)(player.statLifeMax2 * 0.75f);
            modPlayer.SwordSharpenedDespair = true;
            player.buffTime[buffIndex]++;
        }
    }
}