using LobotomyCorp.Players;
using Terraria;
using Terraria.ModLoader;

namespace LobotomyCorp.Buffs
{
	public class Fairy : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Fairy");
			// Description.SetDefault("Under the Fairy's care");
		}
		
		public override void Update(NPC npc, ref int BuffIndex)
		{
			LobotomyGlobalNPC.LNPC(npc).WingbeatFairyMeal = true;
            if (npc.defense < -5)
            {
                npc.defense = 0;
            }
            else
                npc.defense -= 5;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<LobotomyZayinPlayer>().WingbeatFairyMeal = true;
            player.statDefense -= 5;
        }
    }
}