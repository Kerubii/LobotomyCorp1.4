using LobotomyCorp.Players;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Buffs
{
	public class PleasureTail : ModBuff
	{
        public override void SetStaticDefaults()
        {
        // DisplayName.SetDefault("Pleasure");
			// Description.SetDefault("Pleasant feelings fills your head");
            //Main.debuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            BuffID.Sets.TimeLeftDoesNotDecrease[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
		
        public override void Update(Player player, ref int buffIndex)
        {
            player.wingRunAccelerationMult += 0.15f;
            player.GetModPlayer<LobotomyWawPlayer>().PleasureTail = true;
        }

        public override bool RightClick(int buffIndex)
        {            
            return Main.LocalPlayer.CheckMana(Main.LocalPlayer.statManaMax2 / 2, true, true);
        }
    }
}