using LobotomyCorp.Visuals.ParticlesAura;
using LobotomyCorp.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Buffs
{
	public class Alertness : ModBuff
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Alerted");
            // Description.SetDefault("What careless fool stepped on my baby?");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.detectCreature = true;
            player.endurance -= 0.1f;
            player.GetModPlayer<LobotomyTethPlayer>().RedEyesAlerted = true;
            //LobotomyModPlayer.ModPlayer(player).CurrentAura.Add(new RedEyesEye());
            //LobotomyModPlayer.ModPlayer(player).CurrentAura.Add(new RedEyesMist());
        }

        public override bool RightClick(int buffIndex)
        {
            return false;
        }
    }
}