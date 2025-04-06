using LobotomyCorp.Players;
using Terraria;
using Terraria.ModLoader;

namespace LobotomyCorp.Buffs
{
	public class Gluttony : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Predation");
			// Description.SetDefault("I need something fresh...");
			Main.debuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            LobotomyZayinPlayer modPlayer = player.GetModPlayer<LobotomyZayinPlayer>();
            modPlayer.WingbeatGluttony = true;
            if (modPlayer.WingbeatFairyMeal)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}