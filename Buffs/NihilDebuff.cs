using System;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Audio;
using Terraria.Localization;
using LobotomyCorp.Players;
using Terraria.DataStructures;

namespace LobotomyCorp.Buffs
{
	public class NihilDebuff : ModBuff
	{
		public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<LobotomyGlobalNPC>().NihilDebuff = true;
        }
    }
}