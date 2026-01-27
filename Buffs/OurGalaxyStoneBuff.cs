using LobotomyCorp.Players;
using Microsoft.Xna.Framework.Audio;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

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
            NetworkText text = NetworkText.FromKey("Mods.LobotomyCorp.DeathMessages.StoneRemove", Main.LocalPlayer.name);
            PlayerDeathReason playerDeath = PlayerDeathReason.ByCustomReason(text);
            Main.LocalPlayer.Hurt(playerDeath, Main.LocalPlayer.statLifeMax2 / 4, 0, dodgeable: false, scalingArmorPenetration: 1);
            return true;
        }
    }
}