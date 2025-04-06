using LobotomyCorp.Players;
using Terraria;
using Terraria.ModLoader;

namespace LobotomyCorp.Buffs
{
	public class CrookedNotes : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Crooked Notes");
			// Description.SetDefault("...What is that noise?");
		}

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Generic) += 0.08f;

            player.GetModPlayer<LobotomyHePlayer>().HarmonyConnected = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            //npc.defense = npc.defDefense - 15;
            npc.GetGlobalNPC<LobotomyGlobalNPC>().HarmonyMusicalAddiction = true;
        }
    }
}