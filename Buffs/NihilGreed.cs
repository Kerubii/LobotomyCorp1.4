using Terraria;
using Terraria.ModLoader;
using LobotomyCorp.Players;
using static LobotomyCorp.Players.LobotomyAlephPlayer;

namespace LobotomyCorp.Buffs
{
	public class NihilGreed : Nihil
	{
		public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;

            DefenseA = 78;

            DamageA = .75f;
            DamageB = .15f;

            SpeedA = .24f;
            SpeedB = .08f;

            CriticalA = .3f;
            CriticalB = .02f;
        }

        public override NihilType GetMode => NihilType.Greed;

        public override DamageClass GetDamageType => DamageClass.Melee;
    }
}