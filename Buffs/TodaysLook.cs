using LobotomyCorp.Players;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace LobotomyCorp.Buffs
{
	public class TodaysLook : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Look for the Day");
			// Description.SetDefault("The Look for the Day");
			Main.buffNoTimeDisplay[Type] = true;
		}

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
			int todaysLook = Main.LocalPlayer.GetModPlayer<LobotomyTethPlayer>().TodaysExpressionFace;
			switch (todaysLook)
            {
				case 0://Happy
					tip = Language.GetTextValue("Mods.LobotomyCorp.Buffs.TodaysLook.Happy");
					break;
				case 1://Smile
					tip = Language.GetTextValue("Mods.LobotomyCorp.Buffs.TodaysLook.Smile");
					break;
				default://Neutral
					tip = Language.GetTextValue("Mods.LobotomyCorp.Buffs.TodaysLook.Neutral");
					break;
				case 3://Sad
					tip = Language.GetTextValue("Mods.LobotomyCorp.Buffs.TodaysLook.Sad");
					break;
				case 4://Angry
					tip = Language.GetTextValue("Mods.LobotomyCorp.Buffs.TodaysLook.Angry");
					break;
            }
        }

		public const float TODAYDAMAGEHAPPY = 0.85f;
		public const float TODAYDAMAGESMILE = 0.9f;
		public const float TODAYDAMAGENEUTRAL = 1;
		public const float TODAYDAMAGESAD = 1.1f;
		public const float TODAYDAMAGEANGRY = 1.2f;

		public override void Update(Player player, ref int buffIndex)
        {
			LobotomyTethPlayer modPlayer = player.GetModPlayer<LobotomyTethPlayer>();

            modPlayer.TodaysExpressionActive = true;
			player.buffTime[buffIndex] = 10;
			int todaysLook = modPlayer.TodaysExpressionFace;

			switch (todaysLook)
            {
				case 0://Happy
					player.endurance += 0.2f;
					player.statDefense += 30;
					break;
				case 1://Smile
					player.statDefense += 15;
					break;
				default://Neutral
					break;
				case 3://Sad
					player.statDefense -= 15;
					break;
				case 4://Angry
					player.endurance -= 0.3f;
					break;
            }
        }
    }
}