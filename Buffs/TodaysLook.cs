using LobotomyCorp.Players;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace LobotomyCorp.Buffs
{
	public class TodaysLook : ModBuff
	{
		public static LocalizedText Happy { get; private set; }
        public static LocalizedText Smile { get; private set; }
        public static LocalizedText Neutral { get; private set; }
        public static LocalizedText Sad { get; private set; }
        public static LocalizedText Angry { get; private set; }

        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Look for the Day");
			// Description.SetDefault("The Look for the Day");
			Main.buffNoTimeDisplay[Type] = true;
			Happy = this.GetLocalization("Happy");
            Smile = this.GetLocalization("Smile");
            Neutral = this.GetLocalization("Neutral");
            Sad = this.GetLocalization("Sad");
            Angry = this.GetLocalization("Angry");
        }

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
			int todaysLook = Main.LocalPlayer.GetModPlayer<LobotomyTethPlayer>().TodaysExpressionFace;
			switch (todaysLook)
            {
				case 0://Happy
					tip = Happy.Value;
					break;
				case 1://Smile
					tip = Smile.Value;
                    break;
				default://Neutral
					tip = Neutral.Value;
                    break;
				case 3://Sad
					tip = Sad.Value;
                    break;
				case 4://Angry
					tip = Angry.Value;
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