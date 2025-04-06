using System;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Audio;
using LobotomyCorp.Players;

namespace LobotomyCorp.Buffs
{
	public class OurGalaxyStoneBuff : ModBuff
	{
		public override void SetStaticDefaults()
        {
			// DisplayName.SetDefault("Token of Friendship");
			// Description.SetDefault("Let's walk the galaxy together");
            Main.buffNoTimeDisplay[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            LobotomyHePlayer modPlayer = player.GetModPlayer<LobotomyHePlayer>();
            modPlayer.OurGalaxyStone = true;
            player.buffTime[buffIndex] = 5;

            if (modPlayer.OurGalaxyOwner < 0)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }

        public override bool RightClick(int buffIndex)
        {
            return false;
        }
    }
}