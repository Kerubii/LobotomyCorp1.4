using Terraria;
using Terraria.ModLoader;
using static LobotomyCorp.Players.LobotomyAlephPlayer;

namespace LobotomyCorp.Buffs
{
	public class NihilDespair : Nihil
	{
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;

            DefenseA = 68;

            DamageA = .68f;
            DamageB = .06f;

            SpeedA = .18f;
            SpeedB = .05f;

            CriticalA = .50f;
            CriticalB = .02f;
        }

        public override NihilType GetMode => NihilType.Despair;

        public override DamageClass GetDamageType => DamageClass.Ranged;
    }
}