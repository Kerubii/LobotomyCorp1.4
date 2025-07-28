using System;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Audio;

namespace LobotomyCorp.Buffs
{
	public class Villain : ModBuff
	{
		public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            BuffID.Sets.IsATagBuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<LobotomyGlobalNPC>().InTheNameOfLoveAndHateVillain = true;
        }
    }
}