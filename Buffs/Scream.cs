using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Buffs
{
	public class Scream : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Horrid Screech");
			// Description.SetDefault("Wings disabled, 8% decreased movement speed");
            Main.debuff[Type] = true;
            //Main.buffNoTimeDisplay[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.wingTime = 0;
            player.moveSpeed -= 0.08f;

            if (Main.rand.NextBool(4))
            {
                Dust d = Main.dust[Dust.NewDust(player.position, player.width, player.height, DustID.Wraith)];
                d.noGravity = true;
            }
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.defense < -5)
            {
                npc.defense = 0;
            }
            else
                npc.defense -= 5;

            if (Main.rand.NextBool(4))
            {
                Dust d = Main.dust[Dust.NewDust(npc.position, npc.width, npc.height, DustID.Wraith)];
                d.noGravity = true;
            }
        }
    }
}